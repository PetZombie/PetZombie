using System;

namespace PetZombie
{
	public class Item
	{
		protected Coordinates coordinates;

		public Coordinates Coordinates
		{
			get { return this.coordinates; }
		}

		public Item (Coordinates coord)
		{
			this.coordinates = coord;
		}

		public Item (int x, int y)
		{
			this.coordinates = new Coordinates (x,y);
		}
	}
}

