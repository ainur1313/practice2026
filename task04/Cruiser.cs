using System;
using System.Collections.Generic;
using System.Text;

namespace task04
{
    public class Cruiser : ISpaceship
    {
        public int Speed => 50;
        public int FirePower => 100;

        public void MoveForward()
        {
            return;
        }
        public void Rotate(int angle)
        {
            return;
        }
        public void Fire()
        {
            return;
        }
    }
}
