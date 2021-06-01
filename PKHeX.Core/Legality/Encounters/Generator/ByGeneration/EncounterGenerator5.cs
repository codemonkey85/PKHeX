using System.Collections.Generic;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core
{
    internal static class EncounterGenerator5
    {
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm)
        {
            int ctr = 0;

            IReadOnlyList<EvoCriteria>? chain = EncounterOrigin.GetOriginChain(pkm);
            if (pkm.WasEvent || pkm.WasEventEgg)
            {
                foreach (MysteryGift? z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }

            if (pkm.WasBredEgg)
            {
                foreach (EncounterEgg? z in GenerateEggs(pkm, 5))
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
    }
}
