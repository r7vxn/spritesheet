using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace spritesheet
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Texture2D runbodySpritesheet;
        Texture2D runheadSpritesheet;
        Texture2D runshadowSpritesheet;
        Texture2D runswordSpritesheet;
        Texture2D runswordbackSpritesheet;
        Texture2D rectangleTexture;

        RunBody playerBody;

        KeyboardState keyboardState;
        int rows, columns;
        int frame; // The frame number (column) in the sequence to draw
        int frames; // The number of frames for each direction, usually the same as columns
        int directionRow; // The row number containing the frames for the current direction
        int leftRow, rightRow, upRow, downRow; // Row number of directional set of frames
        int width; // The width of each frame
        int height; // The height of each frame
        float speed = 5f; // How fast the character sprite will travel
        float time; // Used to store elapsed time
        float frameSpeed; // Sets how fast player frames transition
        Vector2 playerLocation; // Stored the location of the players collision sprite
        Vector2 playerDirection; // The directional vector of the player
        Rectangle playerCollisionRect, playerDrawRect;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //columns = 8;
            //rows = 4;
            //upRow = 0;
            //leftRow = 1;
            //rightRow = 2;
            //downRow = 3;
            base.Initialize();
            //directionRow = downRow; // Player will start facing down
            //width = runbodySpritesheet.Width / columns;
            //height = runbodySpritesheet.Height / rows;
            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(80, 60, 40, 70);
            playerDrawRect = new Rectangle(20, 20, 150, 150);
            UpdatePlayerRects();

            playerBody = new RunBody(runbodySpritesheet);

            //time = 0.0f;
            //frameSpeed = 0.1f;
            //frames = 8;
            //frame = 0;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            runbodySpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Run_body");
            runheadSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Run_head");
            runshadowSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Run_shadow");
            runswordSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Run_sword");
            runswordbackSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Run_sword_back");
            rectangleTexture = Content.Load<Texture2D>("rectangle");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > frameSpeed && playerDirection != Vector2.Zero)
            {
                time = 0f;
                frame += 1;
                if (frame >= frames)
                    frame = 0;
            }
            keyboardState = Keyboard.GetState();

            SetPlayerDirection();
            playerLocation += playerDirection * speed;
            UpdatePlayerRects();

            playerBody.Update(gameTime, playerDirection);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.3f);
            _spriteBatch.Draw(runbodySpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            _spriteBatch.Draw(runheadSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            _spriteBatch.Draw(runswordSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            _spriteBatch.Draw(runswordbackSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            _spriteBatch.Draw(runshadowSpritesheet, playerDrawRect, new Rectangle(frame * width, directionRow * height, width, height), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        public void UpdatePlayerRects()
        {
            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.X = playerCollisionRect.X - 55;
            playerDrawRect.Y = playerCollisionRect.Y - 40;
        }
        private void SetPlayerDirection()
        {
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
                    directionRow = upRow;

                else
                    directionRow = downRow;

            }
            else
                frame = 0;
        }
    }
}
