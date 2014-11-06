using System;

namespace PetZombie
{
    public class ZombieBlock
    {
        bool canEat;

        public bool CanEat
        {
            get{ return this.canEat; }
            set{ this.canEat = value; }
        }

        //public ZombieBlock(Position position)
        //{
        //   this.Position = new Position(position.RowIndex, position.ColumnIndex);
        //   this.Type = BlockType.Zombie;
        //   this.canEat = true;
        //}
    }
}

