using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace spritesheet
{
    public enum Animation
    {
        None = 2,
        Idle = 0,
        Running = 1
    }

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

        Rectangle playerCollisionRect, playerDrawRect, rectangle;

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

        SpritesheetManager spritesheetManager;

        List<Texture2D> Idlespritesheets;
        Texture2D runbodySpritesheet;
        Texture2D runheadSpritesheet;
        Texture2D runshadowSpritesheet;
        Texture2D runswordSpritesheet;
        Texture2D runswordbackSpritesheet;

        Animation state;
        //KeyboardState keyboardState;
        //int rows, columns;
        //int frame; // The frame number (column) in the sequence to draw
        //int frames; // The number of frames for each direction, usually the same as columns
        //int directionRow; // The row number containing the frames for the current direction
        //int leftRow, rightRow, upRow, downRow; // Row number of directional set of frames
        //int width; // The width of each frame
        //int height; // The height of each frame
        //float speed = 5f; // How fast the character sprite will travel
        //float time; // Used to store elapsed time
        //float frameSpeed; // Sets how fast player frames transition
        //Vector2 playerLocation; // Stored the location of the players collision sprite
        //Vector2 playerDirection; // The directional vector of the player
        //Rectangle playerCollisionRect, playerDrawRect;

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



            Animation state = new Animation();
            state = 0;
            rectangle = new Rectangle(20, 20, 150, 150);
            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(80, 60, 40, 70);
            //playerDrawRect = new Rectangle(20, 20, 150, 150);
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


            //time = 0.0f;
            //frameSpeed = 0.1f;
            //frames = 8;
            //frame = 0;
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

            List<List<Texture2D>> wholelist = new List<List<Texture2D>>();
            wholelist.Add(Idlespritesheets);
            spritesheetManager = new SpritesheetManager(wholelist);
            spritesheetDraw = new SpritesheetDraw(wholelist);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            spritesheetManager.SetPlayerDirection(rectangle: rectangle, keyboardState: keyboardState, playerDirection: playerDirection, directionRow: directionRow, leftRow: leftRow, rightRow: rightRow, downRow: downRow, upRow: upRow, frame: frame);
            playerLocation += playerDirection * speed;
            UpdatePlayerRects();
            //playerBody.Update(gameTime);


            //time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            //if (time > frameSpeed && playerDirection != Vector2.Zero)
            //{
            //    time = 0f;
            //    frame += 1;
            //    if (frame >= frames)
            //        frame = 0;
            //}
            //keyboardState = Keyboard.GetState();

            //SetPlayerDirection();
            //playerLocation += playerDirection * speed;
            //UpdatePlayerRects();


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            playerDrawRect = spritesheetManager.Rectangle;
            spritesheetDraw.Draw(_spriteBatch, playerDrawRect, directionRow, state, frame);

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void GrandList()
        {
            //List<List<Texture2D>> wholelist = new List<List<Texture2D>>();
            //wholelist.Add(Idlespritesheets);
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
