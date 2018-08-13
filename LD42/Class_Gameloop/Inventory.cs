using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonoGame.FZT.Assets;
using MonoGame.FZT.Data;
using MonoGame.FZT.Drawing;
using MonoGame.FZT.Sound;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using System.Xml.Linq;

namespace LD42
{
    public class Inventory
    {
        //test
        bool reverse;
        //not test
        ContentManager content;
        List<Entity> slots, items;
        int[,] taken;
        int size, minSize, maxSize;

        public Inventory(ContentManager content_)
        {
            items = new List<Entity>();
            slots = new List<Entity>();
            minSize = 1;
            maxSize = 32;
            taken = new int[7, 5];
            content = content_;
            for(int x = 0; x < minSize; x++)
            {
                AddSlot();
            }
        }

        public bool PlayerDead()
        {
            if(size == 32)
            {
                SoundManager.PlayEffect("hit6");
            }
            return (GetHP() == 0 || size == 32);
        }

        public int GetHP()
        {
            int hp = 0;
            foreach (Entity i in items)
            {
                if (i.Name == "heart")
                    hp++;
            }
             return hp;
        }

        public void LoseHP()
        {
            for (int x = items.Count - 1; x >= 0; x--)
            {
                if (items[x].Name == "heart")
                {
                    ParticleSystem.CreateInstance(items[x].pos - new Vector2(16, 16), "appear", true, 0.23f);
                    items[x].exists = false;
                    items.RemoveAt(x);
                    break;
                }
            }
        }

        public void LoseItem()
        {
            for (int x = items.Count - 1; x >= 0; x--)
            {
                if (items[x].exists)
                {
                    ParticleSystem.CreateInstance(items[x].pos - new Vector2(16, 16), "appear", true, 0.23f);
                    items[x].exists = false;
                    items.RemoveAt(x);
                    break;
                }    
            }
        }

        public void AddItem(Entity item_)
        {
            items.Add(item_);
            for (int x = items.Count - 1; x >= 0; x--)
            {
                if (items[x] == null || items[x].exists == false)
                {
                    items.RemoveAt(x);
                }
                else if (x >= 0)
                {
                    items[x].pos = GetSlotPos(x + 1).ToVector2() * 32;
                    
                }
            }
            ParticleSystem.CreateInstance(item_.pos - new Vector2(16, 16), "appear", true, 0.23f);
            CheckRecipes(GetPool());
            if (item_.Name != "mask")
                SoundManager.PlayEffect("hit5");
            else
                SoundManager.PlayEffect("hit2");
        }

        List<string> GetPool()
        {
            List<string> pool = new List<string>();
            foreach(Entity e in items)
            {
                pool.Add(e.Name);
            }
            return pool;
        }

        Point GetSlotPos(int index)
        {
            Point dir = new Point(1, 0);
            Point pos = new Point(0, 4);

            Point innerLimits = Point.Zero;
            Point outerLimits = new Point(6, 4);

            for (int x = 1; x < index; x++)
            {
                if (pos == new Point(0,3))
                {
                    innerLimits = new Point(0, 1);
                    outerLimits = new Point(5, 3);
                }
                if (pos == new Point(1, 3))
                {
                    innerLimits = new Point(1, 1);

                }

                if (pos.X == outerLimits.X && pos.Y == outerLimits.Y) {
                    dir = new Point(0, -1); }
                if (pos.X == outerLimits.X && pos.Y == innerLimits.Y) {
                    dir = new Point(-1, 0); }
                if (pos.X == innerLimits.X && pos.Y == outerLimits.Y) {
                    dir = new Point(1, 0); }
                if (pos.X == innerLimits.X && pos.Y == innerLimits.Y) {
                    dir = new Point(0, 1); }

                pos += dir;
            }

            return pos;
        }

        void AddSlot()
        {
            if(size != maxSize)
            {
                size++;
                AddSlotTo(GetSlotPos(size));
            }
        }    

        void AddSlotTo(Point target)
        {
            taken[target.X, target.Y] = 1;
            Vector2 pos = target.ToVector2() * 32;

            slots.Add(Assembler.GetEnt(ElementCollection.GetEntRef("slot"), pos, content, new EntBuilder42()));
            
        }

        void RemoveSlot()
        {
            if (size > minSize)
            {
               

                slots[size - 1].exists = false;

                slots.RemoveAt(size - 1);

                size--;
            }
        }

        public void Update(float es_)
        {
            SortItems();
        }

       public void UpdateSlots()
        {
            if(size < items.Count) { AddSlot(); }
            if(size > items.Count) { RemoveSlot(); }
        }

        void SortItems()
        {
            for(int x = items.Count-1; x >= 0; x--)
            {
                if(items[x] == null || items[x].exists == false)
                {
                    items.RemoveAt(x);
                }
                else if (x >= 0)
                    items[x].pos = GetSlotPos(x+1).ToVector2() * 32;
            }
        }

        public int GetScore()
        {
            int s = 0;
            foreach(Item i in items)
            {
                s += i.IntProperty("worth");
            }
            return s;
        }

        void CheckRecipes(List<string> pool)
        {
            Recipe r = RecipeBook.FindRecipe(pool);

            if (r != null)
            {
                //do stuff
                foreach(string rname in r.ingredients)
                {
                    for(int x=items.Count-1; x >= 0; x--)
                    {
                        if(items[x].Name == rname)
                        {
                            items[x].exists = false;
                            items.RemoveAt(x);
                        }
                    }
                }

                AddItem(Assembler.GetEnt(ElementCollection.GetEntRef(r.result), Vector2.Zero, content, new EntBuilder42()));
            }
        }
    }
}
