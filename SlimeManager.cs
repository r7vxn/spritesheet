using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spritesheet
{
    public class SlimeManager
    {
        List<SlimeDraw>slimeDraw;

        public SlimeManager(List<List<Texture2D>> slimelist)
        {
            slimeDraw = new List<SlimeDraw>();
            slimeDraw.Add(new SlimeDraw(slimelist));
        }

        public void Draw(SpriteBatch spriteBatch, SlimeAnimation slimestate, int slimeframe, Rectangle rectangle, int slimedirectionRow, int slimecolumns, int slimerows)
        {
            foreach (var sheet in slimeDraw)
            {
                sheet.Draw(spriteBatch, rectangle, slimedirectionRow, slimestate, slimeframe, slimecolumns, slimerows);
            }
        }
    }
}
