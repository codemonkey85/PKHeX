using System;

namespace PKHeX.Core;

/// <summary>
/// Item storage for <see cref="GameVersion.SN"/> and <see cref="GameVersion.MN"/>
/// </summary>
public sealed class ItemStorage7SM : IItemStorage
{
    public static readonly ItemStorage7SM Instance = new();

    public static ReadOnlySpan<ushort> General =>
    [
        001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013, 014, 015, 016,
        055, 056, 057, 058, 059, 060, 061, 062, 063, 064,
        068, 069,
        070, 071, 072, 073, 074, 075, 076, 077, 078, 079,
        080, 081, 082, 083, 084, 085, 086, 087, 088, 089,
        090, 091, 092, 093, 094, 099,
        100, 101, 102, 103, 104, 105, 106, 107, 108, 109,
        110, 111, 112, 116, 117, 118, 119,
        135, 136, 137,
        213, 214, 215, 217, 218, 219,
        220, 221, 222, 223, 224, 225, 226, 227, 228, 229,
        230, 231, 232, 233, 234, 235, 236, 237, 238, 239,
        240, 241, 242, 243, 244, 245, 246, 247, 248, 249,
        250, 251, 252, 253, 254, 255, 256, 257, 258, 259,
        260, 261, 262, 263, 264, 265, 266, 267, 268, 269,
        270, 271, 272, 273, 274, 275, 276, 277, 278, 279,
        280, 281, 282, 283, 284, 285, 286, 287, 288, 289,
        290, 291, 292, 293, 294, 295, 296, 297, 298, 299,
        300, 301, 302, 303, 304, 305, 306, 307, 308, 309,
        310, 311, 312, 313, 314, 315, 316, 317, 318, 319,
        320, 321, 322, 323, 324, 325, 326, 327,

        492, 493, 494, 495, 496, 497, 498,
        499, 534, 535, 537, 538, 539,
        540, 541, 542, 543, 544, 545, 546, 547, 548, 549,
        550, 551, 552, 553, 554, 555, 556, 557, 558, 559,
        560, 561, 562, 563, 564,
        571, 572, 573, 576, 577, // Poké Toy
        580, 581, 582, 583,
        584, 585, 586, 587, 588, 589, 590,
                                                     639,
        640,                644,      646, 647, 648, 649,
        650,                          656, 657, 658, 659,
        660, 661, 662, 663, 664, 665, 666, 667, 668, 669,
        670, 671, 672, 673, 674, 675, 676, 677, 678, 679,
        680, 681, 682, 683, 684, 685,
        699,
                            704,
        710, 711,                715,
                  752, 753, 754, 755, 756, 757, 758, 759,
        760, 761, 762, 763, 764,           767, 768, 769,
        770,
        795, 796, 844, 846, 849,
             851,      853, 854, 855, 856,           879,
        880, 881, 882,
        883, 884, 904, 905, 906, 907, 908, 909, 910, 911, 912, 913, 914, 915, 916, 917, 918, 919, 920,
    ];

    public static ReadOnlySpan<ushort> Key =>
    [
        216,
        465, 466, 628, 629, 631, 632, 638,
        705, 706, 765, 773, 797,
        841, 842, 843, 845, 847, 850, 857, 858, 860,
    ];

    public static ReadOnlySpan<ushort> Machine =>
    [
        328, 329, 330, 331, 332, 333, 334, 335, 336, 337,
        338, 339, 340, 341, 342, 343, 344, 345, 346, 347,
        348, 349, 350, 351, 352, 353, 354, 355, 356, 357,
        358, 359, 360, 361, 362, 363, 364, 365, 366, 367,
        368, 369, 370, 371, 372, 373, 374, 375, 376, 377,
        378, 379, 380, 381, 382, 383, 384, 385, 386, 387,
        388, 389, 390, 391, 392, 393, 394, 395, 396, 397,
        398, 399, 400, 401, 402, 403, 404, 405, 406, 407,
        408, 409, 410, 411, 412, 413, 414, 415, 416, 417,
        418, 419, // 01-92

        618, 619, 620, // 93-95
        690, 691, 692, 693, 694, // 96-100
    ];

