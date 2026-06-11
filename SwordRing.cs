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

        public void Update(GameTime gameTime, Vector2 playerCenter, List<SlimeAnimationClass> slimes)
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

                CheckCollisions(playerCenter, slimes);
            }
        }

        private void CheckCollisions(Vector2 playerCenter, List<SlimeAnimationClass> slimes)
        {
            if (swordTex == null || slimes == null) return;

            const float radius = 70f;
            int bladeW = swordTex.Width;
            int bladeH = swordTex.Height;

            Vector2 origin = new Vector2(bladeW * 0.5f, bladeH * 0.9f);

            for (int i = 0; i < 4; i++)
            {
                float angle = rotationAngle + i * MathHelper.PiOver2;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 bladePos = playerCenter + dir * radius;

                Rectangle bladeRect = new Rectangle(
                    (int)(bladePos.X - origin.X),
                    (int)(bladePos.Y - origin.Y),
                    bladeW,
                    bladeH);

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

        public void DrawSkill(SpriteBatch spriteBatch, Vector2 playerCenter)
        {
            if (!isActive || swordTex == null) return;

            int bladeW = swordTex.Width;
            int bladeH = swordTex.Height;
            Vector2 origin = new Vector2(bladeW * 0.5f, bladeH * 0.9f);
            const float radius = 78f;

            for (int i = 0; i < 4; i++)
            {
                float angle = rotationAngle + i * MathHelper.PiOver2;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 bladePos = playerCenter + dir * radius;

                float drawRotation = angle + MathHelper.PiOver2;

                spriteBatch.Draw(swordTex, bladePos, null, Color.White, drawRotation, origin, 1f, SpriteEffects.None, 0f);
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
