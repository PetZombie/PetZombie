using System;

namespace PetZombie
{
	public class GameCreator
	{
		public GameCreator ()
		{
		}

		static public IGame CreateGame(int rowsCount, int columnsCount, int target, int steps, int level)
        {
			return new ThreeInRowGame(rowsCount, columnsCount, target, steps, level);
        }
	}
}

