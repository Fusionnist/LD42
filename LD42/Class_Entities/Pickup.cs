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
    public class Pickup : Entity
    {
        Entity item;

        public Pickup(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_) :base(texes_, pos_, props_, name_)
        {
            type = "pickup";
        }

        public override void FeedEntity(Entity e_, string context_)
        {
            item = e_;
            base.FeedEntity(e_, context_);
        }

        public override List<Entity> SubEntities(string specifics_ = "none")
        {
            return new List<Entity>() { item };
        }

        public override void Draw(SpriteBatch sb_, bool flipH_ = false, bool flipV_ = false, float angle_ = 0)
        {
            pos.Y -= 8;
            base.Draw(sb_, flipH_, flipV_, angle_);
            pos.Y += 8;
        }
    }
}
