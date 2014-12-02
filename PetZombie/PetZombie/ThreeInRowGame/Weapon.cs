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

		//Использовать оружие
		//Принимает блок и всю матрицу блоков, на которые применяется оружие
		//Вовзращает матрицу, над которой применили оружие
        List<Tuple<List<Block>,int>> Use (Block block, int rowCount, int columnCount);
	}
}

