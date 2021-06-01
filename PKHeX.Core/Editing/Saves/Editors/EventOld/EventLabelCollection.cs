using System;
using System.Collections.Generic;

namespace PKHeX.Core
{
    public sealed class EventLabelCollection
    {
        public readonly IReadOnlyList<NamedEventWork> Work;
        public readonly IReadOnlyList<NamedEventValue> Flag;

        public EventLabelCollection(string game, int maxFlag = int.MaxValue, int maxValue = int.MaxValue)
        {
            string[]? f = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "flags");
            string[]? c = GameLanguage.GetStrings(game, GameInfo.CurrentLanguage, "const");
            Flag = GetFlags(f, maxFlag);
            Work = GetValues(c, maxValue);
        }

        private static List<NamedEventValue> GetFlags(IReadOnlyCollection<string> strings, int maxValue)
        {
            List<NamedEventValue>? result = new List<NamedEventValue>(strings.Count);
            HashSet<int>? processed = new HashSet<int>();
            foreach (string? s in strings)
            {
                string[]? split = s.Split('\t');
                if (split.Length != 2)
                    continue;

                int index = TryParseHexDec(split[0]);
                if (index >= maxValue)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (processed.Contains(index))
                    throw new ArgumentException("Already have an entry for this!", nameof(index));

                string? desc = split[1];

                NamedEventValue? item = new NamedEventValue(desc, index);
                result.Add(item);
                processed.Add(index);
            }

            return result;
        }

        private static readonly NamedEventConst Custom = new("Custom", NamedEventConst.CustomMagicValue);
        private static readonly NamedEventConst[] Empty = {Custom};

        private static IReadOnlyList<NamedEventWork> GetValues(IReadOnlyCollection<string> strings, int maxValue)
        {
            List<NamedEventWork>? result = new List<NamedEventWork>(strings.Count);
            HashSet<int>? processed = new HashSet<int>();
            foreach (string? s in strings)
            {
                string[]? split = s.Split('\t');
                if (split.Length is not (2 or 3))
                    continue;

                int index = TryParseHexDecConst(split[0]);
                if (index >= maxValue)
                    throw new ArgumentOutOfRangeException(nameof(index));

                if (processed.Contains(index))
                    throw new ArgumentException("Already have an entry for this!", nameof(index));

                string? desc = split[1];
                IReadOnlyList<NamedEventConst>? predefined = split.Length is 2 ? Empty : GetPredefinedArray(split[2]);
                NamedEventWork? item = new NamedEventWork(desc, index, predefined);
                result.Add(item);
                processed.Add(index);
            }

            return result;
        }

        private static IReadOnlyList<NamedEventConst> GetPredefinedArray(string combined)
        {
            List<NamedEventConst>? result = new List<NamedEventConst> {Custom};
            string[]? split = combined.Split(',');
            foreach (string? entry in split)
            {
                string[]? subsplit = entry.Split(':');
                string? name = subsplit[1];
                ushort value = Convert.ToUInt16(subsplit[0]);
                result.Add(new NamedEventConst(name, value));
            }
            return result;
        }

        private static int TryParseHexDec(string flag)
        {
            if (!flag.StartsWith("0x"))
                return Convert.ToInt16(flag);
            flag = flag[2..];
            return Convert.ToInt16(flag, 16);
        }

        private static int TryParseHexDecConst(string c)
        {
            if (!c.StartsWith("0x40"))
                return Convert.ToInt16(c);
            c = c[4..];
            return Convert.ToInt16(c, 16);
        }
    }

    public record NamedEventValue(string Name, int Index);

    public sealed record NamedEventWork : NamedEventValue
    {
        public readonly IReadOnlyList<NamedEventConst> PredefinedValues;

        public NamedEventWork(string Name, int Index, IReadOnlyList<NamedEventConst> values) : base(Name, Index)
        {
            PredefinedValues = values;
        }
    }

    public sealed record NamedEventConst(string Name, ushort Value)
    {
        public bool IsCustom => Value == CustomMagicValue;
        public const ushort CustomMagicValue = ushort.MaxValue;
    }
}
