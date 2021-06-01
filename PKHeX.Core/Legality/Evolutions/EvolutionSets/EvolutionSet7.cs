using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    /// <summary>
    /// Generation 7 Evolution Branch Entries
    /// </summary>
    public static class EvolutionSet7
    {
        private const int SIZE = 8;

        private static EvolutionMethod[] GetMethods(byte[] data)
        {
            EvolutionMethod[]? evos = new EvolutionMethod[data.Length / SIZE];
            for (int i = 0; i < data.Length; i += SIZE)
            {
                ushort method = BitConverter.ToUInt16(data, i + 0);
                ushort arg = BitConverter.ToUInt16(data, i + 2);
                ushort species = BitConverter.ToUInt16(data, i + 4);
                sbyte form = (sbyte) data[i + 6];
                byte level = data[i + 7];
                evos[i / SIZE] = new EvolutionMethod(method, species, argument: arg, level: level, form: form);
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