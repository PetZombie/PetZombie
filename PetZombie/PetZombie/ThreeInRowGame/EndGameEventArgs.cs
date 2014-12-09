using System;

namespace PetZombie
{
    public class EndGameEventArgs : EventArgs
    {
        public bool win;
        public EndGameEventArgs(bool win)
        {
            this.win = win;
        }
    }
}

