using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core
{
    internal static class EncounterGenerator7
    {
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm)
        {
            IReadOnlyList<EvoCriteria>? chain = EncounterOrigin.GetOriginChain(pkm);
            return pkm.Version switch
            {
                  (int)GameVersion.GO => GetEncountersGO(pkm, chain),
                > (int)GameVersion.GO => GetEncountersGG(pkm, chain),
                _ => GetEncountersMainline(pkm, chain)
            };
        }

        internal static IEnumerable<IEncounterable> GetEncountersGO(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            int ctr = 0;
            foreach (EncounterSlot? z in GetValidWildEncounters(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; ++ctr; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
            if (ctr != 0) yield break;

            if (deferred != null)
                yield return deferred;

            if (partial != null)
                yield return partial;
        }

        private static IEnumerable<IEncounterable> GetEncountersGG(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            int ctr = 0;
            if (pkm.WasEvent)
            {
                foreach (MysteryGift? z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            foreach (EncounterStatic? z in GetValidStaticEncounter(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; ++ctr; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
            if (ctr != 0) yield break;

            foreach (EncounterSlot? z in GetValidWildEncounters(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; ++ctr; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
            if (ctr != 0) yield break;

            foreach (EncounterTrade? z in GetValidEncounterTrades(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; /*++ctr*/ break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }

            if (deferred != null)
                yield return deferred;

            if (partial != null)
                yield return partial;
        }

        private static IEnumerable<IEncounterable> GetEncountersMainline(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            int ctr = 0;
            if (pkm.WasEvent || pkm.WasEventEgg)
            {
                foreach (MysteryGift? z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasBredEgg)
            {
                foreach (EncounterEgg? z in GenerateEggs(pkm, 7))
                { yield return z; ++ctr; }
                if (ctr == 0) yield break;
            }

            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            foreach (EncounterStatic? z in GetValidStaticEncounter(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; ++ctr; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
            if (ctr != 0) yield break;

            foreach (EncounterSlot? z in GetValidWildEncounters(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; ++ctr; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
            }
            if (ctr != 0) yield break;

            foreach (EncounterTrade? z in GetValidEncounterTrades(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                switch (match)
                {
                    case Match: yield return z; break;
                    case Deferred: deferred ??= z; break;
                    case PartialMatch: partial ??= z; break;
                }
                //++ctr;
            }

            if (deferred != null)
                yield return deferred;

            if (partial != null)
                yield return partial;
        }
    }
}
