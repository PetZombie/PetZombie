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
        public string timer;
        public string time;

        public string GetTimer()
        {
            return timer;
        }

        public string Timer
        {
            get
            {
                if (timer == "" && livesCount < maxLivesCount)
                {
                    time = DateTime.UtcNow.ToString();
                    this.timer = new DateTime(1,1,1,0,5,0).ToString();
                    return timer;
                }
                if (timer == "")
                {
                    time = DateTime.UtcNow.ToString();
                    return timer;
                }
                DateTime now = DateTime.UtcNow;
                try
                {
                var dt = DateTime.Parse(this.timer) - (now - DateTime.Parse(time));
                timer = dt.ToString();
                time = DateTime.UtcNow.ToString();
                return timer; 
                }
                catch
                {
                    time = DateTime.UtcNow.ToString();
                    this.timer = new DateTime(1,1,1,0,5,0).ToString();
                    return timer;
                }
            }
            set
            {
                time = DateTime.UtcNow.ToString();
            }
        }

        public int LivesCount
        {
            get{ return this.livesCount; }
            set
            {
                if (value < this.livesCount)
                {
                    //time = DateTime.UtcNow.ToString();
                    //DateTime now = DateTime.UtcNow;
                    //now.AddMinutes(1);
                    if (this.timer == "")
                        this.timer = new DateTime(1,1,1,0,5,0).ToString();
                    //this.timer = now.ToString();
                }
                this.livesCount = value;
            }
        }

        public int BrainsCount
        { 
            get { return this.brainsCount; }
            set {this.brainsCount = value; }
        }

        public ZombiePet Zombie
        {
            get{ return this.zombie; }
        }

        public List<Weapon> Weapon
        {
            get{ return this.weapons; }
        }

        public int LastLevel
        { 
            get
            {
                return lastLevel;
            }
            set
            {
                this.lastLevel = value;
            }
        }

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
            this.money = money;
            this.lastLevel = lastLevel;
            weapons = new List<PetZombie.Weapon>();
            weapons.Add(new Soporific(5));
            weapons.Add(new Bomb(5));
            weapons.Add(new Gun(5));
            this.time = DateTime.UtcNow.ToString();// + DateTime.UtcNow.Hour.ToString() + DateTime.UtcNow.Minute.ToString();
            this.timer = "";
            this.maxLivesCount = 5;
        }

        public User(int livesCount, int brainsCount, ZombiePet zombie, List<Weapon> weapons, int money, int lastLevel,string time, string timer)
        {
            this.livesCount = livesCount;
            this.brainsCount = brainsCount;
            this.zombie = zombie;
            this.lastLevel = lastLevel;
            this.weapons = new List<Weapon>(weapons);
            this.money = money;
            this.time = time;
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

