using System;
using System.Collections.Generic;
using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    public static class MysteryGiftVerifier
    {
        private static readonly Dictionary<int, MysteryGiftRestriction>?[] RestrictionSet = Get();

        private static Dictionary<int, MysteryGiftRestriction>?[] Get()
        {
            Dictionary<int, MysteryGiftRestriction>?[]? s = new Dictionary<int, MysteryGiftRestriction>?[PKX.Generation + 1];
            for (int i = 3; i < s.Length; i++)
                s[i] = GetRestriction(i);
            return s;
        }

        private static string RestrictionSetName(int i) => $"mgrestrict{i}.pkl";

        private static Dictionary<int, MysteryGiftRestriction> GetRestriction(int generation)
        {
            string? resource = RestrictionSetName(generation);
            byte[]? data = Util.GetBinaryResource(resource);
            Dictionary<int, MysteryGiftRestriction>? dict = new Dictionary<int, MysteryGiftRestriction>();
            for (int i = 0; i < data.Length; i += 8)
            {
                int hash = BitConverter.ToInt32(data, i + 0);
                int restrict = BitConverter.ToInt32(data, i + 4);
                dict.Add(hash, (MysteryGiftRestriction)restrict);
            }
            return dict;
        }

        public static CheckResult VerifyGift(PKM pk, MysteryGift g)
        {
            bool restricted = TryGetRestriction(g, out MysteryGiftRestriction val);
            if (!restricted)
                return new CheckResult(CheckIdentifier.GameOrigin);

            int ver = (int)val >> 16;
            if (ver != 0 && !CanVersionReceiveGift(g.Generation, ver, pk.Version))
                return new CheckResult(Severity.Invalid, LEncGiftVersionNotDistributed, CheckIdentifier.GameOrigin);

            MysteryGiftRestriction lang = val & MysteryGiftRestriction.LangRestrict;
            if (lang != 0 && !lang.HasFlagFast((MysteryGiftRestriction) (1 << pk.Language)))
                return new CheckResult(Severity.Invalid, string.Format(LOTLanguage, lang.GetSuggestedLanguage(), pk.Language), CheckIdentifier.GameOrigin);

            if (pk is IRegionOrigin tr)
            {
                MysteryGiftRestriction region = val & MysteryGiftRestriction.RegionRestrict;
                if (region != 0 && !region.HasFlagFast((MysteryGiftRestriction)((int)MysteryGiftRestriction.RegionBase << tr.ConsoleRegion)))
                    return new CheckResult(Severity.Invalid, LGeoHardwareRange, CheckIdentifier.GameOrigin);
            }

            return new CheckResult(CheckIdentifier.GameOrigin);
        }

        private static bool TryGetRestriction(MysteryGift g, out MysteryGiftRestriction val)
        {
            Dictionary<int, MysteryGiftRestriction>? restrict = RestrictionSet[g.Generation];
            if (restrict != null)
                return restrict.TryGetValue(g.GetHashCode(), out val);
            val = MysteryGiftRestriction.None;
            return false;
        }

        public static bool IsValidChangedOTName(PKM pk, MysteryGift g)
        {
            bool restricted = TryGetRestriction(g, out MysteryGiftRestriction val);
            if (!restricted)
                return false; // no data
            if (!val.HasFlagFast(MysteryGiftRestriction.OTReplacedOnTrade))
                return false;
            return CurrentOTMatchesReplaced(g.Generation, pk.OT_Name);
        }

        private static bool CanVersionReceiveGift(int generation, int version4bit, int version)
        {
            return generation switch
            {
                _ => false
            };
        }

        private static bool CurrentOTMatchesReplaced(int format, string pkOtName)
        {
            if (format <= 4 && IsMatchName(pkOtName, 4))
                return true;
            if (format <= 5 && IsMatchName(pkOtName, 5))
                return true;
            if (format <= 6 && IsMatchName(pkOtName, 6))
                return true;
            if (format <= 7 && IsMatchName(pkOtName, 7))
                return true;
            return false;
        }

        private static bool IsMatchName(string pkOtName, int generation)
        {
            return generation switch
            {
                _ => false
            };
        }
    }
}
