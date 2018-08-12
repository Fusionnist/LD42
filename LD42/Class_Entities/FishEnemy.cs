using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonoGame.FZT.Assets;
using MonoGame.FZT.Drawing;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LD42
{
    public class FishEnemy : Enemy
    {
        bool awake;
        public FishEnemy(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_, string type_ = "enemy") : base(texes_, pos_, props_, name_, type_)
        {
            type = "enemy";
        }

        public override void Draw(SpriteBatch sb_, bool flipH_ = false, bool flipV_ = false, float angle_ = 0)
        {
            if(!awake)
            SetTexture("shadow");

            float p = EntityCollection.GetGroup("players")[0].pos.X;
            if (pos.X - p > -32 && pos.X - p < 140 && !awake)
            {
                awake = true;
                SetTexture("appear");
                currentTex.Reset();
            }
            if (textures.GetTex("appear").Ended() && awake)
            {
                SetTexture("idle");
            }
            base.Draw(sb_, flipH_, flipV_, angle_);
        }

        public override void Move()
        {
            base.Move();
        }

        public override void Update(float elapsedTime_)
        {
            base.Update(elapsedTime_);
        }
    }
}
