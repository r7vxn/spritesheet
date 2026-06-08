using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace spritesheet.Skills
{
    public abstract class SkillBase : ISkill
    {
        public string Name { get; protected set; }
        public float Cooldown { get; protected set; }
        public float CooldownTimer { get; protected set; }

        public virtual bool IsAvailable => CooldownTimer <= 0f;

        public SkillBase(string name, float cooldown)
        {
            Name = name;
            Cooldown = cooldown;
            CooldownTimer = 0f;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (CooldownTimer > 0f)
            {
                CooldownTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (CooldownTimer < 0f) CooldownTimer = 0f;
            }
        }

        public abstract void HandleInput(KeyboardState current, KeyboardState previous);
    }
}
