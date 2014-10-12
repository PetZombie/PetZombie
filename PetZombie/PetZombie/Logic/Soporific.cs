using System;

namespace PetZombie
{
	//Снотворное
	class Soporific: Weapon
	{
		public Soporific (int x, int y, int size) : base (x, y, size)
		{
		}
		public Soporific (Coordinates coords, int size) : base (coords, size)
		{
		}
		public override void Use()
		{
		}
	}
}

