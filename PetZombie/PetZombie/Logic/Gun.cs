using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Gun : Weapon
	{
		public Gun (int count): base(count)
		{
		}

        public override List<List<Block>> Use(Block block, List<List<Block>> blocks, 
            ThreeInRowGame.BlockGenerator GenerateBlocks, ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent)
		{
            this.count--;
            List<Block> del = new List<Block>();
            del.Add(new Block(block));
            List<Tuple<List<Block>,int>> blocksForDelete = new List<Tuple<List<Block>, int>>();
            blocksForDelete.Add(new Tuple<List<Block>, int>(del, 1));
            return Operation.DeleteBlock(blocksForDelete, blocks, GenerateBlocks, game, DeleteEvent);
		}
	}
}

