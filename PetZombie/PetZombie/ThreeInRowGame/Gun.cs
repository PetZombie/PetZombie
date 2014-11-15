using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Gun : Weapon
	{
        int count;

        public int Count 
        {
            get{return this.count; }
        }

		public Gun (int count)
		{
            this.count = count;
		}

        public List<List<Block>> Use(Block block, List<List<Block>> blocks, 
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

