using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LD42
{
    public class Recipe
    {
        public List<string> ingredients;
        public string result;
        public int resultWorth;

        public Recipe(List<string> ingredients_, string result_, int worth_)
        {
            ingredients = ingredients_;
            result = result_;
            resultWorth = worth_;
        }
    }
}
