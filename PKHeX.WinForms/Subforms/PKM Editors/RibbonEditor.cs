using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PKHeX.Core;
using PKHeX.Drawing.Misc;

namespace PKHeX.WinForms;

public partial class RibbonEditor : Form
{
    private readonly PKM Entity;
    private readonly IReadOnlyList<RibbonInfo> riblist;

    private const string PrefixNUD = "NUD_";
    private const string PrefixLabel = "L_";
    private const string PrefixCHK = "CHK_";
    private const string PrefixPB = "PB_";

    public RibbonEditor(PKM pk)
    {
        Entity = pk;
        InitializeComponent();
        WinFormsUtil.TranslateInterface(this, Main.CurrentLanguage);
        riblist = RibbonInfo.GetRibbonInfo(pk);
        int vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
        TLP_Ribbons.Padding = FLP_Ribbons.Padding = new Padding(0, 0, vertScrollWidth, 0);

        // Updating a Control display with autosized elements on every row addition is cpu intensive. Disable layout updates while populating.
        TLP_Ribbons.SuspendLayout();
        FLP_Ribbons.Scroll += WinFormsUtil.PanelScroll;
        TLP_Ribbons.Scroll += WinFormsUtil.PanelScroll;
        PopulateRibbons();
        TLP_Ribbons.ResumeLayout();

        InitializeAffixed(pk);
    }

    private void InitializeAffixed(PKM pk)
    {
        if (pk is not IRibbonSetAffixed affixed)
        {
            CB_Affixed.Visible = false;
            return;
        }

        const int count = (int)RibbonIndex.MAX_COUNT;
        static string GetRibbonPropertyName(int z) => RibbonStrings.GetName($"Ribbon{(RibbonIndex)z}");
        static ComboItem GetComboItem(int ribbonIndex) => new(GetRibbonPropertyName(ribbonIndex), ribbonIndex);

        var none = GameInfo.GetStrings(Main.CurrentLanguage).Move[0];
        var ds = new List<ComboItem>(1 + count) { new(none, -1) };
        var list = Enumerable.Range(0, count).Select(GetComboItem).OrderBy(z => z.Text);
        ds.AddRange(list);

        CB_Affixed.InitializeBinding();
        CB_Affixed.DataSource = ds;
        CB_Affixed.SelectedValue = (int)affixed.AffixedRibbon;
    }

    private void B_Cancel_Click(object sender, EventArgs e) => Close();

    private void B_Save_Click(object sender, EventArgs e)
    {
        Save();
        Close();
    }

    private void PopulateRibbons()
    {
        TLP_Ribbons.ColumnCount = 2;
        TLP_Ribbons.RowCount = 0;

        // Add Ribbons
        foreach (var rib in riblist)
            AddRibbonSprite(rib);
        foreach (var rib in riblist.OrderBy(z => RibbonStrings.GetName(z.Name)))
            AddRibbonChoice(rib);

        // Force auto-size
        foreach (var style in TLP_Ribbons.RowStyles.OfType<RowStyle>())
            style.SizeType = SizeType.AutoSize;
        foreach (var style in TLP_Ribbons.ColumnStyles.OfType<ColumnStyle>())
            style.SizeType = SizeType.AutoSize;
    }

    private void AddRibbonSprite(RibbonInfo rib)
    {
        var name = rib.Name;
        var pb = new PictureBox { AutoSize = false, Size = new Size(40,40), BackgroundImageLayout = ImageLayout.Center, Visible = false, Name = PrefixPB + name };
        var img = RibbonSpriteUtil.GetRibbonSprite(name);
        pb.BackgroundImage = img;

        var display = RibbonStrings.GetName(name);
        pb.MouseEnter += (s, e) => tipName.SetToolTip(pb, display);
        FLP_Ribbons.Controls.Add(pb);
    }

    private void AddRibbonChoice(RibbonInfo rib)
    {
        // Get row we add to
        int row = TLP_Ribbons.RowCount;
        TLP_Ribbons.RowCount++;

        var label = new Label
        {
            Anchor = AnchorStyles.Left,
            Name = PrefixLabel + rib.Name,
            Text = RibbonStrings.GetName(rib.Name),
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            AutoSize = true,
        };
        TLP_Ribbons.Controls.Add(label, 1, row);

        if (rib.Type is RibbonValueType.Byte) // numeric count ribbon
            AddRibbonNumericUpDown(rib, row);
        else // boolean ribbon
            AddRibbonCheckBox(rib, row, label);
    }

    private void AddRibbonNumericUpDown(RibbonInfo rib, int row)
    {
        var nud = new NumericUpDown
        {
            Anchor = AnchorStyles.Right,
            Name = PrefixNUD + rib.Name,
            Minimum = 0,
            Width = 35,
            Increment = 1,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
            Maximum = rib.MaxCount,
        };

        nud.ValueChanged += (sender, e) =>
        {
            var pb = FLP_Ribbons.Controls[PrefixPB + rib.Name];
            pb.Visible = (rib.RibbonCount = (byte)nud.Value) != 0;
            pb.BackgroundImage = RibbonSpriteUtil.GetRibbonSprite(rib.Name, (int)nud.Maximum, (int)nud.Value);
        };
        nud.Value = rib.RibbonCount > nud.Maximum ? nud.Maximum : rib.RibbonCount;
        TLP_Ribbons.Controls.Add(nud, 0, row);
    }

    private void AddRibbonCheckBox(RibbonInfo rib, int row, Control label)
    {
        var chk = new CheckBox
        {
            Anchor = AnchorStyles.Right,
            Name = PrefixCHK + rib.Name,
            AutoSize = true,
            Padding = Padding.Empty,
            Margin = Padding.Empty,
        };
        chk.CheckedChanged += (sender, e) =>
        {
            rib.HasRibbon = chk.Checked;
            FLP_Ribbons.Controls[PrefixPB + rib.Name].Visible = rib.HasRibbon;
        };
        chk.Checked = rib.HasRibbon;
        TLP_Ribbons.Controls.Add(chk, 0, row);

        label.Click += (s, e) => chk.Checked ^= true;
    }

    private void Save()
    {
        foreach (var rib in riblist)
            ReflectUtil.SetValue(Entity, rib.Name, rib.Type is RibbonValueType.Boolean ? rib.HasRibbon : rib.RibbonCount);

        if (Entity is IRibbonSetAffixed affixed)
            affixed.AffixedRibbon = (sbyte)WinFormsUtil.GetIndex(CB_Affixed);
    }

    private void B_All_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            RibbonApplicator.RemoveAllValidRibbons(Entity);
            RibbonApplicator.SetAllValidRibbons(Entity);
            Close();
            return;
        }

        foreach (var c in TLP_Ribbons.Controls.OfType<CheckBox>())
            c.Checked = true;
        foreach (var n in TLP_Ribbons.Controls.OfType<NumericUpDown>())
            n.Value = n.Maximum;
    }

    private void B_None_Click(object sender, EventArgs e)
    {
        if (ModifierKeys == Keys.Shift)
        {
            RibbonApplicator.RemoveAllValidRibbons(Entity);
            if (Entity is IRibbonSetAffixed affixed)
                affixed.AffixedRibbon = -1;
            Close();
            return;
        }

        CB_Affixed.SelectedValue = -1;
        foreach (var c in TLP_Ribbons.Controls.OfType<CheckBox>())
            c.Checked = false;
        foreach (var n in TLP_Ribbons.Controls.OfType<NumericUpDown>())
            n.Value = 0;
    }
}
