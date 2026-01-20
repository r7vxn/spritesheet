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
        Attack = 2,
        Death = 3,  
        Hurt = 4,
        
    }
    public enum SlimeAnimation
    {
        SlimeIdle = 0,
        SlimeRunning = 1,
        SlimeAttack = 2,
        SlimeHurt = 3,
        SlimeDeath = 4,
    }

    public class Game1 : Game
    {

        // using chatgpt to code the air barriers for background
        // Player size
        public const int PLAYER_WIDTH = 32;
        public const int PLAYER_HEIGHT = 32;
        // List of air barriers (river, rocks, map edges)
        List<Rectangle> airBarriers = new List<Rectangle>();







        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Rectangle window = new Rectangle(0, 0, 1920, 1080);
        private Texture2D rectangleTexture;
        private Texture2D backgroundTexture;
        private Rectangle playerCollisionRect, playerDrawRect, attackCollisionRect;
        private bool attack = false;
        
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
        private Dictionary<SlimeAnimation, Dictionary<int, int>> slimeFramesPerDirection;
        private Dictionary<SlimeAnimation, int> slimeRowsPerState;


        private Rectangle slimeDrawRect, slimeCollisionRect, slimeAttackRect;
        private Vector2 slimeLocation;
        private Vector2 slimeDirection;
        private int slimeDirectionRow, slimeLeftRow, slimeRightRow, slimeUpRow, slimeDownRow;
        private SlimeAnimation slimeState;
        private int slimeFrame;
        private float slimeTime, slimeFrameSpeed = 0.1f, slimeSpeed = 5f;
        private int slimeFrames;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();


            SetupCollision();


            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            state = Animation.Idle;
            playerLocation = new Vector2(1920/2, 1080/3);
            playerCollisionRect = new Rectangle(80, 60, 40, 70);
            attackCollisionRect = new Rectangle(0, 0, 0, 0);
            playerDrawRect = new Rectangle(0,0, 150, 150);
            leftRow = 1;
            rightRow = 2;
            upRow = 0;
            downRow = 3;
            directionRow = downRow;

            slimeState = SlimeAnimation.SlimeIdle;
            slimeLeftRow = 1;
            slimeRightRow = 2;
            slimeUpRow = 0;
            slimeDownRow = 3;
            slimeDirectionRow = slimeDownRow;


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
            framesPerDirection[Animation.Death] = new Dictionary<int, int>()
            {
                { downRow, 7 },
                { leftRow, 7 },
                { rightRow, 7 },
                { upRow, 7 }
            };
            framesPerDirection[Animation.Hurt] = new Dictionary<int, int>()
            {
                { downRow, 5 },
                { leftRow, 5 },
                { rightRow, 5 },
                { upRow, 5 }
            };

            slimeFramesPerDirection = new Dictionary<SlimeAnimation, Dictionary<int, int>>();
            slimeFramesPerDirection[SlimeAnimation.SlimeIdle] = new Dictionary<int, int>()
            {
                { downRow, 6 },
                { leftRow, 6 },
                { rightRow, 6 },
                { upRow, 6 }
            };
            slimeFramesPerDirection[SlimeAnimation.SlimeRunning] = new Dictionary<int, int>()
            {
                { downRow, 8 },
                { leftRow, 8 },
                { rightRow, 8 },
                { upRow, 8 }
            };
            slimeFramesPerDirection[SlimeAnimation.SlimeAttack] = new Dictionary<int, int>()
            {
                { downRow, 9 },
                { leftRow, 9 },
                { rightRow, 9 },
                { upRow, 9 }
            };
            slimeFramesPerDirection[SlimeAnimation.SlimeDeath] = new Dictionary<int, int>()
            {
                { downRow, 9 },
                { leftRow, 9 },
                { rightRow, 9 },
                { upRow, 9 }
            };
            slimeFramesPerDirection[SlimeAnimation.SlimeHurt] = new Dictionary<int, int>()
            {
                { downRow, 5 },
                { leftRow, 5 },
                { rightRow, 5 },
                { upRow, 5 }
            };


            rowsPerState = new Dictionary<Animation, int>()
            {
                { Animation.Idle, 4 },
                { Animation.Running, 4 },
                { Animation.Attack, 4 },
                { Animation.Death, 4 },
                { Animation.Hurt, 4 },
                
            };

            slimeRowsPerState = new Dictionary<SlimeAnimation, int>()
            {
                { SlimeAnimation.SlimeIdle, 4 },
                { SlimeAnimation.SlimeRunning, 4 },
                { SlimeAnimation.SlimeAttack, 4 },
                { SlimeAnimation.SlimeDeath, 4 },
                { SlimeAnimation.SlimeHurt, 4 },
            };

        }


        private bool CanMoveTo(Vector2 newPosition)  // <- Step 3 goes here
        {
            Rectangle nextHitbox = new Rectangle(
                (int)newPosition.X,
                (int)newPosition.Y,
                PLAYER_WIDTH,
                PLAYER_HEIGHT
            );

            foreach (Rectangle barrier in airBarriers)
                if (nextHitbox.Intersects(barrier))
                    return false;

            return true;
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
            var Deathspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Swordsman_lvl1_Death_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Death_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Death_red"),
                Content.Load<Texture2D>("Swordsman_lvl1_Death_shadow"),
                Content.Load<Texture2D>("Swordsman_lvl1_Death_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Death_sword_back")
            };
            var Hurtspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Swordsman_lvl1_Hurt_body"),
                Content.Load<Texture2D>("Swordsman_lvl1_Hurt_head"),
                Content.Load<Texture2D>("Swordsman_lvl1_Hurt_red"),
                Content.Load<Texture2D>("Swordsman_lvl1_Hurt_sword"),
                Content.Load<Texture2D>("Swordsman_lvl1_Hurt_sword_back")
            };
            var SlimeIdlespritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Slime1_Idle_body"),
                Content.Load<Texture2D>("Slime1_Idle_shadow"),

            };
            var SlimeRunningspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Slime1_Run_body"),
                Content.Load<Texture2D>("Slime1_Run_shadow"),
            };
            var SlimeAttackspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Slime1_Attack_body"),
                Content.Load<Texture2D>("Slime1_Attack_shadow"),
            };
            var SlimeDeathspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Slime1_Death_body"),
                Content.Load<Texture2D>("Slime1_Death_shadow"),
            };
            var SlimeHurtspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Slime1_Hurt_body"),
                Content.Load<Texture2D>("Slime1_Hurt_shadow"),
                Content.Load<Texture2D>("Slime1_Hurt")
            };
            rectangleTexture = Content.Load<Texture2D>("rectangle");
            backgroundTexture = Content.Load<Texture2D>("forest background");

            var wholelist = new List<List<Texture2D>>() { Idlespritesheets, Runningspritesheets, Attackspritesheets, Deathspritesheets, Hurtspritesheets };

            spritesheetManager = new SpritesheetManager(wholelist);
            spritesheetDraw = new SpritesheetDraw(wholelist);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboardState = Keyboard.GetState();
            playerDirection = Vector2.Zero;

            KeyboardState barrierstate = Keyboard.GetState();

            if (barrierstate.IsKeyDown(Keys.W)) playerDirection.Y -= 5;
            if (barrierstate.IsKeyDown(Keys.S)) playerDirection.Y += 5;
            if (barrierstate.IsKeyDown(Keys.A)) playerDirection.X -= 5;
            if (barrierstate.IsKeyDown(Keys.D)) playerDirection.X += 5;

            // Split movement to prevent sticking to barriers
            Vector2 newPosX = playerLocation + new Vector2(playerDirection.X, 0);
            if (CanMoveTo(newPosX))
                playerLocation = newPosX;

            Vector2 newPosY = playerLocation + new Vector2(0, playerDirection.Y);
            if (CanMoveTo(newPosY))
                playerLocation = newPosY;




            if (keyboardState.IsKeyDown(Keys.Space))
            {
                state = Animation.Attack;
                attack = true;
                if (directionRow == 0)
                {
                    attackCollisionRect = new Rectangle(playerDrawRect.X + 35, playerDrawRect.Y + 95, 80, 40);
                }
                if (directionRow == 1)
                {
                    attackCollisionRect = new Rectangle(playerDrawRect.X + 20, playerDrawRect.Y + 35, 40, 80);
                }
                if (directionRow == 2)
                {
                    attackCollisionRect = new Rectangle(playerDrawRect.X + 90, playerDrawRect.Y + 35, 40, 80);
                }
                if (directionRow == 3)
                {
                    attackCollisionRect = new Rectangle(playerDrawRect.X + 35, playerDrawRect.Y + 20, 80, 40);
                }
            }
            else if (keyboardState.IsKeyDown(Keys.R))
            {
                state = Animation.Death;
            }
            else if (keyboardState.IsKeyDown(Keys.Q))
            {
                state = Animation.Hurt;
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
            GraphicsDevice.Clear(Color.White);

            _spriteBatch.Begin();

            int currentColumns = framesPerDirection[state][directionRow];
            int currentRows = rowsPerState[state];

            _spriteBatch.Draw(backgroundTexture, window, Color.White);

            spritesheetManager.Draw(_spriteBatch, state, frame, playerDrawRect, directionRow, currentColumns, currentRows);

            _spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.4f);

            _spriteBatch.Draw(rectangleTexture, attackCollisionRect, Color.Black * 0.4f);

            

            _spriteBatch.End();

            Texture2D debugTex = new Texture2D(GraphicsDevice, 1, 1);
            debugTex.SetData(new[] { Color.Red });

            _spriteBatch.Begin();
            foreach (Rectangle r in airBarriers)
                _spriteBatch.Draw(debugTex, r, Color.Red * 0.3f); // semi-transparent overlay
            _spriteBatch.End();
            base.Draw(gameTime);
        }
        public void AttackCollision()
        {
            if (attack == true)
            {

            }
        }

        private void SetupCollision()
        {
            // Map edges (prevent leaving screen)
            airBarriers.Add(new Rectangle(0, 0, 150, 1080));
            airBarriers.Add(new Rectangle(0, 0, 1920, 200));
            airBarriers.Add(new Rectangle(0, 0, 600, 260));
            airBarriers.Add(new Rectangle(0, 0, 400, 300));
            airBarriers.Add(new Rectangle(0, 0, 300, 400));
            airBarriers.Add(new Rectangle(1780, 0, 150, 1080));
            airBarriers.Add(new Rectangle(0, 850, 400, 500));

        }

    }
}