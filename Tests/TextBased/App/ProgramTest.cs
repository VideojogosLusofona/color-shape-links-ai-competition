/// @file
/// @brief This file contains the ::ColorShapeLinks.Common.Board class.
///
/// @author Nuno Fachada
/// @date 2021
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;
using ColorShapeLinks.TextBased.Lib;
using ColorShapeLinks.TextBased.App;

namespace App
{
    public class ProgramTest
    {
        public static IEnumerable<object[]> GetInvalidArgs()
        {
            yield return new object[] { new string[] { "" } };
            yield return new object[] { new string[] { "lalala" } };
            yield return new object[] { new string[] { "multi", "word", "invalid" } };
        }

        [Theory]
        [MemberData(nameof(GetInvalidArgs))]
        public void Main_Args_Invalid(object[] args)
        {
            Type type = typeof(Program);
            MethodInfo info = type.GetMethod(
                "Main", BindingFlags.NonPublic | BindingFlags.Static);

            int value = (int)info.Invoke(null, new object[] { args } );

            Assert.Equal((int)ExitStatus.Exception, value);
        }
    }
}
