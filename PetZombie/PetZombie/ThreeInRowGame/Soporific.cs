using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class Soporific: Weapon
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
        }

        public Soporific(int count)
        {
            this.count = count;
            this.cost = 20;
        }

        public List<List<Block>> GetAsleepZombieInBlocks(Block block, List<List<Block>> blocks)
        {
            if (block.Type != BlockType.Zombie)
                return blocks;
            blocks[block.Position.RowIndex][block.Position.ColumnIndex] = new ZombieBlock(block.Position, false);
            return blocks;
        }

        public List<Tuple<List<Block>,int>> Use(Block block, int rowCount, int columnCount)
        {
            return new List<Tuple<List<Block>, int>>();
        }
    }
}

