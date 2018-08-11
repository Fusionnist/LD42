using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MonoGame.FZT.Assets;

using Microsoft.Xna.Framework;

namespace LD42
{
    public class Inventory
    {
        List<InventorySlot> slots;
        int[,] taken;
        int size, minSize, maxSize;

        public Inventory()
        {

        }

        public void AddItem()
        {

        }

        void AddSlot()
        {
            if(size != maxSize)
            {
                size++;
                AddSlotTo(Vector2.Zero);
            }
        }

        void RemoveSlot()
        {
            if(size != minSize)
            {
                size--;
                slots.RemoveAt(size - 1);
            }
        }

        void AddSlotTo(Vector2 target)
        {

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
