﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blish_HUD.Utils {

    public static class World {

        private const float INCH_TO_METER = 0.0254f;

        /// <summary>
        /// Converts a world (meters) coordinate to game (inches) coordinate.
        /// </summary>
        public static float WorldToGameCoord(float worldCoord) {
            return worldCoord / INCH_TO_METER;
        }

        /// <summary>
        /// Converts a game (inches) coordinate to world (meters) coordinate.
        ///
        /// World coordinates are GW2 coordinates.  Game coordinates are Blish HUD coordinates.
        /// </summary>
        public static float GameToWorldCoord(float gameCoord) {
            return gameCoord * INCH_TO_METER;
        }

    }

}
