using System;

namespace PetZombie
{
	public class Block
	{
		Colors color;
		Position position;

		public Position Position{
			get {return this.position; }
		}

		public Colors Color{
			get { return this.color; }
			set{this.color = value; }
		}

		public Block (Colors color)
		{
			this.color = color;
			this.position = new Position ();
		}

		public Block (Position position)
		{
			this.position = position;
		}

		public Block (Colors color, Position position)
		{
			this.color = color;
			this.position = position;
		}
	}
}

