using System.Collections.Generic;
using System.Linq;

namespace PKHeX.Core
{
    /// <summary>
    /// Provides a summary for an <see cref="IEncounterTemplate"/> object.
    /// </summary>
    public record EncounterSummary
    {
        private readonly GameVersion Version;
        private readonly string LocationName;

        private EncounterSummary(IEncounterTemplate z, string type)
        {
            Version = z.Version;
            LocationName = GetLocationName(z) + $"({type}) ";
        }

        private EncounterSummary(IEncounterTemplate z)
        {
            Version = z.Version;
            LocationName = GetLocationName(z);
        }

        private static string GetLocationName(IEncounterTemplate z)
        {
            int gen = z.Generation;
            GameVersion version = z.Version;
            if (gen < 0 && version > 0)
                gen = version.GetGeneration();

            if (z is not ILocation l)
                return $"[Gen{gen}]\t";
            string? loc = l.GetEncounterLocation(gen, (int)version);

            if (string.IsNullOrWhiteSpace(loc))
                return $"[Gen{gen}]\t";
            return $"[Gen{gen}]\t{loc}: ";
        }

        public static IEnumerable<string> SummarizeGroup(IEnumerable<IEncounterTemplate> items, string header = "", bool advanced = false)
        {
            if (!string.IsNullOrWhiteSpace(header))
                yield return $"=={header}==";
            IEnumerable<EncounterSummary>? summaries = advanced ? GetSummaries(items) : items.Select(z => new EncounterSummary(z));
            IEnumerable<IGrouping<string, EncounterSummary>>? objs = summaries.GroupBy(z => z.LocationName);
            foreach (IGrouping<string, EncounterSummary>? g in objs)
                yield return $"\t{g.Key}{string.Join(", ", g.Select(z => z.Version).Distinct())}";
        }

        public static IEnumerable<EncounterSummary> GetSummaries(IEnumerable<IEncounterTemplate> items) => items.Select(GetSummary);

        private static EncounterSummary GetSummary(IEncounterTemplate item)
        {
            return item switch
            {
                EncounterSlot s when s.Area.Type != 0 => new EncounterSummary(item, s.Area.Type.ToString()),
                _ => new EncounterSummary(item)
            };
        }
    }
}