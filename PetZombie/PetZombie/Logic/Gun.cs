using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Gun : Weapon
	{
		public Gun (int count): base(count)
		{
		}

        public override List<List<Block>> Use(Block block, List<List<Block>> blocks, Delegate GenerateBlock)
		{
            this.count--;
            return Operation.DeleteBlock(block, blocks, GenerateBlock);
		}
	}
}

