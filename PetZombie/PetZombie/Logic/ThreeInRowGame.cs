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
		int currentBrainCount;

		public List<List<Block>> Blocks {
			get { return this.blocks; }
		}

		public int Points {
			get{ return this.points; }
		}

		public int Level {
			get { return this.level; }
		}

		public int StepsCount {
			get { return this.stepsCount; }
		}

		public int BrainCount
		{
			get { return this.currentBrainCount; }
		}

		public ThreeInRowGame (int rowsCount, int columnsCount, int target, int steps, int level)
		{
			this.random = new Random ();
			do {
				this.blocks = new List<List<Block>> ();
				for (int i = 0; i < rowsCount; i++) {
					List<Block> row = new List<Block> ();
					for (int j = 0; j < columnsCount; j++) {
						row.Add (GenerateBlock (false, i, j));
					}
					this.blocks.Add (row);
				}
				this.GenerateZombie ();
				this.GenerateBrain ();
			} while (this.CheckDelete ().Count != 0);

			this.target = target;
			this.stepsCount = steps;
			this.level = level;

			this.weapons = new List<Weapon> ();
			this.points = 0;
			this.currentBrainCount = 0;
		}

		//Генерация блока
		//
		//Аргументы:
		//int x - индекс строки
		//int y - индекс столбца
		//
		//Возвращает Block - новый случайный блок, исключая зомби-блок
		private Block GenerateBlock (bool brain, int x = 0, int y = 0)
		{
			int number;
			if (brain)
				number = random.Next (0, 6);
			else
				number = random.Next (0, 5);
			BlockType type = (BlockType)BlockType.ToObject (typeof(BlockType), number);
			Position position = new Position (x, y);
			Block block = new Block (type, position);
			return block;
		}

		//Генерация одного блока зомби, путем замещения типа случайного блока на поле.
		private void GenerateZombie(){
			int randomRow = random.Next (0, this.blocks.Count);
			int randomColumn = random.Next (0, this.blocks[randomRow].Count);
			this.blocks [randomRow] [randomColumn].Type = BlockType.Zombie;
		}

		//Генерация одного блока зомби, путем замещения типа случайного блока на поле.
		private void GenerateBrain(){
			int randomRow = random.Next (0, this.blocks.Count);
			int randomColumn = random.Next (0, this.blocks[randomRow].Count);
			this.blocks [randomRow] [randomColumn].Type = BlockType.Brain;
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
						this.stepsCount--;
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
			if (Math.Abs (block1.Position.RowIndex - block2.Position.RowIndex) == 1 &&
			    (block1.Position.ColumnIndex == block2.Position.ColumnIndex))
				return true;
			if (Math.Abs (block1.Position.ColumnIndex - block2.Position.ColumnIndex) == 1 &&
			    (block1.Position.RowIndex == block2.Position.RowIndex))
				return true;
			return false;
		}

		private List<List<Block>> CheckDelete ()
		{
			List<List<Block>> delBlocks = new List<List<Block>> ();
			int rowsCount = this.blocks.Count;
			int columnsCount;
			List <Block> tmpRow, tmpColumn;
			for (int i = 0; i < rowsCount; i++) {
				columnsCount = this.blocks [i].Count;
				for (int j = 0; j < columnsCount; j++) {
					tmpRow = new List<Block> ();
					tmpColumn = new List<Block> ();
					tmpColumn.Add (this.blocks [i] [j]);
					tmpRow.Add (this.blocks [i] [j]);

					int k = i + 1;
					int l = j + 1;
					while (k < rowsCount || l < columnsCount) {
						if (k < rowsCount && this.blocks [k] [j].Type == this.blocks [i] [j].Type) {
							tmpColumn.Add (this.blocks [k] [j]);
						} else {
							k = rowsCount;
						}
						if (l < columnsCount && this.blocks [i] [l].Type == this.blocks [i] [j].Type) {
							tmpRow.Add (this.blocks [i] [l]);
						} else {
							l = columnsCount;
						}
						k++;
						l++;
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
					while (row < this.blocks.Count) {
						int nextRow = row + 1;
						if (nextRow < this.blocks.Count)
							this.blocks [row] [block.Position.ColumnIndex].Type = this.blocks [nextRow] [block.Position.ColumnIndex].Type;
						else {
							Block newBlock = this.GenerateBlock (true);
							this.blocks [row] [block.Position.ColumnIndex].Type = newBlock.Type;
						}
						movingBlocks.Add (new Block (this.blocks [row] [block.Position.ColumnIndex].Type, this.blocks [row] [block.Position.ColumnIndex].Position));
						row++;
					}
					points += 10;
				}
			}
				
			return new Tuple<List<Block>, List<Block>> (delBlocks, movingBlocks);
		}

		private void BrainChecking ()
		{
			for (int i = 0; i < this.blocks [0].Count; i++) {
				if (this.blocks [0] [i].Type == BlockType.Brain) {
					this.currentBrainCount++;
					int row = 0;
					while (row < this.blocks.Count) {
						int nextRow = row + 1;
						if (nextRow < this.blocks.Count)
							this.blocks [row] [i].Type = this.blocks [nextRow] [i].Type;
						else {
							Block newBlock = this.GenerateBlock (true);
							this.blocks [row] [i].Type = newBlock.Type;
						}
						//movingBlocks.Add (new Block (this.blocks [row] [i].Type, this.blocks [row] [i].Position));
						row++;
					}
				}
			}
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

