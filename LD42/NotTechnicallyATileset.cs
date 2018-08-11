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
        float firstTilePos, movementSpeed;
        Point vdims;
        public Texture2D[] tileTexes;

        public NotTechnicallyATileset(Texture2D[] tileTexes_, Point vdims_, EntBuilder42 ebuilder_)
        {
            firstTilePos = 0;
            tileTexes = tileTexes_;
            vdims = vdims_;
            ebuilder = ebuilder_;
            movementSpeed = 10;
            SetupTiles();
            EntityCollection.CreateGroup(new Property("isTile", "isTile", "isTile"), "tiles");
        }

        public void AddTileGroup(int groupId_, float xpos_)
        {
            List<Entity> ents = new List<Entity>();
            switch (groupId_)
            {
                case 0:
                    for (int i = 7; i < 10; i++)
                    {
                        ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(0), new Vector2(xpos_, i * vdims.X / 14), new List<Property>() { new Property("isTile", "isTile", "isTile") }, "tile"));
                    }
                    break;
                case 1:
                    break;
            }
            EntityCollection.AddEntities(ents);
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
            for (int i = 0; i < 15; i++)
            {
                AddTileGroup(0, i * vdims.X / 14);
            }
        }

        public void Update(float es_)
        {
            firstTilePos -= es_ * movementSpeed;
            foreach (var tile in EntityCollection.GetGroup("tiles"))
            {
                tile.pos.X -= es_ * movementSpeed;
            }
            if (firstTilePos <= -16)
                RemoveFirstTileGroup();
        }

        public void RemoveFirstTileGroup()
        {
            foreach (var ent in EntityCollection.GetGroup("tiles"))
            {
                if (ent.pos.X <= firstTilePos + 2)
                    ent.exists = false;
            }
            EntityCollection.RecycleAll();
            firstTilePos += 16;
            AddTileGroup(0, vdims.X + firstTilePos);
        }

        public void Draw(SpriteBatch sb_)
        {
            foreach (var ent in EntityCollection.GetGroup("tiles"))
                ent.Draw(sb_);
        }
    }
}
