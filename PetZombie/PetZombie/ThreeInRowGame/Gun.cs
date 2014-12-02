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

        public List<Tuple<List<Block>,int>> Use(Block block, int rowCount, int columnCount)
		{
            this.count--;
            List<Block> del = new List<Block>();
            del.Add(new Block(block));
            List<Tuple<List<Block>,int>> blocksForDelete = new List<Tuple<List<Block>, int>>();
            blocksForDelete.Add(new Tuple<List<Block>, int>(del, 1));
            return blocksForDelete;
        }
	}
}

