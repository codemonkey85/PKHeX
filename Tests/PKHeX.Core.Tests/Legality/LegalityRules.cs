using FluentAssertions;
using PKHeX.Core;
using Xunit;

namespace PKHeX.Tests.Legality
{
    public class LegalityRules
    {
        [Theory]
        [InlineData(GameVersion.B)]
        public void HasOriginalMetLocation5(GameVersion g)
        {
            PK5? pk5 = new PK5 {Version = (int) g};
            pk5.HasOriginalMetLocation.Should().BeTrue();
        }

        [Theory]
        [InlineData(GameVersion.B)]
        [InlineData(GameVersion.X)]
        public void HasOriginalMetLocation6(GameVersion g)
        {
            PK6? pk5 = new PK6 { Version = (int)g };
            pk5.HasOriginalMetLocation.Should().BeTrue();
        }

        [Theory]
        [InlineData(GameVersion.B)]
        [InlineData(GameVersion.X)]
        [InlineData(GameVersion.SN)]
        public void HasOriginalMetLocation7(GameVersion g)
        {
            PK7? pk5 = new PK7 { Version = (int)g };
            pk5.HasOriginalMetLocation.Should().BeTrue();
        }
    }
}