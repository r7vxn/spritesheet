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


        KeyboardState keyboardState;
        int rows, columns;
        int frame; // The frame number (column) in the sequence to draw
        int frames; // The number of frames for each direction, usually the same as columns
        int directionRow; // The row number containing the frames for the current direction
        int leftRow, rightRow, upRow, downRow; // Row number of directional set of frames
        int width; // The width of each frame
        int height; // The height of each frame
        float speed; // How fast the character sprite will travel
        float time; // Used to store elapsed time
        float frameSpeed; // Sets how fast player frames transition
        Vector2 playerLocation; // Stored the location of the players collision sprite
        Vector2 playerDirection; // The directional vector of the player
        Rectangle playerCollisionRect, playerDrawRect;
        int playerscale = 180;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            columns = 8;
            rows = 4;
            upRow = 0;
            leftRow = 1;
            rightRow = 2;
            downRow = 3;
            base.Initialize();
            directionRow = downRow; // Player will start facing down
            width = runbodySpritesheet.Width / columns;
            height = runbodySpritesheet.Height / rows;
            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(20, 20, playerscale, playerscale);
            playerDrawRect = new Rectangle(20, 20, playerscale, playerscale);
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

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            _spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.3f);
            _spriteBatch.Draw(runbodySpritesheet, playerDrawRect, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.Draw(runheadSpritesheet, playerDrawRect, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.Draw(runswordSpritesheet, playerDrawRect, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.Draw(runswordbackSpritesheet, playerDrawRect, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.Draw(runshadowSpritesheet, playerDrawRect, new Rectangle(0, 0, width, height), Color.White);
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
