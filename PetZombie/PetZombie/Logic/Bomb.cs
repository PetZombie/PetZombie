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
            List<Block> column1 = new List<Block>();
            column1.Add(new Block(new Position(row-1, column-1)));
            column1.Add(new Block(new Position(row, column-1)));
            column1.Add(new Block(new Position(row+1, column-1)));

            List<Block> column2 = new List<Block>();
            column2.Add(new Block(new Position(row-1, column)));
            column2.Add(new Block(new Position(row, column)));
            column2.Add(new Block(new Position(row+1, column)));

            List<Block> column3 = new List<Block>();
            column3.Add(new Block(new Position(row-1, column+1)));
            column3.Add(new Block(new Position(row, column+1)));
            column3.Add(new Block(new Position(row+1, column+1)));

            List<Tuple<List<Block>,int>> blocksForDelete = new List<Tuple<List<Block>, int>>();
            blocksForDelete.Add(new Tuple<List<Block>, int>(column1, 1));
            blocksForDelete.Add(new Tuple<List<Block>, int>(column2, 1));
            blocksForDelete.Add(new Tuple<List<Block>, int>(column3, 1));
            return Operation.DeleteBlock(blocksForDelete, blocks, GenerateBlocks, game, DeleteEvent);
		}
	}
}

