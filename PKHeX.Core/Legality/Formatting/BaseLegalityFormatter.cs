using System;
using System.Collections.Generic;
using System.Linq;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Default formatter for Legality Result displays.
    /// </summary>
    public class BaseLegalityFormatter : ILegalityFormatter
    {
        public string GetReport(LegalityAnalysis l)
        {
            if (l.Valid)
                return L_ALegal;
            if (!l.Parsed)
                return L_AnalysisUnavailable;

            List<string>? lines = GetLegalityReportLines(l);
            return string.Join(Environment.NewLine, lines);
        }

        public string GetReportVerbose(LegalityAnalysis l)
        {
            if (!l.Parsed)
                return L_AnalysisUnavailable;

            IReadOnlyList<string>? lines = GetVerboseLegalityReportLines(l);
            return string.Join(Environment.NewLine, lines);
        }

        private static List<string> GetLegalityReportLines(LegalityAnalysis l)
        {
            List<string>? lines = new List<string>();
            LegalInfo? info = l.Info;
            CheckMoveResult[]? vMoves = info.Moves;
            PKM? pkm = l.pkm;
            for (int i = 0; i < 4; i++)
            {
                if (!vMoves[i].Valid)
                    lines.Add(vMoves[i].Format(L_F0_M_1_2, i + 1));
            }

            if (pkm.Format >= 6)
            {
                CheckResult[]? vRelearn = info.Relearn;
                for (int i = 0; i < 4; i++)
                {
                    if (!vRelearn[i].Valid)
                        lines.Add(vRelearn[i].Format(L_F0_RM_1_2, i + 1));
                }
            }

            // Build result string...
            IEnumerable<CheckResult>? outputLines = l.Results.Where(chk => !chk.Valid);
            lines.AddRange(outputLines.Select(chk => chk.Format(L_F0_1)));
            return lines;
        }

        private IReadOnlyList<string> GetVerboseLegalityReportLines(LegalityAnalysis l)
        {
            List<string>? lines = l.Valid ? new List<string> {L_ALegal} : GetLegalityReportLines(l);
            LegalInfo? info = l.Info;
            PKM? pkm = l.pkm;
            const string separator = "===";
            lines.Add(separator);
            lines.Add(string.Empty);
            int initialCount = lines.Count;

            int format = pkm.Format;
            LegalityFormatting.AddValidMoves(info, lines, format);

            if (format >= 6)
                LegalityFormatting.AddValidMovesRelearn(info, lines);

            if (lines.Count != initialCount) // move info added, break for next section
                lines.Add(string.Empty);

            LegalityFormatting.AddValidSecondaryChecks(l.Results, lines);

            lines.Add(separator);
            lines.Add(string.Empty);
            LegalityFormatting.AddEncounterInfo(l, lines);

            LegalityFormatting.AddInvalidMatchesIfAny(l, info, lines);

            return lines;
        }
    }
}
