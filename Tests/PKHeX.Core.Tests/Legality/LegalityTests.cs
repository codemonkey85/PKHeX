using System;
using FluentAssertions;
using PKHeX.Core;
using System.IO;
using System.Linq;
using Xunit;

namespace PKHeX.Tests.Legality
{
    public class LegalityTest
    {
        static LegalityTest()
        {
            if (EncounterEvent.Initialized)
                return;

            RibbonStrings.ResetDictionary(GameInfo.Strings.ribbons);
            EncounterEvent.RefreshMGDB();
        }

        [Theory]
        [InlineData("censor")]
        [InlineData("buttnugget")]
        [InlineData("18넘")]
        public void CensorsBadWords(string badword)
        {
            WordFilter.IsFiltered(badword, out _).Should().BeTrue("the word should have been identified as a bad word");
        }

        [Fact]
        public void TestFilesPassOrFailLegalityChecks()
        {
            string? folder = TestUtil.GetRepoPath();
            folder = Path.Combine(folder, "Legality");
            ParseSettings.AllowGBCartEra = true;
            VerifyAll(folder, "Legal", true);
            VerifyAll(folder, "Illegal", false);
        }

        // ReSharper disable once UnusedParameter.Local
        private static void VerifyAll(string folder, string name, bool isValid)
        {
            string? path = Path.Combine(folder, name);
            Directory.Exists(path).Should().BeTrue($"the specified test directory at '{path}' should exist");
            System.Collections.Generic.IEnumerable<string>? files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            int ctr = 0;
            foreach (string? file in files)
            {
                FileInfo? fi = new FileInfo(file);
                fi.Should().NotBeNull($"the test file '{file}' should be a valid file");
                PKX.IsPKM(fi.Length).Should().BeTrue($"the test file '{file}' should have a valid file length");

                byte[]? data = File.ReadAllBytes(file);
                int format = PKX.GetPKMFormatFromExtension(file[^1], -1);
                format.Should().BeLessOrEqualTo(PKX.Generation, "filename is expected to have a valid extension");

                string? dn = fi.DirectoryName ?? string.Empty;
                ParseSettings.AllowGBCartEra = dn.Contains("GBCartEra");
                ParseSettings.AllowGen1Tradeback = dn.Contains("1 Tradeback");
                Core.PKM? pkm = PKMConverter.GetPKMfromBytes(data, prefer: format);
                pkm.Should().NotBeNull($"the PKM '{new FileInfo(file).Name}' should have been loaded");
                if (pkm == null)
                    continue;
                LegalityAnalysis? legality = new LegalityAnalysis(pkm);
                if (legality.Valid == isValid)
                {
                    ctr++;
                    continue;
                }

                string? fn = Path.Combine(dn, fi.Name);
                if (isValid)
                {
                    LegalInfo? info = legality.Info;
                    System.Collections.Generic.IEnumerable<CheckResult>? result = legality.Results.Concat(info.Moves).Concat(info.Relearn);
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    System.Collections.Generic.IEnumerable<CheckResult>? invalid = result.Where(z => z?.Valid == false);
                    string? msg = string.Join(Environment.NewLine, invalid.Select(z => z.Comment));
                    legality.Valid.Should().BeTrue($"because the file '{fn}' should be Valid, but found:{Environment.NewLine}{msg}");
                }
                else
                {
                    legality.Valid.Should().BeFalse($"because the file '{fn}' should be invalid, but found Valid.");
                }
            }
            ctr.Should().BeGreaterThan(0);
        }
    }
}
