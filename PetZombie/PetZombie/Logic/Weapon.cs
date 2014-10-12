using System;

namespace PetZombie
{
	abstract class Weapon : Item
	{
		int size;
		public int count;

		public int Size
		{
			get { return this.size; }
		}

		public Weapon (int x, int y, int size, int count=1) : base (x, y)
		{
			this.size = size;
			this.count = count;
		}

		public Weapon (Coordinates coord, int size, int count=1) : base (coord)
		{
			this.size = size;
			this.count = count;
		}

		abstract public void Use ();
	}
}

