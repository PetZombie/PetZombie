using System;
using System.Collections.Generic;

namespace PetZombie
{
    public interface Weapon
	{
        //protected int count;
        //int Count
        //{
        //  get;// { return this.count; }
        //}

        List<Tuple<List<Block>,int>> Use (Block block, int rowCount, int columnCount);
	}
}

