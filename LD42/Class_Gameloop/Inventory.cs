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
            for(int x = 0; x < 32; x++)
            {
                AddSlot();
            }
        }

        public void AddItem()
        {

        }

        Point GetSlotPos(int index)
        {
            Point dir = new Point(1, 0);
            Point pos = new Point(0, 4);

            Point innerLimits = Point.Zero;
            Point outerLimits = new Point(6, 4);

            if (taken[0,3] == 1) {
                innerLimits = new Point(1, 1);
                outerLimits = new Point(5, 3);
            }

            for (int x = 1; x < index; x++)
            {
                dir = Point.Zero;

                if (pos.X == outerLimits.X && pos.Y == outerLimits.Y) { dir.Y = -1; }
                if (pos.X == outerLimits.X && pos.Y == innerLimits.Y) { dir.X = -1; }
                if (pos.X == innerLimits.X && pos.Y == outerLimits.Y) { dir.X = 1; }
                if (pos.X == innerLimits.X && pos.Y == innerLimits.Y) { dir.Y = 1; }


                pos += dir;

                if (pos.X >= 7) { pos.X = 6; }
                if (pos.X < 0) { pos.X = 0; }
                if (pos.Y >= 5) { pos.Y = 4; }
                if (pos.Y < 0) { pos.Y = 0; }
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
                size--;
                slots.RemoveAt(size - 1);
            }
        }

        public void Update(float es_)
        {

        }

        void UpdatePool()
        {

        }

        void CheckRecipes(List<Entity> pool)
        {

        }
    }
}
