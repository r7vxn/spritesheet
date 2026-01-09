using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace spritesheet
{
    public class Game1 : Game
    {

        //make a class for drawing spritesheet, figure out how to make it apply 
        //make a class to determine what spritesheet to give to the class
        //make a class for the collision
        //make a class to always update the location and move
        
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

        Rectangle test;
        
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

            //playerBody = new RunBody(runbodySpritesheet, runheadSpritesheet, runswordSpritesheet, runswordbackSpritesheet, runshadowSpritesheet, rectangleTexture);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            List<Texture2D> Runningspritesheets = new()
            {
                Content.Load<Texture2D>("Swordsman_lvl1_Run_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_shadow"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_sword_back")
            };

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

        public void GrandList()
        {
            List<List<Texture2D>>wholelist = new List<Texture2D>();
            List<Texture2D> currentList = wholelist[0];
            Texture2D currentTexture = currentList[0];

        }

    }
}
