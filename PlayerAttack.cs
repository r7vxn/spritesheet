using Microsoft.Xna.Framework;

namespace spritesheet
{
    internal class PlayerAttack
    {
        private int attackTriggerFrame;

        public PlayerAttack(int triggerFrame = 4)
        {
            attackTriggerFrame = triggerFrame;
        }

        // Returns true when damage was applied
        internal bool TryApplyAttack(int currentFrame, Rectangle attackRect, SlimeAnimationClass slime, int damage)
        {
            if (slime == null) return false;

            if (currentFrame == attackTriggerFrame)
            {
                if (slime.CurrentCollisionRect.Intersects(attackRect))
                {
                    slime.ApplyDamage(damage);
                    return true;
                }
            }

            return false;
        }
    }
}
