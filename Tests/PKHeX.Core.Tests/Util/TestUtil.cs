using System;
using System.IO;

namespace PKHeX.Tests
{
    internal static class TestUtil
    {
        public static string GetRepoPath()
        {
            string? folder = Directory.GetCurrentDirectory();
            while (!folder.EndsWith(nameof(Tests)))
            {
                DirectoryInfo? dir = Directory.GetParent(folder);
                if (dir == null)
                    throw new ArgumentNullException(nameof(dir));
                folder = dir.FullName;
            }
            return folder;
        }
    }
}
