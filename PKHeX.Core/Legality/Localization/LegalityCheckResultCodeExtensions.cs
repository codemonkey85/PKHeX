using System;
using static PKHeX.Core.LegalityCheckResultCode;

namespace PKHeX.Core;

/// <summary>
/// Provides extension methods for <see cref="LegalityCheckResultCode"/> to convert to human-readable strings.
/// </summary>
public static class LegalityCheckResultCodeExtensions
{
    public static bool IsArgument(this LegalityCheckResultCode code) => code is < FirstWithMove and >= FirstWithArgument;
    public static bool IsMove(this LegalityCheckResultCode code) => code is < FirstWithLanguage and >= FirstWithMove;
    public static bool IsLanguage(this LegalityCheckResultCode code) => code is < FirstWithMemory and >= FirstWithLanguage;
    public static bool IsMemory(this LegalityCheckResultCode code) => code is < FirstComplex and >= FirstWithMemory;

    /// <summary>
    /// Returns the template string for the given result code.
    /// </summary>
    public static string GetTemplate(this LegalityCheckResultCode code, LegalityCheckLocalization localization) => code switch
    {
        // General Strings
        External => localization.NotImplemented,
        Valid => localization.Valid,
        Error => localization.Error,

        // Ability
        AbilityCapsuleUsed => localization.AbilityCapsuleUsed,
        AbilityPatchUsed => localization.AbilityPatchUsed,
        AbilityPatchRevertUsed => localization.AbilityPatchRevertUsed,
        AbilityFlag => localization.AbilityFlag,
        AbilityHiddenFail => localization.AbilityHiddenFail,
        AbilityHiddenUnavailable => localization.AbilityHiddenUnavailable,
        AbilityMismatch => localization.AbilityMismatch,
        AbilityMismatch3 => localization.AbilityMismatch3,
        AbilityMismatchFlag => localization.AbilityMismatchFlag,
        AbilityMismatchGift => localization.AbilityMismatchGift,
        AbilityMismatchPID => localization.AbilityMismatchPID,
        AbilityUnexpected => localization.AbilityUnexpected,

        // Awakened Values
        AwakenedCap => localization.AwakenedCap,
        AwakenedStatGEQ_01 => localization.AwakenedShouldBeValue,

        // Ball
        BallAbility => localization.BallAbility,
        BallEggCherish => localization.BallEggCherish,
        BallEggMaster => localization.BallEggMaster,
        BallEnc => localization.BallEnc,
        BallEncMismatch => localization.BallEncMismatch,
        BallHeavy => localization.BallHeavy,
        BallSpecies => localization.BallSpecies,
        BallSpeciesPass => localization.BallSpeciesPass,
        BallUnavailable => localization.BallUnavailable,

        // Contest
        ContestZero => localization.ContestZero,
        ContestZeroSheen => localization.ContestZeroSheen,
        ContestSheenGEQ_0 => localization.ContestSheenGEQ_0,
        ContestSheenLEQ_0 => localization.ContestSheenLEQ_0,

        // Date & Timestamps
        DateOutsideConsoleWindow => localization.DateOutsideConsoleWindow,
        DateTimeClockInvalid => localization.DateTimeClockInvalid,
        DateOutsideDistributionWindow => localization.DateOutsideDistributionWindow,

        // Egg
        EggContest => localization.EggContest,
        EggEXP => localization.EggEXP,
        EggFMetLevel_0 => localization.EggFMetLevel_0,
        EggHatchCycles => localization.EggHatchCycles,
        EggLocation => localization.EggLocation,
        EggLocationInvalid => localization.EggLocationInvalid,
        EggLocationNone => localization.EggLocationNone,
        EggLocationPalPark => localization.EggLocationPalPark,
        EggLocationTrade => localization.EggLocationTrade,
        EggLocationTradeFail => localization.EggLocationTradeFail,
        EggMetLocationFail => localization.EggMetLocationFail,
        EggNature => localization.EggNature,
        EggPokeathlon => localization.EggPokeathlon,
        EggPP => localization.EggPP,
        EggPPUp => localization.EggPPUp,
        EggRelearnFlags => localization.EggRelearnFlags,
        EggShinyLeaf => localization.EggShinyLeaf,
        EggShinyPokeStar => localization.EggShinyPokeStar,
        EggSpecies => localization.EggSpecies,
        EggUnhatched => localization.EggUnhatched,

        // Encounter
        EncCondition => localization.EncCondition,
        EncConditionBadRNGFrame => localization.EncConditionBadRNGFrame,
        EncConditionBadSpecies => localization.EncConditionBadSpecies,
        EncGift => localization.EncGift,
        EncGiftEggEvent => localization.EncGiftEggEvent,
        EncGiftIVMismatch => localization.EncGiftIVMismatch,
        EncGiftNicknamed => localization.EncGiftNicknamed,
        EncGiftNotFound => localization.EncGiftNotFound,
        EncGiftPIDMismatch => localization.EncGiftPIDMismatch,
        EncGiftShinyMismatch => localization.EncGiftShinyMismatch,
        EncGiftVersionNotDistributed => localization.EncGiftVersionNotDistributed,
        EncInvalid => localization.EncInvalid,
        EncMasteryInitial => localization.EncMasteryInitial,
        EncTradeChangedNickname => localization.EncTradeChangedNickname,
        EncTradeChangedOT => localization.EncTradeChangedOT,
        EncTradeIndexBad => localization.EncTradeIndexBad,
        EncTradeMatch => localization.EncTradeMatch,
        EncTradeUnchanged => localization.EncTradeUnchanged,
        EncStaticPIDShiny => localization.EncStaticPIDShiny,
        EncTypeMatch => localization.EncTypeMatch,
        EncTypeMismatch => localization.EncTypeMismatch,
        EncUnreleased => localization.EncUnreleased,
        EncUnreleasedEMewJP => localization.EncUnreleasedEMewJP,

        // E-Reader
        EReaderAmerica => localization.EReaderAmerica,
        EReaderInvalid => localization.EReaderInvalid,
        EReaderJapan => localization.EReaderJapan,

        // Effort Values
        Effort2Remaining => localization.Effort2Remaining,
        EffortAbove252 => localization.EffortAbove252,
        EffortAbove510 => localization.EffortAbove510,
        EffortAllEqual => localization.EffortAllEqual,
        EffortCap100 => localization.EffortCap100,
        EffortEgg => localization.EffortEgg,
        EffortShouldBeZero => localization.EffortShouldBeZero,
        EffortEXPIncreased => localization.EffortEXPIncreased,
        EffortUntrainedCap_0 => localization.EffortUntrainedCap,

        // Evolution
        EvoInvalid => localization.EvoInvalid,
        EvoTradeReqOutsider_0 => localization.EvoTradeReqOutsider,
        EvoTradeRequired => localization.EvoTradeRequired,

        // Form
        FormArgumentLEQ_0 => localization.FormArgumentLEQ_0,
        FormArgumentGEQ_0 => localization.FormArgumentGEQ_0,
        FormArgumentNotAllowed => localization.FormArgumentNotAllowed,
        FormArgumentValid => localization.FormArgumentValid,
        FormArgumentInvalid => localization.FormArgumentInvalid,
        FormBattle => localization.FormBattle,
        FormEternal => localization.FormEternal,
        FormEternalInvalid => localization.FormEternalInvalid,
        FormInvalidGame => localization.FormInvalidGame,
        FormInvalidNature => localization.FormInvalidNature,
        FormItemMatches => localization.FormItem,
        FormItemInvalid => localization.FormItemInvalid,
        FormParty => localization.FormParty,
        FormPikachuCosplay => localization.FormPikachuCosplay,
        FormPikachuCosplayInvalid => localization.FormPikachuCosplayInvalid,
        FormPikachuEventInvalid => localization.FormPikachuEventInvalid,
        FormInvalidExpect_0 => localization.FormInvalidExpect_0,
        FormValid => localization.FormValid,
        FormVivillon => localization.FormVivillon,
        FormVivillonEventPre => localization.FormVivillonEventPre,
        FormVivillonInvalid => localization.FormVivillonInvalid,
        FormVivillonNonNative => localization.FormVivillonNonNative,

        // Hyper Training
        HyperTrainLevelGEQ_0 => localization.HyperTrainLevelGEQ_0,
        HyperPerfectAll => localization.HyperPerfectAll,
        HyperPerfectOne => localization.HyperPerfectOne,
        HyperPerfectUnavailable => localization.HyperPerfectUnavailable,

        // IVs
        IVAllEqual_0 => localization.IVAllEqual_0,
        IVNotCorrect => localization.IVNotCorrect,
        IVFlawlessCountGEQ_0 => localization.IVFlawlessCountGEQ_0,

        // Markings
        MarkValueOutOfRange_0 => localization.MarkValueOutOfRange_0,
        MarkValueShouldBeZero => localization.MarkValueShouldBeZero,
        MarkValueUnusedBitsPresent => localization.MarkValueUnusedBitsPresent,

        // Moves
        MoveEvoFCombination_0 => localization.MoveEvoFCombination_0,
        MovePPExpectHealed_0 => localization.MovePPExpectHealed_0,
        MovePPTooHigh_0 => localization.MovePPTooHigh_0,
        MovePPUpsTooHigh_0 => localization.MovePPUpsTooHigh_0,
        MoveShopMasterInvalid_0 => localization.MoveShopMasterInvalid_0,
        MoveShopMasterNotLearned_0 => localization.MoveShopMasterNotLearned_0,
        MoveShopPurchaseInvalid_0 => localization.MoveShopPurchaseInvalid_0,
        MoveTechRecordFlagMissing_0 => localization.MoveTechRecordFlagMissing_0,

        // Memory
        MemoryStatSocialLEQ_0 => localization.MemoryStatSocialLEQ_0,

        // Pokerus
        PokerusDaysLEQ_0 => localization.PokerusDaysTooHigh_0,
        PokerusStrainUnobtainable_0 => localization.PokerusStrainUnobtainable_0,

        // Ribbons
        RibbonsInvalid_A => localization.RibbonsInvalid_A,
        RibbonsMissing_A => localization.RibbonsMissing_A,
        RibbonMarkingInvalid_0 => localization.RibbonMarkingInvalid_0,
        RibbonMarkingAffixed_0 => localization.RibbonMarkingAffixed_0,

        // Storage
        StoredSlotSourceInvalid_0 => localization.StoredSlotSourceInvalid_0,

        EncGiftLanguageNotDistributed_0 => localization.EncGiftLanguageNotDistributed,
        EncGiftRegionNotDistributed => localization.EncGiftRegionNotDistributed,
        EncTradeShouldHaveEvolvedToSpecies_0 => localization.EncTradeShouldHaveEvolvedToSpecies_0,
        FatefulGiftMissing => localization.FatefulGiftMissing,
        FatefulInvalid => localization.FatefulInvalid,
        FatefulMissing => localization.FatefulMissing,
        FatefulMystery => localization.FatefulMystery,
        FatefulMysteryMissing => localization.FatefulMysteryMissing,
        FavoriteMarkingUnavailable => localization.FavoriteMarkingUnavailable,
        FormInvalidRangeLEQ_0F => localization.FormInvalidRangeLEQ_0F,
        G1CatchRateChain => localization.G1CatchRateChain,
        G1CatchRateEvo => localization.G1CatchRateEvo,
        G1CatchRateItem => localization.G1CatchRateItem,
        G1CatchRateMatchPrevious => localization.G1CatchRateMatchPrevious,
        G1CatchRateMatchTradeback => localization.G1CatchRateMatchTradeback,
        G1CatchRateNone => localization.G1CatchRateNone,
        G1CharNick => localization.G1CharNick,
        G1CharOT => localization.G1CharOT,
        G1OTGender => localization.G1OTGender,
        G1Stadium => localization.G1Stadium,
        G1Type1Fail => localization.G1Type1Fail,
        G1Type2Fail => localization.G1Type2Fail,
        G1TypeMatch1 => localization.G1TypeMatch1,
        G1TypeMatch2 => localization.G1TypeMatch2,
        G1TypeMatchPorygon => localization.G1TypeMatchPorygon,
        G1TypePorygonFail => localization.G1TypePorygonFail,
        G1TypePorygonFail1 => localization.G1TypePorygonFail1,
        G1TypePorygonFail2 => localization.G1TypePorygonFail2,
        G2InvalidTileTreeNotFound => localization.G2InvalidTileTreeNotFound,
        G2TreeID => localization.G2TreeID,
        G2OTGender => localization.G2OTGender,
        G3EReader => localization.G3EReader,
        G3OTGender => localization.G3OTGender,
        G4InvalidTileR45Surf => localization.G4InvalidTileR45Surf,
        G5IVAll30 => localization.G5IVAll30,
        G5PIDShinyGrotto => localization.G5PIDShinyGrotto,
        G5SparkleInvalid => localization.G5SparkleInvalid,
        G5SparkleRequired => localization.G5SparkleRequired,
        GanbaruStatLEQ_01 => localization.GanbaruStatTooHigh,
        GenderInvalidNone => localization.GenderInvalidNone,
        GeoBadOrder => localization.GeoBadOrder,
        GeoHardwareInvalid => localization.GeoHardwareInvalid,
        GeoHardwareRange => localization.GeoHardwareRange,
        GeoHardwareValid => localization.GeoHardwareValid,
        GeoMemoryMissing => localization.GeoMemoryMissing,
        GeoNoCountryHT => localization.GeoNoCountryHT,
        GeoNoRegion => localization.GeoNoRegion,
        HintEvolvesToSpecies_0 => localization.HintEvolvesToSpecies_0,
        HintEvolvesToRareForm_0 => localization.HintEvolvesToRareForm_0,
        ItemEgg => localization.ItemEgg,
        ItemUnreleased => localization.ItemUnreleased,
        LevelEXPThreshold => localization.LevelEXPThreshold,
        LevelEXPTooHigh => localization.LevelEXPTooHigh,
        LevelMetBelow => localization.LevelMetBelow,
        LevelMetGift => localization.LevelMetGift,
        LevelMetGiftFail => localization.LevelMetGiftFail,
        LevelMetSane => localization.LevelMetSane,

        MemoryArgBadCatch_H => localization.MemoryArgBadCatch_H,
        MemoryArgBadHatch_H => localization.MemoryArgBadHatch_H,
        MemoryArgBadHT => localization.MemoryArgBadHT,
        MemoryArgBadID_H => localization.MemoryArgBadID_H,
        MemoryArgBadItem_H1 => localization.MemoryArgBadItem_H1,
        MemoryArgBadLocation_H => localization.MemoryArgBadLocation_H,
        MemoryArgBadMove_H1 => localization.MemoryArgBadMove_H1,
        MemoryArgBadOTEgg_H => localization.MemoryArgBadOTEgg_H,
        MemoryArgBadSpecies_H1 => localization.MemoryArgBadSpecies_H1,
        MemoryArgSpecies_H => localization.MemoryArgSpecies_H,
        MemoryCleared_H => localization.MemoryCleared_H,
        MemoryValid_H => localization.MemoryValid_H,
        MemoryFeelInvalid_H => localization.MemoryFeelInvalid_H,
        MemoryHTFlagInvalid => localization.MemoryHTFlagInvalid,
        MemoryHTGender_0 => localization.MemoryHTGender_0,
        MemoryHTLanguage => localization.MemoryHTLanguage,
        MemoryIndexArgHT => localization.MemoryIndexArgHT,
        MemoryIndexFeel_H1 => localization.MemoryIndexFeel_H1,
        MemoryIndexFeelHTLEQ9 => localization.MemoryIndexFeelHTLEQ9,
        MemoryIndexID_H1 => localization.MemoryIndexID_H1,
        MemoryIndexIntensity_H1 => localization.MemoryIndexIntensity_H1,
        MemoryIndexIntensityHT1 => localization.MemoryIndexIntensityHT1,
        MemoryIndexIntensityMin_H1 => localization.MemoryIndexIntensityMin_H1,
        MemoryIndexLinkHT => localization.MemoryIndexLinkHT,
        MemoryIndexVar_H1 => localization.MemoryIndexVar,
        MemoryMissingHT => localization.MemoryMissingHT,
        MemoryMissingOT => localization.MemoryMissingOT,
        MemorySocialZero => localization.MemorySocialZero,
        MemoryStatAffectionHT0 => localization.MemoryStatAffectionHT0,
        MemoryStatAffectionOT0 => localization.MemoryStatAffectionOT0,
        MemoryStatFriendshipHT0 => localization.MemoryStatFriendshipHT0,
        MemoryStatFriendshipOTBaseEvent_0 => localization.MemoryStatFriendshipOTBaseEvent_0,
        MemoryStatFullness_0 => localization.MemoryStatFullness_0,
        MemoryStatFullnessLEQ_0 => localization.MemoryStatFullnessLEQ_0,
        MemoryStatEnjoyment_0 => localization.MemoryStatEnjoyment_0,

        MetDetailTimeOfDay => localization.MetDetailTimeOfDay,
        MoveKeldeoMismatch => localization.MoveKeldeoMismatch,
        MovesShouldMatchRelearnMoves => localization.MovesShouldMatchRelearnMoves,
        MoveShopAlphaMoveShouldBeMastered_0 => localization.MoveShopAlphaMoveShouldBeMastered_0,
        MoveShopAlphaMoveShouldBeOther => localization.MoveShopAlphaMoveShouldBeOther,
        MoveShopAlphaMoveShouldBeZero => localization.MoveShopAlphaMoveShouldBeZero,
        NickFlagEggNo => localization.NickFlagEggNo,
        NickFlagEggYes => localization.NickFlagEggYes,
        NickInvalidChar => localization.NickInvalidChar,
        NickLengthLong => localization.NickLengthLong,
        NickLengthShort => localization.NickLengthShort,
        NickMatchLanguage => localization.NickMatchLanguage,
        NickMatchLanguageEgg => localization.NickMatchLanguageEgg,
        NickMatchLanguageEggFail => localization.NickMatchLanguageEggFail,
        NickMatchLanguageFail => localization.NickMatchLanguageFail,
        NickMatchLanguageFlag => localization.NickMatchLanguageFlag,
        NickMatchNoOthers => localization.NickMatchNoOthers,
        NickMatchNoOthersFail => localization.NickMatchNoOthersFail,
        OTLanguage => localization.OTLanguage,
        OTLanguageShouldBe_0 => localization.OTLanguageShouldBe_0,
        OTLanguageShouldBe_0or1 => localization.OTLanguageShouldBe_0or1,
        OTLanguageShouldBeLeq_0 => localization.OTLanguageShouldBeLeq_0,
        OTLanguageCannotPlayOnVersion_0 => localization.OTLanguageCannotPlayOnVersion_0,
        OTLanguageCannotTransferToConsoleRegion_0 => localization.OTLanguageCannotTransferToConsoleRegion_0,
        OTLong => localization.OTLong,
        OTShort => localization.OTShort,
        OTSuspicious => localization.OTSuspicious,
        OT_IDEqual => localization.OT_IDEqual,
        OT_IDs0 => localization.OT_IDs0,
        OT_SID0 => localization.OT_SID0,
        OT_SID0Invalid => localization.OT_SID0Invalid,
        OT_TID0 => localization.OT_TID0,
        OT_IDInvalid => localization.OT_IDInvalid,
        PIDEncryptWurmple => localization.PIDEncryptWurmple,
        PIDEncryptZero => localization.PIDEncryptZero,
        PIDEqualsEC => localization.PIDEqualsEC,
        PIDGenderMatch => localization.PIDGenderMatch,
        PIDGenderMismatch => localization.PIDGenderMismatch,
        PIDNatureMatch => localization.PIDNatureMatch,
        PIDNatureMismatch => localization.PIDNatureMismatch,
        PIDTypeMismatch => localization.PIDTypeMismatch,
        PIDZero => localization.PIDZero,
        RibbonAllValid => localization.RibbonAllValid,
        RibbonEgg => localization.RibbonEgg,
        StatDynamaxInvalid => localization.StatDynamaxInvalid,
        StatIncorrectHeight => localization.StatIncorrectHeight,
        StatIncorrectHeightCopy => localization.StatIncorrectHeightCopy,
        StatIncorrectHeightValue => localization.StatIncorrectHeightValue,
        StatIncorrectWeight => localization.StatIncorrectWeight,
        StatIncorrectWeightValue => localization.StatIncorrectWeightValue,
        StatInvalidHeightWeight => localization.StatInvalidHeightWeight,
        StatIncorrectCP_0 => localization.StatIncorrectCP,
        StatGigantamaxInvalid => localization.StatGigantamaxInvalid,
        StatGigantamaxValid => localization.StatGigantamaxValid,
        StatNatureInvalid => localization.StatNatureInvalid,
        StatBattleVersionInvalid => localization.StatBattleVersionInvalid,
        StatNobleInvalid => localization.StatNobleInvalid,
        StatAlphaInvalid => localization.StatAlphaInvalid,
        StoredSourceEgg => localization.StoredSourceEgg,
        SuperComplete => localization.SuperComplete,
        SuperDistro => localization.SuperDistro,
        SuperEgg => localization.SuperEgg,
        SuperNoComplete => localization.SuperNoComplete,
        SuperNoUnlocked => localization.SuperNoUnlocked,
        SuperUnavailable => localization.SuperUnavailable,
        SuperUnused => localization.SuperUnused,
        TeraTypeIncorrect => localization.TeraTypeIncorrect,
        TeraTypeMismatch => localization.TeraTypeMismatch,
        TradeNotAvailable => localization.TradeNotAvailable,
        TrainerIDNoSeed => localization.TrainerIDNoSeed,
        TransferBad => localization.TransferBad,
        TransferCurrentHandlerInvalid => localization.TransferCurrentHandlerInvalid,
        TransferEgg => localization.TransferEgg,
        TransferEggLocationTransporter => localization.TransferEggLocationTransporter,
        TransferEggMetLevel => localization.TransferEggMetLevel,
        TransferEggVersion => localization.TransferEggVersion,
        TransferFlagIllegal => localization.TransferFlagIllegal,
        TransferHandlerFlagRequired => localization.TransferHTFlagRequired,
        TransferHandlerMismatchName => localization.TransferHTMismatchName,
        TransferHandlerMismatchGender => localization.TransferHTMismatchGender,
        TransferHandlerMismatchLanguage => localization.TransferHTMismatchLanguage,
        TransferMet => localization.TransferMet,
        TransferNotPossible => localization.TransferNotPossible,
        TransferMetLocation => localization.TransferMetLocation,
        TransferNature => localization.TransferNature,
        TransferObedienceLevel => localization.TransferObedienceLevel,
        TransferKoreanGen4 => localization.TransferKoreanGen4,
        TransferEncryptGen6BitFlip => localization.TransferPIDECBitFlip,
        TransferEncryptGen6Equals => localization.TransferPIDECEquals,
        TransferEncryptGen6Xor => localization.TransferPIDECXor,
        TransferTrackerMissing => localization.TransferTrackerMissing,
        TransferTrackerShouldBeZero => localization.TransferTrackerShouldBeZero,
        TrashBytesExpected => localization.TrashBytesExpected,
        TrashBytesMismatchInitial => localization.TrashBytesMismatchInitial,
        TrashBytesMissingTerminator => localization.TrashBytesMissingTerminator,
        TrashBytesShouldBeEmpty => localization.TrashBytesShouldBeEmpty,
        WordFilterInvalidCharacter_0 => localization.WordFilterInvalidCharacter_0,
        WordFilterFlaggedPattern_01 => localization.WordFilterFlaggedPattern_01,
        WordFilterTooManyNumbers_0 => localization.WordFilterTooManyNumbers_0,
        BulkCloneDetectedDetails => localization.BulkCloneDetectedDetails,
        BulkCloneDetectedTracker => localization.BulkCloneDetectedTracker,
        BulkSharingEncryptionConstantGenerationSame => localization.BulkSharingEncryptionConstantGenerationSame,
        BulkSharingEncryptionConstantGenerationDifferent => localization.BulkSharingEncryptionConstantGenerationDifferent,
        BulkSharingEncryptionConstantEncounterType => localization.BulkSharingEncryptionConstantRNGType,
        BulkSharingPIDGenerationDifferent => localization.BulkSharingPIDGenerationDifferent,
        BulkSharingPIDGenerationSame => localization.BulkSharingPIDGenerationSame,
        BulkSharingPIDEncounterType => localization.BulkSharingPIDRNGType,
        BulkDuplicateMysteryGiftEggReceived => localization.BulkDuplicateMysteryGiftEggReceived,
        BulkSharingTrainerIDs => localization.BulkSharingTrainerID,
        BulkSharingTrainerVersion => localization.BulkSharingTrainerVersion,

        >= MAX => throw new ArgumentOutOfRangeException(nameof(code), code, null),
    };
}
