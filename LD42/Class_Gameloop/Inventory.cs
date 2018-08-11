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
            content = content_;
            for(int x = 0; x < 7; x++)
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
            Point pos = new Point(0, 6);

            for(int x = 1; x < index; x++)
            {
                if(pos.X == 6) { dir.Y = -1; }
                if(pos.X == 0) { dir.Y = 1; }
                if(pos.Y == 0) { dir.X = -1; }
                if(pos.Y == 6) { dir.X = 1; }

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
