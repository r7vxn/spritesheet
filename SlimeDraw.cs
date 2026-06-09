using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spritesheet
{
    public class SlimeDraw
    {
        List<List<Texture2D>> slimespritesheets;

        public SlimeDraw(List<List<Texture2D>> slimelist)
        {
            slimespritesheets = slimelist;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle slimeRectangle, int slimedirectionRow, SlimeAnimation slimestate, int slimeFrame, int slimeColumns, int slimeRows)
        {

            List<Texture2D> slimeCurrentSpritesheet = slimespritesheets[(int)slimestate];

            for (int i = 0; i < slimeCurrentSpritesheet.Count; i++)
            {
                Texture2D slimelayer = slimeCurrentSpritesheet[i];
                int frameWidth = slimelayer.Width;
                int frameHeight = slimelayer.Height;

                if (slimeColumns > 0)
                {
                    int candidate = slimelayer.Width / slimeColumns;
                    if (candidate > 0) frameWidth = candidate;
                }
                if (slimeRows > 0)
                {
                    int candidate = slimelayer.Height / slimeRows;
                    if (candidate > 0) frameHeight = candidate;
                }

                if (frameWidth > slimelayer.Width) frameWidth = slimelayer.Width;
                if (frameHeight > slimelayer.Height) frameHeight = slimelayer.Height;

                int srcX = slimeFrame * frameWidth;
                int srcY = slimedirectionRow * frameHeight;

                if (srcX < 0) srcX = 0;
                if (srcY < 0) srcY = 0;
                if (srcX + frameWidth > slimelayer.Width) srcX = Math.Max(0, slimelayer.Width - frameWidth);
                if (srcY + frameHeight > slimelayer.Height) srcY = Math.Max(0, slimelayer.Height - frameHeight);

                Rectangle slimesourceRect = new Rectangle(srcX, srcY, frameWidth, frameHeight);
                spriteBatch.Draw(slimelayer, slimeRectangle, slimesourceRect, Color.White);
            }
        }
    }
}

