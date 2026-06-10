using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace spritesheet
{
    internal class SwordRing
    {
        // Core variables
        private const float cooldownTime = 8f;
        private const float activeDuration = 4f;
        private float currentCooldown = 0f;
        private float currentActiveTime = 0f;
        private bool isActive = false;
        private float rotationAngle = 0f;

        private Texture2D swordTex; // "Sword Ring Sword Sprite"
        private Texture2D uiIconTex; // "Sword Ring Icon"

        private Dictionary<SlimeAnimationClass, float> slimeDamageTimers = new Dictionary<SlimeAnimationClass, float>();

        // One-pixel texture for UI overlay (created lazily)
        private Texture2D pixel;

        // Debugging / inspection
        public bool DebugEnabled { get; set; } = true;
        public bool IsActivePublic => isActive;
        public float CurrentCooldown => currentCooldown;
        public float CurrentActiveTime => currentActiveTime;

        public event Action? OnActivated;

        // Constructor accepts null textures (will be tolerant); pass textures when available.
        public SwordRing(Texture2D sword, Texture2D icon)
        {
            swordTex = sword;
            uiIconTex = icon;

            // pixel will be created lazily when needed and a GraphicsDevice is available
            pixel = null;
        }

        // Activate the skill explicitly. Returns true if activation succeeded.
        public bool Activate()
        {
            if (currentCooldown <= 0f && !isActive)
            {
                isActive = true;
                currentActiveTime = activeDuration;
                currentCooldown = cooldownTime;
                OnActivated?.Invoke();
                if (DebugEnabled) Debug.WriteLine($"SwordRing activated at {DateTime.Now}. cooldown set to {currentCooldown}");
                return true;
            }
            return false;
        }

        // Update method as specified
        public void Update(GameTime gameTime, Vector2 playerCenter, List<SlimeAnimationClass> slimes)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Activation is handled externally via Activate()

            // Cooldown ticking down
            if (currentCooldown > 0f)
            {
                currentCooldown -= elapsed;
                if (currentCooldown < 0f) currentCooldown = 0f;
            }

            if (isActive)
            {
                if (DebugEnabled) Debug.WriteLine("SwordRing isActive update, activeTime=" + currentActiveTime);
                // Active duration ticking down
                currentActiveTime -= elapsed;
                if (currentActiveTime <= 0f)
                {
                    isActive = false;
                    slimeDamageTimers.Clear();
                }

                // Increase rotation so blades spin (one full rotation per second)
                rotationAngle += MathHelper.TwoPi * elapsed; // TwoPi = 360 degrees

                // Update existing slime damage timers
                var keys = new List<SlimeAnimationClass>(slimeDamageTimers.Keys);
                foreach (var s in keys)
                {
                    slimeDamageTimers[s] -= elapsed;
                    if (slimeDamageTimers[s] <= 0f)
                    {
                        slimeDamageTimers.Remove(s);
                    }
                }

                // Collision checking
                CheckCollisions(playerCenter, slimes);
            }
        }

        // Private collision checking method
        private void CheckCollisions(Vector2 playerCenter, List<SlimeAnimationClass> slimes)
        {

            if (swordTex == null || slimes == null) return;

            const float radius = 70f;
            int bladeW = swordTex.Width;
            int bladeH = swordTex.Height;

            // Use an origin that places the sword handle near the player so the blade tip points outward
            Vector2 origin = new Vector2(bladeW * 0.5f, bladeH * 0.9f);

            for (int i = 0; i < 4; i++)
            {
                float angle = rotationAngle + i * MathHelper.PiOver2; // 90 degrees apart
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 bladePos = playerCenter + dir * radius;

                // Blade rectangle positioned consistent with DrawSkill using origin
                Rectangle bladeRect = new Rectangle(
                    (int)(bladePos.X - origin.X),
                    (int)(bladePos.Y - origin.Y),
                    bladeW,
                    bladeH);

                // Check against slimes
                foreach (var slime in slimes)
                {
                    if (slime == null) continue;

                    Rectangle slimeHitbox = slime.CurrentCollisionRect;

                    if (bladeRect.Intersects(slimeHitbox))
                    {
                        bool canHit = true;
                        if (slimeDamageTimers.TryGetValue(slime, out float timer))
                        {
                            if (timer > 0f) canHit = false;
                        }

                        if (canHit)
                        {
                            // Deal damage using SlimeAnimationClass.ApplyDamage which also applies knockback
                            // derive slime position from its draw rect center
                            Vector2 slimePos = new Vector2(slime.CurrentDrawRect.Center.X, slime.CurrentDrawRect.Center.Y);
                            slime.ApplyDamage(3, playerCenter);

                            // Add to damage timers with 1 second interval
                            slimeDamageTimers[slime] = 1.0f;
                        }
                    }
                }
            }
        }

        // Draw the 4 swords orbiting the player when active
        public void DrawSkill(SpriteBatch spriteBatch, Vector2 playerCenter)
        {
            if (!isActive || swordTex == null) return;

            int bladeW = swordTex.Width;
            int bladeH = swordTex.Height;
            Vector2 origin = new Vector2(bladeW * 0.5f, bladeH * 0.9f);
            const float radius = 78f; // slightly larger so swords don't overlap player

            for (int i = 0; i < 4; i++)
            {
                float angle = rotationAngle + i * MathHelper.PiOver2;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 bladePos = playerCenter + dir * radius;

                // Offset the draw rotation so sword tips point outward.
                float drawRotation = angle + MathHelper.PiOver2;

                // Use layerDepth near 0 to draw on top if using SpriteSortMode.FrontToBack
                spriteBatch.Draw(swordTex, bladePos, null, Color.White, drawRotation, origin, 1f, SpriteEffects.None, 0f);
            }
        }

        // Draw the UI icon, cooldown overlay, and remaining cooldown text
        public void DrawUI(SpriteBatch spriteBatch, SpriteFont font, Vector2 screenPosition)
        {
            if (uiIconTex == null) return;

            Rectangle iconRect = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, uiIconTex.Width, uiIconTex.Height);

            // Draw the icon
            spriteBatch.Draw(uiIconTex, iconRect, Color.White);

            if (currentCooldown > 0f)
            {
                // Semi-transparent grey rectangle over the icon
                Color overlay = Color.Gray * 0.6f;
                if (pixel == null)
                {
                    // create lazily
                    pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.White });
                }
                spriteBatch.Draw(pixel, iconRect, overlay);

                // Draw remaining cooldown time centered
                string text = Math.Round(currentCooldown, 1).ToString("0.0");
                Vector2 textSize = font.MeasureString(text);
                Vector2 textPos = new Vector2(iconRect.Center.X - textSize.X * 0.5f, iconRect.Center.Y - textSize.Y * 0.5f);
                spriteBatch.DrawString(font, text, textPos, Color.Yellow);
            }
        }
    }
}
