/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Board class.
///
/// @author Nuno Fachada
/// @date 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using Xunit;
using ColorShapeLinks.Common;
using ColorShapeLinks.TextBased.Lib;

namespace Tests.Lib
{
    public class WinnerExtensionsTests
    {
        public static IEnumerable<object[]> GetRelations()
        {
            yield return new object[] { Winner.Draw, ExitStatus.Draw };
            yield return new object[] { Winner.Red, ExitStatus.RedWins };
            yield return new object[] { Winner.White, ExitStatus.WhiteWins };
            yield return new object[] { int.MaxValue, ExitStatus.Exception };
            yield return new object[] { -1, ExitStatus.Exception };
        }

        [Theory]
        [MemberData(nameof(GetRelations))]
        public void ToExitStatus_ReturnExpected_Yes(Winner winner, ExitStatus es)
        {
            Assert.Equal(es, winner.ToExitStatus());
        }
    }
}
