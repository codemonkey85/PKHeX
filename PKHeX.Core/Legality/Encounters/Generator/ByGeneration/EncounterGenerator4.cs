using System.Collections.Generic;
using System.Linq;

using static PKHeX.Core.MysteryGiftGenerator;
using static PKHeX.Core.EncounterTradeGenerator;
using static PKHeX.Core.EncounterSlotGenerator;
using static PKHeX.Core.EncounterStaticGenerator;
using static PKHeX.Core.EncounterEggGenerator;
using static PKHeX.Core.EncounterMatchRating;

namespace PKHeX.Core
{
    internal static class EncounterGenerator4
    {
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            List<IEncounterable>? deferredPIDIV = new List<IEncounterable>();
            List<IEncounterable>? deferredEType = new List<IEncounterable>();

            foreach (IEncounterable? z in GenerateRawEncounters4(pkm, info))
            {
                if (!info.PIDIV.Type.IsCompatible4(z, pkm))
                    deferredPIDIV.Add(z);
                else if (pkm.Format <= 6 && !(z is IEncounterTypeTile t ? t.TypeEncounter.Contains(pkm.EncounterType) : pkm.EncounterType == 0))
                    deferredEType.Add(z);
                else
                    yield return z;
            }

            foreach (IEncounterable? z in deferredEType)
                yield return z;

            if (deferredPIDIV.Count == 0)
                yield break;

            info.PIDIVMatches = false;
            foreach (IEncounterable? z in deferredPIDIV)
                yield return z;
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters4(PKM pkm, LegalInfo info)
        {
            IReadOnlyList<EvoCriteria>? chain = EncounterOrigin.GetOriginChain(pkm);
            if (pkm.FatefulEncounter)
            {
                int ctr = 0;
                foreach (MysteryGift? z in GetValidGifts(pkm, chain))
                { yield return z; ++ctr; }
                if (ctr != 0) yield break;
            }
            if (pkm.WasBredEgg)
            {
                foreach (EncounterEgg? z in GenerateEggs(pkm, 4))
                    yield return z;
            }
            foreach (EncounterTrade? z in GetValidEncounterTrades(pkm, chain))
                yield return z;

            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            bool sport = pkm.Ball == (int)Ball.Sport; // never static encounters (conflict with non bcc / bcc)
            bool safari = pkm.Ball == (int)Ball.Safari; // never static encounters
            bool safariSport = safari || sport;
            if (!safariSport)
            {
                foreach (EncounterStatic? z in GetValidStaticEncounter(pkm, chain))
                {
                    EncounterMatchRating match = z.GetMatchRating(pkm);
                    if (match == PartialMatch)
                        partial ??= z;
                    else
                        yield return z;
                }
            }

            List<Frame>? slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (EncounterSlot? z in GetValidWildEncounters34(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                {
                    partial ??= z;
                    continue;
                }

                Frame? frame = slots.Find(s => s.IsSlotCompatibile((EncounterSlot4)z, pkm));
                if (frame == null)
                {
                    deferred ??= z;
                    continue;
                }
                yield return z;
            }

            info.FrameMatches = false;
            if (deferred is EncounterSlot4 x)
                yield return x;

            if (partial is EncounterSlot4 y)
            {
                Frame? frame = slots.Find(s => s.IsSlotCompatibile(y, pkm));
                info.FrameMatches = frame != null;
                yield return y;
            }

            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (!safariSport)
                yield break;

            foreach (EncounterStatic? z in GetValidStaticEncounter(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                    partial ??= z;
                else
                    yield return z;
            }

            if (partial is not null)
                yield return partial;
        }
    }
}
