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

        public BasicAttackSkill(Action onPerformHit, Action onStart, Func<bool> canPerform, float cooldown = 0.35f) : base("BasicAttack", cooldown)
        {
            OnPerformHit = onPerformHit;
            OnStart = onStart;
            CanPerform = canPerform;
        }

        public override void HandleInput(KeyboardState current, KeyboardState previous)
        {
            // single trigger on key press (Space)
            if (current.IsKeyDown(Keys.Space) && !previous.IsKeyDown(Keys.Space))
            {
                if (IsAvailable && CanPerform())
                {
                    triggered = true;
                    attackTimer = attackDuration;
                    CooldownTimer = Cooldown;
                    OnStart?.Invoke();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (triggered)
            {
                attackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                // call hit callback at mid-point of attack
                if (attackTimer <= attackDuration / 2f && attackTimer + (float)gameTime.ElapsedGameTime.TotalSeconds > attackDuration / 2f)
                {
                    OnPerformHit?.Invoke();
                }

                if (attackTimer <= 0f)
                {
                    triggered = false;
                }
            }
        }
    }
}
