using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day14_OreToFuel
{
    struct Chem
    {
        public int Num;
        public string Name;
    }

    class ReactionList
    {
        readonly Dictionary<Chem, List<Chem>> reactions = new Dictionary<Chem, List<Chem>>();


        private bool ParseReactionLine(string line)
        {
            bool ok = true;
            var reactionSplit = Regex.Split(line, " => ");

            if (reactionSplit.Count() < 2)
                return false;

            var inputSplit = Regex.Split(reactionSplit[0], ",");

            List<Chem> inputChems = new List<Chem>();
            foreach (var inChem in inputSplit)
            {
                var inChemSplit = inChem.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                bool intConvOk = false;

                if (inChemSplit.Count() != 2)
                {
                    return false;
                }

                int num;
                intConvOk = Int32.TryParse(inChemSplit[0], out num);

                if (!intConvOk)
                    return false;

                Chem chemIn = new Chem()
                {
                    Num = num,
                    Name = inChemSplit[1],
                };

                inputChems.Add(chemIn);
            }


            // now parse the output
            var outChemSplit = reactionSplit[1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
            bool outConvOk = false;

            if (outChemSplit.Count() != 2)
            {
                return false;
            }

            int outCnt;
            outConvOk = Int32.TryParse(outChemSplit[0], out outCnt);

            if (!outConvOk)
                return false;

            Chem chemOut = new Chem()
            {
                Num = outCnt,
                Name = outChemSplit[1],
            };


            reactions.Add(chemOut, inputChems);

            return ok;
        }


        public void ParseReactions(string _reactions)
        {

            reactions.Clear();
            var lines = Regex.Split(_reactions, "\r\n|\r|\n");
            foreach (var line in lines)
            {
                ParseReactionLine(line);
            }
        }

        public int OreToFuel()
        {

            Chem fuel = new Chem()
            {
                Name = "FUEL",
                Num = 1
            };

            int oreCount = 0;

            oreCount = GetOreCount(fuel, out _);

            return oreCount;
        }


        public int OresForOneFuel()
        {
            Chem fuel = new Chem()
            {
                Name = "FUEL",
                Num = 1
            };

            int oreCount = 0;

            List<Chem> needList = new List<Chem>();
            List<Chem> extrasList = new List<Chem>();
            //Queue<Chem> madeList = new Queue<Chem>();

            needList.Add(fuel);

            while (needList.Count > 1 || (needList.Count == 1 && needList[0].Name != "ORE") )
            {
                var componentToMake = needList.First(x => x.Name != "ORE");

                needList.Remove(componentToMake);

                var recipe = reactions.First(x => x.Key.Name == componentToMake.Name);

                foreach (var recipeComp in recipe.Value)
                {
                    if (recipeComp.Name == "ORE")
                    {
                        if (needList.Exists(x => x.Name == recipeComp.Name))
                        {
                            var exists = needList.First(x => x.Name == recipeComp.Name);
                            needList.Remove(exists);
                            exists.Num += recipeComp.Num;
                            needList.Add(exists);
                        }
                        else
                        {
                            needList.Add(recipeComp);
                        }
                        continue;
                    }

                    var extraMatch = extrasList.FirstOrDefault(x => x.Name == recipeComp.Name);
                    if (extraMatch.Num > recipeComp.Num)
                    {
                        extrasList.Remove(extraMatch);
                        extraMatch.Num -= recipeComp.Num;
                        extrasList.Add(extraMatch);
                    }
                    else
                    {
                        // get the recipe and multiple it by the amount needed
                        var recipeCompRecipe = reactions.First(x => x.Key.Name == recipeComp.Name);
                        int neededOrigComp = recipeComp.Num - extraMatch.Num;

                        extrasList.Remove(extraMatch);

                        int multipliers = (int)(Math.Ceiling(neededOrigComp / (double)(recipeCompRecipe.Key.Num)));

                        Chem extraComp = new Chem()
                        {
                            Name = recipeComp.Name,
                            Num = (multipliers * recipeCompRecipe.Key.Num - neededOrigComp)
                        };

                        if (extraComp.Num > 0)
                            extrasList.Add(extraComp);

                        recipeCompRecipe.Value.ForEach(elem =>
                        {
                            if (needList.Exists(x => x.Name == elem.Name))
                            {
                                var exists = needList.First(x => x.Name == elem.Name);
                                needList.Remove(exists);
                                exists.Num += elem.Num;
                                needList.Add(exists);
                            }
                            else
                            {
                                needList.Add(elem);
                            }
                        });
                    }

                }

                // merge all the common values?

                if (needList.Count < 1)
                    break;

            }

            if (needList.Count == 1 && needList[0].Name == "ORE")
                oreCount = needList[0].Num;


                return oreCount;
        }


        private int GetOreCount(Chem outChem, out int chemsMade)
        {
            int oreCount = 0;

            var fuelRecipe = reactions.First(x => x.Key.Name == outChem.Name);
            chemsMade = fuelRecipe.Key.Num;

            foreach (var inChem in fuelRecipe.Value)
            {
                if (inChem.Name == "ORE")
                    oreCount += inChem.Num;
                else
                {
                    int chemCount;
                    int oreToMakeInChem = GetOreCount(inChem, out chemCount);
                    int multiplier = (int)Math.Ceiling(inChem.Num / (decimal)(chemCount));

                    oreCount += (multiplier * oreToMakeInChem);

                }
            }

            return oreCount;
        }

    }
}
