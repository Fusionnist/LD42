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
        EntBuilder42 ebuilder;
        float firstTilePos;
        int firstGroupNb, lastGroupNb;
        Point vdims;
        public Texture2D[] tileTexes;

        public NotTechnicallyATileset(Texture2D[] tileTexes_, Point vdims_, EntBuilder42 ebuilder_)
        {
            firstTilePos = 0;
            tileTexes = tileTexes_;
            vdims = vdims_;
            ebuilder = ebuilder_;
            firstGroupNb = 0;
            lastGroupNb = -1;
            SetupTiles();
        }

        public void AddTileGroup(int groupId_)
        {
            lastGroupNb++;
            List<Entity> ents = new List<Entity>();
            switch (groupId_)
            {
                case 0:
                    for (int i = 7; i < 10; i++)
                    {
                        ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(0), new Vector2(firstTilePos + (lastGroupNb - firstGroupNb) * 16, i * 16), new List<Property>() { new Property("posIs", lastGroupNb.ToString(), "tilePos") }, "tile"));
                    }
                    break;
            }
            EntityCollection.AddEntities(ents);
            EntityCollection.CreateGroup(new Property("posIs", lastGroupNb.ToString(), "tilePos"), "posIs" + lastGroupNb.ToString());
        }

        public DrawerCollection GetDrawerCollection(int id_)
        {
            DrawerCollection dc = null;
            switch (id_)
            {
                case 0:
                    dc = new DrawerCollection(new List<TextureDrawer>() { new TextureDrawer(tileTexes[0], new HitboxCollection[] { new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, 0, vdims.X / 14, vdims.Y / 10) } }, "draw") }) }, "tileDrawer");
                    break;
            }
            return dc;
        }

        public void SetupTiles()
        {
            for (int i = 0; i < 14; i++)
            {
                AddTileGroup(0);
            }
        }
    }
}
