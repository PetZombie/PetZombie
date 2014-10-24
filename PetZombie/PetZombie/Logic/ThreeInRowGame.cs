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
		int points;
		int level;

		public List<List<Block>> Blocks {
			get { return this.blocks; }
		}

		public int Points {
			get{ return this.points; }
		}

		public int Level{
			get { return this.level; }
		}

		public ThreeInRowGame (int rowsCount, int columnsCount, int target, int steps, int level)
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
			this.level = level;

			this.weapons = new List<Weapon> ();
			this.points = 0;
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
			int number = random.Next (0, 7);
			BlockType type = (BlockType)BlockType.ToObject (typeof(BlockType), number);
			Position position = new Position (x, y);
			Block block = new Block (type, position);
			return block;
		}

		//Генерация блока
		//
		//Возвращает Block - новый случайный блок, с индексом [0,0]
		private Block GenerateBlock ()
		{
			int number = random.Next (0, 6);
			BlockType type = (BlockType)BlockType.ToObject (typeof(BlockType), number);
			Position position = new Position ();
			Block block = new Block (type, position);
			return block;
		}

		//Меняет местами два блока (меняет позиции этих блоков)
		//
		//Аргументы:
		//Block block1 - первый блок для передвижения
		//Block block2 - второй блок для передвижения
		//
		//Возвращает Tuple<List<Block>, List<Block>> - тьюпл из двух списков блоков. 
		//Первый список - удаляемые блоки, второй - перемещаемые блоки.
		public Tuple<List<Block>, List<Block>> MoveBlocks (Block block1, Block block2)
		{
			try {
				if (AbilityToReplace (block1, block2)) {

					this.blocks [block1.Position.RowIndex] [block1.Position.ColumnIndex].Type = block2.Type;
					this.blocks [block2.Position.RowIndex] [block2.Position.ColumnIndex].Type = block1.Type;

					List<List<Block>> delBlocks = this.CheckDelete ();
					if (delBlocks.Count > 0) {
						return this.DeleteBlocks (delBlocks);
					} else {
						this.blocks [block1.Position.RowIndex] [block1.Position.ColumnIndex].Type = block1.Type;
						this.blocks [block2.Position.RowIndex] [block2.Position.ColumnIndex].Type = block2.Type;
					}
				}

				return null;
			} catch {
				return null;
			}
		}

		private bool AbilityToReplace (Block block1, Block block2)
		{
			return ((Math.Abs (block1.Position.RowIndex - block2.Position.RowIndex) == 1 &&
			(block1.Position.ColumnIndex == block2.Position.ColumnIndex))
			|| (Math.Abs (block1.Position.ColumnIndex - block2.Position.ColumnIndex) == 1 &&
			(block1.Position.RowIndex == block2.Position.RowIndex)));
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
						if (k < n && this.blocks [k] [j].Type == this.blocks [i] [j].Type) {
							tmpColumn.Add (this.blocks [k] [j]);
							k++;
						} else
							k = n;
						if (l < m && this.blocks [i] [l].Type == this.blocks [i] [j].Type) {
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

		private Tuple<List<Block>, List<Block>> DeleteBlocks (List<List<Block>> blocks)
		{
			List<Block> delBlocks = new List<Block> ();
			List<Block> movingBlocks = new List<Block> ();
			foreach (List<Block> oneSet in blocks) {
				foreach (Block block in oneSet) {
					delBlocks.Add (block);
					int row = block.Position.RowIndex;
					while (row > 0) {
						int prevRow = row - 1;
						if (prevRow > 0)
							this.blocks [row] [block.Position.ColumnIndex].Type = this.blocks [prevRow] [block.Position.ColumnIndex].Type;
						else {
							Block newBlock = this.GenerateBlock ();
							this.blocks [row] [block.Position.ColumnIndex].Type = newBlock.Type;
						}
						movingBlocks.Add (new Block(this.blocks [row] [block.Position.ColumnIndex].Type, this.blocks [row] [block.Position.ColumnIndex].Position));
						row--;
					}
					points += 10;
				}
			}
			return new Tuple<List<Block>, List<Block>> (delBlocks, movingBlocks);
		}

		public void UseWeapon (Weapon weapon, Block block)
		{
			try {
				Block existBlock = this.blocks [block.Position.RowIndex] [block.Position.ColumnIndex];
				weapon.Use ();
			} catch {
			}
		}

		protected List<Block> GetNeighbors (Block block)
		{
			List<Block> neighbors = new List<Block> ();
			int row = block.Position.RowIndex;
			int column = block.Position.ColumnIndex;
			if (row - 1 >= 0)
				neighbors.Add (this.blocks [row - 1] [column]);
			if (row + 1 < this.blocks.Count)
				neighbors.Add (this.blocks [row + 1] [column]);
			if (column - 1 >= 0)
				neighbors.Add (this.blocks [row] [column - 1]);
			if (column + 1 < this.blocks [row].Count)
				neighbors.Add (this.blocks [row] [column + 1]);
			return neighbors;
		}
	}
}

