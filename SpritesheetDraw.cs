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

            for (int i = 0; i < currentSpritesheet.Count; i++)
            {
                Texture2D layer = currentSpritesheet[i];
                int frameWidth = layer.Width;
                int frameHeight = layer.Height;

                if (columns > 0)
                {
                    int candidate = layer.Width / columns;
                    if (candidate > 0) frameWidth = candidate;
                }
                if (rows > 0)
                {
                    int candidate = layer.Height / rows;
                    if (candidate > 0) frameHeight = candidate;
                }

                if (frameWidth > layer.Width) frameWidth = layer.Width;
                if (frameHeight > layer.Height) frameHeight = layer.Height;

                int srcX = frame * frameWidth;
                int srcY = directionRow * frameHeight;

                if (srcX < 0) srcX = 0;
                if (srcY < 0) srcY = 0;
                if (srcX + frameWidth > layer.Width) srcX = Math.Max(0, layer.Width - frameWidth);
                if (srcY + frameHeight > layer.Height) srcY = Math.Max(0, layer.Height - frameHeight);

                Rectangle sourceRect = new Rectangle(srcX, srcY, frameWidth, frameHeight);
                spriteBatch.Draw(layer, rectangle, sourceRect, Color.White);
            }
        }
    }
}
