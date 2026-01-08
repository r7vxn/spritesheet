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

        Texture2D runattackbodySpritesheet;
        Texture2D runattackheadSpritesheet;
        Texture2D runattackshadowSpritesheet;
        Texture2D runattackswingSpritesheet;
        Texture2D runattackswordSpritesheet;
        Texture2D runattackswordbackSpritesheet;
        

        RunBody playerBody;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            playerBody = new RunBody(runbodySpritesheet, runheadSpritesheet, runswordSpritesheet, runswordbackSpritesheet, runshadowSpritesheet, rectangleTexture);

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

            runattackbodySpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_body");
            runattackheadSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Attack_head");
            runattackshadowSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Attack_shadow");
            runattackswingSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Attack_swing");
            runattackswordSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Attack_sword");
            runattackswordbackSpritesheet = Content.Load<Texture2D>("Swordsman_lvl1_Attack_sword_back");

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            playerBody.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            playerBody.Draw(_spriteBatch);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
