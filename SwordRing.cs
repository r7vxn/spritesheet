using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace spritesheet
{
    internal class SwordRing
    {
        private const float cooldownTime = 10f;
        private const float activeDuration = 4f;
        private float currentCooldown = 0f;
        private float currentActiveTime = 0f;
        private bool isActive = false;
        private float rotationAngle = 0f;

        private Texture2D swordTex;
        private Texture2D uiIconTex;

        private Dictionary<SlimeAnimationClass, float> slimeDamageTimers = new Dictionary<SlimeAnimationClass, float>();

        private Texture2D pixel;

        private const float drawScale = 2f;
        
        private static readonly Vector2 drawOffset = new Vector2(50f, 70f);

        public bool DebugEnabled { get; set; } = true;
        public bool IsActivePublic => isActive;
        public float CurrentCooldown => currentCooldown;
        public float CurrentActiveTime => currentActiveTime;

        public event Action? OnActivated;

        public SwordRing(Texture2D sword, Texture2D icon)
        {
            swordTex = sword;
            uiIconTex = icon;
            pixel = null;
        }

        public bool Activate()
        {
            if (currentCooldown <= 0f && !isActive)
            {
                isActive = true;
                currentActiveTime = activeDuration;
                currentCooldown = cooldownTime;
                OnActivated?.Invoke();
                // Debug.WriteLine($"SwordRing activated at {DateTime.Now}. cooldown set to {currentCooldown}");
                return true;
            }
            return false;
        }

        // now receives playerFacing so blades can orient relative to the character
        private Vector2 lastPlayerFacing = new Vector2(0f, 1f);

        public void Update(GameTime gameTime, Vector2 playerCenter, List<SlimeAnimationClass> slimes, Vector2 playerFacing)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (currentCooldown > 0f)
            {
                currentCooldown -= elapsed;
                if (currentCooldown < 0f) currentCooldown = 0f;
            }

            if (isActive)
            {
                // Debug.WriteLine("SwordRing isActive update, activeTime=" + currentActiveTime);
                currentActiveTime -= elapsed;
                if (currentActiveTime <= 0f)
                {
                    isActive = false;
                    slimeDamageTimers.Clear();
                }

                rotationAngle += MathHelper.TwoPi * elapsed;

                var keys = new List<SlimeAnimationClass>(slimeDamageTimers.Keys);
                foreach (var s in keys)
                {
                    slimeDamageTimers[s] -= elapsed;
                    if (slimeDamageTimers[s] <= 0f)
                    {
                        slimeDamageTimers.Remove(s);
                    }
                }

                // store last facing for use during draw
                if (playerFacing != Vector2.Zero) lastPlayerFacing = Vector2.Normalize(playerFacing);
                CheckCollisions(playerCenter, slimes);
            }
        }

        private void CheckCollisions(Vector2 playerCenter, List<SlimeAnimationClass> slimes)
        {
            if (swordTex == null || slimes == null) return;

            const float radius = 78f;
            int bladeW = swordTex.Width;
            int bladeH = swordTex.Height;

            Vector2 origin = new Vector2(bladeW * 0.5f, bladeH * 0.9f);
            // scaled sizes and origin to match drawing with scale
            int bladeWScaled = (int)(bladeW * drawScale);
            int bladeHScaled = (int)(bladeH * drawScale);
            Vector2 originScaled = origin * drawScale;

            for (int i = 0; i < 4; i++)
            {
                float angle = rotationAngle + i * MathHelper.PiOver2;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 bladePos = playerCenter + dir * radius + drawOffset;

                Rectangle bladeRect = new Rectangle(
                    (int)(bladePos.X - originScaled.X),
                    (int)(bladePos.Y - originScaled.Y),
                    bladeWScaled,
                    bladeHScaled);

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
                            Vector2 slimePos = new Vector2(slime.CurrentDrawRect.Center.X, slime.CurrentDrawRect.Center.Y);
                            slime.ApplyDamage(3, playerCenter);
                            slimeDamageTimers[slime] = 1.0f;
                        }
                    }
                }
            }
        }

        // DrawSkill now uses the last known player facing to orient blades perpendicular to the character
        public void DrawSkill(SpriteBatch spriteBatch, Vector2 playerCenter)
        {
            if (!isActive || swordTex == null) return;

            int bladeW = swordTex.Width;
            int bladeH = swordTex.Height;
            Vector2 origin = new Vector2(bladeW * 0.5f, bladeH * 0.9f);
            const float radius = 78f;

            // base perpendicular direction to player's facing: rotate facing by 90 degrees
            Vector2 perp = new Vector2(-lastPlayerFacing.Y, lastPlayerFacing.X);
            // ensure perp is normalized
            if (perp != Vector2.Zero) perp = Vector2.Normalize(perp);

            for (int i = 0; i < 4; i++)
            {
                // place blades around the ring but align rotation to perp vector
                float angle = rotationAngle + i * MathHelper.PiOver2;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 bladePos = playerCenter + dir * radius + drawOffset;

                // rotation that aligns the sprite to point away from the player (radial)
                float drawRotation = (float)Math.Atan2(dir.Y, dir.X) + MathHelper.PiOver2;

                spriteBatch.Draw(swordTex, bladePos, null, Color.White, drawRotation, origin, drawScale, SpriteEffects.None, 0f);
            }
        }

        public void DrawUI(SpriteBatch spriteBatch, SpriteFont font, Vector2 screenPosition)
        {
            if (uiIconTex == null) return;

            Rectangle iconRect = new Rectangle((int)screenPosition.X, (int)screenPosition.Y, uiIconTex.Width, uiIconTex.Height);

            spriteBatch.Draw(uiIconTex, iconRect, Color.White);

            if (currentCooldown > 0f)
            {
                Color overlay = Color.Gray * 0.6f;
                if (pixel == null)
                {
                    pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                    pixel.SetData(new[] { Color.White });
                }
                spriteBatch.Draw(pixel, iconRect, overlay);

                string text = Math.Round(currentCooldown, 1).ToString("0.0");
                Vector2 textSize = font.MeasureString(text);
                Vector2 textPos = new Vector2(iconRect.Center.X - textSize.X * 0.5f, iconRect.Center.Y - textSize.Y * 0.5f);
                spriteBatch.DrawString(font, text, textPos, Color.Yellow);
            }
        }
    }
}
