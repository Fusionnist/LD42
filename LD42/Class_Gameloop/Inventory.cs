using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonoGame.FZT.Assets;
using MonoGame.FZT.Data;

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
        List<Entity> slots;
        int[,] taken;
        int size, minSize, maxSize;

        public Inventory(ContentManager content_)
        {
            slots = new List<Entity>();
            minSize = 7;
            maxSize = 32;
            taken = new int[7, 5];
            content = content_;
            for(int x = 0; x < 7; x++)
            {
                AddSlot();
            }
        }

        public void AddItem(Entity item_)
        {
            foreach(Entity slot in slots)
            {
                if (slot.Answer("empty"))
                {
                    slot.FeedEntity(item_, "eat my ass");
                    break;
                }
            }
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
            if (size != minSize)
            {
                slots[size - 1].exists = false;

                slots.RemoveAt(size - 1);

                size--;
            }
        }

        public void Update(float es_)
        {
            if (size == 32)
            { reverse = true; }
            if (size == 7)
            { reverse = false; }

            if (!reverse)
                AddSlot();

            if(reverse)
                RemoveSlot();            
        }

        void UpdatePool()
        {

        }

        void CheckRecipes(List<Entity> pool)
        {

        }
    }
}
