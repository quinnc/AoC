using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day6.Model
{
    class Orbits: Dictionary<Planet, Orbits>
    {
        public int Depth = 0;
        public Orbits Orbitters = new Orbits();
    }
}
