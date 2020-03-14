using ColorShapeLinks.Common;
using Xunit;

namespace Tests.Common
{
    public class WinnerTests
    {
        [Theory]
        [InlineData(Winner.White, PColor.White)]
        [InlineData(Winner.Red, PColor.Red)]
        public void ToPColor_CorrectPColor_Yes(Winner winner, PColor color)
        {
            Assert.Equal(color, winner.ToPColor());
        }

        [Theory]
        [InlineData(Winner.Red, PColor.White)]
        [InlineData(Winner.None, PColor.White)]
        [InlineData(Winner.Draw, PColor.White)]
        [InlineData(Winner.White, PColor.Red)]
        [InlineData(Winner.None, PColor.Red)]
        [InlineData(Winner.Draw, PColor.Red)]
        public void ToPColor_CorrectPColor_No(Winner winner, PColor color)
        {
            Assert.NotEqual(color, winner.ToPColor());
        }
    }
}
