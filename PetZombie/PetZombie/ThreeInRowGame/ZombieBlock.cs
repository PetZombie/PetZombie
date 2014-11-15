using System;

namespace PetZombie
{
    public class ZombieBlock : Block
    {
        bool canEat;

        public bool CanEat
        {
            get{ return this.canEat; }
        }

        public ZombieBlock(Position position, bool canEat=true):base(position)
        {
           this.Type = BlockType.Zombie;
           this.canEat = canEat;
        }
    }
}

