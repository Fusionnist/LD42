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
    public class ShadowEnt : Entity
    {
        public ShadowEnt(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_, string type_ = "ent") : base(texes_, pos_, props_, name_, type_)
        {
            
        }

        public override void Draw(SpriteBatch sb_, bool flipH_ = false, bool flipV_ = false, float angle_ = 0)
        {
            textures.GetTex("shadow").Draw(sb_, new Vector2((int)pos.X, 62));
            base.Draw(sb_, flipH_, flipV_, angle_);
        }
    }
}
