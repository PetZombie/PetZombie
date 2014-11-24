using System;
using System.Collections.Generic;

namespace PetZombie
{
	public interface IGame
	{
		List<List<Block>> Blocks
        {
            get;
		}

        bool ReplaceBlocks (Block block1, Block block2);
	}
}

