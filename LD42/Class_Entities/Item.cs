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
    public class Item : Entity
    {
        public Item(DrawerCollection texes_, Vector2 pos_, List<Property> props_) : base(texes_, pos_, props_)
        {

        }
    }
}
