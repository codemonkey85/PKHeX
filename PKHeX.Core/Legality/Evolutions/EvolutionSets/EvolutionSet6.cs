using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 6 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet6
    {
        internal static readonly HashSet<int> EvosWithArg = new() {6, 8, 16, 17, 18, 19, 20, 21, 22, 29};
        private const int SIZE = 6;

        private static EvolutionMethod[] GetMethods(byte[] data)
        {
            EvolutionMethod[]? evos = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                ushort method = BitConverter.ToUInt16(data, i + 0);
                ushort arg = BitConverter.ToUInt16(data, i + 2);
                ushort species = BitConverter.ToUInt16(data, i + 4);

                // Argument is used by both Level argument and Item/Move/etc. Clear if appropriate.
                int lvl = EvosWithArg.Contains(method) ? 0 : arg;

                evos[i/SIZE] = new EvolutionMethod(method, species, argument: arg, level: lvl);
            }
            return evos;
        }

        public static IReadOnlyList<EvolutionMethod[]> GetArray(IReadOnlyList<byte[]> data)
        {
            EvolutionMethod[][]? evos = new EvolutionMethod[data.Count][];
            for (int i = 0; i < evos.Length; i++)
                evos[i] = GetMethods(data[i]);
            return evos;
        }
    }
}