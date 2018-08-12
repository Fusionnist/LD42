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
        int voidCooldown, goldCooldown;
        List<string> groups, items, tempGroups, tempItems;
        List<double> groupProbs, itemProbs, tempGroupProbs, tempItemProbs;

        public NotTechnicallyATileset(Texture2D[] tileTexes_, Point vdims_, EntBuilder42 ebuilder_, ContentManager content_)
        {
            tileTexes = tileTexes_;
            vdims = vdims_;
            ebuilder = ebuilder_;
            content = content_;
            nextFloorType = "rand";
            voidCooldown = 0;
            goldCooldown = 0;
            InitialiseGroups();
            SetupTiles();
            EntityCollection.CreateGroup(new Property("isTile", "isTile", "isTile"), "tiles");
            //EntityCollection.CreateGroup(new Property("isCollectible", "isCollectible", "isCollectible"), "pickups");
        }

        public void AddTileGroup(string groupId_, string itemId_, float xpos_)
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
                    break;
            }
            switch(itemId_)
            {
                case "placeholderPickup":
                    Entity ent = Assembler.GetEnt(ElementCollection.GetEntRef("placeholderPickup"), new Vector2(xpos_, height - 16), content, ebuilder);
                    ent.AddProperty(new Property("isCollectible", "isCollectible", "isCollectible"));
                    ents.Add(ent);
                    break;
                case "none":
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
                AddTileGroup("basic", "none", i * vdims.X / 14);
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
            if (nextFloorType == "rand")
            {

            }
            else if (nextFloorType.StartsWith("void"))
            {
                groupStuff = "void";
                if (int.Parse(nextFloorType.Substring(4)) < 3)
                    nextFloorType = "void" + (int.Parse(nextFloorType.Substring(4)) + 1).ToString();
                else
                { nextFloorType = "rand"; voidCooldown = 20; }
            }
            AddTileGroup(groupStuff, itemStuff, vdims.X + camPos_);
            HandleCooldowns();
        }

        public void HandleCooldowns()
        {
            if (voidCooldown > 0)
                voidCooldown--;
            if (goldCooldown > 0)
                goldCooldown--;
        }

        public void AddPossibilitiesToLists()
        {

        }
        public void InitialiseGroups()
        {
            tempGroups = new List<string>();
            tempItems = new List<string>();
            tempGroupProbs = new List<double>();
            tempItemProbs = new List<double>();

            groups = new List<string>() { "basic", "void" };
            groupProbs = new List<double>() { .96, .04 };

            items = new List<string>() { "none" };
            itemProbs = new List<double>() { 1 };

            //XDocument xdoc = new XDocument();
            //xdoc = XDocument.Load("Content\\XML\\PickupInfo.xml");
            //IEnumerable<XElement> xels = xdoc.Elements("Pickup");

            //foreach (var xel in xels)
            //{
            //    items.Add(xel.Attribute("name").ToString());
             //   itemProbs.Add(double.Parse(xel.Attribute("prob").ToString()));
           // }
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
