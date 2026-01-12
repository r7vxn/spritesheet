using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
namespace spritesheet
{
public class RunBody
{





    Texture2D runbodySpritesheet, runheadSpritesheet, runswordSpritesheet, runswordbackSpritesheet, runshadowSpritesheet;
    Texture2D rectangleTexture;



    public RunBody(Texture2D runbodySpritesheet, Texture2D runheadSpritesheet, Texture2D runswordSpritesheet, Texture2D runswordbackSpritesheet, Texture2D runshadowSpritesheet, Texture2D rectangleTexture)
    {
        this.runbodySpritesheet = runbodySpritesheet;
        this.runheadSpritesheet = runheadSpritesheet;
        this.runswordSpritesheet = runswordSpritesheet;
        this.runswordbackSpritesheet = runswordbackSpritesheet;
        this.runshadowSpritesheet = runshadowSpritesheet;
        this.rectangleTexture = rectangleTexture;


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

}
}
*/
