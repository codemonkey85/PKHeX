using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="EntityContext.Gen9"/>
/// </summary>
public sealed class ItemStorage9SV : IItemStorage
{
    public static readonly ItemStorage9SV Instance = new();

    private static ReadOnlySpan<ushort> Pouch_Medicine_SV =>
    [
        0017, 0018, 0019, 0020, 0021, 0022, 0023, 0024, 0025, 0026,
        0027, 0028, 0029, 0030, 0031, 0032, 0033, 0034, 0035, 0036,
        0037, 0038, 0039, 0040, 0041, 0708, 0709,
    ];

    private static ReadOnlySpan<ushort> Pouch_Ball_SV =>
    [
        0001, 0002, 0003, 0004, 0005, 0006, 0007, 0008, 0009, 0010,
        0011, 0012, 0013, 0014, 0015, 0016, 0492, 0493, 0494, 0495,
        0496, 0497, 0498, 0499, 0500, 0576, 0851, 1785,
    ];

    private static ReadOnlySpan<ushort> Pouch_Battle_SV =>
    [
        0055, 0056, 0057, 0058, 0059, 0060, 0061, 0062, 0063,
    ];

    private static ReadOnlySpan<ushort> Pouch_Berries_SV =>
    [
        0149, 0150, 0151, 0152, 0153, 0154, 0155, 0156, 0157, 0158,
        0159, 0160, 0161, 0162, 0163, 0169, 0170, 0171, 0172, 0173,
        0174, 0184, 0185, 0186, 0187, 0188, 0189, 0190, 0191, 0192,
        0193, 0194, 0195, 0196, 0197, 0198, 0199, 0200, 0201, 0202,
        0203, 0204, 0205, 0206, 0207, 0208, 0209, 0210, 0211, 0212,
        0686, 0687, 0688,
    ];

    private static ReadOnlySpan<ushort> Pouch_Other_SV =>
    [
        0045, 0046, 0047, 0048, 0049, 0050, 0051, 0052, 0053, 0080,
        0081, 0082, 0083, 0084, 0085, 0107, 0108, 0109, 0110, 0111,
        0112, 0135, 0136, 0213, 0214, 0217, 0218, 0219, 0220, 0221,
        0222, 0223, 0224, 0225, 0228, 0229, 0230, 0231, 0232, 0233,
        0234, 0235, 0236, 0237, 0238, 0239, 0240, 0241, 0242, 0243,
        0244, 0245, 0246, 0247, 0248, 0249, 0250, 0251, 0252, 0253,
        0265, 0266, 0267, 0268, 0269, 0270, 0271, 0272, 0273, 0275,
        0276, 0277, 0278, 0279, 0280, 0281, 0282, 0283, 0284, 0285,
        0286, 0287, 0288, 0289, 0290, 0291, 0292, 0293, 0294, 0295,
        0296, 0297, 0298, 0299, 0300, 0301, 0302, 0303, 0304, 0305,
        0306, 0307, 0308, 0309, 0310, 0311, 0312, 0313, 0321, 0322,
        0323, 0324, 0325, 0326, 0327, 0485, 0486, 0487, 0488, 0489,
        0490, 0491, 0537, 0538, 0539, 0540, 0541, 0542, 0543, 0544,
        0545, 0546, 0547, 0564, 0565, 0566, 0567, 0568, 0569, 0570,
        0639, 0640, 0644, 0645, 0648, 0649, 0650, 0795, 0796, 0846,
        0849, 0853, 0854, 0855, 0856, 0879, 0880, 0881, 0882, 0883,
        0884, 1103, 1104, 1109, 1110, 1111, 1112, 1113, 1114, 1115,
        1116, 1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125,
        1126, 1127, 1128, 1231, 1232, 1233, 1234, 1235, 1236, 1237,
        1238, 1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246, 1247,
        1248, 1249, 1250, 1251, 1253, 1254, 1581, 1582, 1592, 1606,
        1777, 1778, 1779, 1861, 1862, 1863, 1864, 1865, 1866, 1867,
        1868, 1869, 1870, 1871, 1872, 1873, 1874, 1875, 1876, 1877,
        1878, 1879, 1880, 1881, 1882, 1883, 1884, 1885, 1886, 2344,
        2345, 2401, 2402, 2403, 2404, 2406, 2407, 2408, 2411, 2412,
        2413, 2414, 2415, 2416, 2479, 2482, 2549,
    ];

