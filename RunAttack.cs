using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spritesheet
{
    public class RunAttack
    {
        Texture2D runattackbodySpritesheet, runattackheadSpritesheet, runattackshadowSpritesheet, runattackswingSpritesheet, runattackswordSpritesheet, runattackswordbackSpritesheet;

        public RunAttack(Texture2D runattackbodySpritesheet, Texture2D runattackheadSpritesheet, Texture2D runattackshadowSpritesheet, Texture2D runattackswingSpritesheet, Texture2D runattackswordSpritesheet, Texture2D runattackswordbackSpritesheet)
        {
            this.runattackbodySpritesheet = runattackbodySpritesheet;
            this.runattackheadSpritesheet = runattackheadSpritesheet;
            this.runattackshadowSpritesheet = runattackshadowSpritesheet;
            this.runattackswingSpritesheet = runattackswingSpritesheet;
            this.runattackswordSpritesheet = runattackswordSpritesheet;
            this.runattackswordbackSpritesheet = runattackswordbackSpritesheet;
        }
    }
}
