using System;
using System.Windows;

namespace day12_3BodyProblem
{
    class ThreeD
    {
        public int x, y, z;

        public static ThreeD operator+ (ThreeD lhs, ThreeD rhs)
        {
            ThreeD result = new ThreeD();
            result.x = lhs.x + rhs.x;
            result.y = lhs.y + rhs.y;
            result.z = lhs.z + rhs.z;

            return result;
        }

        public override string ToString()
        {
            string s = "";

            s = "<x=" + x.ToString().PadRight(3) + ", y=" + y.ToString().PadRight(3) + ", z=" + z.ToString().PadRight(3) + ">";
            return s;
        }

        public int Energy => Math.Abs(x) + Math.Abs(y) + Math.Abs(z);
        
    }
    class Moon
    {
        ThreeD location, speed;

        public Moon (int _x, int _y, int _z)
        {
            Init(_x, _y, _z);
        }
        
        private void Init (int _x, int _y, int _z)
        {
            location = new ThreeD()
            { x = _x, y = _y, z = _z };
            speed = new ThreeD()
            { x = 0, y = 0, z = 0 };

        }

        public Moon(string _xS, string _yS, string _zS)
        {
            int _z = 0, _y = 0, _x = 0;

            bool ok = false;

            ok = Int32.TryParse(_xS, out _x);
            if (!ok)
                MessageBox.Show($"failed to parse {_xS}");
            ok = Int32.TryParse(_yS, out _y);
            if (!ok)
                MessageBox.Show($"failed to parse {_yS}");
            ok = Int32.TryParse(_zS, out _z);
            if (!ok)
                MessageBox.Show($"failed to parse {_zS}");

            Init(_x, _y, _z);

        }

        public void UpdateSpeed(Moon other1, Moon other2, Moon other3)
        {
            ApplyGravity(other1);
            ApplyGravity(other2);
            ApplyGravity(other3);
        }

        private void ApplyGravity(Moon other)
        {
            ApplyGravityAxis(location.x, other.location.x, ref speed.x);
            ApplyGravityAxis(location.y, other.location.y, ref speed.y);
            ApplyGravityAxis(location.z, other.location.z, ref speed.z);
        }

        private void ApplyGravityAxis (int thisLoc, int otherLoc, ref int thisSpeed)
        { 
            if (thisLoc < otherLoc)
                thisSpeed++;
            else if (thisLoc > otherLoc)
                thisSpeed--;
            //else speed stays the same
        }

        public void UpdateLocation()
        {
            location += speed;
        }

        public override string ToString()
        {
            string s ="";

            s = $"pos={location} vel={speed} pot={location.Energy} kin={speed.Energy}";
            return s;
        }

        public int TotalEnergy => location.Energy * speed.Energy;
    }
}
