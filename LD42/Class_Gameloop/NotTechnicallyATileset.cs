using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        Point vdims;
        public Texture2D[] tileTexes;
        ContentManager content;

        public NotTechnicallyATileset(Texture2D[] tileTexes_, Point vdims_, EntBuilder42 ebuilder_, ContentManager content_)
        {
            tileTexes = tileTexes_;
            vdims = vdims_;
            ebuilder = ebuilder_;
            SetupTiles();
            EntityCollection.CreateGroup(new Property("isTile", "isTile", "isTile"), "tiles");
            EntityCollection.CreateGroup(new Property("isCollectible", "isCollectible", "isCollectible"), "pickups");
        }

        public void AddTileGroup(string groupId_, float xpos_)
        {
            List<Entity> ents = new List<Entity>();
            switch (groupId_)
            {
                case "basic":
                    for (int i = 7; i < 10; i++)
                    {
                        ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(0), new Vector2(xpos_, i * vdims.X / 14), new List<Property>() { new Property("isTile", "isTile", "isTile") }, "tile"));
                    }
                    break;
                case "void":
                    break;
                case "gold":
                    for (int i = 7; i < 10; i++)
                    {
                        ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(0), new Vector2(xpos_, i * vdims.X / 14), new List<Property>() { new Property("isTile", "isTile", "isTile") }, "tile"));
                    }
                    ents.Add(Assembler.GetEnt(ElementCollection.GetEntRef("placeholderPickup"), new Vector2(xpos_, 3 * vdims.X / 7), content, ebuilder));
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
                    dc = new DrawerCollection
                        (
                            new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[0], 
                                    new HitboxCollection[] 
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, 0, 50, 20) } }, "collision")
                                    }
                                )
                            }, 
                            "tileDrawer"
                        );
                    break;
            }
            return dc;
        }

        public void SetupTiles()
        {
            for (int i = 0; i < 15; i++)
            {
                AddTileGroup("basic", i * vdims.X / 14);
            }
        }

        public void Update(float es_, float camPos_)
        {
            RemoveTiles(camPos_);
            foreach (var ent in EntityCollection.GetGroup("tiles"))
                ent.Update(es_);
            foreach (var ent in EntityCollection.GetGroup("pickups"))
                ent.Update(es_);
        }

        public void RemoveTiles(float camPos_)
        {
            bool x = false;
            foreach (var ent in EntityCollection.GetGroup("tiles"))
            {
                if (ent.pos.X <= camPos_ - vdims.X / 14)
                { ent.exists = false; x = true; }
            }
            if (x)
            {
                AddTileGroup("basic", vdims.X + camPos_);
            }
        }

        public void Draw(SpriteBatch sb_)
        {
            foreach (var ent in EntityCollection.GetGroup("tiles"))
                ent.Draw(sb_);
            foreach (var ent in EntityCollection.GetGroup("pickups"))
                ent.Draw(sb_);
        }
    }
}
