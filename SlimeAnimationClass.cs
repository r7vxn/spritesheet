using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spritesheet
{
    internal class SlimeAnimationClass
    {
        private Rectangle slimeRangeRect;
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
        private bool slimeDeathStarted = false;
        private float endDelayTimer = 2f;
        private bool slimeAttackStarted = false;
        private bool slimeDeathDraw = false;
        // internal rects for slime
        private Rectangle slimeCollisionRect;
        private Rectangle slimeAttackRect;
        private Rectangle slimeDrawRect;

        // Expose important state to the outside world as read-only properties
        public SlimeAnimation CurrentState => slimeState;
        public int CurrentFrame => slimeFrame;
        public int CurrentDirectionRow => slimeDirectionRow;
        public Rectangle CurrentCollisionRect => slimeCollisionRect;
        public Rectangle CurrentDrawRect => slimeDrawRect;
        public Rectangle CurrentAttackRect => slimeAttackRect;
        public bool CurrentAttackCollision => slimeAttackCollision;
        public bool IsDead => slimeDied;
        public bool DeathDraw => slimeDeathDraw;
        public SlimeAnimationClass()
        {

        }
        public void Initialize()
        {
            slimeRangeRect = new Rectangle(0, 0, 70, 80);
            slimeState = SlimeAnimation.SlimeIdle;
            slimeLeftRow = 2;
            slimeRightRow = 3;
            slimeUpRow = 1;
            slimeDownRow = 0;
            slimeDirectionRow = slimeDownRow;
            slimeLocation = new Vector2(960, 540);
            // initialize rects
            slimeDrawRect = new Rectangle((int)slimeLocation.X - 55, (int)slimeLocation.Y - 50, 225, 225);
            slimeCollisionRect = new Rectangle(slimeDrawRect.X + 50, slimeDrawRect.Y + 50, 50, 50);
            slimeAttackRect = new Rectangle((int)slimeLocation.X, (int)slimeLocation.Y + 23, 45, 20);

        }
        public void update(GameTime gameTime, Rectangle playerCollisionRect, Vector2 playerLocation, Dictionary<SlimeAnimation, Dictionary<int, int>> slimeFramesPerDirection)
        {
            // slime's dying process
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

            //slime logic
            if (slimeDied)
            {
                endDelayTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (endDelayTimer <= 0f)
                    slimeDeathDraw = true;
            }

            slimeRangeRect.X = playerCollisionRect.X - 15;
            slimeRangeRect.Y = playerCollisionRect.Y - 5;

            // Update rects based on current slimeLocation
            slimeDrawRect.X = (int)slimeLocation.X - 55;
            slimeDrawRect.Y = (int)slimeLocation.Y - 50;

            slimeCollisionRect.Location = new Point(slimeDrawRect.X + 50, slimeDrawRect.Y + 50);
            slimeAttackRect.Location = new Point((int)slimeLocation.X, (int)slimeLocation.Y + 23);

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
        }
    }
}