    private static ReadOnlySpan<ushort> Pouch_TM_SV =>
    [
        0328, 0329, 0330, 0331, 0332, 0333, 0334, 0335, 0336, 0337,
        0338, 0339, 0340, 0341, 0342, 0343, 0344, 0345, 0346, 0347,
        0348, 0349, 0350, 0351, 0352, 0353, 0354, 0355, 0356, 0357,
        0358, 0359, 0360, 0361, 0362, 0363, 0364, 0365, 0366, 0367,
        0368, 0369, 0370, 0371, 0372, 0373, 0374, 0375, 0376, 0377,
        0378, 0379, 0380, 0381, 0382, 0383, 0384, 0385, 0386, 0387,
        0388, 0389, 0390, 0391, 0392, 0393, 0394, 0395, 0396, 0397,
        0398, 0399, 0400, 0401, 0402, 0403, 0404, 0405, 0406, 0407,
        0408, 0409, 0410, 0411, 0412, 0413, 0414, 0415, 0416, 0417,
        0418, 0419, 0618, 0619, 0620, 0690, 0691, 0692, 0693, 1230,
        2160, 2161, 2162, 2163, 2164, 2165, 2166, 2167, 2168, 2169,
        2170, 2171, 2172, 2173, 2174, 2175, 2176, 2177, 2178, 2179,
        2180, 2181, 2182, 2183, 2184, 2185, 2186, 2187, 2188, 2189,
        2190, 2191, 2192, 2193, 2194, 2195, 2196, 2197, 2198, 2199,
        2200, 2201, 2202, 2203, 2204, 2205, 2206, 2207, 2208, 2209,
        2210, 2211, 2212, 2213, 2214, 2215, 2216, 2217, 2218, 2219,
        2220, 2221, 2222, 2223, 2224, 2225, 2226, 2227, 2228, 2229,
        2230, 2231, 2232, 2233, 2234, 2235, 2236, 2237, 2238, 2239,
        2240, 2241, 2242, 2243, 2244, 2245, 2246, 2247, 2248, 2249,
        2250, 2251, 2252, 2253, 2254, 2255, 2256, 2257, 2258, 2259,
        2260, 2261, 2262, 2263, 2264, 2265, 2266, 2267, 2268, 2269,
        2270, 2271, 2272, 2273, 2274, 2275, 2276, 2277, 2278, 2279,
        2280, 2281, 2282, 2283, 2284, 2285, 2286, 2287, 2288, 2289,
    ];

    private static ReadOnlySpan<ushort> Pouch_Treasure_SV =>
    [
        0086, 0087, 0088, 0089, 0090, 0091, 0092, 0094, 0106, 0571,
        0580, 0581, 0582, 0583, 1842, 1843,
    ];

