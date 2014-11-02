﻿using System;
using System.Collections.Generic;

namespace PetZombie
{
	public abstract class Weapon
	{
		public int count;

        public int Count
        {
            get{ return this.count; }
        }

		public Weapon (int count=1)
		{
			this.count = count;
		}

		//Использовать оружие
		//Принимает блок и всю матрицу блоков, на которые применяется оружие
		//Вовзращает матрицу, над которой применили оружие
        abstract public List<List<Block>> Use (Block block, List<List<Block>> blocks, 
            ThreeInRowGame.BlockGenerator GenerateBlocks, ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent);
	}
}

