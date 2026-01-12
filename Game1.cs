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

        //Texture2D runbodySpritesheet;
        //Texture2D runheadSpritesheet;
        //Texture2D runshadowSpritesheet;
        //Texture2D runswordSpritesheet;
        //Texture2D runswordbackSpritesheet;

        Texture2D rectangleTexture;

        //Texture2D runattackbodySpritesheet;
        //Texture2D runattackheadSpritesheet;
        //Texture2D runattackshadowSpritesheet;
        //Texture2D runattackswingSpritesheet;
        //Texture2D runattackswordSpritesheet;
        //Texture2D runattackswordbackSpritesheet;

        //Texture2D idlebodySpritesheet;
        //Texture2D idleheadSpritesheet;
        //Texture2D idleshadowSpritesheet;
        //Texture2D idleswordspritesheet;
        //Texture2D idleswordbackspritesheet;

        //RunBody playerBody;

        Rectangle playerCollisionRect, playerDrawRect;

        KeyboardState keyboardState;
        Vector2 playerDirection;
        Vector2 playerLocation;

        int columns;
        int rows;
        int upRow, leftRow, rightRow, downRow;
        int directionRow;
        int width;
        int height;
        int frames;
        int frame;

        float time; // Used to store elapsed time
        float frameSpeed; // Sets how fast player frames transition
        float speed = 5f;

        SpritesheetDraw spritesheetDraw;
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
            //width = runbodySpritesheet.Width / columns;
            //height = runbodySpritesheet.Height / rows;
            time = 0.0f;
            frameSpeed = 0.1f;
            frames = 8;
            frame = 0;

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
            List<Texture2D> Runattackspritesheets = new()
            {
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_shadow"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_swing"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_sword_back")
            };
            List<Texture2D> Idlespritesheets = new()
            {
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_shadow"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_sword_back")

            };

            rectangleTexture = Content.Load<Texture2D>("rectangle");



        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            //playerBody.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            //playerBody.Draw(_spriteBatch);
            spritesheetDraw.Draw(_spriteBatch, );
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void GrandList()
        {
            //List<List<Texture2D>>wholelist = new List<Texture2D>();
            //List<Texture2D> currentList = wholelist[0];
            //Texture2D currentTexture = currentList[0];

        }
        public void UpdatePlayerRects()
        {
            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.X = playerCollisionRect.X - 55;
            playerDrawRect.Y = playerCollisionRect.Y - 40;
        }
    }
}
