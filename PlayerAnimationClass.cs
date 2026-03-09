using Microsoft.Xna.Framework;
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace spritesheet
{
    internal class PlayerAnimationClass
    {
        private Vector2 playerLocation;
        private Vector2 playerDirection;
        private int directionRow, leftRow, rightRow, upRow, downRow;
        private Animation state;
        private int frame;
        private float time, frameSpeed = 0.1f, speed = 3f;
        private int frames;

        private Rectangle playerCollisionRect, playerDrawRect, attackCollisionRect;
        private int playerHealth;
        private int playerDamage;

        // invincibility (hurt) handling
        private bool invincible = false;
        private float invincibilityTimer = 0f;
        private float invincibilityDuration = 1.0f; // seconds

        // optional hurt sound
        private SoundEffectInstance hurtSoundInstance;

        public Animation CurrentState => state;
        public int CurrentFrame => frame;
        public int CurrentDirectionRow => directionRow;
        public Rectangle CurrentCollisionRect => playerCollisionRect;
        public Rectangle CurrentDrawRect => playerDrawRect;
        public Rectangle CurrentAttackRect => attackCollisionRect;
        public Vector2 CurrentLocation => playerLocation;
        public int CurrentHealth => playerHealth;
        public int CurrentDamage => playerDamage;

        public PlayerAnimationClass()
        {
        }

        public void Initialize(int left, int right, int up, int down, Vector2 initialLocation, int health = 15, int damage = 5)
        {
            leftRow = left;
            rightRow = right;
            upRow = up;
            downRow = down;

            playerLocation = initialLocation;
            playerHealth = health;
            playerDamage = damage;

            state = Animation.Idle;
            frame = 0;
            playerCollisionRect = new Rectangle((int)playerLocation.X, (int)playerLocation.Y, 40, 70);
            playerDrawRect = new Rectangle((int)playerLocation.X - 55, (int)playerLocation.Y - 40, 225, 225);
            attackCollisionRect = new Rectangle(0, 0, 0, 0);
            directionRow = downRow;
        }

        private bool CanMoveTo(Vector2 newPosition, List<Rectangle> airBarriers)
        {
            Rectangle nextHitbox = new Rectangle(
                (int)newPosition.X + 45,
                (int)newPosition.Y + 40,
                30,
                70
            );

            if (airBarriers != null)
            {
                foreach (Rectangle barrier in airBarriers)
                    if (nextHitbox.Intersects(barrier))
                        return false;
            }

            return true;
        }

        public void ApplyDamage(int amount, Vector2 knockback)
        {
            if (playerHealth <= 0) return;
            if (invincible) return;

            playerHealth -= amount;

            // set hurt animation and reset frame
            state = Animation.Hurt;
            frame = 0;

            // apply knockback immediately
            playerLocation += knockback;

            // start invincibility
            invincible = true;
            invincibilityTimer = invincibilityDuration;

            // play hurt sound if assigned
            try
            {
                hurtSoundInstance?.Play();
            }
            catch { }
        }

        public void SetHurtSoundInstance(SoundEffectInstance inst)
        {
            hurtSoundInstance = inst;
        }

        public void Update(GameTime gameTime, KeyboardState keyboardState, Dictionary<Animation, Dictionary<int, int>> framesPerDirection, Dictionary<Animation, int> rowsPerState, List<Rectangle> airBarriers)
        {
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
            else if (playerDirection.Y < 0) directionRow = upRow;
            else if (playerDirection.Y > 0) directionRow = downRow;

            //collision
            Vector2 newPosX = playerLocation + new Vector2(playerDirection.X * speed, 0);
            if (CanMoveTo(newPosX, airBarriers)) playerLocation = newPosX;

            Vector2 newPosY = playerLocation + new Vector2(0, playerDirection.Y * speed);
            if (CanMoveTo(newPosY, airBarriers)) playerLocation = newPosY;

            playerCollisionRect.Location = playerLocation.ToPoint();
            playerDrawRect.X = playerCollisionRect.X - 55;
            playerDrawRect.Y = playerCollisionRect.Y - 40;

            //animation - defensive: framesPerDirection may be null or missing entries
            int columns = 1;
            if (framesPerDirection != null && framesPerDirection.TryGetValue(state, out var dirDict) && dirDict != null)
            {
                if (!dirDict.TryGetValue(directionRow, out columns))
                    columns = 1;
            }
            frames = Math.Max(1, columns);
            frameSpeed = (state == Animation.Attack) ? 0.08f : (state == Animation.Idle && directionRow == downRow) ? 0.3f : 0.12f;

            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time > frameSpeed)
            {
                time = 0f;
                frame++;
                if (frame >= frames) frame = 0;
            }

            // update invincibility timer
            if (invincible)
            {
                invincibilityTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (invincibilityTimer <= 0f)
                {
                    invincible = false;
                    invincibilityTimer = 0f;
                }
            }
        }
    }
}
