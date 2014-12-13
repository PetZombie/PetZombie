using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class Soporific: Weapon
    {
        public int count;
        public Soporific(int count)
        {
            this.count = count;
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

