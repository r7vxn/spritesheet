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
        KeyboardState keyboardState;
        Vector2 playerDirection;
        int directionRow;
        int leftRow, rightRow, downRow, upRow;
        float frame;
        float speed = 5f;
        float time = 0.0f;
        float frameSpeed = 0.1f;
        int frames = 8;


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
        public void Draw(SpriteBatch spriteBatch, Animation state, int frame)
        {
            this.frame = frame;

            foreach (var spritesheet in spritesheetDraw)
            {
                spritesheet.Draw(spriteBatch, rectangle, directionRow, state, frame);
            }
        }
        public void SetPlayerDirection(Rectangle rectangle, KeyboardState keyboardState, Vector2 playerDirection, int directionRow, int leftRow, int rightRow, int downRow, int upRow, float frame)
        {
            
            this.keyboardState = keyboardState;
            this.playerDirection = playerDirection;
            this.directionRow = directionRow;
            this.leftRow = leftRow;
            this.rightRow = rightRow;
            this.downRow = downRow;
            this.upRow = upRow;
            this.frame = frame;
            this.rectangle = rectangle;

            this.playerDirection = new Vector2(20, 20);

            keyboardState = Keyboard.GetState();
            playerDirection = Vector2.Zero;


            if (keyboardState.IsKeyDown(Keys.A))
                playerDirection.X += -1;

            if (keyboardState.IsKeyDown(Keys.D))
                playerDirection.X += 1;

            if (keyboardState.IsKeyDown(Keys.W))
                playerDirection.Y += -1;

            if (keyboardState.IsKeyDown(Keys.S))
                playerDirection.Y += 1;


            if (playerDirection != Vector2.Zero)
            {
                playerDirection = Vector2.Normalize(playerDirection);
                if (playerDirection.X < 0) // Moving left
                    directionRow = leftRow;

                else if (playerDirection.X > 0) // Moving right
                    directionRow = rightRow;

                else if (playerDirection.Y < 0) // Moving up
                    directionRow = upRow;
                else
                    directionRow = downRow;

            }
            else
                frame = 0;
        }

        public void Update (GameTime gameTime, int frame)
        {
            this.frame = frame;
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > frameSpeed && playerDirection != Vector2.Zero)
            {
                time = 0f;
                frame += 1;
                if (frame >= frames)
                    frame = 0;
            }
            keyboardState = Keyboard.GetState();

        }
    }
}
