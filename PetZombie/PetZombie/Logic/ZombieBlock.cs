using System;

namespace PetZombie
{
    public class ZombieBlock: Block
    {
        bool canEat;

        public bool CanEat
        {
            get{ return this.canEat; }
            set{ this.canEat = value; }
        }

        public ZombieBlock(Position position): base(position)
        {
           this.Type = BlockType.Zombie;
           this.canEat = true;
        }

        public ZombieBlock(ZombieBlock block):base(block)
        {
            this.canEat = block.CanEat;
        }
    }
}

