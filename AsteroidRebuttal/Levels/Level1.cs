using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AsteroidRebuttal.Levels
{
    public class Level1 : Level
    {
        public static Texture2D Level1Texture;

        public Level1(LevelManager thisManager) : base(thisManager) { }

        public override void Initialize()
        {
            SetupBackground(Level1Texture, 100);
        }

        public override IEnumerator<float> LevelScript()
        {
            yield return 1f;
        }
    }
}
