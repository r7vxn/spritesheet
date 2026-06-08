using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace spritesheet.Skills
{
    // Basic single-trigger melee attack skill
    public class BasicAttackSkill : SkillBase
    {
        // callback used to perform the hit detection and apply damage when the hit frame occurs
        private readonly Action OnPerformHit;
        private readonly Action OnStart;
        private readonly Func<bool> CanPerform;

        private bool triggered = false;
        private float attackDuration = 0.5f; // seconds (animation length)
        private float attackTimer = 0f;
        private bool hitPerformed = false; // ensure hit callback runs once per attack

        public BasicAttackSkill(Action onPerformHit, Action onStart, Func<bool> canPerform, float cooldown = 0.35f) : base("BasicAttack", cooldown)
        {
            OnPerformHit = onPerformHit;
            OnStart = onStart;
            CanPerform = canPerform;
        }

        public bool IsActive => triggered;

        public override void HandleInput(KeyboardState current, KeyboardState previous)
        {
            // single trigger on key press (Space)
            if (current.IsKeyDown(Keys.Space) && !previous.IsKeyDown(Keys.Space))
            {
                if (IsAvailable && CanPerform())
                {
                    triggered = true;
                    // Start cooldown immediately but keep triggered true until animation ends
                    CooldownTimer = Cooldown;
                    // reset internal timers/flags for this activation
                    attackTimer = attackDuration;
                    hitPerformed = false;
                    OnStart?.Invoke();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            // drive attack timing and trigger the hit callback at the midpoint of the animation
            if (!triggered) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            attackTimer -= dt;

            // perform the hit once at ~50% of the attackDuration
            if (!hitPerformed && attackTimer <= attackDuration * 0.5f)
            {
                hitPerformed = true;
                OnPerformHit?.Invoke();
            }

            // if timer expired, end the skill (animation may also call End externally)
            if (attackTimer <= 0f)
            {
                End();
            }
        }

        // Called externally when the animation finished to end the attack activity
        public void End()
        {
            triggered = false;
            attackTimer = 0f;
        }
    }
}
