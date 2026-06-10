using Microsoft.Xna.Framework;

namespace spritesheet
{
    // Minimal Slime implementation to satisfy SwordRing usage during compilation/testing.
    internal class Slime
    {
        public Rectangle Hitbox { get; set; }
        public Vector2 Position { get; set; }

        public Slime()
        {
            Hitbox = new Rectangle(0, 0, 32, 32);
            Position = Vector2.Zero;
        }

        public void TakeDamage(int amount)
        {
            // minimal stub: integrate with actual health system in game
        }

        public void ApplyKnockback(Vector2 direction, float force)
        {
            // minimal stub: integrate with physics/velocity in game
            Position += direction * (force * 0.01f);
        }

        public void CancelAttack()
        {
            // minimal stub
        }
    }
}
