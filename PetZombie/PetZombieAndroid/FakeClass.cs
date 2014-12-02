using System;
using System.Collections.Generic;

namespace PetZombieAndroid
{
    public class FakeClass
    {
        int number;
        string name;
        List<int> positions;

        public FakeClass()
        {
            this.name = "Kitty";
            this.number = 5;
            positions = new List<int>();
            positions.Add(10);
            positions.Add(0);
        }
    }
}

