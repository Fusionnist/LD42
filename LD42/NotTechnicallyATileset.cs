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
    public class NotTechnicallyATileset
    {
        int firstTilePos;
        public List<EntGroup> tiles;
        Point vdims;
        public Texture2D[] tileTexes;

        public NotTechnicallyATileset(Texture2D[] tileTexes_, Point vdims_)
        {
            tiles = new List<EntGroup>();
            firstTilePos = 0;
            tileTexes = tileTexes_;
            vdims = vdims_;
        }

        public EntGroup GetTileGroup(int groupId_)
        {
            List<Entity> ents = new List<Entity>();
            switch (groupId_)
            {
                case 0:
                    for (int i = 0; i < 14; i++)
                    { }
                    break;
            }
            return new EntGroup(ents);
        }
    }
}
