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
        ContentManager content;
        string nextFloorType;
        List<string> groups, items, tempGroups, tempItems;
        List<double> groupProbs, itemProbs, tempGroupProbs, tempItemProbs, groupCooldowns, itemCooldowns, itemYs, tempYs, basegcds, baseicds;
        double totalProbs, totalGroupProbs;

        public NotTechnicallyATileset(Texture2D[] tileTexes_, Point vdims_, EntBuilder42 ebuilder_, ContentManager content_)
        {
            tileTexes = tileTexes_;
            vdims = vdims_;
            ebuilder = ebuilder_;
            content = content_;
            nextFloorType = "rand";
            InitialiseGroups();
            SetupTiles();
            EntityCollection.CreateGroup(new Property("isTile", "isTile", "isTile"), "tiles");
            //EntityCollection.CreateGroup(new Property("isCollectible", "isCollectible", "isCollectible"), "pickups");
        }

        public void AddTileGroup(string groupId_, string itemId_, float xpos_, float itemY_)
        {
            float height = 96;
            List<Entity> ents = new List<Entity>();
            ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(0), new Vector2(xpos_, 0), new List<Property>() { new Property("isTile", "isTile", "isTile") }, "tile"));
            switch (groupId_)
            {
                case "basic":
                    for (int i = 0; i < 2; i++)
                    {
                        ents.Add(ebuilder.CreateEntity("tile", GetDrawerCollection(0), new Vector2(xpos_, i*32 + height), new List<Property>() { new Property("isTile", "isTile", "isTile") }, "tile"));
                    }
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
                Entity ent = Assembler.GetEnt(ElementCollection.GetEntRef(itemId_), new Vector2(xpos_, itemY_), content, ebuilder);
                ent.AddProperty(new Property("isCollectible", "isCollectible", "isCollectible"));
                ents.Add(ent);
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
                AddTileGroup("basic", "none", i * vdims.X / 14, 80);
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
                if (ent.pos.X <= camPos_ - vdims.X / 14)
                    ent.exists = false;
                if (ent.pos.X >= camPos_ + 13 * vdims.X / 14)
                    x = true;
            }
            foreach (var ent in EntityCollection.GetGroup("pickups"))
            {
                if (ent.pos.X <= camPos_ - vdims.X / 14)
                    ent.exists = false;
            }
            if (!x)
            {
                HandleNewTileSpawns(camPos_);
            }
        }

        public void HandleNewTileSpawns(float camPos_)
        {
            string groupStuff = null, itemStuff = null;
            double y = 80;
            totalProbs = 0;
            totalGroupProbs = 0;
            if (nextFloorType == "rand")
            {
                AddPossibilitiesToLists();
                Random r = new Random();

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
                                itemCooldowns[j] = baseicds[j];
                        }
                    }
                }

                x = r.NextDouble() * totalGroupProbs;
                for (int i = 0; x > 0; i++)
                {
                    x -= tempGroupProbs[i];
                    if (x <= 0)
                        groupStuff = tempGroups[i];
                }
            }
            else if (nextFloorType.StartsWith("void"))
            {
                groupStuff = "void";
                itemStuff = "none";
            }
            AddTileGroup(groupStuff, itemStuff, vdims.X + camPos_, (float)y);


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
        }

        public void AddPossibilitiesToLists()
        {
            for (int i = 0; i < itemCooldowns.Count; i++)
            {
                if (itemCooldowns[i] == 0)
                { tempItems.Add(items[i]); tempItemProbs.Add(itemProbs[i]); totalProbs += itemProbs[i]; tempYs.Add(itemYs[i]); }
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

            groups = new List<string>() { "basic", "void" };
            groupProbs = new List<double>() { .9, .1 };
            groupCooldowns = new List<double>() { 0, 0 };
            basegcds = new List<double>() { 0, 20 };

            items = new List<string>() { "none" };
            itemProbs = new List<double>() { 1 };
            baseicds = new List<double>() { 0 };
            itemCooldowns = new List<double>() { 0 };
            itemYs = new List<double>() { 80 };

            XDocument xdoc = new XDocument();
            xdoc = XDocument.Load("Content\\XML\\PickupInfo.xml");
            IEnumerable<XElement> xels = xdoc.Root.Elements("Pickup");

            foreach (var xel in xels)
            {
                items.Add(xel.Attribute("name").Value);
                itemProbs.Add(double.Parse(xel.Attribute("prob").Value));
                baseicds.Add(double.Parse(xel.Attribute("cooldown").Value));
                itemCooldowns.Add(0);
                if (xel.Attribute("y") != null)
                {
                    itemYs.Add(double.Parse(xel.Attribute("y").Value));
                }
                else
                    itemYs.Add(80);
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
