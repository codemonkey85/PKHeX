using static PKHeX.Core.LegalityCheckStrings;

namespace PKHeX.Core
{
    /// <summary>
    /// Verifies the <see cref="PK4.EncounterType"/>.
    /// </summary>
    public sealed class EncounterTypeVerifier : Verifier
    {
        protected override CheckIdentifier Identifier => CheckIdentifier.Encounter;

        public override void Verify(LegalityAnalysis data)
        {
            EncounterType type = data.EncounterMatch is IEncounterTypeTile t ? t.TypeEncounter : EncounterType.None;
            CheckResult? result = !type.Contains(data.pkm.EncounterType) ? GetInvalid(LEncTypeMismatch) : GetValid(LEncTypeMatch);
            data.AddLine(result);
        }
    }
}
