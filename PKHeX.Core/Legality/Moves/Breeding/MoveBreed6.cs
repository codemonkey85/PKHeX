using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static PKHeX.Core.EggSource6;

namespace PKHeX.Core
{
    /// <summary>
    /// Inheritance logic for Generations 6+.
    /// </summary>
    /// <remarks>Refer to <see cref="EggSource6"/> for inheritance ordering.</remarks>
    public static class MoveBreed6
    {
        private const int level = 1;

        public static EggSource6[] Validate(int generation, int species, int form, GameVersion version, int[] moves, out bool valid)
        {
            int count = Array.IndexOf(moves, 0);
            if (count == 0)
            {
                valid = false; // empty moveset
                return Array.Empty<EggSource6>();
            }
            if (count == -1)
                count = moves.Length;

            Learnset[]? learn = GameData.GetLearnsets(version);
            PersonalTable? table = GameData.GetPersonal(version);
            int index = table.GetFormIndex(species, form);
            Learnset? learnset = learn[index];
            int[]? egg = MoveEgg.GetEggMoves(generation, species, form, version);

            BreedInfo<EggSource6> value = new BreedInfo<EggSource6>(count, learnset, moves, level);
            if (moves[count - 1] is (int)Move.VoltTackle)
                value.Actual[--count] = VoltTackle;

            if (count == 0)
            {
                valid = VerifyBaseMoves(value);
            }
            else
            {
                bool inherit = Breeding.GetCanInheritMoves(species);
                MarkMovesForOrigin(value, egg, count, inherit);
                valid = RecurseMovesForOrigin(value, count - 1);
            }

            if (!valid)
                CleanResult(value.Actual, value.Possible);
            return value.Actual;
        }

        private static void CleanResult(EggSource6[] valueActual, byte[] valuePossible)
        {
            for (int i = 0; i < valueActual.Length; i++)
            {
                if (valueActual[i] != 0)
                    continue;
                byte poss = valuePossible[i];
                if (poss == 0)
                    continue;

                for (int j = 0; j < (int)Max; j++)
                {
                    if ((poss & (1 << j)) == 0)
                        continue;
                    valueActual[i] = (EggSource6)j;
                    break;
                }
            }
        }

        private static bool RecurseMovesForOrigin(in BreedInfo<EggSource6> info, int start, EggSource6 type = Max - 1)
        {
            int i = start;
            do
            {
                EggSource6 unpeel = type - 1;
                if (unpeel != 0 && RecurseMovesForOrigin(info, i, unpeel))
                    return true;

                byte permit = info.Possible[i];
                if ((permit & (1 << (int)type)) == 0)
                    return false;

                info.Actual[i] = type;
            } while (--i >= 0);

            return VerifyBaseMoves(info);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool VerifyBaseMoves(in BreedInfo<EggSource6> info)
        {
            int count = 0;
            foreach (EggSource6 x in info.Actual)
            {
                if (x == Base)
                    count++;
                else
                    break;
            }

            int[]? moves = info.Moves;
            if (count == -1)
                return moves[^1] != 0;

            ReadOnlySpan<int> baseMoves = info.Learnset.GetBaseEggMoves(info.Level);
            if (baseMoves.Length < count)
                return false;
            if (moves[^1] == 0 && count != baseMoves.Length)
                return false;

            for (int i = count - 1, b = baseMoves.Length - 1; i >= 0; i--, b--)
            {
                int move = moves[i];
                int expect = baseMoves[b];
                if (expect != move)
                    return false;
            }

            // A low-index base egg move may be nudged out, but can only reappear if sufficient non-base moves are before it.
            if (baseMoves.Length == count)
                return true;

            for (int i = count; i < info.Actual.Length; i++)
            {
                bool isBase = (info.Possible[i] & (1 << (int)Base)) != 0;
                if (!isBase)
                    continue;

                int baseIndex = baseMoves.IndexOf(info.Moves[i]);
                int min = info.Moves.Length - baseMoves.Length + baseIndex;
                if (i <= min + count)
                    return false;
            }

            return true;
        }

        private static void MarkMovesForOrigin(in BreedInfo<EggSource6> value, ICollection<int> eggMoves, int count, bool inheritLevelUp)
        {
            byte[]? possible = value.Possible;
            Learnset? learn = value.Learnset;
            ReadOnlySpan<int> baseEgg = value.Learnset.GetBaseEggMoves(value.Level);

            int[]? moves = value.Moves;
            for (int i = 0; i < count; i++)
            {
                int move = moves[i];

                if (baseEgg.IndexOf(move) != -1)
                    possible[i] |= 1 << (int)Base;

                if (inheritLevelUp && learn.GetLevelLearnMove(move) != -1)
                    possible[i] |= 1 << (int)ParentLevelUp;

                if (eggMoves.Contains(move))
                    possible[i] |= 1 << (int)ParentEgg;
            }
        }
    }
}
