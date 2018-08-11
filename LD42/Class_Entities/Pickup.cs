using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonoGame.FZT.Assets;
using MonoGame.FZT.Drawing;

using Microsoft.Xna.Framework;

namespace LD42
{
    public class Pickup : Entity
    {
        Item item;

        public Pickup(DrawerCollection texes_, Vector2 pos_, List<Property> props_):base(texes_, pos_, props_)
        {

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
    }
}
