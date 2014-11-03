using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class Bomb : Weapon
    {
        public Bomb(int count) : base(count)
        {
        }

        public override List<List<Block>> Use(Block block, List<List<Block>> blocks, 
                                              ThreeInRowGame.BlockGenerator GenerateBlocks, ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent)
        {
            this.count--;
            int row = block.Position.RowIndex;
            int column = block.Position.ColumnIndex;
        
            List<Block> column1 = new List<Block>();
            if (column - 1 > 0)
            {
                if (row - 1 > 0)
                    column1.Add(blocks[row - 1][column - 1]);
                column1.Add(blocks[row][column - 1]);
                if (row + 1 < blocks.Count)
                    column1.Add(blocks[row + 1][column - 1]);
            }

            List<Block> column2 = new List<Block>();
            if (row - 1 > 0)
                column2.Add(blocks[row - 1][column]);
            column2.Add(blocks[row][column]);
            if (row + 1 < blocks.Count)
                column2.Add(blocks[row + 1][column]);

            List<Block> column3 = new List<Block>();
            if (column + 1 < blocks[row].Count)
            {
                if (row - 1 > 0)
                    column3.Add(blocks[row - 1][column + 1]);
                column3.Add(blocks[row][column + 1]);
                if (row + 1 < blocks.Count)
                    column3.Add(blocks[row + 1][column + 1]);
            }

            List<Tuple<List<Block>,int>> blocksForDelete = new List<Tuple<List<Block>, int>>();
            blocksForDelete.Add(new Tuple<List<Block>, int>(column1, column1.Count));
            blocksForDelete.Add(new Tuple<List<Block>, int>(column2, column2.Count));
            blocksForDelete.Add(new Tuple<List<Block>, int>(column3, column3.Count));
            return Operation.DeleteBlock(blocksForDelete, blocks, GenerateBlocks, game, DeleteEvent);
        }
    }
}

