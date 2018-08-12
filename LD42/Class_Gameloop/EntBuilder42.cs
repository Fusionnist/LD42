using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.FZT.Assets;
using MonoGame.FZT.Data;
using MonoGame.FZT.Drawing;

namespace LD42
{
    public class EntBuilder42 : EntityBuilder
    {
        public EntBuilder42() : base()
        {

        }

        public override Entity CreateEntity(string type_, DrawerCollection dc_, Vector2 pos_, List<Property> props_, string name_)
        {
            if(type_ == "item")
            {
                return new Item(dc_, pos_, props_, name_);
            }

            if (type_ == "pickup")
            {
                return new Pickup(dc_, pos_, props_, name_);
            }

            if (type_ == "slot")
            {
                return new InventorySlot(dc_, pos_, props_, name_);
            }

            if (type_ == "tile")
            {
                return new Tile(dc_, pos_, props_);
            }

            if (type_ == "player")
            {
                return new Player(dc_, pos_, props_);
            }

            return base.CreateEntity(type_, dc_, pos_, props_, name_);
        }
    }
}
