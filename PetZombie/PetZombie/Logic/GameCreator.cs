using System;

namespace PetZombie
{
	public class GameCreator
	{
		public GameCreator ()
		{
		}

        static public IGame CreateGame()
        {
            return new ThreeInRowGame();
        }
	}
}

