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
                int slimewidth = slimelayer.Width / slimeColumns;
                int slimeheight = slimelayer.Height / slimeRows;

                Rectangle slimesourceRect = new Rectangle(slimeFrame * slimewidth, slimedirectionRow * slimeheight, slimewidth, slimeheight);
                spriteBatch.Draw(slimelayer, slimeRectangle, slimesourceRect, Color.White);
            }
        }
    }
}

