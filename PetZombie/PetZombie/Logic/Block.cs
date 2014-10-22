﻿using System;

namespace PetZombie
{
	protected internal class Block
	{
		BlockType type;
		Position position;

		protected internal Position Position{
			get {return this.position; }
		}

		protected internal BlockType Type{
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
	}
}

