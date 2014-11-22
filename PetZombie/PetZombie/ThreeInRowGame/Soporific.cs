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

        public List<List<Block>> Use(Block block, List<List<Block>> blocks, ThreeInRowGame.BlockGenerator GenerateBlocks, 
            ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent)
        {
            ZombieBlock zombie = block as ZombieBlock;
            if (zombie == null)
                return blocks;
            blocks[zombie.Position.RowIndex][zombie.Position.ColumnIndex] = new ZombieBlock(zombie.Position, false);
            return blocks;
        }
    }
}

