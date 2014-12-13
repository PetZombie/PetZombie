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
        int lastLevel;
        int money;

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

        public int LastLevel{ get { return lastLevel; } }

        public int Money
        {
            get{ return this.money; }
            set { this.money = value; }
        }

        public User()
        {
        }

        public User(int livesCount, int brainsCount, ZombiePet zombie, int money, int lastLevel=0)
        {
            this.livesCount = livesCount;
            this.brainsCount = brainsCount;
            this.zombie = zombie;
            this.lastLevel = lastLevel;
            weapons = new List<PetZombie.Weapon>();
            weapons.Add(new Soporific(2));
            weapons.Add(new Bomb(2));
            weapons.Add(new Gun(3));
        }

        public User(int livesCount, int brainsCount, ZombiePet zombie, List<Weapon> weapons, int money, int lastLevel)
        {
            this.livesCount = livesCount;
            this.brainsCount = brainsCount;
            this.zombie = zombie;
            this.lastLevel = lastLevel;
            weapons = new List<Weapon>(weapons);
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

