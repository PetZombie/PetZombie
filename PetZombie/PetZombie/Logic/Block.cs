using System;

namespace PetZombie
{
    public class Block
	{
		BlockType type;
		Position position;

        public Position Position{
			get {return this.position; }
            set { this.position = value; }
		}

        public BlockType Type{
			get { return this.type; }
			set{this.type = value; }
		}

		protected internal Block (BlockType type)
		{
			this.type = type;
			this.position = new Position ();
		}

		protected internal Block (Position position)
		{
			this.position = position;
		}

		protected internal Block (BlockType type, Position position)
		{
			this.type = type;
			this.position = position;
		}

        protected internal Block (Block block)
        {
            this.type = block.Type;
            this.position = block.Position;
        }
	}
}

