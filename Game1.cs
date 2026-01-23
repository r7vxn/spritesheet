using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace spritesheet
{
    public enum Screen
    {
        intro, game, end
    }
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
        SlimeHurt = 4,
        SlimeDeath = 3,
    }

    public class Game1 : Game
    {

        public const int PLAYER_WIDTH = 35;
        public const int PLAYER_HEIGHT = 70;
        List<Rectangle> airBarriers = new List<Rectangle>();


        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Rectangle window = new Rectangle(0, 0, 960, 540);
        private Texture2D rectangleTexture;
        private Texture2D backgroundTexture;
        private Texture2D introTexture;
        private Rectangle introRect = new Rectangle(280, 220, 400, 100);
        private SpriteFont font;
        private Vector2 introVector;
        private MouseState mouseState;
        private bool playerDied = false;
        private string endScreenMessage;



        private Rectangle playerCollisionRect, playerDrawRect, attackCollisionRect;
        private bool attack = false;
        private int playerHealth;
        private int playerDamage;


        private Vector2 playerLocation;
        private Vector2 playerDirection;
        private int directionRow, leftRow, rightRow, upRow, downRow;
        private Animation state;
        private int frame;
        private float time, frameSpeed = 0.1f, speed = 3f;
        private int frames;
        private bool attacked = false;
        private float attackTimer;

        private SpritesheetDraw spritesheetDraw;
        private SpritesheetManager spritesheetManager;

        private Dictionary<Animation, Dictionary<int, int>> framesPerDirection;
        private Dictionary<Animation, int> rowsPerState;


        private Dictionary<SlimeAnimation, Dictionary<int, int>> slimeFramesPerDirection;
        private Dictionary<SlimeAnimation, int> slimeRowsPerState;

        private Rectangle slimeRangeRect;
        private Rectangle slimeDrawRect, slimeCollisionRect, slimeAttackRect;
        private Vector2 slimeLocation;
        private Vector2 slimeDirection;
        private int slimeDirectionRow, slimeLeftRow, slimeRightRow, slimeUpRow, slimeDownRow;
        private SlimeAnimation slimeState;
        private int slimeFrame;
        private float slimeTime, slimeFrameSpeed = 1f, slimeSpeed = 5f;
        private int slimeFrames;
        private bool slimeReset = true;
        private bool slimeAttackState = false;
        private bool slimeFrameCheck = false;
        private bool slimeAttackCollision = false;
        private bool slimeAttacked = false;
        private int slimeHealth = 15;
        private int slimeDamage;
        private float slimeAttackTimer;
        private bool slimeDied = false;
        bool slimeDeathStarted = false;
        float endDelayTimer = 2f;
        private bool slimeAttackStarted = false;
        private bool slimeDeathDraw = false;



        private SlimeDraw slimeDraw;
        private SlimeManager slimeManager;

        private Screen screen;


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

            screen = Screen.intro;

            introVector.X = window.Width / 2 - 50;
            introVector.Y = window.Height / 2 - 15;

            _graphics.PreferredBackBufferWidth = 960;
            _graphics.PreferredBackBufferHeight = 540;
            _graphics.ApplyChanges();

            playerHealth = 15;
            playerDamage = 5;
            slimeDamage = 3;

            state = Animation.Idle;
            playerLocation = new Vector2(460, 460);
            playerCollisionRect = new Rectangle(80, 60, 40, 70);
            attackCollisionRect = new Rectangle(0, 0, 0, 0);
            playerDrawRect = new Rectangle(0, 0, 150, 150);
            leftRow = 1;
            rightRow = 2;
            upRow = 0;
            downRow = 3;
            directionRow = downRow;

            slimeRangeRect = new Rectangle(0, 0, 70, 80);
            slimeCollisionRect = new Rectangle(0, 0, 50, 50);
            slimeAttackRect = new Rectangle(0, 0, 45, 20);
            slimeState = SlimeAnimation.SlimeIdle;
            slimeLeftRow = 2;
            slimeRightRow = 3;
            slimeUpRow = 1;
            slimeDownRow = 0;
            slimeDirectionRow = slimeDownRow;
            slimeLocation = new Vector2(960 / 2, 540 / 2);
            slimeDrawRect = new Rectangle(960 / 2, 540 / 2, 150, 150);


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
                { downRow, 10 },
                { leftRow, 10 },
                { rightRow, 10 },
                { upRow, 10 }
            };
            slimeFramesPerDirection[SlimeAnimation.SlimeDeath] = new Dictionary<int, int>()
            {
                { downRow, 10 },
                { leftRow, 10 },
                { rightRow, 10 },
                { upRow, 10 }
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


        private bool CanMoveTo(Vector2 newPosition)
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
                Content.Load<Texture2D>("Slime1_Death")
            };
            var SlimeHurtspritesheets = new List<Texture2D>()
            {
                Content.Load<Texture2D>("Slime1_Hurt_body"),
                Content.Load<Texture2D>("Slime1_Hurt_shadow"),
                Content.Load<Texture2D>("Slime1_Hurt")
            };
            rectangleTexture = Content.Load<Texture2D>("rectangle");
            backgroundTexture = Content.Load<Texture2D>("forest background");
            introTexture = Content.Load<Texture2D>("forest intro");
            font = Content.Load<SpriteFont>("Font");

            var wholelist = new List<List<Texture2D>>() { Idlespritesheets, Runningspritesheets, Attackspritesheets, Deathspritesheets, Hurtspritesheets };
            spritesheetManager = new SpritesheetManager(wholelist);
            spritesheetDraw = new SpritesheetDraw(wholelist);

            var slimelist = new List<List<Texture2D>>() { SlimeIdlespritesheets, SlimeRunningspritesheets, SlimeAttackspritesheets, SlimeDeathspritesheets, SlimeHurtspritesheets };
            slimeDraw = new SlimeDraw(slimelist);
            slimeManager = new SlimeManager(slimelist);
        }

        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (screen == Screen.intro)
            {
                mouseState = Mouse.GetState();
                if (mouseState.LeftButton == ButtonState.Pressed && introRect.Contains(mouseState.Position))
                {
                    screen = Screen.game;
                }
            }
            if (screen == Screen.game)
            {

                //slime's painful dying process

                if (slimeHealth <= 0 && !slimeDeathStarted)
                {
                    slimeDied = true;
                    slimeDeathStarted = true;
                    slimeState = SlimeAnimation.SlimeDeath;
                }
                if (slimeState == SlimeAnimation.SlimeDeath && slimeReset)
                {
                    slimeFrame = 0;
                    slimeReset = false;
                }
                playerDirection = Vector2.Zero;
                if (keyboardState.IsKeyDown(Keys.W)) playerDirection.Y -= 3;
                if (keyboardState.IsKeyDown(Keys.S)) playerDirection.Y += 3;
                if (keyboardState.IsKeyDown(Keys.A)) playerDirection.X -= 3;
                if (keyboardState.IsKeyDown(Keys.D)) playerDirection.X += 3;

                if (playerDirection != Vector2.Zero)
                {
                    playerDirection = Vector2.Normalize(playerDirection);
                    state = Animation.Running;
                }
                else
                {
                    state = Animation.Idle;
                }

                //attack collision
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    state = Animation.Attack;
                    attack = true;

                    if (directionRow == upRow) attackCollisionRect = new Rectangle(playerDrawRect.X + 35, playerDrawRect.Y + 95, 80, 40);
                    else if (directionRow == leftRow) attackCollisionRect = new Rectangle(playerDrawRect.X + 20, playerDrawRect.Y + 35, 40, 80);
                    else if (directionRow == rightRow) attackCollisionRect = new Rectangle(playerDrawRect.X + 90, playerDrawRect.Y + 35, 40, 80);
                    else if (directionRow == downRow) attackCollisionRect = new Rectangle(playerDrawRect.X + 35, playerDrawRect.Y + 20, 80, 40);
                }
                else if (keyboardState.IsKeyDown(Keys.R)) state = Animation.Death;
                else if (keyboardState.IsKeyDown(Keys.Q)) state = Animation.Hurt;

                // Set player facing direction
                if (playerDirection.X < 0) directionRow = leftRow;
                else if (playerDirection.X > 0) directionRow = rightRow;
                else if (playerDirection.Y < 0) directionRow = downRow;
                else if (playerDirection.Y > 0) directionRow = upRow;

                //collision
                Vector2 newPosX = playerLocation + new Vector2(playerDirection.X * speed, 0);
                if (CanMoveTo(newPosX)) playerLocation = newPosX;

                Vector2 newPosY = playerLocation + new Vector2(0, playerDirection.Y * speed);
                if (CanMoveTo(newPosY)) playerLocation = newPosY;

                playerCollisionRect.Location = playerLocation.ToPoint();
                playerDrawRect.X = playerCollisionRect.X - 55;
                playerDrawRect.Y = playerCollisionRect.Y - 40;

                //animation
                frames = framesPerDirection[state][directionRow];
                frameSpeed = (state == Animation.Attack) ? 0.08f : (state == Animation.Idle && directionRow == downRow) ? 0.3f : 0.12f;

                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (time > frameSpeed)
                {
                    time = 0f;
                    frame++;
                    if (frame >= frames) frame = 0;
                }
                //player attack

                if (state == Animation.Attack)
                {
                    if (frame == 4 && !attacked && slimeCollisionRect.Intersects(attackCollisionRect)) 
                    {
                        slimeHealth -= playerDamage;
                        attacked = true;
                    }
                    if (frame == 0) attacked = false; 
                }


                //player death
                if (playerHealth <= 0)
                {
                    playerDied = true;
                    screen = Screen.end;
                }

                //slime logic
                if (slimeDied)
                {
                    endDelayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (endDelayTimer <= 0f)
                        screen = Screen.end;
                }

                slimeRangeRect.X = playerCollisionRect.X - 15;
                slimeRangeRect.Y = playerCollisionRect.Y - 5;

                slimeCollisionRect.Location = slimeLocation.ToPoint();
                slimeCollisionRect.X = slimeDrawRect.X + 50;
                slimeCollisionRect.Y = slimeDrawRect.Y + 50;

                slimeDrawRect.X = (int)slimeLocation.X - 55;
                slimeDrawRect.Y = (int)slimeLocation.Y - 50;

                slimeAttackRect.Location = slimeLocation.ToPoint();
                slimeAttackRect.X = (int)slimeLocation.X;
                slimeAttackRect.Y = (int)slimeLocation.Y + 23;

                //slime movement
                if (!slimeDeathStarted && !slimeAttackState)
                {
                    if (!slimeAttackState && !slimeDeathStarted)
                        slimeLocation += slimeDirection * slimeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        slimeDirection = playerLocation - slimeLocation;

                    if (slimeDirection != Vector2.Zero)
                    {
                        slimeDirection.Normalize();
                        slimeLocation += slimeDirection * slimeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                        //slime direction
                        if (Math.Abs(slimeDirection.X) > Math.Abs(slimeDirection.Y))
                            slimeDirectionRow = (slimeDirection.X > 0) ? slimeRightRow : slimeLeftRow;
                        else
                            slimeDirectionRow = (slimeDirection.Y > 0) ? slimeDownRow : slimeUpRow;


                    }
                }

                //slime animation
                slimeFrames = slimeFramesPerDirection[slimeState][slimeDirectionRow];
                slimeFrameSpeed = 0.12f;
                slimeTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (slimeState == SlimeAnimation.SlimeDeath)
                {
                    if (slimeTime > slimeFrameSpeed)
                    {
                        if (slimeFrame < slimeFrames - 1)
                        { 
                            slimeFrame++;
                        }
                        slimeTime = 0f;
                        
                    }

                }
                else if (slimeTime > slimeFrameSpeed)
                {
                    slimeTime = 0f;
                    slimeFrame++;

                    if (slimeState != SlimeAnimation.SlimeAttack)
                    {
                        if (slimeFrame >= slimeFrames)
                        {
                            slimeFrame = 0;
                            slimeFrameCheck = true;
                        }
                    }
                }

                //slime attack logic
                if (!slimeDied)
                {
                    if (slimeCollisionRect.Intersects(slimeRangeRect))
                    {
                        slimeAttackState = true;
                    }

                    if (slimeAttackState)
                    {
                        if (!slimeAttackStarted)
                        {
                            slimeFrame = 0;
                            slimeAttackStarted = true;
                        }

                        slimeState = SlimeAnimation.SlimeAttack;

                        if (slimeFrame > 4)
                            slimeAttackCollision = true;

                        if (slimeFrame >= slimeFrames - 1)
                        {
                            slimeAttackState = false;
                            slimeAttackStarted = false;
                            slimeFrame = 0;
                        }
                    }
                    else if (!slimeDeathStarted)
                    {
                        slimeState = SlimeAnimation.SlimeRunning;
                    }
                }

                //dmg to player
                if (slimeAttackCollision && playerCollisionRect.Intersects(slimeAttackRect) && !slimeAttacked)
                {
                    playerHealth -= slimeDamage;
                    slimeAttacked = true;
                    slimeAttackTimer = 1f;
                }

                if (slimeAttacked)
                {
                    slimeAttackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (slimeAttackTimer <= 0f) slimeAttacked = false;
                }

    
            }

            if (screen == Screen.end)
            {

                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    Exit();
                }
            }
            base.Update(gameTime);
        }
    


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            if (screen == Screen.intro)
            {
                _spriteBatch.Begin();

                _spriteBatch.Draw(introTexture, window, Color.White);
                _spriteBatch.Draw(rectangleTexture, introRect, Color.White * 0.8f);
                _spriteBatch.DrawString(font, "Play", introVector, Color.Black);
                
                _spriteBatch.End();
            }
            if (screen == Screen.game)
            {

                _spriteBatch.Begin();

                int currentColumns = framesPerDirection[state][directionRow];
                int currentRows = rowsPerState[state];

                int slimeColumns = slimeFramesPerDirection[slimeState][slimeDirectionRow];
                int slimeRows = slimeRowsPerState[slimeState];

                _spriteBatch.Draw(backgroundTexture, window, Color.White);

                spritesheetManager.Draw(_spriteBatch, state, frame, playerDrawRect, directionRow, currentColumns, currentRows);

                if (!slimeDeathDraw)
                {
                    slimeManager.Draw(_spriteBatch, slimeState, slimeFrame, slimeDrawRect, slimeDirectionRow, slimeColumns, slimeRows);
                }

                //_spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, attackCollisionRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, slimeRangeRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, slimeCollisionRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, slimeAttackRect, Color.Black * 0.4f);

                _spriteBatch.DrawString(font, slimeHealth.ToString(), new Vector2(0,0), Color.White);
                _spriteBatch.End();
            }
            if (screen == Screen.end)
            {
                if (slimeDied)
                {
                    endScreenMessage = "Nice";
                }
                else if (!playerDied)
                {
                    endScreenMessage = "Byee";
                }
                else
                {
                    endScreenMessage = "LMAO";
                }
                    _spriteBatch.Begin();

                _spriteBatch.Draw(introTexture, window, Color.White);
                _spriteBatch.Draw(rectangleTexture, introRect, Color.White * 0.8f);
                _spriteBatch.DrawString(font, endScreenMessage, introVector, Color.Black);


                _spriteBatch.End();
            }
        }


        private void SetupCollision()
        {
            // Map edges (prevent leaving screen)
            airBarriers.Add(new Rectangle(0, 0, 45, 540));
            airBarriers.Add(new Rectangle(0, 0, 960, 100));
            airBarriers.Add(new Rectangle(0, 0, 200, 150));
            airBarriers.Add(new Rectangle(890, 0, 75, 540));
            airBarriers.Add(new Rectangle(0, 300, 200, 500));
            airBarriers.Add(new Rectangle(0, 250, 140, 100));
            airBarriers.Add(new Rectangle(0, 330, 300, 100));
            airBarriers.Add(new Rectangle(0, 420, 400, 10));
            airBarriers.Add(new Rectangle(0, 520, 320, 30));
            airBarriers.Add(new Rectangle(0, 500, 260, 28));
            airBarriers.Add(new Rectangle(0, 550, 960, 10));
            airBarriers.Add(new Rectangle(730, 0, 200, 150));
            airBarriers.Add(new Rectangle(0, 330, 370, 100));
            airBarriers.Add(new Rectangle(630, 330, 370, 100));
            airBarriers.Add(new Rectangle(720, 400, 200, 140));
            airBarriers.Add(new Rectangle(820, 250, 200, 200));
            airBarriers.Add(new Rectangle(670, 520, 260, 30));
            airBarriers.Add(new Rectangle(550, 430, 400, 10));
        }

    }
}