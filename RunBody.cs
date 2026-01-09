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
    public class RunBody
    {
        

        Rectangle playerCollisionRect, playerDrawRect;

        KeyboardState keyboardState;
        Vector2 playerDirection;
        Vector2 playerLocation;


        Texture2D runbodySpritesheet, runheadSpritesheet, runswordSpritesheet, runswordbackSpritesheet, runshadowSpritesheet;
        Texture2D rectangleTexture;

        float time; // Used to store elapsed time
        float frameSpeed; // Sets how fast player frames transition
        float speed = 5f;

        public RunBody(Texture2D runbodySpritesheet, Texture2D runheadSpritesheet, Texture2D runswordSpritesheet, Texture2D runswordbackSpritesheet, Texture2D runshadowSpritesheet, Texture2D rectangleTexture)
        {
            this.runbodySpritesheet = runbodySpritesheet;
            this.runheadSpritesheet = runheadSpritesheet;
            this.runswordSpritesheet = runswordSpritesheet;
            this.runswordbackSpritesheet = runswordbackSpritesheet;
            this.runshadowSpritesheet = runshadowSpritesheet;
            this.rectangleTexture = rectangleTexture;

            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(80, 60, 40, 70);
            playerDrawRect = new Rectangle(20, 20, 150, 150);
            UpdatePlayerRects();
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

        public void Update(GameTime gameTime)
        {
            SetPlayerDirection();
            playerLocation += playerDirection * speed;
            UpdatePlayerRects();
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > frameSpeed && playerDirection != Vector2.Zero)
            {
                time = 0f;
                frame += 1;
                if (frame >= frames)
                    frame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(runbodySpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runheadSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runswordSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runswordbackSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(runshadowSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.3f);
        }
        private void SetPlayerDirection()
        {
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
                playerDirection.Normalize();
                if (playerDirection.X < 0) // Moving left
                    directionRow = leftRow;

                else if (playerDirection.X > 0) // Moving right
                    directionRow = rightRow;

                else if (playerDirection.Y < 0) // Moving up
                    directionRow = downRow;

                else
                    directionRow = upRow;

            }
            else
                frame = 0;
        }
        public void UpdatePlayerRects()
        {
            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.X = playerCollisionRect.X - 55;
            playerDrawRect.Y = playerCollisionRect.Y - 40;
        }
    }
}
