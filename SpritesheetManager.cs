using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spritesheet
{
    public class SpritesheetManager
    {
        SpritesheetDraw spritesheetDraw;
        Rectangle rectangle;
        public SpritesheetManager(SpritesheetDraw spritesheetDraw)
        { 

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spritesheetDraw.Draw(spriteBatch, rectangle);
        }
    }
}
