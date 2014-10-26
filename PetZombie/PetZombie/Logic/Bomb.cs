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
            this.count--;
            return Operation.DeleteBlock(block, blocks, GenerateBlocks);
            return blocks;
		}
	}
}

