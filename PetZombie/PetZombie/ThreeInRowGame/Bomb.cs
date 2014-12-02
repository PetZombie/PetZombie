﻿using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Bomb : Weapon
	{
        int count;

        public int Count 
        {
            get{return this.count; }
        }

		public Bomb (int count)
		{
            this.count = count;
		}

        public List<Tuple<List<Block>,int>> Use(Block block, int rowCount, int columnCount)
		{
            this.count--;
            int row = block.Position.RowIndex;
            int column = block.Position.ColumnIndex;
            List<Block> row1 = new List<Block>();
            if (row - 1 > 0)
            {
                if (column - 1 > 0)
                    row1.Add(new Block(new Position(row - 1, column - 1)));
                row1.Add(new Block(new Position(row - 1, column)));
                if (column + 1 < columnCount)
                    row1.Add(new Block(new Position(row - 1, column + 1)));
            }

            List<Block> row2 = new List<Block>();
            if (column - 1 > 0)
                row2.Add(new Block(new Position(row, column-1)));
            row2.Add(new Block(new Position(row, column)));
            if (column + 1 < columnCount)
                row2.Add(new Block(new Position(row, column+1)));

            List<Block> row3 = new List<Block>();
            if (row + 1 < rowCount)
            {
                if (column - 1 > 0)
                    row3.Add(new Block(new Position(row + 1, column - 1)));
                row3.Add(new Block(new Position(row + 1, column)));
                if (column + 1 < columnCount)
                    row3.Add(new Block(new Position(row + 1, column + 1)));
            }

            List<Tuple<List<Block>,int>> blocksForDelete = new List<Tuple<List<Block>, int>>();
            blocksForDelete.Add(new Tuple<List<Block>, int>(row1, 1));
            blocksForDelete.Add(new Tuple<List<Block>, int>(row2, 1));
            blocksForDelete.Add(new Tuple<List<Block>, int>(row3, 1));

            return blocksForDelete;

		}
	}
}

