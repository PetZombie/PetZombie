using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Bomb : Weapon
	{
		public Bomb (int count): base(count)
		{
		}
        public override List<List<Block>> Use(Block block, List<List<Block>> blocks, ThreeInRowGame.BlockGenerator GenerateBlocks)
		{
            this.count--;
            int row = block.Position.RowIndex;
            int column = block.Position.ColumnIndex;
            List<Block> delBlocks = new List<Block>();
            delBlocks.Add(new Block(new Position(row-1, column-1)));
            delBlocks.Add(new Block(new Position(row-1, column)));
            delBlocks.Add(new Block(new Position(row-1, column+1)));

            delBlocks.Add(new Block(new Position(row, column-1)));
            delBlocks.Add(new Block(new Position(row, column)));
            delBlocks.Add(new Block(new Position(row, column+1)));

            delBlocks.Add(new Block(new Position(row+1, column-1)));
            delBlocks.Add(new Block(new Position(row+1, column)));
            delBlocks.Add(new Block(new Position(row+1, column+1)));

            foreach (Block b in delBlocks)
                blocks = new List<List<Block>>(Operation.DeleteBlock(b, blocks, GenerateBlocks));
            return blocks;
            //return 
		}
	}
}

