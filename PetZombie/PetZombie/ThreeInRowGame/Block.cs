using System;

namespace PetZombie
{
    public class Block
	{
		BlockType type;
		Position position;
        bool cage;

        public bool Cage
        {
            get { return this.cage; }
            set{ this.cage = value; }
        }

        public Position Position{
			get {return this.position; }
            set { this.position = value; }
		}

        public BlockType Type{
			get { return this.type; }
			set{this.type = value; }
		}

        protected internal Block (BlockType type, bool cage=false)
		{
			this.type = type;
			this.position = new Position ();
            this.cage = cage;
		}

        protected internal Block (Position position, bool cage=false)
		{
			this.position = position;
            this.cage = cage;
		}

        protected internal Block (BlockType type, Position position, bool cage=false)
		{
			this.type = type;
			this.position = position;
            this.cage = cage;
		}

        protected internal Block (Block block)
        {
            this.type = block.Type;
            this.position = block.Position;
            this.cage = block.cage;
        }
	}
}

