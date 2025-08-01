using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Verifies the <see cref="PKM.OriginalTrainerName"/>.
/// </summary>
public sealed class TrainerNameVerifier : Verifier
{
    protected override CheckIdentifier Identifier => CheckIdentifier.Trainer;

    private static readonly string[] SuspiciousOTNames =
    [
        "PKHeX",
        "ＰＫＨｅＸ",
    ];

    public override void Verify(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterOriginal;
        if (!IsPlayerOriginalTrainer(enc))
            return; // already verified

        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        if (len == 0)
        {
            data.AddLine(GetInvalid(OTShort));
            return;
        }
        trainer = trainer[..len];
        if (trainer.Contains('\uffff') && pk is { Format: 4 })
        {
            data.AddLine(GetInvalid(CheckIdentifier.Trainer, WordFilterInvalidCharacter_0, 0xFFFF));
            return;
        }

        if (IsOTNameSuspicious(trainer))
        {
            data.AddLine(Get(Severity.Fishy, OTSuspicious));
        }

        if (pk.VC)
        {
            VerifyOTGB(data);
        }
        else if (trainer.Length > Legal.GetMaxLengthOT(enc.Generation, (LanguageID)pk.Language))
        {
            if (!IsEdgeCaseLength(pk, enc, trainer))
                data.AddLine(Get(Severity.Invalid, OTLong));
        }

        if (ParseSettings.Settings.WordFilter.IsEnabled(pk.Format))
        {
            if (WordFilter.IsFiltered(trainer, pk.Context, enc.Context, out var type, out var badPattern))
                data.AddLine(GetInvalid(CheckIdentifier.Trainer, WordFilterFlaggedPattern_01, (ushort)type, (ushort)badPattern));
            if (ContainsTooManyNumbers(trainer, enc.Generation))
                data.AddLine(GetInvalid(CheckIdentifier.Trainer, WordFilterTooManyNumbers_0, (ushort)GetMaxNumberCount(enc.Generation)));

            Span<char> ht = stackalloc char[pk.TrashCharCountTrainer];
            int nameLen = pk.LoadString(pk.HandlingTrainerTrash, ht);
            if (WordFilter.IsFiltered(ht[..nameLen], pk.Context, out type, out badPattern)) // HT context is always the current context
                data.AddLine(GetInvalid(CheckIdentifier.Handler, WordFilterFlaggedPattern_01, (ushort)type, (ushort)badPattern));
        }
    }

    /// <summary>
    /// Checks if any player (human) was the original OT.
    /// </summary>
    internal static bool IsPlayerOriginalTrainer(IEncounterable enc) => enc switch
    {
        IFixedTrainer { IsFixedTrainer: true } => false,
        MysteryGift { IsEgg: false } => false,
        ITrainerID16ReadOnly => false,
        _ => true,
    };

    public static bool IsEdgeCaseLength(PKM pk, IEncounterTemplate e, ReadOnlySpan<char> ot)
    {
        if (e.IsEgg)
        {
            if (e is EncounterGift3 wc3 && pk.IsEgg && ot.SequenceEqual(wc3.OriginalTrainerName))
                return true; // Fixed OT Mystery Gift Egg
            bool eggEdge = pk.IsEgg ? pk.IsTradedEgg || pk.Format == 3 : pk.WasTradedEgg;
            if (!eggEdge)
                return false;
            var len = Legal.GetMaxLengthOT(e.Generation, LanguageID.English); // max case
            return ot.Length <= len;
        }

        if (e is IFixedTrainer { IsFixedTrainer: true })
            return true; // already verified
        return false;
    }

    public void VerifyOTGB(LegalityAnalysis data)
    {
        var pk = data.Entity;
        var enc = data.EncounterOriginal;
        if (pk.OriginalTrainerGender == 1)
        {
            // Transferring from RBY->Gen7 won't have OT Gender in PK1, nor will PK1 originated encounters.
            // GSC Trades already checked for OT Gender matching.
            if (pk is { Format: > 2, VC1: true } || enc is { Generation: 1 } or EncounterGift2 { IsEgg: false })
                data.AddLine(GetInvalid(G1OTGender));
        }

        if (enc is IFixedTrainer { IsFixedTrainer: true })
            return; // already verified

        Span<char> trainer = stackalloc char[pk.TrashCharCountTrainer];
        int len = pk.LoadString(pk.OriginalTrainerTrash, trainer);
        trainer = trainer[..len];

        if (trainer.Length == 0)
        {
            if (pk is SK2 {TID16: 0, IsRental: true})
            {
                data.AddLine(Get(Severity.Fishy, OTShort));
            }
            else
            {
                data.AddLine(GetInvalid(OTShort));
                return;
            }
        }
        VerifyGBOTWithinBounds(data, trainer);
    }

    private void VerifyGBOTWithinBounds(LegalityAnalysis data, ReadOnlySpan<char> str)
    {
        var pk = data.Entity;

        // Filtered OT names use unavailable characters and can be too long
        if (pk.Format >= 7)
        {
            // Check if it was profanity filtered.
            var filtered = StringConverter12Transporter.GetFilteredOT(pk.Language, pk.Version);
            if (str.SequenceEqual(filtered))
                return;
        }

        if (pk.Japanese)
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(OTLong, 5));
            if (!StringConverter1.GetIsJapanese(str))
                data.AddLine(GetInvalid(G1CharOT));
        }
        else if (pk.Korean)
        {
            if (str.Length > 5)
                data.AddLine(GetInvalid(OTLong, 5));
            if (!StringConverter2KOR.GetIsKorean(str))
                data.AddLine(GetInvalid(G1CharOT));
        }
        else
        {
            if (str.Length > 7)
                data.AddLine(GetInvalid(OTLong, 7));
            if (!StringConverter1.GetIsEnglish(str))
                data.AddLine(GetInvalid(G1CharOT));
        }
    }

    private static bool IsOTNameSuspicious(ReadOnlySpan<char> name)
    {
        foreach (var s in SuspiciousOTNames)
        {
            if (name.StartsWith(s, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    public static bool ContainsTooManyNumbers(ReadOnlySpan<char> str, byte originalGeneration)
    {
        if (originalGeneration <= 3)
            return false; // no limit from these generations
        int max = GetMaxNumberCount(originalGeneration);
        if (str.Length <= max)
            return false;
        int count = GetNumberCount(str);
        return count > max;
    }

    public static int GetMaxNumberCount(byte originalGeneration) => originalGeneration < 6 ? 4 : 5;

    private static int GetNumberCount(ReadOnlySpan<char> str)
    {
        int ctr = 0;
        foreach (var c in str)
        {
            if (char.IsNumber(c))
                ++ctr;
        }
        return ctr;
    }
}
