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

            var lines = Regex.Split(inputs, "\r\n|\r|\n"); // ("\n|\r|\r\n");

            foreach (var line in lines)
            {
                var orbitDef = Regex.Split(line, "\\)");

                if (orbitDef.Length != 2)
                {
                    MessageBox.Show($"didn't split line into 2 part! [{line}]");
                    continue;
                }


                //string orbittee = orbitDef[0];
                //string orbitter = orbitDef[1];



                //Planet pOrbittee = new Planet();
                //pOrbittee.Name = orbittee;

                //Planet pOrbitter = new Planet();
                //pOrbitter.Name = orbitter;
                //pOrbitter.OrbitsAround = orbittee;

                //// find the orbittee in the 
                ///



                // put the pairs into a list
                map.Add(new Tuple<string, string>(orbitDef[0], orbitDef[1]));

            }

            GetAllOrbits(com);

            int sumAllPaths = OrbitAndSuborbitDepths(com);
            return sumAllPaths;
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


        private void GetAllOrbits (Planet currPlanet)
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
    }
}
