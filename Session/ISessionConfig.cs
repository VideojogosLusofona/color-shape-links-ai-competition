/// @file
/// @brief This file contains the
/// ::ColorShapeLinks.Common.Session.ISessionConfig interface.
///
/// @author Nuno Fachada
/// @date 2020
/// @copyright [MPLv2](http://mozilla.org/MPL/2.0/)

namespace ColorShapeLinks.Common.Session
{
    /// <summary>
    /// Defines a session configuration, points per win, per loss, etc.
    /// </summary>
    public interface ISessionConfig
    {
        /// <summary>Points per win.</summary>
        int PointsPerWin { get; }

        /// <summary>Points per loss.</summary>
        int PointsPerLoss { get; }

        /// <summary>Points per draw.</summary>
        int PointsPerDraw { get; }
    }
}
