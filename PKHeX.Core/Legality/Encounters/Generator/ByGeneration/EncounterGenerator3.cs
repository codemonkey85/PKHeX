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
    public static class EncounterGenerator3
    {
        public static IEnumerable<IEncounterable> GetEncounters(PKM pkm, LegalInfo info)
        {
            if (pkm.Version == (int) GameVersion.CXD)
                return GetEncounters3CXD(pkm, info);
            return GetEncounters3(pkm, info);
        }

        private static IEnumerable<IEncounterable> GetEncounters3(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            IEncounterable? Partial = null;

            foreach (IEncounterable? z in GenerateRawEncounters3(pkm, info))
            {
                if (info.PIDIV.Type.IsCompatible3(z, pkm))
                    yield return z;
                else
                    Partial ??= z;
            }
            if (Partial == null)
                yield break;

            info.PIDIVMatches = false;
            yield return Partial;
        }

        private static IEnumerable<IEncounterable> GetEncounters3CXD(PKM pkm, LegalInfo info)
        {
            info.PIDIV = MethodFinder.Analyze(pkm);
            IEncounterable? Partial = null;
            foreach (IEncounterable? z in GenerateRawEncounters3CXD(pkm))
            {
                if (z is EncounterSlot3PokeSpot w)
                {
                    PIDIV? seeds = MethodFinder.GetPokeSpotSeeds(pkm, w.SlotNumber).FirstOrDefault();
                    info.PIDIV = seeds ?? info.PIDIV;
                }
                else if (z is EncounterStaticShadow s)
                {
                    bool valid = GetIsShadowLockValid(pkm, info, s);
                    if (!valid)
                    {
                        Partial ??= s;
                        continue;
                    }
                }

                if (info.PIDIV.Type.IsCompatible3(z, pkm))
                    yield return z;
                else
                    Partial ??= z;
            }
            if (Partial == null)
                yield break;

            info.PIDIVMatches = false;
            yield return Partial;
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters3CXD(PKM pkm)
        {
            IReadOnlyList<EvoCriteria>? chain = EncounterOrigin.GetOriginChain(pkm);

            // Mystery Gifts
            foreach (MysteryGift? z in GetValidGifts(pkm, chain))
            {
                // Don't bother deferring matches.
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match != PartialMatch)
                    yield return z;
            }

            // Trades
            foreach (EncounterTrade? z in GetValidEncounterTrades(pkm, chain))
            {
                // Don't bother deferring matches.
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match != PartialMatch)
                    yield return z;
            }

            IEncounterable? partial = null;

            // Static Encounter
            foreach (EncounterStatic? z in GetValidStaticEncounter(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                    partial ??= z;
                else
                    yield return z;
            }

            // Encounter Slots
            foreach (EncounterSlot? z in GetValidWildEncounters34(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                {
                    partial ??= z;
                    continue;
                }
                yield return z;
            }

            if (partial is not null)
                yield return partial;
        }

        private static IEnumerable<IEncounterable> GenerateRawEncounters3(PKM pkm, LegalInfo info)
        {
            IReadOnlyList<EvoCriteria>? chain = EncounterOrigin.GetOriginChain(pkm);

            // Mystery Gifts
            foreach (MysteryGift? z in GetValidGifts(pkm, chain))
            {
                // Don't bother deferring matches.
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match != PartialMatch)
                    yield return z;
            }

            // Trades
            foreach (EncounterTrade? z in GetValidEncounterTrades(pkm, chain))
            {
                // Don't bother deferring matches.
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match != PartialMatch)
                    yield return z;
            }

            IEncounterable? deferred = null;
            IEncounterable? partial = null;

            // Static Encounter
            // Defer everything if Safari Ball
            bool safari = pkm.Ball == 0x05; // never static encounters
            if (!safari)
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

            // Encounter Slots
            List<Frame>? slots = FrameFinder.GetFrames(info.PIDIV, pkm).ToList();
            foreach (EncounterSlot? z in GetValidWildEncounters34(pkm, chain))
            {
                EncounterMatchRating match = z.GetMatchRating(pkm);
                if (match == PartialMatch)
                {
                    partial ??= z;
                    continue;
                }

                Frame? frame = slots.Find(s => s.IsSlotCompatibile((EncounterSlot3)z, pkm));
                if (frame == null)
                {
                    deferred ??= z;
                    continue;
                }
                yield return z;
            }

            info.FrameMatches = false;
            if (deferred is EncounterSlot3 x)
                yield return x;

            if (pkm.Version != (int)GameVersion.CXD) // no eggs in C/XD
            {
                foreach (EncounterEgg? z in GenerateEggs(pkm, 3))
                    yield return z;
            }

            if (partial is EncounterSlot3 y)
            {
                Frame? frame = slots.Find(s => s.IsSlotCompatibile(y, pkm));
                info.FrameMatches = frame != null;
                yield return y;
            }

            // do static encounters if they were deferred to end, spit out any possible encounters for invalid pkm
            if (!safari)
                yield break;

            partial = null;

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

        private static bool GetIsShadowLockValid(PKM pkm, LegalInfo info, EncounterStaticShadow s)
        {
            if (s.IVs.Count == 0) // not E-Reader
                return LockFinder.IsAllShadowLockValid(s, info.PIDIV, pkm);

            // E-Reader have fixed IVs, and aren't recognized as CXD (no PID-IV correlation).
            IEnumerable<PIDIV>? possible = MethodFinder.GetColoEReaderMatches(pkm.EncryptionConstant);
            foreach (PIDIV? poss in possible)
            {
                if (!LockFinder.IsAllShadowLockValid(s, poss, pkm))
                    continue;
                info.PIDIV = poss;
                return true;
            }

            return false;
        }
    }
}
