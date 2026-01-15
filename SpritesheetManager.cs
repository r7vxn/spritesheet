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
    public class SpritesheetManager
    {
        List<SpritesheetDraw> spritesheetDraw;
        Rectangle rectangle;

        public Rectangle Rectangle
        {
            get => rectangle;
            set => rectangle = value;
        }

        public SpritesheetManager(List<List<Texture2D>> wholelist)
        {
            spritesheetDraw = new List<SpritesheetDraw>();
            spritesheetDraw.Add(new SpritesheetDraw(wholelist));
        }

        public void Draw(SpriteBatch spriteBatch, Animation state, int frame, Rectangle rectangle, int directionRow, int columns, int rows)
        {
            foreach (var sheet in spritesheetDraw)
            {
                sheet.Draw(spriteBatch, rectangle, directionRow, state, frame, columns, rows);
            }
        }
    }
}
