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
            if ((int)state >= spritesheets.Count) return;

            List<Texture2D> currentSpritesheet = spritesheets[(int)state];

            for (int i = 0; i < currentSpritesheet.Count; i++)
            {
                Texture2D layer = currentSpritesheet[i];
                int width = layer.Width / columns;
                int height = layer.Height / rows;

                Rectangle sourceRect = new Rectangle(frame * width, directionRow * height, width, height);
                spriteBatch.Draw(layer, rectangle, sourceRect, Color.White);
            }
        }
    }
}