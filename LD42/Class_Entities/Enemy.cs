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
    public class Enemy : ShadowEnt
    {
        public Enemy(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_, string type_ = "enemy") : base(texes_, pos_, props_, name_, type_)
        {

        }

        public override void Draw(SpriteBatch sb_, bool flipH_ = false, bool flipV_ = false, float angle_ = 0)
        {
            if (isDestroyed)
            {
                SetTexture("dead");
            }
            base.Draw(sb_, flipH_, flipV_, angle_);
        }

        public override void React(string id_)
        {
            if(id_ == "headJump")
            {
                isDestroyed = true;
            }
            base.React(id_);
        }

        public override void Move()
        {
            vel.Y += 1;
            base.Move();
        }
    }
}
