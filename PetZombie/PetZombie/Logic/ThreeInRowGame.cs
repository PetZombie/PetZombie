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
			for (int i = 0; i < rowsCount; i++) {
				this.blocks.Add(GenerateBlock (columnsCount));
			}
			this.target = target;
			this.stepsCount = steps;
			this.weapons = new List<Weapon> ();
		}

		private List<Block> GenerateBlock (int count)
		{
			List <Block> newBlocks = new List <Block> ();
			for (int i = 0; i < count; i++) {
				int number = random.Next (0, 4);
				Block block = new Block (number);
			}
			return newBlocks;
		}

		public void MoveBlocks (int row1, int column1, int row2, int column2)
		{
			try {
				Block block1 = this.blocks [row1] [column1];
				Block block2 = this.blocks [row2] [column2];
				List<List<Block>> delBlocks = this.CheckDelete ();
				if (delBlocks.Count > 0)
					this.DeleteBlocks (delBlocks);
			} catch (Exception e) {
			}
		}

		private List<Block> CheckDelete ()
		{
			List<List<Block>> delBlocks = new List<Block> ();
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
						if (k < n && this.blocks [k] [j].ColorType == this.blocks [i] [j].ColorType) {
							tmpColumn.Add (this.blocks [k] [j]);
							k++;
						}
						else
							k = n;
						if (l < m && this.blocks [i] [l].ColorType == this.blocks [i] [j].ColorType) {
							tmpRow.Add (this.blocks [i] [l]);
							l++;
						}
						else
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

		public void UseWeapon (Weapon weapon, int row, int column)
		{
			try {
				Block block = this.blocks [row] [column];
				weapon.Use ();
			} catch (Exception e) {
			}
		}

	}
}

