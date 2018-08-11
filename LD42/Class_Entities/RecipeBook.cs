using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace LD42
{
    public static class RecipeBook
    {
        public static List<Recipe> recipes = new List<Recipe>();

        public static Recipe FindRecipe(List<string> names_)
        {
            int record = 0;
            Recipe retained = null;

            foreach(Recipe r in recipes)
            {
                bool match = true;

                bool[] paired = new bool[names_.Count];

                foreach(string rname in r.ingredients)
                {
                    bool pairFound = false;

                    for(int x = 0; x < names_.Count; x++)
                    {
                        if (!paired[x])
                        {
                            if(names_[x] == rname)
                            {
                                paired[x] = true;
                                pairFound = true;
                                break;
                            }
                        }
                    }
                    if (!pairFound)
                    {
                        match = false;
                        break;
                    }
                }


                if (match) 
                {
                    if(r.resultWorth > record)
                    {
                        record = r.resultWorth;
                        retained = r;
                    }
                }
            }
            return retained;
        }
        public static void ReadDocument(XDocument doc_)
        {
            //ADD RECIPES HERE
            foreach(XElement recipe in doc_.Element("Recipes").Elements("Recipe"))
            {
                List<string> ingredients = new List<string>();
                foreach(XElement ingredient in recipe.Elements("Ingredient"))
                {
                    ingredients.Add(ingredient.Value);
                }

                string result = recipe.Element("Result").Value;

                int resultworth = int.Parse(recipe.Element("ResultWorth").Value);

                recipes.Add(new Recipe(ingredients, result, resultworth));
            }
        }

    }
}
