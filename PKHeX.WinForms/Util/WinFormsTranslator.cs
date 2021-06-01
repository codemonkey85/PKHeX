using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using PKHeX.Core;

namespace PKHeX.WinForms
{
    public static class WinFormsTranslator
    {
        private static readonly Dictionary<string, TranslationContext> Context = new();
        internal static void TranslateInterface(this Control form, string lang) => TranslateForm(form, GetContext(lang));

        private static string GetTranslationFileNameInternal(string lang) => $"lang_{lang}";
        private static string GetTranslationFileNameExternal(string lang) => $"lang_{lang}.txt";

        public static IReadOnlyDictionary<string, string> GetDictionary(string lang) => GetContext(lang).Lookup;

        private static TranslationContext GetContext(string lang)
        {
            if (Context.TryGetValue(lang, out TranslationContext? context))
                return context;

            IEnumerable<string>? lines = GetTranslationFile(lang);
            Context.Add(lang, context = new TranslationContext(lines));
            return context;
        }

        private static void TranslateForm(Control form, TranslationContext context)
        {
            form.SuspendLayout();
            string? formname = form.Name;
            // Translate Title
            form.Text = context.GetTranslatedText(formname, form.Text);
            IEnumerable<object>? translatable = GetTranslatableControls(form);
            foreach (object? c in translatable)
            {
                if (c is Control r)
                {
                    string? current = r.Text;
                    string? updated = context.GetTranslatedText($"{formname}.{r.Name}", current);
                    if (!ReferenceEquals(current, updated))
                        r.Text = updated;
                }
                else if (c is ToolStripItem t)
                {
                    string? current = t.Text;
                    string? updated = context.GetTranslatedText($"{formname}.{t.Name}", current);
                    if (!ReferenceEquals(current, updated))
                        t.Text = updated;
                }
            }
            form.ResumeLayout();
        }

        private static IEnumerable<string> GetTranslationFile(string lang)
        {
            string? file = GetTranslationFileNameInternal(lang);
            // Check to see if a the translation file exists in the same folder as the executable
            string externalLangPath = GetTranslationFileNameExternal(file);
            if (File.Exists(externalLangPath))
            {
                try { return File.ReadAllLines(externalLangPath); }
#pragma warning disable CA1031 // Do not catch general exception types
                catch { /* In use? Just return the internal resource. */ }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            if (Util.IsStringListCached(file, out string[]? result))
                return result;
            string? txt = (string?)Properties.Resources.ResourceManager.GetObject(file);
            return Util.LoadStringList(file, txt);
        }

        private static IEnumerable<object> GetTranslatableControls(Control f)
        {
            foreach (Control? z in f.GetChildrenOfType<Control>())
            {
                switch (z)
                {
                    case ToolStrip menu:
                        foreach (object? obj in GetToolStripMenuItems(menu))
                            yield return obj;

                        break;
                    default:
                        if (string.IsNullOrWhiteSpace(z.Name))
                            break;

                        if (z.ContextMenuStrip != null) // control has attached MenuStrip
                        {
                            foreach (object? obj in GetToolStripMenuItems(z.ContextMenuStrip))
                                yield return obj;
                        }

                        if (z is ListControl or TextBoxBase or LinkLabel or NumericUpDown or ContainerControl)
                            break; // undesirable to modify, ignore

                        if (!string.IsNullOrWhiteSpace(z.Text))
                            yield return z;
                        break;
                }
            }
        }

        private static IEnumerable<T> GetChildrenOfType<T>(this Control control) where T : class
        {
            foreach (Control? child in control.Controls.OfType<Control>())
            {
                if (child is T childOfT)
                    yield return childOfT;

                if (!child.HasChildren) continue;
                foreach (T? descendant in GetChildrenOfType<T>(child))
                    yield return descendant;
            }
        }

        private static IEnumerable<object> GetToolStripMenuItems(ToolStrip menu)
        {
            foreach (ToolStripMenuItem? i in menu.Items.OfType<ToolStripMenuItem>())
            {
                if (!string.IsNullOrWhiteSpace(i.Text))
                    yield return i;
                foreach (ToolStripMenuItem? sub in GetToolsStripDropDownItems(i).Where(z => !string.IsNullOrWhiteSpace(z.Text)))
                    yield return sub;
            }
        }

        private static IEnumerable<ToolStripMenuItem> GetToolsStripDropDownItems(ToolStripDropDownItem item)
        {
            foreach (ToolStripMenuItem? dropDownItem in item.DropDownItems.OfType<ToolStripMenuItem>())
            {
                yield return dropDownItem;
                if (!dropDownItem.HasDropDownItems) continue;
                foreach (ToolStripMenuItem subItem in GetToolsStripDropDownItems(dropDownItem))
                    yield return subItem;
            }
        }

        public static void UpdateAll(string baseLanguage, IEnumerable<string> others)
        {
            TranslationContext? baseContext = GetContext(baseLanguage);
            foreach (string? lang in others)
            {
                TranslationContext? c = GetContext(lang);
                c.UpdateFrom(baseContext);
            }
        }

        public static void DumpAll(params string[] banlist)
        {
            var results = Context.Select(z => new {Lang = z.Key, Lines = z.Value.Write()});
            foreach (var c in results)
            {
                string? lang = c.Lang;
                string? fn = GetTranslationFileNameExternal(lang);
                IEnumerable<string>? lines = c.Lines;
                IEnumerable<string>? result = lines.Where(z => !banlist.Any(z.Contains));
                File.WriteAllLines(fn, result);
            }
        }

        public static void LoadAllForms(params string[] banlist)
        {
            IEnumerable<System.Type>? q = from t in System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
                where t.BaseType == typeof(Form) && !banlist.Contains(t.Name)
                select t;
            foreach (System.Type? t in q)
            {
                System.Reflection.ConstructorInfo[]? constructors = t.GetConstructors();
                if (constructors.Length == 0)
                { System.Console.WriteLine($"No constructors: {t.Name}"); continue; }
                int argCount = constructors[0].GetParameters().Length;
                try
                {
                    Form? _ = (Form?)System.Activator.CreateInstance(t, new object[argCount]);
                }
#pragma warning disable CA1031 // Do not catch general exception types
                // This is a debug utility method, will always be logging. Shouldn't ever fail.
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Debug.Write($"Failed to create a new form {t}");
                }
            }
        }

