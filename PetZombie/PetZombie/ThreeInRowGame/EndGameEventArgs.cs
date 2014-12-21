using System;

namespace PetZombie
{
    public class EndGameEventArgs : EventArgs
    {
        public bool win;
        public User user;
        public EndGameEventArgs(bool win, User user)
        {
            this.win = win;
            this.user = user;
        }
    }
}

