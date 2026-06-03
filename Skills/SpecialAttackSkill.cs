using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace spritesheet.Skills
{
    // Special attack: multi-phase skill that disables movement while active
    public class SpecialAttackSkill : SkillBase
    {
        private readonly Action OnStart;
        private readonly Action<float> OnPhaseUpdate; // phase progress
        private readonly Action OnEnd;
        private readonly Func<bool> CanPerform;

        private bool active = false;
        private float duration = 1.2f; // total duration of special attack
        private float timer = 0f;

        public bool IsActive => active;

        public SpecialAttackSkill(Action onStart, Action<float> onPhaseUpdate, Action onEnd, Func<bool> canPerform, float cooldown = 1.5f) : base("SpecialAttack", cooldown)
        {
            OnStart = onStart;
            OnPhaseUpdate = onPhaseUpdate;
            OnEnd = onEnd;
            CanPerform = canPerform;
        }

        public override void HandleInput(KeyboardState current, KeyboardState previous)
        {
            if (current.IsKeyDown(Keys.Q) && !previous.IsKeyDown(Keys.Q))
            {
                if (IsAvailable && CanPerform())
                {
                    active = true;
                    timer = duration;
                    CooldownTimer = Cooldown;
                    OnStart?.Invoke();
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!active) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= dt;
            float progress = Math.Clamp(1f - (timer / duration), 0f, 1f);
            OnPhaseUpdate?.Invoke(progress);

            if (timer <= 0f)
            {
                active = false;
                OnEnd?.Invoke();
            }
        }
    }
}
