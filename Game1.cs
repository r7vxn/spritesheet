using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace spritesheet
{
    public enum Animation
    {
        Idle = 0,
        Running = 1,
        Attack = 2
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Texture2D rectangleTexture;
        private Rectangle playerCollisionRect, playerDrawRect;

        private Vector2 playerLocation;
        private Vector2 playerDirection;
        private int directionRow, leftRow, rightRow, upRow, downRow;
        private Animation state;
        private int frame;
        private float time, frameSpeed = 0.1f, speed = 5f;
        private int frames;

        private SpritesheetDraw spritesheetDraw;
        private SpritesheetManager spritesheetManager;

        private Dictionary<Animation, Dictionary<int, int>> framesPerDirection;
        private Dictionary<Animation, int> rowsPerState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            state = Animation.Idle;
            playerLocation = new Vector2(20, 20);
            playerCollisionRect = new Rectangle(80, 60, 40, 70);
            playerDrawRect = new Rectangle(20, 20, 150, 150);
            leftRow = 1;
            rightRow = 2;
            upRow = 0;
            downRow = 3;
            directionRow = downRow;

            framesPerDirection = new Dictionary<Animation, Dictionary<int, int>>();
            framesPerDirection[Animation.Idle] = new Dictionary<int, int>()
            {
                { downRow, 12 },
                { leftRow, 12 },
                { rightRow, 12 },
                { upRow, 12 }
            };
            framesPerDirection[Animation.Running] = new Dictionary<int, int>()
            {
                { downRow, 8 },
                { leftRow, 8 },
                { rightRow, 8 },
                { upRow, 8 }
            };
            framesPerDirection[Animation.Attack] = new Dictionary<int, int>()
            {
                { downRow, 8 },
                { leftRow, 8 },
                { rightRow, 8 },
                { upRow, 8 }
            };
            rowsPerState = new Dictionary<Animation, int>()
            {
                { Animation.Idle, 4 },
                { Animation.Running, 4 },
                { Animation.Attack, 4 }
            };
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var Idlespritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_shadow"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Idle_sword_back")
            };

            var Runningspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Swordsman_lvl1_Run_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_shadow"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_sword_back")
            };

            var Attackspritesheets = new List<Texture2D>()
            { 
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_shadow"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_swing"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Run_Attack_sword_back")
            };
            rectangleTexture = Content.Load<Texture2D>("rectangle");

            var wholelist = new List<List<Texture2D>>() { Idlespritesheets, Runningspritesheets, Attackspritesheets };

            spritesheetManager = new SpritesheetManager(wholelist);
            spritesheetDraw = new SpritesheetDraw(wholelist);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboardState = Keyboard.GetState();
            playerDirection = Vector2.Zero;

            if (keyboardState.IsKeyDown(Keys.W)) playerDirection.Y -= 1;
            if (keyboardState.IsKeyDown(Keys.S)) playerDirection.Y += 1;
            if (keyboardState.IsKeyDown(Keys.A)) playerDirection.X -= 1;
            if (keyboardState.IsKeyDown(Keys.D)) playerDirection.X += 1;



            if (keyboardState.IsKeyDown(Keys.Space))
            {
                state = Animation.Attack;
            }
            else if (playerDirection != Vector2.Zero)
            {
                playerDirection = Vector2.Normalize(playerDirection);
                state = Animation.Running;
            }
            else
            {
                state = Animation.Idle;
            }

            if (playerDirection.X < 0) directionRow = leftRow;
            else if (playerDirection.X > 0) directionRow = rightRow;
            else if (playerDirection.Y < 0) directionRow = downRow;
            else if (playerDirection.Y > 0) directionRow = upRow;

            playerLocation += playerDirection * speed;

            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.X = playerCollisionRect.X - 55;
            playerDrawRect.Y = playerCollisionRect.Y - 40;

            int frames = framesPerDirection[state][directionRow];
            float frameSpeed = 0.12f;
            if (state == Animation.Attack)
            {
                frameSpeed = 0.08f;
            }
            if (state == Animation.Idle && directionRow == downRow)
            {
                frameSpeed = 0.3f;
            }

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > frameSpeed)
            {
                time = 0f;
                frame++;
                if (frame >= frames) frame = 0;
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            int currentColumns = framesPerDirection[state][directionRow];
            int currentRows = rowsPerState[state];

            spritesheetManager.Draw(_spriteBatch, state,  frame, playerDrawRect, directionRow, currentColumns, currentRows);

            _spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.4f);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}