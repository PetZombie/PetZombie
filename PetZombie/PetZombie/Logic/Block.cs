using System;

namespace PetZombie
{
	public class Block
	{
		int colorType;
		Position position;

		public Position Position{
			get {return this.position; }
		}

		public int ColorType{
			get { return this.colorType; }
		}

		public Block (int colorType)
		{
			this.colorType = colorType;
			this.position = new Position ();
		}

		public Block (Position position)
		{
			this.position = position;
		}
	}
}

