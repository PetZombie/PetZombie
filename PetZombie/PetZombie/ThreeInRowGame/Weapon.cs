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
       List<List<Block>> Use (Block block, List<List<Block>> blocks, ThreeInRowGame.BlockGenerator GenerateBlocks, ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent);
	}
}

