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
        // hurt / knockback state
        private bool slimeHurt = false;
        private float slimeHurtTimer = 0f;
        private float slimeHurtDuration = 0.4f;
        private Vector2 slimeKnockbackVelocity = Vector2.Zero;
        private float slimeKnockbackSpeed = 300f;
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
        // Initialize slime with a specific start position
        public void Initialize(Vector2 startPosition)
        {
            Initialize();
            slimeLocation = startPosition;
            // update rects to match the new position
            slimeDrawRect = new Rectangle((int)slimeLocation.X - 55, (int)slimeLocation.Y - 50, 225, 225);
            slimeCollisionRect = new Rectangle(slimeDrawRect.X + 50, slimeDrawRect.Y + 50, 50, 50);
            slimeAttackRect = new Rectangle((int)slimeLocation.X, (int)slimeLocation.Y + 23, 45, 20);
        }

        // Allow external code (e.g. player) to apply damage to this slime
        // Now accepts the source position so we can apply knockback away from the attacker
        public void ApplyDamage(int amount, Vector2 sourcePosition)
        {
            // reduce health
            slimeHealth -= amount;

            // start hurt animation and knockback
            slimeState = SlimeAnimation.SlimeHurt;
            slimeFrame = 0;
            slimeHurt = true;
            slimeHurtTimer = slimeHurtDuration;

            // compute knockback direction away from the source
            Vector2 dir = slimeLocation - sourcePosition;
            if (dir != Vector2.Zero)
                dir.Normalize();
            else
                dir = new Vector2(0, -1);

            slimeKnockbackVelocity = dir * slimeKnockbackSpeed;

            // cancel any ongoing attack
            slimeAttackState = false;
            slimeAttackStarted = false;
            slimeAttackCollision = false;
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


            // Update rects based on current slimeLocation
            slimeDrawRect.X = (int)slimeLocation.X - 55;
            slimeDrawRect.Y = (int)slimeLocation.Y - 50;

            slimeCollisionRect.Location = new Point(slimeDrawRect.X + 50, slimeDrawRect.Y + 50);
            slimeAttackRect.Location = new Point((int)slimeLocation.X, (int)slimeLocation.Y + 23);
            // after updating the slime's attack rect, use it as the effective
            // detection area so that range == attack collision
            slimeRangeRect = slimeAttackRect;

            // slime movement and hurt/knockback handling
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (slimeHurt && !slimeDeathStarted)
            {
                // apply knockback while hurt
                slimeHurtTimer -= dt;
                slimeLocation += slimeKnockbackVelocity * dt;
                // simple damping so knockback slows down
                slimeKnockbackVelocity *= 0.4f;

                if (slimeHurtTimer <= 0f)
                {
                    slimeHurt = false;
                    slimeKnockbackVelocity = Vector2.Zero;
                    if (!slimeDied && !slimeDeathStarted)
                        slimeState = SlimeAnimation.SlimeRunning;
                }
                else
                {
                    // remain in hurt state
                    slimeState = SlimeAnimation.SlimeHurt;
                }
            }

            if (!slimeDeathStarted && !slimeAttackState && !slimeHurt)
            {
                if (!slimeAttackState && !slimeDeathStarted)
                    slimeLocation += slimeDirection * slimeSpeed * dt;

                slimeDirection = playerLocation - slimeLocation;

                if (slimeDirection != Vector2.Zero)
                {
                    slimeDirection.Normalize();
                    slimeLocation += slimeDirection * slimeSpeed * dt;

                    //slime direction
                    if (Math.Abs(slimeDirection.X) > Math.Abs(slimeDirection.Y))
                        slimeDirectionRow = (slimeDirection.X > 0) ? slimeRightRow : slimeLeftRow;
                    else
                        slimeDirectionRow = (slimeDirection.Y > 0) ? slimeDownRow : slimeUpRow;
                }
            }

            //slime animation
            slimeFrames = slimeFramesPerDirection[slimeState][slimeDirectionRow];
            if (slimeState == SlimeAnimation.SlimeHurt)
                slimeFrameSpeed = 0.08f;
            else
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
             
                // trigger an attack when the player's collision rect intersects
                // the slime's attack hitbox (so range == attack collision)
                if (slimeAttackRect.Intersects(playerCollisionRect) && !slimeHurt && !slimeDeathStarted)
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

                    if (!slimeHurt)
                        slimeState = SlimeAnimation.SlimeAttack;

                    if (slimeFrame >= 5 && slimeFrame <= 8)
                    {
                        slimeAttackCollision = true;
                    }
                    else
                    {
                        slimeAttackCollision = false;
                    }

                    if (slimeFrame >= slimeFrames - 1)
                    {
                        slimeAttackState = false;
                        slimeAttackStarted = false;
                        slimeFrame = 0;
                    }
                }
                else if (!slimeDeathStarted && !slimeHurt)
                {
                    slimeState = SlimeAnimation.SlimeRunning;
                }
            }
        }
    }
}
