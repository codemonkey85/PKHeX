using System;

namespace PKHeX.Core
{
    public static class GeoLocation
    {
        private static readonly string[]?[] CountryList = GetCountryList();
        internal static readonly string[] lang_geo = { "ja", "en", "fr", "de", "it", "es", "zh", "ko" };
        private static readonly string[]?[]?[] RegionList = new string[CountryList.Length][][];

        public static string[]? GetCountryList(string language)
        {
            int index = GetLanguageIndex(language);
            return CountryList[index];
        }

        private const string INVALID = nameof(INVALID);

        private static string[]?[] GetCountryList()
        {
            string[]? input = Util.GetStringList("countries");
            return UnpackList(input);
        }

        private static string[]?[] GetRegionList(int country)
        {
            string[]? input = Util.GetStringList($"sr_{country:000}");
            return UnpackList(input);
        }

        private static string[]?[] UnpackList(string[] input)
        {
            string[]? last = GetEntry(input[^1], out int lastIndex);
            string[]?[] list = new string[lastIndex+1][];
            list[lastIndex] = last;
            foreach (string? line in input)
            {
                string[]? entry = GetEntry(line, out int index);
                list[index] = entry;
            }
            return list;
        }

        private static string[] GetEntry(string line, out int index)
        {
            string[]? entries = line.Split(',');
            index = int.Parse(entries[0]);
            return entries;
        }

        private static string GetCountryName(int country, int l)
        {
            if (l < 0)
                return INVALID;
            if ((uint)country >= CountryList.Length)
                return INVALID;
            string[]? countryNames = CountryList[country];
            if (countryNames is not null && l < countryNames.Length)
                return countryNames[l + 1];
            return INVALID;
        }

        private static string GetRegionName(int country, int region, int l)
        {
            if (l < 0)
                return INVALID;
            if ((uint)country >= RegionList.Length)
                return INVALID;
            string[]?[]? regionNames = RegionList[country] ??= GetRegionList(country);
            if ((uint)region >= regionNames.Length)
                return INVALID;
            string[]? localized = regionNames[region];
            if (localized is not null && l < localized.Length)
                return localized[l + 1];
            return INVALID;
        }

        /// <summary>
        /// Gets the Country string for a given Country ID
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <returns>Country ID string</returns>
        public static string GetCountryName(string language, int country) => GetCountryName(country, GetLanguageIndex(language));

        /// <summary>
        /// Gets the Region string for a specified country ID.
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <param name="region">Region ID</param>
        /// <returns>Region ID string</returns>
        public static string GetRegionName(string language, int country, int region) => GetRegionName(country, region, GetLanguageIndex(language));

        /// <summary>
        /// Gets the Country string for a given Country ID
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <returns>Country ID string</returns>
        public static string GetCountryName(LanguageID language, int country) => GetCountryName(country, GetLanguageIndex(language));

        /// <summary>
        /// Gets the Region string for a specified country ID.
        /// </summary>
        /// <param name="language">Language ID</param>
        /// <param name="country">Country ID</param>
        /// <param name="region">Region ID</param>
        /// <returns>Region ID string</returns>
        public static string GetRegionName(LanguageID language, int country, int region) => GetRegionName(country, region, GetLanguageIndex(language));

        /// <summary>
        /// Gets Country and Region strings for corresponding IDs and language.
        /// </summary>
        /// <param name="country">Country ID</param>
        /// <param name="region">Region ID</param>
        /// <param name="language">Language ID</param>
        /// <returns>Tuple containing country and region</returns>
        public static (string Country, string Region) GetCountryRegionText(int country, int region, string language)
        {
            // Get Language we're fetching for
            int lang = Array.IndexOf(lang_geo, language);
            string? countryName = GetCountryName(country, lang);
            string? regionName = GetRegionName(country, region, lang);
            return (countryName, regionName);
        }

        public static int GetLanguageIndex(string language) => Array.IndexOf(lang_geo, language);
        private static int GetLanguageIndex(LanguageID language) => GetLanguageIndex(language.GetLanguage2CharName());
    }
}
