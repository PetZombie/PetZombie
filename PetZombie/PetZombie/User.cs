using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class User
    {
        int livesCount;
        int brainsCount;
        ZombiePet zombie;
        List<Weapon> weapons;

        public int LivesCount
        {
            get{ return this.livesCount; }
        }

        public int BrainsCount
        { 
            get { return this.brainsCount; }
        }

        public ZombiePet Zombie
        {
            get{ return this.zombie; }
        }

        public List<Weapon> Weapon
        {
            get{ return this.weapons; }
        }

        public User(int livesCount, int brainsCount, ZombiePet zombie)
        {
            this.livesCount = livesCount;
            this.brainsCount = brainsCount;
            this.zombie = zombie;
            weapons = new List<PetZombie.Weapon>();
            weapons.Add(new Soporific(2));
            weapons.Add(new Bomb(2));
            weapons.Add(new Gun(3));
        }

        public bool CanFeed()
        {
            return this.brainsCount > 0;
        }

        public void FeedZombie()
        {
            this.brainsCount--;
        }
    }
}

