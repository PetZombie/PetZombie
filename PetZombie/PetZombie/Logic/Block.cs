using System;

namespace PetZombie
{
	public class Block
	{
		int colorType;

		public int ColorType{
			get { this.colorType; }
		}

		public Block (int colorType)
		{
			this.colorType = colorType;
		}
	}
}

