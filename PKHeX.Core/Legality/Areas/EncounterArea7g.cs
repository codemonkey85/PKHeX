using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <inheritdoc cref="EncounterArea" />
    /// <summary>
    /// <see cref="GameVersion.GO"/> encounter area for <see cref="GameVersion.GG"/>
    /// </summary>
    public sealed record EncounterArea7g : EncounterArea, ISpeciesForm
    {
        /// <summary> Species for the area </summary>
        /// <remarks> Due to how the encounter data is packaged by PKHeX, each species-form is grouped together. </remarks>
        public int Species { get; }
        /// <summary> Form of the Species </summary>
        public int Form { get; }

        private EncounterArea7g(int species, int form) : base(GameVersion.GO)
        {
            Species = species;
            Form = form;
            Location = Locations.GO7;
        }

        internal static EncounterArea7g[] GetArea(byte[][] data)
        {
            EncounterArea7g[]? areas = new EncounterArea7g[data.Length];
            for (int i = 0; i < areas.Length; i++)
                areas[i] = GetArea(data[i]);
            return areas;
        }

        private const int entrySize = (2 * sizeof(int)) + 2;

        private static EncounterArea7g GetArea(byte[] data)
        {
            ushort sf = BitConverter.ToUInt16(data, 0);
            int species = sf & 0x7FF;
            int form = sf >> 11;

            EncounterSlot7GO[]? result = new EncounterSlot7GO[(data.Length - 2) / entrySize];
            EncounterArea7g? area = new EncounterArea7g(species, form) { Slots = result };
            for (int i = 0; i < result.Length; i++)
            {
                int offset = (i * entrySize) + 2;
                result[i] = ReadSlot(data, offset, area, species, form);
            }

            return area;
        }

        private static EncounterSlot7GO ReadSlot(byte[] data, int offset, EncounterArea7g area, int species, int form)
        {
            int start = BitConverter.ToInt32(data, offset);
            int end = BitConverter.ToInt32(data, offset + 4);
            byte sg = data[offset + 8];
            Shiny shiny = (Shiny)(sg & 0x3F);
            Gender gender = (Gender)(sg >> 6);
            PogoType type = (PogoType)data[offset + 9];
            return new EncounterSlot7GO(area, species, form, start, end, shiny, gender, type);
        }

        public override IEnumerable<EncounterSlot> GetMatchingSlots(PKM pkm, IReadOnlyList<EvoCriteria> chain)
        {
            // Find the first chain that has slots defined.
            // Since it is possible to evolve before transferring, we only need the highest evolution species possible.
            // PoGoEncTool has already extrapolated the evolutions to separate encounters!
            EvoCriteria? sf = chain.FirstOrDefault(z => z.Species == Species && z.Form == Form);
            if (sf == null)
                yield break;

            int stamp = EncounterSlotGO.GetTimeStamp(pkm.Met_Year + 2000, pkm.Met_Month, pkm.Met_Day);
            int met = Math.Max(sf.MinLevel, pkm.Met_Level);
            EncounterSlot? deferredIV = null;

            foreach (EncounterSlot? s in Slots)
            {
                EncounterSlot7GO? slot = (EncounterSlot7GO)s;
                if (!slot.IsLevelWithinRange(met))
                    continue;
                //if (!slot.IsBallValid(ball)) -- can have any of the in-game balls due to re-capture
                //    continue;
                if (!slot.Shiny.IsValid(pkm))
                    continue;
                //if (slot.Gender != Gender.Random && (int) slot.Gender != pkm.Gender)
                //    continue;
                if (!slot.IsWithinStartEnd(stamp))
                    continue;

                if (!slot.GetIVsValid(pkm))
                {
                    deferredIV ??= slot;
                    continue;
                }

                yield return slot;
            }

            if (deferredIV != null)
                yield return deferredIV;
        }
    }
}
