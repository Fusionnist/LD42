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
    class FlyEnemy : Enemy
    {
        bool up;

        public FlyEnemy(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_, string type_ = "enemy") : base(texes_, pos_, props_, name_, type_)
        {
            type = "enemy";
            up = false;
        }

        public override void Draw(SpriteBatch sb_, bool flipH_ = false, bool flipV_ = false, float angle_ = 0)
        {
            SetTexture("fly");
            base.Draw(sb_, flipH_, flipV_, angle_);
        }

        public override void Move()
        {
            if (up)
                vel.Y -= 1/2;
            else
                vel.Y += 1/2;
            if (vel.Y >= 10 || vel.Y <= -10)
                up = !up;
            base.Move();
        }

        public override void Update(float elapsedTime_)
        {
            base.Update(elapsedTime_);
        }
    }
}
