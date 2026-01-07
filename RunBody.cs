using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spritesheet
{
    public class RunBody
    {
        int rows, columns;
        int frame; // The frame number (column) in the sequence to draw
        int frames; // The number of frames for each direction, usually the same as columns
        int directionRow; // The row number containing the frames for the current direction
        int leftRow, rightRow, upRow, downRow; // Row number of directional set of frames
        int width; // The width of each frame
        int height; // The height of each frame

        Texture2D runbodySpritesheet, ;

        float time; // Used to store elapsed time
        float frameSpeed; // Sets how fast player frames transition

        public RunBody(Texture2D runbodySpritesheet)
        {
            this.runbodySpritesheet = runbodySpritesheet;
            columns = 8;
            rows = 4;
            upRow = 0;
            leftRow = 1;
            rightRow = 2;
            downRow = 3;
            directionRow = downRow; // Player will start facing down
            width = runbodySpritesheet.Width / columns;
            height = runbodySpritesheet.Height / rows;
            time = 0.0f;
            frameSpeed = 0.1f;
            frames = 8;
            frame = 0;
        }

        public void Update(GameTime gameTime, Vector2 direction)
        {
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > frameSpeed && direction != Vector2.Zero)
            {
                time = 0f;
                frame += 1;
                if (frame >= frames)
                    frame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle location)
        {
            spriteBatch.Draw(runbodySpritesheet, location, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runheadSpritesheet, location, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runswordSpritesheet, location, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runswordbackSpritesheet, location, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runshadowSpritesheet, location, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
        }

    }
}