    private static ReadOnlySpan<ushort> Pouch_Picnic_SV =>
    [
        1888, 1889, 1890, 1891, 1892, 1893, 1894, 1895, 1896, 1897,
        1898, 1899, 1900, 1901, 1902, 1903, 1904, 1905, 1906, 1907,
        1908, 1909, 1910, 1911, 1912, 1913, 1914, 1915, 1916, 1917,
        1918, 1919, 1920, 1921, 1922, 1923, 1924, 1925, 1926, 1927,
        1928, 1929, 1930, 1931, 1932, 1933, 1934, 1935, 1936, 1937,
        1938, 1939, 1940, 1941, 1942, 1943, 1944, 1945, 1946, 2311,
        2313, 2314, 2315, 2316, 2317, 2318, 2319, 2320, 2321, 2322,
        2323, 2324, 2325, 2326, 2327, 2329, 2330, 2331, 2332, 2333,
        2334, 2335, 2336, 2337, 2338, 2339, 2340, 2341, 2342, 2348,
        2349, 2350, 2351, 2352, 2353, 2354, 2355, 2356, 2357, 2358,
        2359, 2360, 2361, 2362, 2363, 2364, 2365, 2366, 2367, 2368,
        2369, 2370, 2371, 2372, 2373, 2374, 2375, 2376, 2377, 2378,
        2379, 2380, 2381, 2382, 2383, 2384, 2385, 2386, 2387, 2388,
        2389, 2390, 2391, 2392, 2393, 2394, 2395, 2396, 2397, 2398,
        2399, 2400, 2417, 2418, 2419, 2420, 2421, 2422, 2423, 2424,
        2425, 2426, 2427, 2428, 2429, 2430, 2431, 2432, 2433, 2434,
        2435, 2436, 2437, 2548, 2551, 2552,
    ];

    private static ReadOnlySpan<ushort> Pouch_Event_SV =>
    [
        0078, 0466, 0628, 0629, 0631, 0632, 0638, 0703, 0765, 0943,
        0944, 0945, 0946, 1267, 1278, 1587, 1589, 1590, 1591, 1829,
        1830, 1831, 1832, 1833, 1834, 1835, 1836, 1857, 1858, 2405,
        2409, 2410, 2480, 2481, 2483, 2522, 2523, 2524, 2525, 2526,
        2527, 2528, 2529, 2530, 2531, 2532, 2533, 2534, 2535, 2536,
        2537, 2538, 2539, 2540, 2541, 2542, 2543, 2544, 2545, 2546,
        2547, 2550, 2553, 2554, 2555, 2556, 2557,
    ];

    private static ReadOnlySpan<ushort> Pouch_Material_SV =>
    [
        1956, 1957, 1958, 1959, 1960, 1961, 1962, 1963, 1964, 1965,
        1966, 1967, 1968, 1969, 1970, 1971, 1972, 1973, 1974, 1975,
        1976, 1977, 1978, 1979, 1980, 1981, 1982, 1983, 1984, 1985,
        1986, 1987, 1988, 1989, 1990, 1991, 1992, 1993, 1994, 1995,
        1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003, 2004, 2005,
        2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2015,
        2016, 2017, 2018, 2019, 2020, 2021, 2022, 2023, 2024, 2025,
        2026, 2027, 2028, 2029, 2030, 2031, 2032, 2033, 2034, 2035,
        2036, 2037, 2038, 2039, 2040, 2041, 2042, 2043, 2044, 2045,
        2046, 2047, 2048, 2049, 2050, 2051, 2052, 2053, 2054, 2055,
        2056, 2057, 2058, 2059, 2060, 2061, 2062, 2063, 2064, 2065,
        2066, 2067, 2068, 2069, 2070, 2071, 2072, 2073, 2074, 2075,
        2076, 2077, 2078, 2079, 2080, 2081, 2082, 2083, 2084, 2085,
        2086, 2087, 2088, 2089, 2090, 2091, 2092, 2093, 2094, 2095,
        2096, 2097, 2098, 2099, 2103, 2104, 2105, 2106, 2107, 2108,
        2109, 2110, 2111, 2112, 2113, 2114, 2115, 2116, 2117, 2118,
        2119, 2120, 2121, 2122, 2123, 2126, 2127, 2128, 2129, 2130,
        2131, 2132, 2133, 2134, 2135, 2136, 2137, 2156, 2157, 2158,
        2159, 2438, 2439, 2440, 2441, 2442, 2443, 2444, 2445, 2446,
        2447, 2448, 2449, 2450, 2451, 2452, 2453, 2454, 2455, 2456,
        2457, 2458, 2459, 2460, 2461, 2462, 2463, 2464, 2465, 2466,
        2467, 2468, 2469, 2470, 2471, 2472, 2473, 2474, 2475, 2476,
        2477, 2478, 2484, 2485, 2486, 2487, 2488, 2489, 2490, 2491,
        2492, 2493, 2494, 2495, 2496, 2497, 2498, 2499, 2500, 2501,
        2502, 2503, 2504, 2505, 2506, 2507, 2508, 2509, 2510, 2511,
        2512, 2513, 2514, 2515, 2516, 2517, 2518, 2519, 2520, 2521,
    ];

