using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace spritesheet.Skills
{
    public interface ISkill
    {
        string Name { get; }
        bool IsAvailable { get; }
        void Update(GameTime gameTime);
        void HandleInput(KeyboardState current, KeyboardState previous);
    }
}
