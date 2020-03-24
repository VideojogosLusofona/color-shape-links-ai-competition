/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Board class.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System.Collections.Generic;
using ColorShapeLinks.Common;
using Xunit;

namespace Tests.Common
{
    public class PColorTests
    {
        public static IEnumerable<object[]> GetNameFormatValues()
        {
            yield return new object[] { PColor.Red, "Name One" };
            yield return new object[] { PColor.White, "Name Two" };
            yield return new object[] { PColor.Red, "lowercase_withunderscore" };
            yield return new object[] { PColor.White, "UPPER-CASE-WITH-DASH" };
        }

        public static IEnumerable<object[]> GetFriends()
        {
            yield return new object[] {
                PColor.White, new Piece(PColor.White, PShape.Round) };
            yield return new object[] {
                PColor.White, new Piece(PColor.White, PShape.Square) };
            yield return new object[] {
                PColor.White, new Piece(PColor.Red, PShape.Round) };
            yield return new object[] {
                PColor.Red, new Piece(PColor.Red, PShape.Square) };
            yield return new object[] {
                PColor.Red, new Piece(PColor.Red, PShape.Round) };
            yield return new object[] {
                PColor.Red, new Piece(PColor.White, PShape.Square) };
        }

        public static IEnumerable<object[]> GetEnemies()
        {
            yield return new object[] {
                PColor.White, new Piece(PColor.Red, PShape.Square) };
            yield return new object[] {
                PColor.Red, new Piece(PColor.White, PShape.Round) };
        }

        [Theory]
        [MemberData(nameof(GetNameFormatValues))]
        public void FormatName_ShouldContainName_Always(
            PColor color, string name)
        {
            string formatedName = color.FormatName(name);
            Assert.Contains(name, formatedName);
        }

        [Theory]
        [MemberData(nameof(GetNameFormatValues))]
        public void FormatName_ShouldContainColor_Always(
            PColor color, string name)
        {
            string formatedName = color.FormatName(name);
            Assert.Contains(color.ToString(), formatedName);
        }

        [Fact]
        public void Other_OfWhite_IsRed()
        {
            Assert.Equal(PColor.Red, PColor.White.Other());
        }

        [Fact]
        public void Other_OfRed_IsWhite()
        {
            Assert.Equal(PColor.White, PColor.Red.Other());
        }

        [Fact]
        public void Shape_OfWhite_IsRound()
        {
            Assert.Equal(PShape.Round, PColor.White.Shape());
        }

        [Fact]
        public void Shape_OfRed_IsSquare()
        {
            Assert.Equal(PShape.Square, PColor.Red.Shape());
        }

        [Theory]
        [MemberData(nameof(GetFriends))]
        public void FriendOf_IsFriend_Yes(PColor color, Piece piece)
        {
            Assert.True(color.FriendOf(piece));
        }

        [Theory]
        [MemberData(nameof(GetEnemies))]
        public void FriendOf_IsFriend_No(PColor color, Piece piece)
        {
            Assert.False(color.FriendOf(piece));
        }

        [Theory]
        [InlineData(PColor.White, Winner.White)]
        [InlineData(PColor.Red, Winner.Red)]
        public void ToWinner_CorrectWinner_Yes(PColor color, Winner winner)
        {
            Assert.Equal(winner, color.ToWinner());
        }

        [Theory]
        [InlineData(PColor.White, Winner.Red)]
        [InlineData(PColor.White, Winner.None)]
        [InlineData(PColor.White, Winner.Draw)]
        [InlineData(PColor.Red, Winner.White)]
        [InlineData(PColor.Red, Winner.None)]
        [InlineData(PColor.Red, Winner.Draw)]
        public void ToWinner_CorrectWinner_No(PColor color, Winner winner)
        {
            Assert.NotEqual(winner, color.ToWinner());
        }
    }
}
