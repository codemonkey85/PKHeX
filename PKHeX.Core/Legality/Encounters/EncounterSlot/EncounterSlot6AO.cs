namespace PKHeX.Core
{
    /// <summary>
    /// Encounter Slot found in <see cref="GameVersion.ORAS"/>.
    /// </summary>
    /// <inheritdoc cref="EncounterSlot"/>
    public sealed record EncounterSlot6AO : EncounterSlot
    {
        public override int Generation => 6;
        public bool CanDexNav => Area.Type != SlotType.Rock_Smash;

        public bool Pressure { get; init; }
        public bool DexNav { get; init; }
        public bool WhiteFlute { get; init; }
        public bool BlackFlute { get; init; }

        public EncounterSlot6AO(EncounterArea6AO area, int species, int form, int min, int max) : base(area, species, form, min, max)
        {
        }

        protected override void SetFormatSpecificData(PKM pk)
        {
            PK6? pk6 = (PK6)pk;
            if (CanDexNav)
            {
                int[]? eggMoves = GetDexNavMoves();
                if (eggMoves.Length > 0)
                    pk6.RelearnMove1 = eggMoves[Util.Rand.Next(eggMoves.Length)];
            }
            pk6.SetRandomMemory6();
            pk6.SetRandomEC();
        }

        public override string GetConditionString(out bool valid)
        {
            valid = true;
            if (WhiteFlute) // Decreased Level Encounters
                return Pressure ? LegalityCheckStrings.LEncConditionWhiteLead : LegalityCheckStrings.LEncConditionWhite;
            if (BlackFlute) // Increased Level Encounters
                return Pressure ? LegalityCheckStrings.LEncConditionBlackLead : LegalityCheckStrings.LEncConditionBlack;
            if (DexNav)
                return LegalityCheckStrings.LEncConditionDexNav;

            return Pressure ? LegalityCheckStrings.LEncConditionLead : LegalityCheckStrings.LEncCondition;
        }

        protected override HiddenAbilityPermission IsHiddenAbilitySlot() => CanDexNav || Area.Type == SlotType.Horde ? HiddenAbilityPermission.Possible : HiddenAbilityPermission.Never;

        private int[] GetDexNavMoves()
        {
            EvolutionTree? et = EvolutionTree.GetEvolutionTree(6);
            int sf = et.GetBaseSpeciesForm(Species, Form);
            return MoveEgg.GetEggMoves(6, sf & 0x7FF, sf >> 11, Version);
        }

        public bool CanBeDexNavMove(int move)
        {
            int[]? baseEgg = GetDexNavMoves();
            return System.Array.IndexOf(baseEgg, move) >= 0;
        }
    }
}
