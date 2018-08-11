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
        float speed;
        public Player(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_ = null): base(texes_, pos_, props_, name_, "player")
        {

        }

        public override void Input(Vector2 input_)
        {
            if (input_.Y == -1)
                vel.Y = -75;
            base.Input(input_);
        }

        public override void Move()
        {
            mov.X += 16;
            vel.Y += 1;
            base.Move();
        }

        public override void Update(float es_)
        {
            textures.Update(es_);
            
            base.Update(es_);
        }

        public void Draw(SpriteBatch sb_)
        {
            currentTex.Draw(sb_, pos);
        }
    }
}
