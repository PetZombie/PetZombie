using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class Soporific: Weapon
    {
        int count;

        public int Count
        {
            get{ return this.count; }
        }

        public Soporific(int count)
        {
            this.count = count;
        }

        public List<List<Block>> GetAsleepZombieInBlocks(Block block, List<List<Block>> blocks)
        {
            ZombieBlock zombie = block as ZombieBlock;
            if (zombie == null)
                return blocks;
            blocks[zombie.Position.RowIndex][zombie.Position.ColumnIndex] = new ZombieBlock(zombie.Position, false);
            return blocks;
        }

        public List<Tuple<List<Block>,int>> Use(Block block, int rowCount, int columnCount)
        {
            return new List<Tuple<List<Block>, int>>();
        }
    }
}

