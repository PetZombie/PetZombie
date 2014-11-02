using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Bomb : Weapon
	{
		public Bomb (int count): base(count)
		{
		}
        public override List<List<Block>> Use(Block block, List<List<Block>> blocks, 
            ThreeInRowGame.BlockGenerator GenerateBlocks, ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent)
		{
            this.count--;
            int row = block.Position.RowIndex;
            int column = block.Position.ColumnIndex;
            List<Block> row1 = new List<Block>();
            row1.Add(new Block(new Position(row-1, column-1)));
            row1.Add(new Block(new Position(row-1, column)));
            row1.Add(new Block(new Position(row-1, column+1)));

            List<Block> row2 = new List<Block>();
            row2.Add(new Block(new Position(row, column-1)));
            row2.Add(new Block(new Position(row, column)));
            row2.Add(new Block(new Position(row, column+1)));

            List<Block> row3 = new List<Block>();
            row3.Add(new Block(new Position(row+1, column-1)));
            row3.Add(new Block(new Position(row+1, column)));
            row3.Add(new Block(new Position(row+1, column+1)));

            List<Tuple<List<Block>,int>> blocksForDelete = new List<Tuple<List<Block>, int>>();
            blocksForDelete.Add(new Tuple<List<Block>, int>(row1, 1));
            blocksForDelete.Add(new Tuple<List<Block>, int>(row2, 1));
            blocksForDelete.Add(new Tuple<List<Block>, int>(row3, 1));
            return Operation.DeleteBlock(blocksForDelete, blocks, GenerateBlocks, game, DeleteEvent);
		}
	}
}

