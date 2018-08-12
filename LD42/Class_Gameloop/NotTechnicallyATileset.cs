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
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LD42
{
    public class NotTechnicallyATileset
    {
        EntBuilder42 ebuilder;
        Point vdims;
        public Texture2D[] tileTexes;
        Texture2D bgtex;
        ContentManager content;
        string nextFloorType;
        List<string> groups, items, tempGroups, tempItems;
        List<double> groupProbs, itemProbs, tempGroupProbs, tempItemProbs, groupCooldowns, itemCooldowns, tempGlobalCountdowns, itemYs, tempYs, basegcds, baseicds, baseglobalcds;
        double totalProbs, totalGroupProbs, globalCooldown;
        bool addCeiling;
        float lastPos;
        Random r;
        int ticktock;

        public NotTechnicallyATileset(Texture2D[] dick, Point vdims_, EntBuilder42 ebuilder_, ContentManager content_)
        {
            vdims = vdims_;
            addCeiling = true;
            globalCooldown = 0;
            r = new Random();
            ebuilder = ebuilder_;
            content = content_;
            nextFloorType = "rand";
            SetupTexes();
            InitialiseGroups();
            SetupTiles();
            ticktock = 0;

            EntityCollection.CreateGroup(new Property("isTile", "isTile", "isTile"), "tiles");
            EntityCollection.CreateGroup(new Property("isBG", "isBG", "isBG"), "backgrounds");
        }

        public void AddTileGroup(string groupId_, string itemId_, float xpos_, float itemY_)
        {
            float height = 112;
            if (ticktock == 5)
                ticktock = 0;
            List<Entity> ents = new List<Entity>();
            if (addCeiling)
                ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(6), new Vector2(xpos_, 0), new List<Property>() { new Property("isTile", "isTile", "isTile") }, "tile"));
            addCeiling = !addCeiling;
            switch (groupId_)
            {
                case "basic":
                    if (ticktock == 0)
                        ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(12), new Vector2(xpos_, height), new List<Property>() { new Property("isTile", "isTile", "isTile") }, "tile"));
                    break;
                case "void":
                    if (nextFloorType == "rand")
                        nextFloorType = "void2";
                    else if (int.Parse(nextFloorType.Substring(4)) < 10)
                        nextFloorType = "void" + (int.Parse(nextFloorType.Substring(4)) + 1).ToString();
                    else
                    { nextFloorType = "rand"; groupCooldowns[1] = 20; }
                    break;
            }
            if (itemId_ != "none")
            {
                Entity ent = Assembler.GetEnt(ElementCollection.GetEntRef(itemId_), new Vector2(xpos_, itemY_), content, ebuilder, false);
                ent.AddProperty(new Property("isCollectible", "isCollectible", "isCollectible"));
                ents.Add(ent);
            }
            EntityCollection.AddEntities(ents);
            ticktock++;
        }

        public DrawerCollection GetDrawerCollection(int id_)
        {
            DrawerCollection dc = null;
            switch (id_)
            {
                case 0:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[0], 
                                    new HitboxCollection[] 
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, 0, 50, 20) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
                case 1:
                    dc = new DrawerCollection(new List<TextureDrawer>() { new TextureDrawer(bgtex, new TextureFrame(new Rectangle(0, 0, 96, 96), new Point(48, 48))) }, "bg");
                    break;
                case 2:
                    dc = new DrawerCollection(new List<TextureDrawer>() { new TextureDrawer(bgtex, new TextureFrame(new Rectangle(96, 0, 96, 96), new Point(48, 48))) }, "bg");
                    break;
                case 3:
                    dc = new DrawerCollection(new List<TextureDrawer>() { new TextureDrawer(bgtex, new TextureFrame(new Rectangle(192, 0, 96, 96), new Point(48, 48))) }, "bg");
                    break;
                case 4:
                    dc = new DrawerCollection(new List<TextureDrawer>() { new TextureDrawer(bgtex, new TextureFrame(new Rectangle(288, 0, 96, 96), new Point(48, 48))) }, "bg");
                    break;
                case 5:
                    dc = new DrawerCollection(new List<TextureDrawer>() { new TextureDrawer(bgtex, new TextureFrame(new Rectangle(384, 0, 96, 96), new Point(48, 48))) }, "bg");
                    break;
                case 6:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[1],
                                    new HitboxCollection[]
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, 0, 32, 16) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
                case 7:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[2],
                                    new TextureFrame(new Rectangle(0, 0, 32, 64), new Point(0, 0)),
                                    new HitboxCollection[]
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, -4, 16, 36) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
                case 8:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[2],
                                    new TextureFrame(new Rectangle(32, 0, 32, 64), new Point(0, 0)),
                                    new HitboxCollection[]
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, -4, 16, 36) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
                case 9:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[2],
                                    new TextureFrame(new Rectangle(64, 0, 32, 64), new Point(0, 0)),
                                    new HitboxCollection[]
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, -4, 16, 36) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
                case 10:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[2],
                                    new TextureFrame(new Rectangle(96, 0, 32, 64), new Point(0, 0)),
                                    new HitboxCollection[]
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, -4, 16, 36) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
                case 11:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[2],
                                    new TextureFrame(new Rectangle(128, 0, 32, 64), new Point(0, 0)),
                                    new HitboxCollection[]
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, -4, 16, 36) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
                case 12:
                    dc = new DrawerCollection(new List<TextureDrawer>()
                            {
                                new TextureDrawer
                                (
                                    tileTexes[2],
                                    new TextureFrame(new Rectangle(0, 0, 160, 64), new Point(0, 0)),
                                    new HitboxCollection[]
                                    {
                                        new HitboxCollection(new FRectangle[][] { new FRectangle[] { new FRectangle(0, -4, 160, 68) } }, "collision")
                                    }
                                )
                            }, "tileDrawer");
                    break;
            }
            return dc;
        }

        public void SetupTiles()
        {
            for (int i = 0; i < 30; i++)
            {
                AddTileGroup("basic", "none", i * vdims.X / 14, 80);
            }
            for (int i = 0; i < 3; i++)
            {
                Random r = new Random();
                int x = r.Next(1, 5);
                Entity ent = ebuilder.CreateEntity("bg", GetDrawerCollection(x), new Vector2(i * 96 + 48, 64), new List<Property>() { new Property("isBG", "isBG", "isBG") }, "bg");
                EntityCollection.AddEntity(ent);
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
            List<Entity> ents = EntityCollection.GetGroup("tiles");
            foreach (var ent in EntityCollection.GetGroup("tiles"))
            {
                if (ent.pos.X <= camPos_ - vdims.X)
                {
                    ent.exists = false;  lastPos = ent.pos.X - camPos_;
                }
                if (ent.pos.X >= camPos_ + vdims.X)
                    x = true;
            }
            foreach (var ent in EntityCollection.GetGroup("pickups"))
            {
                if (ent.pos.X <= camPos_ - vdims.X / 14)
                    ent.exists = false;
            }
            foreach (var ent in EntityCollection.GetGroup("enemies"))
            {
                if (ent.pos.X <= camPos_ - vdims.X)
                    ent.exists = false;
            }
            if (!x)
            {
                float n = (float)(Math.Floor(camPos_ / 16) * 16);
                HandleNewTileSpawns(n + vdims.X + 16);
            }

            x = false;
            foreach (var bg in EntityCollection.GetGroup("backgrounds"))
            {
                if (bg.pos.X < camPos_ - 48)
                    bg.exists = false;
                if (bg.pos.X >= camPos_ + vdims.X)
                    x = true;
            }
            if (!x)
            {
                HandleNewBgSpawns(camPos_ + vdims.X + 48);
            }
        }

        public void HandleNewTileSpawns(float camPos_)
        {

                string groupStuff = null, itemStuff = "none";
                double y = 80;
                totalProbs = 0;
                totalGroupProbs = 0;
                if (nextFloorType == "rand")
                {
                    AddPossibilitiesToLists();
                    Random r = new Random();

                if (globalCooldown == 0)
                {
                    double x = r.NextDouble() * totalProbs;
                    for (int i = 0; x > 0; i++)
                    {
                        x -= tempItemProbs[i];
                        if (x <= 0)
                        {
                            itemStuff = tempItems[i];
                            y = tempYs[i];
                            for (int j = 0; j < items.Count; j++)
                            {
                                if (items[j] == tempItems[i])
                                { itemCooldowns[j] = baseicds[j]; globalCooldown = baseglobalcds[j]; }
                            }
                        }
                    }
                }
                else
                {
                    int e = 5;
                }

                    double z = r.NextDouble() * totalGroupProbs;
                    for (int i = 0; z > 0; i++)
                    {
                        z -= tempGroupProbs[i];
                        if (z <= 0)
                            groupStuff = tempGroups[i];
                    }
                }
                else if (nextFloorType.StartsWith("void"))
                {
                    groupStuff = "void";
                    itemStuff = "none";
                }

                AddTileGroup(groupStuff, itemStuff, camPos_, (float)y);
            


            HandleCooldowns();
            tempGroups.Clear();
            tempGroupProbs.Clear();
            tempItemProbs.Clear();
            tempItems.Clear();
        }

        public void HandleCooldowns()
        {
            for (int i = 0; i < itemCooldowns.Count; i++)
            {
                if (itemCooldowns[i] > 0)
                    itemCooldowns[i]--;
            }
            for (int i = 0; i < groupCooldowns.Count; i++)
            {
                if (groupCooldowns[i] > 0)
                    groupCooldowns[i]--;
            }
            if (globalCooldown > 0)
                globalCooldown--;
        }

        public void AddPossibilitiesToLists()
        {
            for (int i = 0; i < itemCooldowns.Count; i++)
            {
                if (itemCooldowns[i] == 0)
                { tempItems.Add(items[i]); tempItemProbs.Add(itemProbs[i]); totalProbs += itemProbs[i]; tempYs.Add(itemYs[i]); tempGlobalCountdowns.Add(baseglobalcds[i]); }
            }
            for (int i = 0; i < groupCooldowns.Count; i++)
            {
                if (groupCooldowns[i] == 0)
                { tempGroups.Add(groups[i]); tempGroupProbs.Add(groupProbs[i]); totalGroupProbs += groupProbs[i]; }
            }
        }

        public void InitialiseGroups()
        {
            tempGroups = new List<string>();
            tempItems = new List<string>();
            tempGroupProbs = new List<double>();
            tempItemProbs = new List<double>();
            tempYs = new List<double>();
            tempGlobalCountdowns = new List<double>();

            groups = new List<string>() { "basic" };
            groupProbs = new List<double>() { 1 };
            groupCooldowns = new List<double>() { 0 };
            basegcds = new List<double>() { 0 };

            items = new List<string>() { "none" };
            itemProbs = new List<double>() { 1 };
            baseicds = new List<double>() { 0 };
            itemCooldowns = new List<double>() { 0 };
            itemYs = new List<double>() { 80 };
            baseglobalcds = new List<double>() { 0 };

            XDocument xdoc = new XDocument();
            xdoc = XDocument.Load("Content\\XML\\PickupInfo.xml");
            IEnumerable<XElement> xels = xdoc.Root.Elements("Pickup");

            foreach (var xel in xels)
            {
                items.Add(xel.Attribute("name").Value);
                itemProbs.Add(double.Parse(xel.Attribute("prob").Value));
                baseicds.Add(double.Parse(xel.Attribute("cooldown").Value));
                itemCooldowns.Add(0);
                baseglobalcds.Add(double.Parse(xel.Attribute("globalCooldown").Value));
                if (xel.Attribute("y") != null)
                {
                    itemYs.Add(double.Parse(xel.Attribute("y").Value));
                }
                else
                    itemYs.Add(96);
            }
        }

        public void Draw(SpriteBatch sb_)
        {
            foreach (var ent in EntityCollection.GetGroup("backgrounds"))
                ent.Draw(sb_);
            foreach (var ent in EntityCollection.GetGroup("tiles"))
                ent.Draw(sb_);
            foreach (var ent in EntityCollection.GetGroup("pickups"))
                ent.Draw(sb_);
        }

        public void SetupTexes()
        {
            tileTexes = new Texture2D[] { content.Load<Texture2D>("yesnpressed"), content.Load<Texture2D>("Tile/tileceiling"), content.Load<Texture2D>("Tile/tilefloor") };
            bgtex = content.Load<Texture2D>("Tile/tilebg");
        }

        public void HandleNewBgSpawns(float camPos_)
        {
            Random r = new Random();
            int x = r.Next(1, 5);
            Entity ent = ebuilder.CreateEntity("bg", GetDrawerCollection(x), new Vector2(camPos_, 64), new List<Property>() { new Property("isBG", "isBG", "isBG") }, "bg");
            EntityCollection.AddEntity(ent);
        }
    }
}
