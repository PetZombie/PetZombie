using System;

namespace PetZombie
{
    public class User
    {
        int livesCount;
        int brainsCount;
        ZombiePet zombie;

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

        public User(int livesCount, int brainsCount, ZombiePet zombie)
        {
            this.livesCount = livesCount;
            this.brainsCount = brainsCount;
            this.zombie = zombie;
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

