using System;
using System.Collections.Generic;

namespace PetZombie
{
    //Снотворное
    class Soporific: Weapon
    {
        public Soporific(int count) : base(count)
        {
        }

        public override List<List<Block>> Use(Block block, List<List<Block>> blocks, ThreeInRowGame.BlockGenerator GenerateBlocks,
            ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent)
        {
            ZombieBlock zombie = block as ZombieBlock;
            if (zombie == null)
                return blocks;

            this.count--;
            zombie.CanEat = false;
            blocks[zombie.Position.RowIndex][zombie.Position.ColumnIndex] = new ZombieBlock(zombie);
            return blocks;
        }
    }
}