    internal static ReadOnlySpan<InventoryType> ValidTypes =>
    [
        InventoryType.Items, InventoryType.KeyItems,
        InventoryType.TMHMs,
        InventoryType.Medicine, InventoryType.Berries, InventoryType.Balls, InventoryType.BattleItems,
        InventoryType.Treasure,
        InventoryType.Ingredients, InventoryType.Candy,
    ];

    private static ReadOnlySpan<InventoryType> ValidHeldTypes =>
    [
        InventoryType.Items,
        InventoryType.TMHMs,
        InventoryType.Medicine, InventoryType.Berries, InventoryType.Balls, InventoryType.BattleItems,
        InventoryType.Treasure,
    ];

    public static ReadOnlySpan<ushort> Unreleased =>
    [
        0016, // Cherish Ball

        0111, // Odd Keystone

        0485, // Red Apricorn
        0486, // Blue Apricorn
        0487, // Yellow Apricorn
        0488, // Green Apricorn
        0489, // Pink Apricorn
        0490, // White Apricorn
        0491, // Black Apricorn

        0500, // Park Ball
        0708, // Lumiose Galette
        0709, // Shalour Sable

        1230, // TM00 - Mega Punch (Nothing learns, not obtainable even though it is assigned a move.)

        1785, // Strange Ball
    ];

    public int GetMax(InventoryType type) => type switch
    {
        InventoryType.Items => 999,
        InventoryType.KeyItems => 1,
        InventoryType.TMHMs => 999,
        InventoryType.Medicine => 999,
        InventoryType.Berries => 999,
        InventoryType.Balls => 999,
        InventoryType.BattleItems => 999,
        InventoryType.Treasure => 999,
        InventoryType.Ingredients => 999, // 999
        InventoryType.Candy => 999, // 999
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount)
    {
        return Unreleased.BinarySearch((ushort)itemIndex) < 0;
    }

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => GetLegal(type);

    public static ReadOnlySpan<ushort> GetLegal(InventoryType type) => type switch
    {
        InventoryType.Items => Pouch_Other_SV,
        InventoryType.KeyItems => Pouch_Event_SV,
        InventoryType.TMHMs => Pouch_TM_SV,
        InventoryType.Medicine => Pouch_Medicine_SV,
        InventoryType.Berries => Pouch_Berries_SV,
        InventoryType.Balls => Pouch_Ball_SV,
        InventoryType.BattleItems => Pouch_Battle_SV,
        InventoryType.Treasure => Pouch_Treasure_SV,
        InventoryType.Ingredients => Pouch_Picnic_SV,
        InventoryType.Candy => Pouch_Material_SV,
        _ => throw new ArgumentOutOfRangeException(nameof(type)),
    };

    public static ushort[] GetAllHeld()
    {
        var valid = ValidHeldTypes;
        var sum = 0;
        foreach (var type in valid)
            sum += GetLegal(type).Length;

        var result = new ushort[sum];
        LoadAllHeld(valid, result);
        return result;
    }

    private static void LoadAllHeld(ReadOnlySpan<InventoryType> valid, Span<ushort> dest)
    {
        foreach (var type in valid)
        {
            var legal = GetLegal(type);
            legal.CopyTo(dest);
            dest = dest[legal.Length..];
        }
    }

    public static InventoryType GetInventoryPouch(ushort itemIndex)
    {
        foreach (var type in ValidTypes)
        {
            var legal = GetLegal(type);
            if (legal.Contains(itemIndex))
                return type;
        }
        return InventoryType.None;
    }
}
