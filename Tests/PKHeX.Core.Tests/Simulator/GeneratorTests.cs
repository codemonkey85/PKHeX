using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Simulator
{
    public class GeneratorTests
    {
        static GeneratorTests()
        {
            if (!EncounterEvent.Initialized)
                EncounterEvent.RefreshMGDB();
        }

        public static IEnumerable<object[]> PokemonGenerationTestData()
        {
            for (int i = 1; i <= 807; i++)
            {
                yield return new object[] { i };
            }
        }

        [Theory(Skip = "Feature not ready yet")]
        [MemberData(nameof(PokemonGenerationTestData))]
        public void PokemonGenerationReturnsLegalPokemon(int species)
        {
            int count = 0;
            SimpleTrainerInfo? tr = new SimpleTrainerInfo(GameVersion.SN);

            PK7? pk = new PK7 { Species = species };
            pk.Gender = pk.GetSaneGender();
            IEnumerable<Core.PKM>? ez = EncounterMovesetGenerator.GeneratePKMs(pk, tr);
            foreach (Core.PKM? e in ez)
            {
                LegalityAnalysis? la = new LegalityAnalysis(e);
                la.Valid.Should().BeTrue($"Because generated Pokemon {count} for {species:000} should be valid");
                Assert.True(la.Valid);
                count++;
            }
        }

        [Fact]
        public void CanGenerateMG5Case()
        {
            const Species species = Species.Haxorus;
            PK5? pk = new PK5 {Species = (int) species};
            EncounterStatic? ez = EncounterMovesetGenerator.GenerateEncounters(pk, pk.Moves, GameVersion.W2).OfType<EncounterStatic>().First();
            ez.Should().NotBeNull("Shiny Haxorus stationary encounter exists for B2/W2");

            EncounterCriteria? criteria = new EncounterCriteria();
            SimpleTrainerInfo? tr = new SimpleTrainerInfo(GameVersion.B2)
            {
                TID = 57600,
                SID = 62446,
            };
            for (Nature nature = Nature.Hardy; nature <= Nature.Quirky; nature++)
            {
                criteria = criteria with {Nature = nature};
                Core.PKM? pkm = ez.ConvertToPKM(tr, criteria);
                pkm.Nature.Should().Be((int)nature, "not nature locked");
                pkm.IsShiny.Should().BeTrue("encounter is shiny locked");
                pkm.TID.Should().Be(tr.TID);
                pkm.SID.Should().Be(tr.SID);
            }
        }
    }
}