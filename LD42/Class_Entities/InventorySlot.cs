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
    public class InventorySlot : Entity
    {
        Entity item;

        public InventorySlot(DrawerCollection texes_, Vector2 pos_, List<Property> props_) : base(texes_, pos_, props_)
        {
            type = "slot";
        }

        public override void FeedEntity(Entity e_, string context_)
        {
            item = e_;
            item.pos = pos;
            base.FeedEntity(e_, context_);
        }

        public override List<Entity> SubEntities(string specifics_ = "none")
        {
            return new List<Entity>() { item };
        }

        public override void React(string id_)
        {
            if(id_ == "empty")
            {
                item = null;
            }
            base.React(id_);
        }

        public override bool Answer(string question_)
        {
            if(question_ == "empty")
            {
                return item == null;
            }
            return base.Answer(question_);
        }

        public override void Draw(SpriteBatch sb_, bool flipH_ = false, bool flipV_ = false, float angle_ = 0)
        {
            base.Draw(sb_, flipH_, flipV_, angle_);
            if (item!=null)
            item.Draw(sb_);

        }
    }
}
