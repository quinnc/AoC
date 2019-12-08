using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;

namespace Day6.Model
{
    class SolarSystem
    {
        readonly Planet com = new Planet { Name = "COM", OrbitsAround = "", Depth = 0 };

        public int Init(string inputs)
        {
            MakeMap(inputs);
            GetAllOrbits(com);

            int sumAllPaths = OrbitAndSuborbitDepths(com);
            return sumAllPaths;
        }

        private void MakeMap(string inputs)
        {
            var lines = Regex.Split(inputs, "\r\n|\r|\n"); // ("\n|\r|\r\n");

            foreach (var line in lines)
            {
                var orbitDef = Regex.Split(line, "\\)");

                if (orbitDef.Length != 2)
                {
                    MessageBox.Show($"didn't split line into 2 part! [{line}]");
                    continue;
                }

                // put the pairs into a list
                map.Add(new Tuple<string, string>(orbitDef[0], orbitDef[1]));

            }
        }

        private int OrbitAndSuborbitDepths(Planet planet)
        {
            int suborbitDepths = 0;

            foreach (var subplanet in planet.Orbitters)
            {
                suborbitDepths += OrbitAndSuborbitDepths(subplanet);
            }

            return planet.Depth + suborbitDepths;
        }

        private List<Tuple<string, string>> map = new List<Tuple<string, string>>();


        private void GetAllOrbits(Planet currPlanet)
        {

            var currOrbit = map.Find(x => x.Item1 == currPlanet.Name);

            while (currOrbit != null)
            {
                // first remove it from the map list
                map.Remove(currOrbit);

                var newPlanet = new Planet { Name = currOrbit.Item2, OrbitsAround = currPlanet.Name, Depth = currPlanet.Depth + 1 };
                currPlanet.Orbitters.Add(newPlanet);

                GetAllOrbits(newPlanet);

                currOrbit = map.Find(x => x.Item1 == currPlanet.Name);
            }
        }


        public int OrbitalHops(string src, string dest)
        {
            List<string> srcAncestry = new List<string>();
            List<string> destAncestry = new List<string>();
            var srcFound = GetParents(src, com, srcAncestry);
            var destFound = GetParents(dest, com, destAncestry);

            if (!srcFound || !destFound)
            {
                MessageBox.Show("either didn't find the src or the dest!");
                return -1;
            }

            //var lastCommon = GetLastCommonAncenstor(srcParentList, destParentList);

            //return OrbitDistance(srcParentList, lastCommon) + OrbitDistance(destParentList, lastCommon);


            int minLength = Math.Min(srcAncestry.Count, destAncestry.Count);
            bool stillMatch = true;
            int index = 0;

            while (stillMatch && index < minLength)
            {
                if (srcAncestry[index] != destAncestry[index])
                    stillMatch = false;
                else
                    index++;
            }

            return (srcAncestry.Count - index) + (destAncestry.Count - index);
        }

        private bool GetParents(string src, Planet curr, List<string> ancestry)
        {
            if (curr.Name == src)
            {
                // start the list

                ancestry.Add(src);
                return true;
            }

            // else
            bool found = false;

            foreach (var childPlanet in curr.Orbitters)
            {
                found = GetParents(src, childPlanet, ancestry);

                if (found)
                {
                    ancestry.Add(childPlanet.Name);
                    break;
                }
                // else loop to the next child

            }

            return found;
        }

    }
}
