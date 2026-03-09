using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
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


        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Rectangle window = new Rectangle(0, 0, 1920, 1080);
        Texture2D rectangleTexture;
        Texture2D backgroundTexture;
        Texture2D introTexture;
        Rectangle introRect = new Rectangle(560, 440, 800, 200);
        SpriteFont font;
        Vector2 introVector;
        MouseState mouseState;
        bool playerDied = false;
        string endScreenMessage;
        // runtime debug
        string debugMessage = null;
        bool hasLoadError = false;



        Rectangle playerCollisionRect, playerDrawRect, attackCollisionRect;
        bool attack = false;


        Vector2 playerLocation;
        Vector2 playerDirection;
        int directionRow, leftRow, rightRow, upRow, downRow;
        Animation state;
        int frame;
        float time, frameSpeed = 0.1f, speed = 3f;
        int frames;
        bool attacked = false;
        float attackTimer;

        SpritesheetDraw spritesheetDraw;
        SpritesheetManager spritesheetManager;

        Dictionary<Animation, Dictionary<int, int>> framesPerDirection;
        Dictionary<Animation, int> rowsPerState;


        Dictionary<SlimeAnimation, Dictionary<int, int>> slimeFramesPerDirection;
        Dictionary<SlimeAnimation, int> slimeRowsPerState;

        // slime state is managed by SlimeAnimationClass; Game1 keeps only the player-facing cooldown
        bool slimeAttacked = false;
        float slimeAttackTimer;

        int resChange = 2;

        SlimeDraw slimeDraw;
        SlimeManager slimeManager;
        SlimeSoundEffect slimeSoundEffect;
        SlimeAnimationClass slimeAnimationClass;

        PlayerAttack playerAttack;
        PlayerAnimationClass playerAnimation;
        PlayerManager playerManager;

        Screen screen;

        Song song;
        SoundEffect slimeJump;
        SoundEffectInstance slimeJumpInstance;
        SoundEffect slimeBeingSlashed;
        SoundEffectInstance slimeBeingSlashInstance;
        SoundEffect slimeHittingGround;
        SoundEffectInstance slimeHittingGroundInstance;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            // Defer audio playback and SoundEffectInstance creation until assets are loaded in LoadContent
            SetupCollision();

            screen = Screen.intro;

            introVector.X = window.Width / 2 - 100;
            introVector.Y = window.Height / 2 - 40;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();


            state = Animation.Idle;
            playerLocation = new Vector2(920, 920);
            playerCollisionRect = new Rectangle(80, 60, 40, 70);
            attackCollisionRect = new Rectangle(0, 0, 0, 0);

            playerDrawRect = new Rectangle(0, 0, 225, 225);
            leftRow = 1;
            rightRow = 2;
            upRow = 0;
            downRow = 3;
            directionRow = downRow;

            // Slime rects/state are managed by SlimeAnimationClass
            //slimeLeftRow = 2;
            //slimeRightRow = 3;
            //slimeUpRow = 1;
            //slimeDownRow = 0;
            //slimeDirectionRow = slimeDownRow;
            //slimeLocation = new Vector2(960, 540);
            // Slime draw rect is managed by SlimeAnimationClass


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
                (int)newPosition.X + 45,
                (int)newPosition.Y + 40,
                30,
                70
            );

            foreach (Rectangle barrier in airBarriers)
                if (nextHitbox.Intersects(barrier))
                    return false;

            return true;
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            try
            {
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
            song = Content.Load<Song>("Forest Bgm");
            slimeJump = Content.Load<SoundEffect>("slime jump");
            slimeBeingSlashed = Content.Load<SoundEffect>("slime impact");
            slimeHittingGround = Content.Load<SoundEffect>("slime hit ground");

            // Audio setup: create instances and start background music now that assets are loaded
            MediaPlayer.Play(song);
            MediaPlayer.Volume = 0.18f;

            slimeJumpInstance = slimeJump.CreateInstance();
            slimeJumpInstance.Pitch = -0.2f;
            slimeJumpInstance.Volume = 0.6f;

            slimeBeingSlashInstance = slimeBeingSlashed.CreateInstance();
            slimeBeingSlashInstance.Volume = 0.6f;

            slimeHittingGroundInstance = slimeHittingGround.CreateInstance();
            slimeHittingGroundInstance.Volume = 0.6f;

            // create a player hurt sound instance (reuse slime impact if no separate asset)
            var playerHurtInstance = slimeBeingSlashed.CreateInstance();
            playerHurtInstance.Volume = 0.6f;

            var wholelist = new List<List<Texture2D>>() { Idlespritesheets, Runningspritesheets, Attackspritesheets, Deathspritesheets, Hurtspritesheets };
            spritesheetManager = new SpritesheetManager(wholelist);
            spritesheetDraw = new SpritesheetDraw(wholelist);  
            
            // player manager will use the spritesheet manager and frame dictionaries to render the player
            playerManager = new PlayerManager(spritesheetManager, framesPerDirection, rowsPerState);
         
            var slimelist = new List<List<Texture2D>>() { SlimeIdlespritesheets, SlimeRunningspritesheets, SlimeAttackspritesheets, SlimeDeathspritesheets, SlimeHurtspritesheets };
            slimeDraw = new SlimeDraw(slimelist);
            slimeManager = new SlimeManager(slimelist);
            slimeSoundEffect = new SlimeSoundEffect();
                slimeAnimationClass = new SlimeAnimationClass();
                slimeAnimationClass.Initialize();

                playerAnimation = new PlayerAnimationClass();
                playerAnimation.Initialize(leftRow, rightRow, upRow, downRow, new Vector2(920, 920), 15, 5);
                // set hurt sound for player animation
                playerAnimation.SetHurtSoundInstance(playerHurtInstance);

                playerAttack = new PlayerAttack();
            }
            catch (System.Exception ex)
            {
                // capture load error so it can be displayed on-screen
                debugMessage = "LoadContent error: " + ex.Message + "\n" + ex.StackTrace;
                hasLoadError = true;
            }

        }

        protected override void Update(GameTime gameTime)
        {
            MediaPlayer.IsRepeating = true;

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
                // Update slime internal state (pass map barriers so slime can't walk through them)
                slimeAnimationClass.update(gameTime, playerAnimation.CurrentCollisionRect, playerAnimation.CurrentLocation, slimeFramesPerDirection, airBarriers);

                // Update player movement and animation (handles setting CurrentState, CurrentFrame, CurrentDirectionRow, and attack rect)
                playerAnimation.Update(gameTime, keyboardState, framesPerDirection, rowsPerState, airBarriers);

                // player attack handling (apply damage once per attack frame)
                if (playerAnimation.CurrentState == Animation.Attack)
                {
                    if (!attacked && playerAttack.TryApplyAttack(playerAnimation.CurrentFrame, playerAnimation.CurrentAttackRect, slimeAnimationClass, playerAnimation.CurrentDamage))
                    {
                        attacked = true;
                    }
                    if (playerAnimation.CurrentFrame == 0) attacked = false;
                }

                // update sounds after slime/player attack state is determined
                slimeSoundEffect.update(slimeAnimationClass.CurrentFrame, slimeAnimationClass.CurrentState, attacked, slimeJumpInstance, slimeHittingGroundInstance, slimeBeingSlashInstance);

                 //player death
                if (playerAnimation.CurrentHealth <= 0)
                 {
                     playerDied = true;
                     screen = Screen.end;
                 }

                // dmg to player (use slimeAnimationClass properties)
                if (slimeAnimationClass.CurrentAttackCollision && playerAnimation.CurrentCollisionRect.Intersects(slimeAnimationClass.CurrentAttackRect) && !slimeAttacked)
                {
                    // apply knockback away from slime center
                    var slimeCenter = new Vector2(slimeAnimationClass.CurrentDrawRect.X + slimeAnimationClass.CurrentDrawRect.Width / 2, slimeAnimationClass.CurrentDrawRect.Y + slimeAnimationClass.CurrentDrawRect.Height / 2);
                    var knockbackDir = playerAnimation.CurrentLocation - slimeCenter;
                    if (knockbackDir != Vector2.Zero) knockbackDir.Normalize();
                    var knockback = knockbackDir * 30f; // pixels of immediate knockback

                    // delegate damage + knockback to player animation class
                    playerAnimation.ApplyDamage(slimeAnimationClass.CurrentDamage, knockback);

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
            // If there was a LoadContent error, show it and stop normal drawing
            if (hasLoadError)
            {
                _spriteBatch.Begin();
                _spriteBatch.DrawString(font ?? Content.Load<SpriteFont>("Font"), debugMessage ?? "Unknown load error", new Vector2(20, 20), Color.Red);
                _spriteBatch.End();
                return;
            }
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

                // player drawing handled by PlayerManager (reads playerAnimation state)

                int slimeColumns = slimeFramesPerDirection[slimeAnimationClass.CurrentState][slimeAnimationClass.CurrentDirectionRow];
                int slimeRows = slimeRowsPerState[slimeAnimationClass.CurrentState];

                _spriteBatch.Draw(backgroundTexture, window, Color.White);

                // compute player columns/rows for debug
                int playerColumns = 1;
                if (framesPerDirection != null && framesPerDirection.TryGetValue(playerAnimation.CurrentState, out var pdict) && pdict != null)
                {
                    pdict.TryGetValue(playerAnimation.CurrentDirectionRow, out playerColumns);
                }
                int playerRows = 1;
                if (rowsPerState != null) rowsPerState.TryGetValue(playerAnimation.CurrentState, out playerRows);

                playerManager.Draw(_spriteBatch, playerAnimation);

                // debug: show computed columns/rows and per-layer sizes
                if (font != null)
                {
                    _spriteBatch.DrawString(font, $"cols:{playerColumns} rows:{playerRows}", new Vector2(20, 40), Color.White);
                    try
                    {
                        var sizes = spritesheetDraw.GetLayerSizes(playerAnimation.CurrentState);
                        for (int i = 0; i < sizes.Count; i++)
                        {
                            _spriteBatch.DrawString(font, $"L{i}:{sizes[i].X}x{sizes[i].Y}", new Vector2(20, 80 + i * 16), Color.LightYellow);
                        }
                        var srcs = spritesheetDraw.ComputeLayerSourceRects(playerAnimation.CurrentState, playerAnimation.CurrentFrame, playerAnimation.CurrentDirectionRow, playerColumns, playerRows);
                        for (int i = 0; i < srcs.Count; i++)
                        {
                            var r = srcs[i];
                            string txt = r == Rectangle.Empty ? "SR:SKIP" : $"SR{i}:{r.X},{r.Y} {r.Width}x{r.Height}";
                            _spriteBatch.DrawString(font, txt, new Vector2(200, 80 + i * 16), Color.LightGreen);
                        }
                    }
                    catch { }
                }

                // Debug: draw player rects to help locate missing player
                if (rectangleTexture != null)
                {
                    _spriteBatch.Draw(rectangleTexture, playerAnimation.CurrentDrawRect, Color.Green * 0.4f);
                    _spriteBatch.Draw(rectangleTexture, playerAnimation.CurrentCollisionRect, Color.Blue * 0.6f);
                    _spriteBatch.Draw(rectangleTexture, playerAnimation.CurrentAttackRect, Color.Yellow * 0.6f);
                }
                if (font != null)
                {
                    _spriteBatch.DrawString(font, $"Player: {playerAnimation.CurrentState} F:{playerAnimation.CurrentFrame}", new Vector2(20, 60), Color.White);
                }

                if (!slimeAnimationClass.DeathDraw)
                {
                    slimeManager.Draw(_spriteBatch, slimeAnimationClass.CurrentState, slimeAnimationClass.CurrentFrame, slimeAnimationClass.CurrentDrawRect, slimeAnimationClass.CurrentDirectionRow, slimeColumns, slimeRows);
                }

                //_spriteBatch.Draw(rectangleTexture, playerCollisionRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, attackCollisionRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, slimeRangeRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, slimeCollisionRect, Color.Black * 0.4f);

                //_spriteBatch.Draw(rectangleTexture, slimeAttackRect, Color.Black * 0.4f);

                //_spriteBatch.DrawString(font, slimeHealth.ToString(), new Vector2(0,0), Color.White);

                foreach (Rectangle barrier in airBarriers)
                {
                    _spriteBatch.Draw(rectangleTexture, barrier, Color.Red * 0.3f);
                }
                _spriteBatch.End();
            }
            if (screen == Screen.end)
            {
                if (slimeAnimationClass.IsDead)
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
            airBarriers.Add(new Rectangle(0, 0, 20, 1080));
            airBarriers.Add(new Rectangle(0, 0, 1920, 130));
            airBarriers.Add(new Rectangle(0, 0, 370, 350));
            airBarriers.Add(new Rectangle(1900, 0, 75, 1080));
            airBarriers.Add(new Rectangle(0, 1080, 1920, 75));
            airBarriers.Add(new Rectangle(1550, 0, 370, 370));
            airBarriers.Add(new Rectangle(0, 890, 410, 370));
            airBarriers.Add(new Rectangle(1500, 900, 430, 350));
            airBarriers.Add(new Rectangle(0, 790, 525, 105));
            airBarriers.Add(new Rectangle(525, 820, 120, 80));
            airBarriers.Add(new Rectangle(645, 840, 160, 50));
        }

    }
}