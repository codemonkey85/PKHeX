using System;
using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    public static class MoveBreed
    {
        public static bool Process(int generation, int species, int form, GameVersion version, int[] moves)
        {
            _  = Process(generation, species, form, version, moves, out bool valid);
            return valid;
        }

        public static object Process(int generation, int species, int form, GameVersion version, int[] moves, out bool valid) => generation switch
        {
            2 => MoveBreed2.Validate(species, version, moves, out valid),
            3 => MoveBreed3.Validate(species, version, moves, out valid),
            4 => MoveBreed4.Validate(species, version, moves, out valid),
            5 => MoveBreed5.Validate(species, version, moves, out valid),
            _ => MoveBreed6.Validate(generation, species, form, version, moves, out valid),
        };

        public static int[] GetExpectedMoves(int[] moves, IEncounterTemplate enc)
        {
            object? parse = Process(enc.Generation, enc.Species, enc.Form, enc.Version, moves, out bool valid);
            if (valid)
                return moves;
            return GetExpectedMoves(enc.Generation, enc.Species, enc.Form, enc.Version, moves, parse);
        }

        public static int[] GetExpectedMoves(int generation, int species, int form, GameVersion version, int[] moves, object parse)
        {
            // Try rearranging the order of the moves.
            // Build an info table
            byte[]? x = (byte[])parse;
            MoveOrder[]? details = new MoveOrder[moves.Length];
            for (byte i = 0; i < x.Length; i++)
                details[i] = new MoveOrder((ushort) moves[i], x[i]);

            // Kick empty slots to the end, then order by source priority.
            IOrderedEnumerable<MoveOrder> expect = generation != 2
                ? details.OrderBy(z => z.Move == 0).ThenBy(z => z.Source)
                : details.OrderBy(z => z.Move == 0).ThenBy(z => z.Source != (byte) EggSource2.Base);

            // Reorder the moves.
            int[]? reorder1 = new int[moves.Length];
            List<MoveOrder>? exp = expect.ToList();
            for (int i = 0; i < moves.Length; i++)
                reorder1[i] = exp[i].Move;

            // Check if that worked...
            _ = Process(generation, species, form, version, reorder1, out bool valid);
            if (valid)
                return reorder1;

            // Well, that didn't work; probably because the moves aren't valid. Let's remove all the base moves, and get a fresh set.
            int[]? reorder2 = reorder1; // reuse instead of reallocate
            Learnset[]? learn = GameData.GetLearnsets(version);
            PersonalTable? table = GameData.GetPersonal(version);
            int index = table.GetFormIndex(species, form);
            Learnset? learnset = learn[index];
            ReadOnlySpan<int> baseMoves = learnset.GetBaseEggMoves(generation >= 4 ? 1 : 5);

            RebuildMoves(baseMoves, exp, reorder2);

            // Check if that worked...
            _ = Process(generation, species, form, version, reorder2, out valid);
            if (valid)
                return reorder2;

            // Total failure; just return the base moves.
            baseMoves.CopyTo(reorder2);
            for (int i = baseMoves.Length; i < reorder2.Length; i++)
                reorder2[i] = 0;
            return reorder2;
        }

        private static void RebuildMoves(ReadOnlySpan<int> baseMoves, List<MoveOrder> exp, int[] result)
        {
            List<int>? notBase = new List<int>();
            foreach (MoveOrder m in exp)
            {
                if (m.Source == 0)
                    continue; // invalid
                int move = m.Move;
                if (baseMoves.IndexOf(move) == -1)
                    notBase.Add(move);
            }

            int baseCount = 4 - notBase.Count;
            if (baseCount > baseMoves.Length)
                baseCount = baseMoves.Length;
            int ctr = 0;
            for (; ctr < baseCount; ctr++)
                result[ctr] = baseMoves[baseMoves.Length - baseCount + ctr];
            foreach (int m in notBase)
                result[ctr++] = m;

            for (int i = ctr; i < result.Length; i++)
                result[i] = 0;
        }

        private readonly struct MoveOrder
        {
            public readonly ushort Move;
            public readonly byte Source;

            public MoveOrder(ushort move, byte source)
            {
                Move = move;
                Source = source;
            }
        }
    }
}
