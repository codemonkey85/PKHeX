using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static PKHeX.Core.EggSource2;

namespace PKHeX.Core
{
    public static class MoveBreed2
    {
        private const int level = 5;

        public static EggSource2[] Validate(int species, GameVersion version, int[] moves, out bool valid)
        {
            int count = Array.IndexOf(moves, 0);
            if (count == 0)
            {
                valid = false; // empty moveset
                return Array.Empty<EggSource2>();
            }
            if (count == -1)
                count = moves.Length;

            Learnset[]? learn = GameData.GetLearnsets(version);
            PersonalTable? table = GameData.GetPersonal(version);
            Learnset? learnset = learn[species];
            PersonalInfo? pi = table[species];
            int[]? egg = (version == GameVersion.C ? Legal.EggMovesC : Legal.EggMovesGS)[species].Moves;

            BreedInfo<EggSource2> value = new BreedInfo<EggSource2>(count, learnset, moves, level);
            {
                bool inherit = Breeding.GetCanInheritMoves(species);
                MarkMovesForOrigin(value, egg, count, inherit, pi, version);
                valid = RecurseMovesForOrigin(value, count - 1);
            }

            if (!valid)
                CleanResult(value.Actual, value.Possible);
            return value.Actual;
        }

        private static void CleanResult(EggSource2[] valueActual, byte[] valuePossible)
        {
            for (int i = 0; i < valueActual.Length; i++)
            {
                if (valueActual[i] != 0)
                    continue;
                byte poss = valuePossible[i];
                if (poss == 0)
                    continue;

                for (int j = 0; j < (int) Max; j++)
                {
                    if ((poss & (1 << j)) == 0)
                        continue;
                    valueActual[i] = (EggSource2)j;
                    break;
                }
            }
        }

        private static bool RecurseMovesForOrigin(in BreedInfo<EggSource2> info, int start, EggSource2 type = Max)
        {
            int i = start;
            do
            {
                if (type != Base)
                {
                    if (RecurseMovesForOrigin(info, i, Base))
                        return true;
                }

                int flag = 1 << (int)Base;
                if (type != Base)
                    flag = ~flag;

                byte permit = info.Possible[i];
                if ((permit & flag) == 0)
                    return false;

                info.Actual[i] = type == Base ? Base : GetFirstType(permit);
            } while (--i >= 0);

            return VerifyBaseMoves(info);
        }

        private static EggSource2 GetFirstType(byte permit)
        {
            for (EggSource2 type = FatherEgg; type < Max; type++)
            {
                if ((permit & (1 << (int)type)) != 0)
                    return type;
            }
            throw new ArgumentOutOfRangeException(nameof(permit), permit, null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool VerifyBaseMoves(in BreedInfo<EggSource2> info)
        {
            int count = 0;
            foreach (EggSource2 x in info.Actual)
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

                int baseIndex = baseMoves.IndexOf(moves[i]);
                int min = moves.Length - baseMoves.Length + baseIndex;
                if (i <= min + count)
                    return false;
            }

            return true;
        }

        private static void MarkMovesForOrigin(in BreedInfo<EggSource2> value, ICollection<int> eggMoves, int count, bool inheritLevelUp, PersonalInfo info, GameVersion version)
        {
            byte[]? possible = value.Possible;
            Learnset? learn = value.Learnset;
            ReadOnlySpan<int> baseEgg = value.Learnset.GetBaseEggMoves(value.Level);
            bool[]? tm = info.TMHM;

            int[]? moves = value.Moves;
            for (int i = 0; i < count; i++)
            {
                int move = moves[i];

                if (baseEgg.IndexOf(move) != -1)
                    possible[i] |= 1 << (int)Base;

                if (inheritLevelUp && learn.GetLevelLearnMove(move) != -1)
                    possible[i] |= 1 << (int)ParentLevelUp;

                if (eggMoves.Contains(move))
                    possible[i] |= 1 << (int)FatherEgg;

                int tmIndex = Array.IndexOf(Legal.TMHM_GSC, move, 0, 50);
                if (tmIndex != -1 && tm[tmIndex])
                    possible[i] |= 1 << (int)FatherTM;

                if (version is GameVersion.C)
                {
                    int tutorIndex = Array.IndexOf(Legal.Tutors_GSC, move);
                    if (tutorIndex != -1 && tm[57 + tutorIndex])
                        possible[i] |= 1 << (int)Tutor;
                }
            }
        }
    }
}