        public static void SetRemovalMode(bool status = true)
        {
            foreach (TranslationContext c in Context.Values)
            {
                c.RemoveUsedKeys = status;
                c.AddNew = !status;
            }
        }

        public static void RemoveAll(string defaultLanguage, params string[] banlist)
        {
            TranslationContext? badKeys = Context[defaultLanguage];
            string[]? split = badKeys.Write().Select(z => z.Split(TranslationContext.Separator)[0])
                .Where(l => !banlist.Any(l.StartsWith)).ToArray();
            foreach (KeyValuePair<string, TranslationContext> c in Context)
            {
                string? lang = c.Key;
                string? fn = GetTranslationFileNameExternal(lang);
                string[]? lines = File.ReadAllLines(fn);
                IEnumerable<string>? result = lines.Where(l => !split.Any(s => l.StartsWith(s + TranslationContext.Separator)));
                File.WriteAllLines(fn, result);
            }
        }

        public static void LoadSettings<T>(string defaultLanguage, bool add = true)
        {
            Dictionary<string, string>? context = (Dictionary<string, string>)Context[defaultLanguage].Lookup;
            System.Reflection.PropertyInfo[]? props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            foreach (System.Reflection.PropertyInfo? prop in props)
            {
                System.Reflection.PropertyInfo[]? p = prop.PropertyType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                foreach (System.Reflection.PropertyInfo? x in p)
                {
                    LocalizedDescriptionAttribute[]? individual = (LocalizedDescriptionAttribute[])x.GetCustomAttributes(typeof(LocalizedDescriptionAttribute), false);
                    foreach (LocalizedDescriptionAttribute? v in individual)
                    {
                        bool hasKey = context.ContainsKey(v.Key);
                        if (add)
                        {
                            if (!hasKey)
                                context.Add(v.Key, v.Fallback);
                        }
                        else
                        {
                            if (hasKey)
                                context.Remove(v.Key);
                        }
                    }
                }
            }
        }
    }

    public sealed class TranslationContext
    {
        public bool AddNew { private get; set; }
        public bool RemoveUsedKeys { private get; set; }
        public const char Separator = '=';
        private readonly Dictionary<string, string> Translation = new();
        public IReadOnlyDictionary<string, string> Lookup => Translation;

        public TranslationContext(IEnumerable<string> content, char separator = Separator)
        {
            IEnumerable<string[]>? entries = content.Select(z => z.Split(separator)).Where(z => z.Length == 2);
            foreach (string[]? kvp in entries.Where(z => !Translation.ContainsKey(z[0])))
                Translation.Add(kvp[0], kvp[1]);
        }

        public string? GetTranslatedText(string val, string? fallback)
        {
            if (RemoveUsedKeys)
                Translation.Remove(val);

            if (Translation.TryGetValue(val, out string? translated))
                return translated;

            if (fallback != null && AddNew)
                Translation.Add(val, fallback);
            return fallback;
        }

        public IEnumerable<string> Write(char separator = Separator)
        {
            return Translation.Select(z => $"{z.Key}{separator}{z.Value}").OrderBy(z => z.Contains(".")).ThenBy(z => z);
        }

        public void UpdateFrom(TranslationContext other)
        {
            bool oldAdd = AddNew;
            AddNew = true;
            foreach (KeyValuePair<string, string> kvp in other.Translation)
                GetTranslatedText(kvp.Key, kvp.Value);
            AddNew = oldAdd;
        }

        public void RemoveKeys(TranslationContext other)
        {
            foreach (KeyValuePair<string, string> kvp in other.Translation)
                Translation.Remove(kvp.Key);
        }
    }
}
