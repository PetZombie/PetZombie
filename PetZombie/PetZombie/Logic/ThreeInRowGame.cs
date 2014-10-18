using System;
using System.Collections.Generic;

namespace PetZombie
{
	public class ThreeInRowGame : IGame
	{
		List<List<Block>> blocks;
		public int target;
		public int stepsCount;
		List<Weapon> weapons;
		Random random;

		public ThreeInRowGame (int rowsCount = 10, int columnsCount = 6, int target = 5, int steps = 25)
		{
			this.random = new Random ();
			this.blocks = new List<List<Block>> ();
			for (int i = 0; i < rowsCount; i++) {
				List<Block> row = new List<Block> ();
				for (int j = 0; j < columnsCount; j++) {
					row.Add (GenerateBlock (i, j));
				}
				this.blocks.Add (row);
			}
			this.target = target;
			this.stepsCount = steps;
			this.weapons = new List<Weapon> ();
		}

		//Генерация блока
		//
		//Аргументы:
		//int x - индекс строки
		//int y - индекс столбца
		//
		//Возвращает Block - новый случайный блок
		private Block GenerateBlock (int x, int y)
		{
			int number = random.Next (0, 4);
			Colors color = (Colors)Colors.ToObject (typeof(Colors), number);
			Position position = new Position(x, y);
			Block block = new Block (color, position);
			return block;
		}

		//Меняет местами два блока (меняет позиции этих блоков)
		//
		//Аргументы:
		//Block block1 - первый блок для передвижения
		//Block block2 - второй блок для передвижения
		//
		//Возвращает List<List<Block>> - матрица с подвинутыми блоками || null если переданные блоки не нашлись
		public List<List<Block>> MoveBlocks (Block block1, Block block2)
		{
			try {
				Block existBlock1 = this.blocks [block1.Position.X] [block1.Position.Y];
				Block existBlock2 = this.blocks [block2.Position.X] [block2.Position.X];

				this.blocks [block1.Position.X] [block1.Position.Y].Color = existBlock2.Color;
				this.blocks [block2.Position.X] [block2.Position.Y].Color = existBlock1.Color;

				List<List<Block>> delBlocks = this.CheckDelete ();
				if (delBlocks.Count > 0)
					this.DeleteBlocks (delBlocks);

				return this.blocks;
			} catch {
				return null;
			}
		}

		private List<List<Block>> CheckDelete ()
		{
			List<List<Block>> delBlocks = new List<List<Block>> ();
			int n = this.blocks.Count;
			int m;
			List <Block> tmpRow, tmpColumn;
			for (int i = 0; i < n; i++) {
				m = this.blocks [i].Count;
				for (int j = 0; j < m; j++) {

					tmpRow = new List<Block> ();
					tmpColumn = new List<Block> ();
					tmpColumn.Add (this.blocks [i] [j]);
					tmpRow.Add (this.blocks [i] [j]);

					int k = i + 1;
					int l = j + 1;
					while (k < n || l < m) {
						if (k < n && this.blocks [k] [j].Color == this.blocks [i] [j].Color) {
							tmpColumn.Add (this.blocks [k] [j]);
							k++;
						} else
							k = n;
						if (l < m && this.blocks [i] [l].Color == this.blocks [i] [j].Color) {
							tmpRow.Add (this.blocks [i] [l]);
							l++;
						} else
							l = m;
					}
					if (tmpColumn.Count > 2)
						delBlocks.Add (tmpColumn);
					if (tmpRow.Count > 2)
						delBlocks.Add (tmpRow);
				}
			}

			return delBlocks;
		}

		private void DeleteBlocks (List<List<Block>> blocks)
		{
			//если удалили строку, то все верхние в тех же столбцах падают вниз на 1 строку
			// если удалили столбец, то все верхние в этом столбце блоки падают вниз на количество строк = удаленным блокам
		}

		public void UseWeapon (Weapon weapon, Block block)
		{
			try {
				Block existBlock = this.blocks [block.Position.X] [block.Position.Y];
				weapon.Use ();
			} catch {
			}
		}
	}
}

