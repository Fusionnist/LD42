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
    enum PlayerState { walk, jump, dash, slide }
    class Player : ShadowEnt
    {
        float speed; bool queueJump;
        PlayerState state;

        Timer dashTimer;

        public Player(DrawerCollection texes_, Vector2 pos_, List<Property> props_, string name_ = null) : base(texes_, pos_, props_, name_, "player")
        {
            type = "player";
            dashTimer = new Timer(1f);
            dashTimer.Stop();
            invinTimer = new Timer(1f);
        }

        public override void Input(Vector2 input_)
        {
            state = PlayerState.jump;
            if (touchWallD) { state = PlayerState.walk; }
            if (input_.Y == 1 && touchWallD)
            {
                state = PlayerState.slide;
            }
            if (input_.Y == -1 && touchWallD)
            {
                state = PlayerState.jump;
                vel.Y = -280;
            }
            if (queueJump)
            {
                queueJump = false;
                vel.Y -= 200;
            }
            if (input_.X == 1 && dashTimer.Complete())
            {
                dashTimer.Reset();
               

            }
            if (!dashTimer.Complete())
            {
                mov.X += 200 * dashTimer.timer;
                state = PlayerState.dash;
            }

            base.Input(input_);
        }

        public override void Move()
        {
            mov.X += 120;
            
            {
                vel.Y += 15;
            }
            base.Move();
        }

        public override void Update(float es_)
        {
            //textures.Update(es_);
            dashTimer.Update(es_);

            base.Update(es_);
        }

        public override void Draw(SpriteBatch sb_, bool flipH_ = false, bool flipV_ = false, float angle_ = 0)
        {
            SetTexture("run");

            if(state == PlayerState.jump)
            {
                SetTexture("jump");
            }

            if (state == PlayerState.dash)
            {
                SetTexture("jump");
            }
            if (invin)
            {
                SetTexture("shadow");
            }
            base.Draw(sb_, flipH_, flipV_, angle_);
        }

        public override void React(string id_)
        {
            if(id_ == "headJump")
            {
                queueJump = true;
            }
            base.React(id_);
        }
    }
}
