using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
        // barriers the user creates at runtime (persisted). Keep separate to avoid duplicating built-in map barriers
        List<Rectangle> customBarriers = new List<Rectangle>();


        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        Texture2D customButtons;
        Rectangle window = new Rectangle(0, 0, 1920, 1080);
        Texture2D rectangleTexture;
        Texture2D backgroundTexture;
        Texture2D introTexture;
        Rectangle introTitleRect = new Rectangle(570, 20, 800, 500);
        Texture2D introTitleTexture;
        Rectangle introRect = new Rectangle(560, 440, 800, 200);
        SpriteFont font;
        Vector2 introVector;
        MouseState mouseState;
        // previous mouse state for UI click detection
        MouseState _previousMouse;
        Buttons uiButtons;
        // barrier editor fields
        bool barrierEditMode = false;
        Point barrierStart;
        bool isDragging = false;
        Rectangle currentBarrier = Rectangle.Empty;
        KeyboardState previousKeyboardState;
        string barriersFilePath;
        bool playerDied = false;
        string endScreenMessage;



        Rectangle playerCollisionRect, playerDrawRect, attackCollisionRect;
        bool attack = false;
        int playerHealth;
        int playerDamage;
        // player hurt / knockback state
        bool playerHurt = false;
        float playerHurtTimer = 0f;
        float playerHurtDuration = 0.2f;
        Vector2 playerKnockbackVelocity = Vector2.Zero;
        // reduced knockback speed to make hits feel less jarring
        float playerKnockbackSpeed = 450f;
        // invincibility frames after being hit
        bool playerInvincible = false;
        float playerInvincibleTimer = 0f;
        // slightly longer invincibility so player can move smoothly between hits
        float playerInvincibleDuration = 0.8f;
        // flag to run hurt animation initialization once
        bool playerHurtStarted = false;


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

        Rectangle slimeCollisionRect, slimeAttackRect, slimeRangeRect, slimeDrawRect;
        Vector2 slimeLocation;
        Vector2 slimeDirection;
        int slimeDirectionRow, slimeLeftRow, slimeRightRow, slimeUpRow, slimeDownRow;
        SlimeAnimation slimeState;
        int slimeFrame;
        float slimeTime, slimeFrameSpeed = 1f, slimeSpeed = 5f;
        int slimeFrames;
        bool slimeReset = true;
        bool slimeAttackState = false;
        bool slimeFrameCheck = false;
        bool slimeAttackCollision = false;
        bool slimeAttacked = false;
        int slimeHealth = 15;
        int slimeDamage;
        float slimeAttackTimer;
        bool slimeDied = false;
        bool slimeDeathStarted = false;
        float endDelayTimer = 2f;
        bool slimeAttackStarted = false;
        bool slimeDeathDraw = false;

        int resChange = 2;

        SlimeDraw slimeDraw;
        SlimeManager slimeManager;
        SlimeSoundEffect slimeSoundEffect;
        SlimeAnimationClass slimeAnimationClass;
        List<SlimeAnimationClass> slimes = new List<SlimeAnimationClass>();

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

            // initialize previous keyboard state for single-key detection
            previousKeyboardState = Keyboard.GetState();
            barriersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "barriers.json");

            MediaPlayer.Play(song);
            MediaPlayer.Volume = 0.18f;

            slimeJumpInstance = slimeJump.CreateInstance();
            slimeJumpInstance.Pitch = -0.2f;
            slimeJumpInstance.Volume = 0.6f;

            slimeBeingSlashInstance = slimeBeingSlashed.CreateInstance();
            slimeJumpInstance.Volume = 0.6f;
            slimeHittingGroundInstance = slimeHittingGround.CreateInstance();
            slimeHittingGroundInstance.Volume = 0.6f;

            SetupCollision();

            // load saved custom barriers (appended to built-in map barriers)
            LoadBarriers();

            // ensure barriers are saved when the game exits
            this.Exiting += Game1_Exiting;

            screen = Screen.intro;

            introVector.X = window.Width / 2 - 100;
            introVector.Y = window.Height / 2 - 40;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            playerHealth = 20;
            playerDamage = 5;
            slimeDamage = 3;

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

            slimeRangeRect = new Rectangle(0, 0, 70, 80);
            slimeCollisionRect = new Rectangle(0, 0, 50, 50);
            slimeAttackRect = new Rectangle(0, 0, 45, 20);
            slimeDrawRect = new Rectangle(960, 540, 225, 225);


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
            // check built-in barriers
            foreach (Rectangle barrier in airBarriers)
                if (nextHitbox.Intersects(barrier))
                    return false;
            // check user-created barriers (persisted separately)
            foreach (Rectangle barrier in customBarriers)
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

            introTitleTexture = Content.Load<Texture2D>("Slime Fall logo");
            customButtons = Content.Load<Texture2D>("Custom Buttons");
            rectangleTexture = Content.Load<Texture2D>("rectangle");
            backgroundTexture = Content.Load<Texture2D>("forest background");
            introTexture = Content.Load<Texture2D>("forest intro");
            font = Content.Load<SpriteFont>("Font");
            song = Content.Load<Song>("Forest Bgm");
            slimeJump = Content.Load<SoundEffect>("slime jump");
            slimeBeingSlashed = Content.Load<SoundEffect>("slime impact");
            slimeHittingGround = Content.Load<SoundEffect>("slime hit ground");

            var wholelist = new List<List<Texture2D>>() { Idlespritesheets, Runningspritesheets, Attackspritesheets, Deathspritesheets, Hurtspritesheets };
            spritesheetManager = new SpritesheetManager(wholelist);
            spritesheetDraw = new SpritesheetDraw(wholelist);  
         
            var slimelist = new List<List<Texture2D>>() { SlimeIdlespritesheets, SlimeRunningspritesheets, SlimeAttackspritesheets, SlimeDeathspritesheets, SlimeHurtspritesheets };
            slimeDraw = new SlimeDraw(slimelist);
            slimeManager = new SlimeManager(slimelist);
            slimeSoundEffect = new SlimeSoundEffect();
            // create multiple slimes at different positions
            var slimePositions = new List<Vector2>()
            {
                new Vector2(910, 400),
                new Vector2(610, 400),
                new Vector2(1210, 400),
                new Vector2(310, 450),
                new Vector2(1510, 450)
            };

            foreach (var pos in slimePositions)
            {
                var s = new SlimeAnimationClass();
                s.Initialize(pos);
                slimes.Add(s);
            }


            float scale = 1f; 
            int frameWidth  = customButtons.Width  / 4; 
            int frameHeight = customButtons.Height / 8; 

            // Mapping: btn_{row}_{col} -> frameIndex = row * columns + col
            var customSourceRects = new Dictionary<int, Rectangle>()
            {
                { 0*4 + 0, new Rectangle(148,  57, 295, 89) },
                { 0*4 + 1, new Rectangle(488, 57, 259, 89) },
                { 0*4 + 2, new Rectangle(790, 57, 258, 89) },
                { 0*4 + 3, new Rectangle(1095, 57, 293, 89) },

                { 1*4 + 0, new Rectangle(148,  175, 295, 85) },
                { 1*4 + 1, new Rectangle(488, 175, 259, 85) },
                { 1*4 + 2, new Rectangle(790, 175, 258, 85) },
                { 1*4 + 3, new Rectangle(1095, 175, 293, 85) },

                { 2*4 + 0, new Rectangle(168,  288, 255, 85) },
                { 2*4 + 1, new Rectangle(508, 288, 222, 85) },
                { 2*4 + 2, new Rectangle(809, 288, 219, 85) },
                { 2*4 + 3, new Rectangle(1113, 288, 257, 85) },

                { 3*4 + 0, new Rectangle(188,  402, 214, 82) },
                { 3*4 + 1, new Rectangle(528, 402, 180, 82) },
                { 3*4 + 2, new Rectangle(830, 402, 177, 82) },
                { 3*4 + 3, new Rectangle(1133, 402, 214, 82) },

                { 4*4 + 0, new Rectangle(208, 513, 174, 80) },
                { 4*4 + 1, new Rectangle(546, 513, 146, 80) },
                { 4*4 + 2, new Rectangle(845, 513, 145, 80) },
                { 4*4 + 3, new Rectangle(1152, 513, 175, 80) },

                { 5*4 + 0, new Rectangle(228,  622, 138, 80) },
                { 5*4 + 1, new Rectangle(546, 513, 146,  80) },
                { 5*4 + 2, new Rectangle(844, 513, 146, 80) },
                { 5*4 + 3, new Rectangle(1171, 621, 138,  80) },

                { 6*4 + 0, new Rectangle(247,  730, 98,  80) },
                { 6*4 + 1, new Rectangle(583, 730, 77,  80) },
                { 6*4 + 2, new Rectangle(878, 730, 79,  80) },
                { 6*4 + 3, new Rectangle(1191, 730, 99,  80) },

                { 7*4 + 0, new Rectangle(262, 836, 70, 72) },
                { 7*4 + 1, new Rectangle(588, 836, 65, 72) },
                { 7*4 + 2, new Rectangle(886, 836, 67, 72) },
                { 7*4 + 3, new Rectangle(1207, 836, 69, 72) }
            };

            uiButtons = new Buttons(customButtons, frameWidth, frameHeight, columns: 4, customSourceRects);
            int scaledWidth = (int)(frameWidth * scale);
            int scaledHeight = (int)(frameHeight * scale);
            var playBtnBounds = new Rectangle(
                introRect.Center.X - scaledWidth / 2,
                introRect.Center.Y - scaledHeight / 2,
                scaledWidth,
                scaledHeight);
            // store reference so we can disable/hide it after use
            Button playBtn = null;
            playBtn = uiButtons.Create(playBtnBounds, frameDefault: 0, frameHovered: 1, framePressed: 2,
                onClick: () => { screen = Screen.game; if (playBtn != null) playBtn.Enabled = false; }, text: "Play");

        }

        protected override void Update(GameTime gameTime)
        {
            MediaPlayer.IsRepeating = true;

            // drive sounds from a primary slime (first alive, or first existing)
            int primaryFrame = 0;
            SlimeAnimation primaryState = SlimeAnimation.SlimeIdle;
            if (slimes.Count > 0)
            {
                var primary = slimes.FirstOrDefault(s => !s.IsDead) ?? slimes[0];
                primaryFrame = primary.CurrentFrame;
                primaryState = primary.CurrentState;
            }
            slimeSoundEffect.update(primaryFrame, primaryState, attacked, slimeJumpInstance, slimeHittingGroundInstance, slimeBeingSlashInstance);

            KeyboardState keyboardState = Keyboard.GetState();

            // Update UI buttons with current and previous mouse state
            mouseState = Mouse.GetState();
            uiButtons?.Update(gameTime, mouseState, _previousMouse);

            if (screen == Screen.intro)
            {
                // intro button click handled by uiButtons Play button's OnClick
            }
            if (screen == Screen.game)
            {

                //// toggle and handle barrier editor
                //if (keyboardState.IsKeyDown(Keys.B) && !previousKeyboardState.IsKeyDown(Keys.B))
                //{
                //    barrierEditMode = !barrierEditMode;
                //    // reset any in-progress drag when toggling
                //    isDragging = false;
                //    currentBarrier = Rectangle.Empty;
                //    // save when disabling editor so edits persist immediately
                //    if (!barrierEditMode)
                //        SaveBarriers();
                //}

                //// clear persisted custom barriers (press V once)
                //if (keyboardState.IsKeyDown(Keys.V) && !previousKeyboardState.IsKeyDown(Keys.V))
                //{
                //    customBarriers.Clear();
                //    try
                //    {
                //        if (File.Exists(barriersFilePath)) File.Delete(barriersFilePath);
                //    }
                //    catch
                //    {
                //        // ignore deletion errors
                //    }
                //}


                if (barrierEditMode)
                {
                    mouseState = Mouse.GetState();
                    if (mouseState.LeftButton == ButtonState.Pressed && !isDragging)
                    {
                        isDragging = true;
                        barrierStart = mouseState.Position;
                    }
                    else if (mouseState.LeftButton == ButtonState.Released && isDragging)
                    {
                        isDragging = false;
                        var end = mouseState.Position;
                        var rect = new Rectangle(Math.Min(barrierStart.X, end.X), Math.Min(barrierStart.Y, end.Y),
                            Math.Abs(end.X - barrierStart.X), Math.Abs(end.Y - barrierStart.Y));
                        if (rect.Width > 4 && rect.Height > 4)
                            customBarriers.Add(rect);
                        currentBarrier = Rectangle.Empty;
                    }

                    if (isDragging)
                    {
                        var cur = Mouse.GetState().Position;
                        currentBarrier = new Rectangle(Math.Min(barrierStart.X, cur.X), Math.Min(barrierStart.Y, cur.Y),
                            Math.Abs(cur.X - barrierStart.X), Math.Abs(cur.Y - barrierStart.Y));
                    }

                    // undo last barrier
                    if (keyboardState.IsKeyDown(Keys.C) && !previousKeyboardState.IsKeyDown(Keys.C))
                    {
                        if (customBarriers.Count > 0) customBarriers.RemoveAt(customBarriers.Count - 1);
                    }
                }
                // Update all slimes
                foreach (var s in slimes)
                    s.update(gameTime, playerCollisionRect, playerLocation, slimeFramesPerDirection);

                var primarySlime = slimes.FirstOrDefault(s => !s.IsDead) ?? (slimes.Count > 0 ? slimes[0] : null);
                if (primarySlime != null)
                {
                    slimeState = primarySlime.CurrentState;
                    slimeFrame = primarySlime.CurrentFrame;
                    slimeDirectionRow = primarySlime.CurrentDirectionRow;
                    slimeCollisionRect = primarySlime.CurrentCollisionRect;
                    slimeDrawRect = primarySlime.CurrentDrawRect;
                    slimeAttackRect = primarySlime.CurrentAttackRect;
                    slimeAttackCollision = primarySlime.CurrentAttackCollision;
                    slimeDied = primarySlime.IsDead;
                    slimeDeathDraw = primarySlime.DeathDraw;
                }

                // handle player input-driven movement unless knocked back
                playerDirection = Vector2.Zero;
                if (!playerHurt)
                {
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
                }
                else
                {
                    // while hurt, apply knockback velocity but allow player input to adjust movement
                    state = Animation.Hurt;
                    float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // primary knockback application
                    Vector2 knockbackMove = playerKnockbackVelocity * dt;
                    Vector2 newPos = playerLocation + knockbackMove;
                    if (CanMoveTo(newPos))
                    {
                        playerLocation = newPos;
                    }
                    else
                    {
                        // cancel knockback if blocked
                        playerKnockbackVelocity = Vector2.Zero;
                        playerHurtTimer = 0f;
                        playerHurt = false;
                    }

                    // allow the player to influence movement while hurt for smoother control
                    if (playerDirection != Vector2.Zero)
                    {
                        // smaller contribution from input while hurt so knockback still dominates
                        Vector2 inputMove = playerDirection * speed * dt * 0.6f;
                        Vector2 attempted = playerLocation + inputMove;
                        if (CanMoveTo(attempted)) playerLocation = attempted;
                    }

                    // simple damping
                    playerKnockbackVelocity *= 0.4f;
                    playerHurtTimer -= dt;
                    if (playerHurtTimer <= 0f)
                    {
                        playerHurt = false;
                        playerHurtStarted = false;
                        // when hurt ends, reset animation timing so idle frame shows
                        frame = 0;
                        time = 0f;
                    }
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

                // animation
                frames = framesPerDirection[state][directionRow];
                // shorten hurt animation time by reducing frame interval (plays faster)
                frameSpeed = (state == Animation.Attack) ? 0.08f : (state == Animation.Hurt) ? 0.08f : (state == Animation.Idle && directionRow == downRow) ? 0.3f : 0.12f;

                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (time > frameSpeed)
                {
                    time = 0f;
                    if (state == Animation.Hurt && playerHurtStarted)
                    {
                        // advance hurt animation but do not loop; stop at last frame
                        frame++;
                        if (frame >= frames)
                        {
                            frame = frames - 1;
                            // mark hurt animation as having finished its single play-through
                            playerHurtStarted = false;
                        }
                    }
                    else
                    {
                        frame++;
                        if (frame >= frames) frame = 0;
                    }
                }
                //player attack (apply damage to any slime hit)
                if (state == Animation.Attack)
                {
                    if (frame == 4 && !attacked)
                    {
                        foreach (var s in slimes)
                        {
                            if (!s.IsDead && s.CurrentCollisionRect.Intersects(attackCollisionRect))
                            {
                                // pass player location so the slime can be knocked back away from the attacker
                                s.ApplyDamage(playerDamage, playerLocation);
                                attacked = true;
                                break;
                            }
                        }
                    }
                    if (frame == 0) attacked = false;
                }


                //player death
                if (playerHealth <= 0)
                {
                    playerDied = true;
                    screen = Screen.end;
                }

                // dmg to player (any slime attacking) + apply knockback. Respect invincibility frames.
                if (!playerInvincible)
                {
                    var attacker = slimes.FirstOrDefault(s => s.CurrentAttackCollision && playerCollisionRect.Intersects(s.CurrentAttackRect));
                    if (attacker != null)
                    {
                        // apply damage
                        playerHealth -= slimeDamage;

                        // compute knockback direction from slime toward player
                        Vector2 sourcePos = new Vector2(attacker.CurrentCollisionRect.Center.X, attacker.CurrentCollisionRect.Center.Y);
                        Vector2 dir = playerLocation - sourcePos;
                        if (dir != Vector2.Zero) dir.Normalize(); else dir = new Vector2(0, -1);

                        // start hurt/knockback
                        // increase knockback slightly when player is moving to give more response while walking
                        float knockbackMultiplier = (playerDirection == Vector2.Zero) ? 2.5f : 1.6f;
                        playerHurt = true;
                        playerHurtStarted = true;
                        playerHurtTimer = playerHurtDuration * knockbackMultiplier;
                        playerKnockbackVelocity = dir * playerKnockbackSpeed * knockbackMultiplier;
                        // reset player animation frame/time so hurt animation displays reliably
                        frame = 0;
                        time = 0f;

                        // set invincibility frames so player can move smoothly without repeated hits
                        playerInvincible = true;
                        playerInvincibleTimer = playerInvincibleDuration;

                        slimeAttacked = true;
                        slimeAttackTimer = 1f;
                    }
                }

                if (slimeAttacked)
                {
                    slimeAttackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (slimeAttackTimer <= 0f) slimeAttacked = false;
                }

                // end the game when all slimes have finished their death animation
                if (slimes.Count > 0 && slimes.All(s => s.DeathDraw))
                {
                    slimeDied = true;
                    screen = Screen.end;
                }

    
            }

            if (screen == Screen.end)
            {

                if (keyboardState.IsKeyDown(Keys.Enter))
                {
                    Exit();
                }
            }
            // store keyboard state for next-frame single-key detection
            previousKeyboardState = keyboardState;

            // store previous mouse state for UI click detection
            _previousMouse = mouseState;

            // update invincibility timer
            if (playerInvincible)
            {
                playerInvincibleTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (playerInvincibleTimer <= 0f)
                    playerInvincible = false;
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
                uiButtons?.Draw(_spriteBatch, font);
                _spriteBatch.Draw(introTitleTexture, introTitleRect, Color.White);

                _spriteBatch.End();
            }
            if (screen == Screen.game)
            {

                _spriteBatch.Begin();

                int currentColumns = framesPerDirection[state][directionRow];
                int currentRows = rowsPerState[state];

                _spriteBatch.Draw(backgroundTexture, window, Color.White);

                // draw UI buttons for in-game HUD if any
                uiButtons?.Draw(_spriteBatch, font);

                //// draw editor UI
                //if (barrierEditMode)
                //{
                //    _spriteBatch.DrawString(font, "Barrier Edit: ON - Press B to toggle, Left-drag to create, C undo", new Vector2(20, 20), Color.Yellow);
                //    if (currentBarrier != Rectangle.Empty)
                //        _spriteBatch.Draw(rectangleTexture, currentBarrier, Color.Blue * 0.5f);
                //}
                //// intentionally hide the "Press B to enable barrier editor" prompt

                spritesheetManager.Draw(_spriteBatch, state, frame, playerDrawRect, directionRow, currentColumns, currentRows);

                // draw all slimes
                foreach (var s in slimes)
                {
                    if (s.DeathDraw) continue;
                    var sState = s.CurrentState;
                    var sDir = s.CurrentDirectionRow;
                    int slimeColumns = slimeFramesPerDirection[sState][sDir];
                    int slimeRows = slimeRowsPerState[sState];
                    slimeManager.Draw(_spriteBatch, sState, s.CurrentFrame, s.CurrentDrawRect, sDir, slimeColumns, slimeRows);
                }


                //// draw built-in barriers
                //foreach (Rectangle barrier in airBarriers)
                //{
                //    _spriteBatch.Draw(rectangleTexture, barrier, Color.Red * 0.3f);
                //}
                //// draw user-created barriers (persisted)
                //foreach (Rectangle barrier in customBarriers)
                //{
                //    _spriteBatch.Draw(rectangleTexture, barrier, Color.Blue * 0.35f);
                //}
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

        // persist barriers to disk so user edits survive restarts
        private void SaveBarriers()
        {
            try
            {
                var data = customBarriers.Select(r => new { r.X, r.Y, Width = r.Width, Height = r.Height }).ToArray();
                var json = JsonSerializer.Serialize(data);
                File.WriteAllText(barriersFilePath, json);
            }
            catch
            {
                // ignore save errors
            }
        }

        private void LoadBarriers()
        {
            try
            {
                if (!File.Exists(barriersFilePath)) return;
                var json = File.ReadAllText(barriersFilePath);
                var items = JsonSerializer.Deserialize<BarrierRect[]>(json);
                if (items == null) return;
                foreach (var b in items)
                {
                    customBarriers.Add(new Rectangle(b.X, b.Y, b.Width, b.Height));
                }
            }
            catch
            {
                // ignore load errors
            }
        }

        private void Game1_Exiting(object? sender, EventArgs e)
        {
            SaveBarriers();
        }

        private class BarrierRect
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

    }
}