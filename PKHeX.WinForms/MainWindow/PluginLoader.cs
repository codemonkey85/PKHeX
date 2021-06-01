using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PKHeX.WinForms
{
    public static class PluginLoader
    {
        public static IEnumerable<T> LoadPlugins<T>(string pluginPath) where T : class
        {
            IEnumerable<string>? dllFileNames = !Directory.Exists(pluginPath)
                ? Enumerable.Empty<string>()
                : Directory.EnumerateFiles(pluginPath, "*.dll", SearchOption.AllDirectories);
            IEnumerable<Assembly>? assemblies = GetAssemblies(dllFileNames);
            IEnumerable<Type>? pluginTypes = GetPluginsOfType<T>(assemblies);
            return LoadPlugins<T>(pluginTypes);
        }

        private static IEnumerable<T> LoadPlugins<T>(IEnumerable<Type> pluginTypes) where T : class
        {
            foreach (Type? t in pluginTypes)
            {
                T? activate = (T?) Activator.CreateInstance(t);
                if (activate != null)
                    yield return activate;
            }
        }

        private static IEnumerable<Assembly> GetAssemblies(IEnumerable<string> dllFileNames)
        {
#if UNSAFEDLL
            var assemblies = dllFileNames.Select(Assembly.UnsafeLoadFrom);
#else
            IEnumerable<Assembly>? assemblies = dllFileNames.Select(Assembly.LoadFrom);
            #endif
            #if MERGED
            assemblies = assemblies.Concat(new[] { Assembly.GetExecutingAssembly() }); // load merged too
            #endif
            return assemblies;
        }

        private static IEnumerable<Type> GetPluginsOfType<T>(IEnumerable<Assembly> assemblies)
        {
            Type? pluginType = typeof(T);
            return assemblies.SelectMany(z => GetPluginTypes(z, pluginType));
        }

        private static IEnumerable<Type> GetPluginTypes(Assembly z, Type pluginType)
        {
            try
            {
                Type[]? types = z.GetTypes();
                return types.Where(type => IsTypePlugin(type, pluginType));
            }
#pragma warning disable CA1031 // Do not catch general exception types
            // User plugins can be out of date, with mismatching API surfaces.
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                System.Diagnostics.Debug.WriteLine($"Unable to load plugin [{pluginType.Name}]: {z.FullName}", ex.Message);
                return Enumerable.Empty<Type>();
            }
        }

        private static bool IsTypePlugin(Type type, Type pluginType)
        {
            if (type.IsInterface || type.IsAbstract)
                return false;
            string? name = pluginType.FullName;
            if (name == null)
                return false;
            if (type.GetInterface(name) == null)
                return false;
            return true;
        }
    }
}
