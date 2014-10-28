using System;
using System.Collections.Generic;

namespace PetZombie
{
	public interface IGame
	{
		List<List<Block>> Blocks {
			get;
		}

        void ReplaceBlocks (Block block1, Block block2);
        //List<Block> GetNeighbors (Block block);
	}
}

