using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day14_OreToFuel
{
    struct Chem
    {
        public ulong Num;
        public string Name;
    }

    class ReactionList
    {
        readonly Dictionary<Chem, List<Chem>> reactions = new Dictionary<Chem, List<Chem>>();

        public ulong NumFuelsToMake { get; set; } = 1;

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
                bool ulongConvOk = false;

                if (inChemSplit.Count() != 2)
                {
                    return false;
                }

                ulong num;
                ulongConvOk = UInt64.TryParse(inChemSplit[0], out num);

                if (!ulongConvOk)
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

            ulong outCnt;
            outConvOk = UInt64.TryParse(outChemSplit[0], out outCnt);

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


        public string Reactions
        {
            get => "";
            set
            {
                reactions.Clear();
                var lines = Regex.Split(value, "\r\n|\r|\n");
                foreach (var line in lines)
                {
                    ParseReactionLine(line);
                }
            }
        }

  

        public ulong OresForOneFuel()
        {
            Chem fuel = new Chem()
            {
                Name = "FUEL",
                Num = NumFuelsToMake,
            };

            ulong oreCount = 0;

            List<Chem> needList = new List<Chem>();
            List<Chem> extrasList = new List<Chem>();

            needList.Add(fuel);

            while (needList.Count > 1 || (needList.Count == 1 && needList[0].Name != "ORE") )
            {
                var componentToMake = needList.First(x => x.Name != "ORE");

                needList.Remove(componentToMake);

                var recipe = reactions.First(x => x.Key.Name == componentToMake.Name);

                ulong needAmount = componentToMake.Num;
                ulong recipeMakesAmount = recipe.Key.Num;

                var extraMatch = extrasList.FirstOrDefault(x => x.Name == componentToMake.Name);
                if (!string.IsNullOrEmpty(extraMatch.Name))
                {
                    extrasList.Remove(extraMatch);
                    if (extraMatch.Num > componentToMake.Num)
                    {
                        extraMatch.Num -= componentToMake.Num;
                        extrasList.Add(extraMatch);
                        continue;
                    }
                    else if (extraMatch.Num < componentToMake.Num)
                    {
                        needAmount -= extraMatch.Num;
                    }
                    else
                    {
                        // exactly the right amount so continue on
                        continue;
                    }
                }

                // need more of the current component.
                ulong recipeMultiplier = (ulong)(Math.Ceiling((double)(needAmount) / (double)(recipeMakesAmount)));
                ulong componentsMade = recipeMultiplier * recipe.Key.Num;

                if (componentsMade > needAmount)
                {
                    Chem extraComp = new Chem()
                    {
                        Name = componentToMake.Name,
                        Num = componentsMade - needAmount
                    };

                    // can't be any of these left in the extras at this poulong, so just add the new extras
                    extrasList.Add(extraComp);
                }

                // now add each of the components in the recipe to the needs list
                foreach (var recipeComp in recipe.Value)
                {
                    var exists = needList.FirstOrDefault(x => x.Name == recipeComp.Name);
                    needList.Remove(exists);

                    exists.Name = recipeComp.Name; // handle the situation where the default (empty Name, 0 Num) is returned
                    exists.Num += recipeMultiplier * recipeComp.Num;
                    needList.Add(exists);
                }

                if (needList.Count < 1)
                    break;

            }

            if (needList.Count == 1 && needList[0].Name == "ORE")
                oreCount = needList[0].Num;


                return oreCount;
        }

    }
}
