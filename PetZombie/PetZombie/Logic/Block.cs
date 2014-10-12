using System;

namespace PetZombie
{
	public class Block: Item
	{
        int size;
		//Object color;

		public Block (int x, int y, int size) : base (x, y)
		{
			this.size = size;
		}

		public Block(Coordinates coords, int size) : base (coords)
        {
            this.size = size;
        }
	}
}