    public static ReadOnlySpan<ushort> Medicine =>
    [
        17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 65, 66, 67, 134,
        504, 565, 566, 567, 568, 569, 570, 591, 645, 708, 709,
        852,
    ];

    public static ReadOnlySpan<ushort> Berry =>
    [
        149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212,
        686, 687, 688,
    ];

    public static ReadOnlySpan<ushort> ZCrystalKey =>
    [
        807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818, 819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833, 834, 835,
    ];

    public static ReadOnlySpan<ushort> ZCrystalHeld =>
    [
        // SM
        776, 777, 778, 779, 780, 781, 782, 783, 784, 785, 786, 787, 788, 789, 790, 791, 792, 793, 794, 798, 799, 800, 801, 802, 803, 804, 805, 806, 836,
    ];

    internal static ReadOnlySpan<ushort> Unreleased =>
    [
        005, // Safari Ball
        016, // Cherish Ball
        064, // Fluffy Tail
        065, // Blue Flute
        066, // Yellow Flute
        067, // Red Flute
        068, // Black Flute
        069, // White Flute
        070, // Shoal Salt
        071, // Shoal Shell
        103, // Old Amber
        111, // Odd Keystone
        164, // Razz Berry
        166, // Nanab Berry
        167, // Wepear Berry
        175, // Cornn Berry
        176, // Magost Berry
        177, // Rabuta Berry
        178, // Nomel Berry
        179, // Spelon Berry
        180, // Pamtre Berry
        181, // Watmel Berry
        182, // Durin Berry
        183, // Belue Berry
        //208, // Enigma Berry
        //209, // Micle Berry
        //210, // Custap Berry
        //211, // Jaboca Berry
        //212, // Rowap Berry
        215, // Macho Brace
        260, // Red Scarf
        261, // Blue Scarf
        262, // Pink Scarf
        263, // Green Scarf
        264, // Yellow Scarf
        499, // Sport Ball
        548, // Fire Gem
        549, // Water Gem
        550, // Electric Gem
        551, // Grass Gem
        552, // Ice Gem
        553, // Fighting Gem
        554, // Poison Gem
        555, // Ground Gem
        556, // Flying Gem
        557, // Psychic Gem
        558, // Bug Gem
        559, // Rock Gem
        560, // Ghost Gem
        561, // Dragon Gem
        562, // Dark Gem
        563, // Steel Gem
        576, // Dream Ball
        584, // Relic Copper
        585, // Relic Silver
        587, // Relic Vase
        588, // Relic Band
        589, // Relic Statue
        590, // Relic Crown
        699, // Discount Coupon
        715, // Fairy Gem
    ];

    public static ushort[] GetAllHeld() => [..General, ..Berry, ..Medicine, ..ZCrystalHeld];

    public static bool GetCrystalHeld(ushort itemKey, out ushort itemHeld)
    {
        var index = ZCrystalKey.IndexOf(itemKey);
        if (index < 0)
        {
            itemHeld = 0;
            return false;
        }
        itemHeld = ZCrystalHeld[index];
        return true;
    }

    public static bool GetCrystalKey(ushort itemHeld, out ushort itemKey)
    {
        var index = ZCrystalHeld.IndexOf(itemHeld);
        if (index < 0)
        {
            itemKey = 0;
            return false;
        }
        itemKey = ZCrystalKey[index];
        return true;
    }

    public bool IsLegal(InventoryType type, int itemIndex, int itemCount)
    {
        if (type is InventoryType.KeyItems or InventoryType.ZCrystals)
            return true;

        var items = GetItems(type);
        if (items.BinarySearch((ushort)itemIndex) < 0)
            return false;

        return Unreleased.BinarySearch((ushort)itemIndex) < 0;
    }

    public ReadOnlySpan<ushort> GetItems(InventoryType type) => type switch
    {
        InventoryType.Medicine => Medicine,
        InventoryType.Items => General,
        InventoryType.TMHMs => Machine,
        InventoryType.Berries => Berry,
        InventoryType.KeyItems => Key,
        InventoryType.ZCrystals => ZCrystalKey,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
