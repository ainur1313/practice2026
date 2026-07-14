using System;
using System.Collections.Generic;
using System.Text;

namespace task04
{
    public class Fighter : ISpaceship
    {
        public int Speed => 100;
        public int FirePower => 70;

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
