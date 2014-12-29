using System;

namespace PetZombie
{
	public class GameCreator
	{
		public GameCreator ()
		{
		}

        static public IGame CreateGame(int rowsCount, int columnsCount, int level, User user)
        {
            return new ThreeInRowGame(rowsCount, columnsCount, level, user);
        }
	}
}

