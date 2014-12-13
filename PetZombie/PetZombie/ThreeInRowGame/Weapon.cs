using System;
using System.Collections.Generic;

namespace PetZombie
{
    public interface Weapon
	{
        int Count
        {
            get;
        }

        int Cost
        {
            get;
        }

        List<Tuple<List<Block>,int>> Use (Block block, int rowCount, int columnCount);
	}
}

