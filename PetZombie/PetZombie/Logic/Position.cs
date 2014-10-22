using System;

namespace PetZombie
{
	public class Position
	{
		int x, y;

		public int X
		{
			get{ return this.x;}
		}

		public int Y
		{
            get{ return this.y;}
		}

		public Position (int x, int y)
		{
			this.x = x;
			this.y = y;
		}

		public Position ()
		{
			this.x = 0;
			this.y = 0;
		}
	}
}

