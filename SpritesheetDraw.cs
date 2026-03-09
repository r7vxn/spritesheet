using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace spritesheet
{
    public class SpritesheetDraw
    {
        List<List<Texture2D>> spritesheets;

        public SpritesheetDraw(List<List<Texture2D>> wholelist)
        {
            spritesheets = wholelist;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle, int directionRow, Animation state, int frame, int columns, int rows)
        {

            List<Texture2D> currentSpritesheet = spritesheets[(int)state];
            // Use the first layer as the base reference for frame size so layered sprites align.
            if (currentSpritesheet.Count == 0)
                return;

            int safeColumns = Math.Max(1, columns);
            int safeRows = Math.Max(1, rows);

            Texture2D baseLayer = currentSpritesheet[0];
            int baseFrameWidth = Math.Max(1, baseLayer.Width / safeColumns);
            int baseFrameHeight = Math.Max(1, baseLayer.Height / safeRows);

            for (int i = 0; i < currentSpritesheet.Count; i++)
            {
                Texture2D layer = currentSpritesheet[i];

                // If this layer doesn't match the expected grid (not divisible by columns/rows), skip it to avoid drawing whole sheet
                if (layer.Width % safeColumns != 0 || layer.Height % safeRows != 0)
                {
                    // skip mismatched layer
                    continue;
                }

                int layerFrameWidth = layer.Width / safeColumns;
                int layerFrameHeight = layer.Height / safeRows;

                // Compute max valid indices for this layer
                int maxFrame = Math.Max(0, (layer.Width / layerFrameWidth) - 1);
                int maxRow = Math.Max(0, (layer.Height / layerFrameHeight) - 1);

                int safeFrame = Math.Max(0, Math.Min(frame, maxFrame));
                int safeRow = Math.Max(0, Math.Min(directionRow, maxRow));

                Rectangle sourceRect = new Rectangle(safeFrame * layerFrameWidth, safeRow * layerFrameHeight, layerFrameWidth, layerFrameHeight);

                spriteBatch.Draw(layer, rectangle, sourceRect, Color.White);
            }
        }

        // Debug helper: return sizes of each layer texture for a given animation state
        public List<Point> GetLayerSizes(Animation state)
        {
            var list = new List<Point>();
            var current = spritesheets[(int)state];
            for (int i = 0; i < current.Count; i++)
            {
                var tex = current[i];
                list.Add(new Point(tex.Width, tex.Height));
            }
            return list;
        }

        // Compute the source rectangles that would be used for each layer for debugging.
        public List<Rectangle> ComputeLayerSourceRects(Animation state, int frame, int directionRow, int columns, int rows)
        {
            var rects = new List<Rectangle>();
            var currentSpritesheet = spritesheets[(int)state];
            if (currentSpritesheet.Count == 0)
                return rects;

            int safeColumns = Math.Max(1, columns);
            int safeRows = Math.Max(1, rows);

            for (int i = 0; i < currentSpritesheet.Count; i++)
            {
                Texture2D layer = currentSpritesheet[i];
                if (layer.Width % safeColumns != 0 || layer.Height % safeRows != 0)
                {
                    rects.Add(Rectangle.Empty);
                    continue;
                }

                int layerFrameWidth = layer.Width / safeColumns;
                int layerFrameHeight = layer.Height / safeRows;

                int maxFrame = Math.Max(0, (layer.Width / layerFrameWidth) - 1);
                int maxRow = Math.Max(0, (layer.Height / layerFrameHeight) - 1);

                int safeFrame = Math.Max(0, Math.Min(frame, maxFrame));
                int safeRow = Math.Max(0, Math.Min(directionRow, maxRow));

                var sourceRect = new Rectangle(safeFrame * layerFrameWidth, safeRow * layerFrameHeight, layerFrameWidth, layerFrameHeight);
                rects.Add(sourceRect);
            }

            return rects;
        }
    }
}