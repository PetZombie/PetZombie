using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class User
    {
        int maxLivesCount;
        int livesCount;
        int brainsCount;
        ZombiePet zombie;
        List<Weapon> weapons;
        int lastLevel;
        int money;
        string timer;
        string time;

        public String Timer 
        {
            get
            {
//                if (timer == "")
//                    return timer;
//                if (DateTime.Parse(this.timer) > (DateTime.UtcNow - DateTime.Parse(time)))
//                {
//                    timer = (DateTime.Parse(this.timer) - (DateTime.UtcNow - DateTime.Parse(time))).ToString();
//                }
//                else
//                    timer = "";
//                this.time = DateTime.UtcNow.ToString();
                return timer; 
            }
        }

        public int LivesCount
        {
            get{ return this.livesCount; }
            set
            {
                this.livesCount = value;
                DateTime now = DateTime.UtcNow;
                now.AddMinutes(60);
                if (this.timer == "")
                    this.timer = now.ToString();
            }
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

        public string Time
        {
            get{return this.time; }
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
            this.time = DateTime.UtcNow.ToString();// + DateTime.UtcNow.Hour.ToString() + DateTime.UtcNow.Minute.ToString();
            this.timer = "";
            this.maxLivesCount = 5;
        }

        public User(int livesCount, int brainsCount, ZombiePet zombie, List<Weapon> weapons, int money, int lastLevel, string time, string timer)
        {
            this.livesCount = livesCount;
            this.brainsCount = brainsCount;
            this.zombie = zombie;
            this.lastLevel = lastLevel;
            this.weapons = new List<Weapon>(weapons);
            this.time = (DateTime.UtcNow - DateTime.Parse(time)).ToString();
            this.timer = timer;
            this.maxLivesCount = 5;
        }

        public bool CanFeed()
        {
            return this.brainsCount > 0;
        }

        public void FeedZombie()
        {
            this.brainsCount--;
            this.zombie.Eat();
        }
    }
}

