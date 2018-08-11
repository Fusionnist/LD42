using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.FZT;
using MonoGame.FZT.Assets;
using MonoGame.FZT.Data;
using MonoGame.FZT.Drawing;
using MonoGame.FZT.Input;
using MonoGame.FZT.Physics;
using MonoGame.FZT.Sound;
using MonoGame.FZT.UI;
using MonoGame.FZT.XML;
using System.Collections.Generic;

namespace LD42
{
    class Player : Entity
    {
        public Player(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_ = null): base(texes_, pos_, props_, name_, "player")
        {

        }

        public void Update(InputProfile ipp, float es_, float speed_)
        {
            textures.Update(es_);

            mov.X += speed_;
            if (ipp.JustPressed("w"))
                vel.Y -= 10;
            else if (vel.Y < 0)
                vel.Y += 1;
            else if (vel.Y > 0)
                vel.Y = 0;
            MultMov(es_);
            base.Update(es_);
        }

        public void Draw(SpriteBatch sb_)
        {
            currentTex.Draw(sb_, pos);
        }
    }
}
