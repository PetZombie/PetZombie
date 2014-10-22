using System;

namespace PetZombie
{
	public class Block
	{
		BlockType type;
		Position position;

		public Position Position{
			get {return this.position; }
		}

		public BlockType Type{
			get { return this.type; }
			set{this.type = value; }
		}

		public Block (BlockType type)
		{
			this.type = type;
			this.position = new Position ();
		}

		public Block (Position position)
		{
			this.position = position;
		}

		public Block (BlockType type, Position position)
		{
			this.type = type;
			this.position = position;
		}
	}
}

