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


        public void ParseReactions(string reactions)
        {
            var lines = Regex.Split(reactions, "\r\n|\r|\n");
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
