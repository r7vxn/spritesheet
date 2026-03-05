using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spritesheet
{

    public class SlimeSoundEffect
    {
        public SlimeSoundEffect()
        {

        }
        public void update(int SlimeFrame, SlimeAnimation SlimeState, bool Attacked, SoundEffectInstance SlimeJumpInstance, SoundEffectInstance SlimeHittingGroundInstance, SoundEffectInstance SlimeBeingSlashInstance)
        {
           
            if (SlimeFrame == 4 && SlimeState == SlimeAnimation.SlimeRunning)
            {
                SlimeJumpInstance.Play();
            }
            if (SlimeFrame == 5 && SlimeState == SlimeAnimation.SlimeAttack)
            {
                SlimeHittingGroundInstance.Play();
            }
            if (Attacked)
            {
                SlimeBeingSlashInstance.Play();
            }

        }
        
    }
}
