using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace spritesheet
{
    internal class PlayerManager
    {
        private SpritesheetManager spritesheetManager;
        private Dictionary<Animation, Dictionary<int, int>> framesPerDirection;
        private Dictionary<Animation, int> rowsPerState;

        public PlayerManager(SpritesheetManager manager, Dictionary<Animation, Dictionary<int, int>> framesPerDirection, Dictionary<Animation, int> rowsPerState)
        {
            this.spritesheetManager = manager;
            this.framesPerDirection = framesPerDirection;
            this.rowsPerState = rowsPerState;
        }

        public void Draw(SpriteBatch spriteBatch, PlayerAnimationClass player)
        {
            var state = player.CurrentState;
            var frame = player.CurrentFrame;
            var rect = player.CurrentDrawRect;
            var directionRow = player.CurrentDirectionRow;

            // Defensive: framesPerDirection or rowsPerState may be null or missing keys at runtime.
            int columns = 1;
            if (framesPerDirection != null && framesPerDirection.TryGetValue(state, out var dirDict) && dirDict != null)
            {
                if (!dirDict.TryGetValue(directionRow, out columns))
                {
                    columns = 1;
                }
            }

            int rows = 1;
            if (rowsPerState != null && rowsPerState.TryGetValue(state, out var r))
            {
                rows = r;
            }

            spritesheetManager.Draw(spriteBatch, state, frame, rect, directionRow, Math.Max(1, columns), Math.Max(1, rows));
        }
    }
}
