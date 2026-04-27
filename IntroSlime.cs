using Microsoft.Xna.Framework;
using System;

namespace spritesheet
{
    // Lightweight intro-only slime that bounces left/right and randomly jumps
    internal class IntroSlime
    {
        private Vector2 location;
        private Vector2 velocity;
        private int direction = -1; // -1 left, 1 right
        private float jumpCooldown = 0f;
        private Random rng = new Random();
        private int frame = 0;
        private float time = 0f;
        private float frameSpeed = 0.12f;
        private SlimeAnimation state = SlimeAnimation.SlimeRunning;
        private int directionRow = 2; // left

        public IntroSlime(Vector2 start)
        {
            location = start;
            direction = rng.Next(0, 2) == 0 ? -1 : 1;
            velocity = new Vector2(direction * 80f, 0);
            jumpCooldown = (float)(0.5 + rng.NextDouble() * 1.5);
            directionRow = direction < 0 ? 2 : 3;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // simple horizontal movement
            location += velocity * dt;

            // bounce from screen-ish edges
            if (location.X < 300) { direction = 1; velocity.X = 80f; directionRow = 3; }
            if (location.X > 1620) { direction = -1; velocity.X = -80f; directionRow = 2; }

            // random jump impulse
            jumpCooldown -= dt;
            if (jumpCooldown <= 0f)
            {
                // apply upward impulse (visual only)
                velocity.Y = -220f;
                jumpCooldown = (float)(0.8 + rng.NextDouble() * 2.0);
            }

            // gravity
            velocity.Y += 600f * dt;
            // ground at Y = 720 (approx)
            if (location.Y >= 720)
            {
                location.Y = 720;
                velocity.Y = 0;
            }

            // animation timing
            time += dt;
            if (time > frameSpeed)
            {
                time = 0f;
                frame++;
                int frames = 8;
                if (frame >= frames) frame = 0;
            }
        }

        public Rectangle GetDrawRect()
        {
            return new Rectangle((int)location.X - 55, (int)location.Y - 50, 225, 225);
        }

        public SlimeAnimation GetState() => state;
        public int GetFrame() => frame;
        public int GetDirectionRow() => directionRow;
    }
}
