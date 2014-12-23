using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class Gun : Weapon
	{
        int cost;
        int count;

        public int Cost
        {
            get { return this.cost; }
        }

        public int Count
        {
            get { return this.count; }
            set{ this.count = value;}
        }

        public Gun()
        {
            this.count = 0;
            this.cost = 35;
        }

		public Gun (int count)
		{
            this.count = count;
            this.cost = 35;
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

