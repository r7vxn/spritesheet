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
        //This class is used for loading in the spritesheet and drawing them

        int rows, columns;
        int frame; // The frame number (column) in the sequence to draw
        int frames; // The number of frames for each direction, usually the same as columns
        int directionRow; // The row number containing the frames for the current direction
        //int leftRow, rightRow, upRow, downRow; // Row number of directional set of frames
        int width; // The width of each frame
        int height; // The height of each frame

        List<List<Texture2D>> spritesheets;
        List<Texture2D> spritesheet;

        Animation currentAnimation;
        enum Animation
        {
            None = 0,
            Idle = 1,
            Running = 2
        }
        public SpritesheetDraw(List<List<Texture2D>>wholelist)
        {
            this.spritesheets = wholelist;
            this.spritesheet = wholelist[0];

            columns = 8;
            rows = 4;
            //upRow = 3;
            //leftRow = 1;
            //rightRow = 2;
            //downRow = 0;
            //directionRow = downRow; // Player will start facing down
            width = spritesheet[0].Width / columns;
            height = spritesheet[0].Height / rows;
            frames = 8;
            frame = 0;
        }
        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle, int directionRow)
        {
            this.directionRow = directionRow;
            foreach (var item in spritesheet)
            {
                spriteBatch.Draw(item, rectangle, new Rectangle(frame * width, directionRow * height, width, height), Color.White);

            }
        }
    }
}
