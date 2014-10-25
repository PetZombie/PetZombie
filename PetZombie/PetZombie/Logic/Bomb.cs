using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Bomb : Weapon
	{
		public Bomb (int count): base(count)
		{
		}
        public override List<List<Block>> Use(Block block, List<List<Block>> blocks, Delegate GenerateBlocks)
		{
            return blocks;
		}
	}
}

