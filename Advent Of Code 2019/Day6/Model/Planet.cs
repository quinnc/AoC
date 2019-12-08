using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day6.Model
{
    class Planet
    {
        public string Name;
        public string OrbitsAround;
        public List<Planet> Orbitters = new List<Planet>();
        public int Depth;
    }
}
