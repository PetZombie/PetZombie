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

        public delegate Block Del(bool brain,int x = 0,int y = 0);

        public List<List<Block>> Blocks
        {
            get { return this.blocks; }
        }

        public int Points
        {
            get{ return this.points; }
        }

        public int Level
        {
            get { return this.level; }
        }

        public int StepsCount
        {
            get { return this.stepsCount; }
        }

        public int BrainCount
        {
            get { return this.currentBrainCount; }
        }

        public ThreeInRowGame(int rowsCount, int columnsCount, int target, int steps, int level)
        {
            this.random = new Random();
            do
            {
                this.blocks = new List<List<Block>>();
                for (int i = 0; i < rowsCount; i++)
                {
                    List<Block> row = new List<Block>();
                    for (int j = 0; j < columnsCount; j++)
                    {
                        row.Add(GenerateBlock(false, i, j));
                    }
                    this.blocks.Add(row);
                }
                this.GenerateZombie();
                this.GenerateBrain();
            } while (this.CheckDelete() != null || !this.CheckBrainAndZombieAreNotNear());

            this.target = target;
            this.stepsCount = steps;
            this.level = level;

            this.weapons = new List<Weapon>();
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
        private Block GenerateBlock(bool brain, int x = 0, int y = 0)
        {
            int number;
            if (brain)
                number = random.Next(0, 6);
            else
                number = random.Next(0, 5);
            BlockType type = (BlockType)BlockType.ToObject(typeof(BlockType), number);
            Position position = new Position(x, y);
            Block block = new Block(type, position);
            return block;
        }
        //Генерация одного блока зомби, путем замещения типа случайного блока на поле.
        private void GenerateZombie()
        {
            int randomRow = random.Next(0, this.blocks.Count);
            int randomColumn = random.Next(0, this.blocks[randomRow].Count);
            this.blocks[randomRow][randomColumn].Type = BlockType.Zombie;
        }
        //Генерация одного блока зомби, путем замещения типа случайного блока на поле.
        private void GenerateBrain()
        {
            int lastRow = this.blocks.Count - 1;
            int randomColumn = random.Next(0, this.blocks[lastRow].Count);
            this.blocks[lastRow][randomColumn].Type = BlockType.Brain;
        }

        private bool CheckBrainAndZombieAreNotNear()
        {
            Block brain = this.blocks[blocks.Count - 1].Find(delegate(Block b)
            {
                return b.Type == BlockType.Brain;
            });
            Block zombie = this.blocks[blocks.Count - 1].Find(delegate(Block b)
            {
                return b.Type == BlockType.Zombie;
            }); 
            return (zombie == null || (Math.Abs(brain.Position.ColumnIndex - zombie.Position.ColumnIndex) > 1
            && Math.Abs(brain.Position.RowIndex - zombie.Position.RowIndex) > 1));
        }
        //Меняет местами два блока (меняет позиции этих блоков)
        //
        //Аргументы:
        //Block block1 - первый блок для передвижения
        //Block block2 - второй блок для передвижения
        //
        //Возвращает Tuple<List<Block>, List<Block>> - тьюпл из двух списков блоков.
        //Первый список - удаляемые блоки, второй - перемещаемые блоки.
        public Tuple<List<Block>, List<Block>, List<Block>, int> ReplaceBlocks(Block block1, Block block2)
        {
            this.blocks[block1.Position.RowIndex][block1.Position.ColumnIndex].Type = block2.Type;
            this.blocks[block2.Position.RowIndex][block2.Position.ColumnIndex].Type = block1.Type;

            Tuple<List<Block>, int> delBlocks = this.CheckDelete();
            if (delBlocks != null)
            {
                this.stepsCount--;
                return this.DeleteBlocks(delBlocks, block1, block2);
            }
            else
            {
                this.blocks[block1.Position.RowIndex][block1.Position.ColumnIndex].Type = block1.Type;
                this.blocks[block2.Position.RowIndex][block2.Position.ColumnIndex].Type = block2.Type;
                return null;
            }
        }
        //Проверка на возможность перемещения двух блоков.
        //Учитывается расположение блоков в матрице, а также тип блоков.
        //Если хотя бы один из блоков - зомби, то перемещение невозможно.
        //Если блоки расположены друг от друга на расстоянии большем 1, то метод вернет false
        //Иначе вернется true
        public bool AbilityToReplace(Block block1, Block block2)
        {
            if (block1.Type == BlockType.Zombie || block2.Type == BlockType.Zombie)
                return false;
            if (Math.Abs(block1.Position.RowIndex - block2.Position.RowIndex) == 1 &&
                (block1.Position.ColumnIndex == block2.Position.ColumnIndex))
                return true;
            if (Math.Abs(block1.Position.ColumnIndex - block2.Position.ColumnIndex) == 1 &&
                (block1.Position.RowIndex == block2.Position.RowIndex))
                return true;
            return false;
        }

        private Tuple<List<Block>, int> CheckDelete()
        {
            int rowsCount = this.blocks.Count;
            int columnsCount;
            for (int i = 0; i < rowsCount; i++)
            {
                columnsCount = this.blocks[i].Count;
                for (int j = 0; j < columnsCount; j++)
                {
                    List <Block> tmpRow = new List<Block>();
                    List <Block> tmpColumn = new List<Block>();
                    tmpColumn.Add(this.blocks[i][j]);
                    tmpRow.Add(this.blocks[i][j]);

                    int k = i + 1;
                    int l = j + 1;
                    while (k < rowsCount || l < columnsCount)
                    {
                        if (k < rowsCount && this.blocks[k][j].Type == this.blocks[i][j].Type)
                        {
                            tmpColumn.Add(this.blocks[k][j]);
                        }
                        else
                        {
                            k = rowsCount;
                        }
                        if (l < columnsCount && this.blocks[i][l].Type == this.blocks[i][j].Type)
                        {
                            tmpRow.Add(this.blocks[i][l]);
                        }
                        else
                        {
                            l = columnsCount;
                        }
                        k++;
                        l++;
                    }
                    if (tmpColumn.Count > 2)
                        return new Tuple<List<Block>, int>(tmpColumn, tmpColumn.Count);
                    if (tmpRow.Count > 2)
                        return new Tuple<List<Block>, int>(tmpRow, 1);
                }
            }

            return null;
        }

        private Tuple<List<Block>, List<Block>, List<Block>, int> DeleteBlocks(Tuple<List<Block>, int> blocksForDelete, Block repBlock1, Block repBlock2)
        {
            List<Block> delBlocks = new List<Block>();
            List<Block> movingBlocks = new List<Block>();
            List<Block> newBlocks = new List<Block>();
            foreach (Block block in blocksForDelete.Item1)
            {

                if (block.Position.RowIndex == repBlock1.Position.RowIndex && block.Position.ColumnIndex == repBlock1.Position.ColumnIndex)
                    delBlocks.Add(new Block(repBlock2.Type, repBlock2.Position));
                else
                {
                    if (block.Position.RowIndex == repBlock2.Position.RowIndex && block.Position.ColumnIndex == repBlock2.Position.ColumnIndex)
                        delBlocks.Add(new Block(repBlock1.Type, repBlock1.Position));
                    else
                        delBlocks.Add(new Block(block.Position));
                }
                int row = block.Position.RowIndex;
                int column = block.Position.ColumnIndex;
                int increment = 1;
                while (row < this.blocks.Count)
                {
                    if (this.blocks[row][column].Type == BlockType.Zombie)
                    {
                        row++;
                        increment--;
                        continue;
                    }
                    int nextRow = row + increment;
                    if (nextRow < this.blocks.Count && this.blocks[nextRow][column].Type == BlockType.Zombie)
                    {
                        increment++;
                        continue;
                    }

                    if (nextRow < this.blocks.Count)
                    {
                        if (nextRow == repBlock1.Position.RowIndex && column == repBlock1.Position.ColumnIndex)
                            movingBlocks.Add(this.blocks[repBlock2.Position.RowIndex][repBlock2.Position.ColumnIndex]);
                        else
                        {
                            if (nextRow == repBlock2.Position.RowIndex && column == repBlock2.Position.ColumnIndex)
                                movingBlocks.Add(this.blocks[repBlock1.Position.RowIndex][repBlock1.Position.ColumnIndex]);
                            else
                                movingBlocks.Add(this.blocks[nextRow][column]);
                        }
                        this.blocks[row][column].Type = this.blocks[nextRow][column].Type;
                    }
                    else
                    {
                        Block newBlock = this.GenerateBlock(true);
                        this.blocks[row][column].Type = newBlock.Type;
                        newBlocks.Add(new Block(newBlock.Type, new Position(nextRow, column)));
                    }

                    row++;
                }
                points += 10;
            }

            return new Tuple<List<Block>, List<Block>, List<Block>, int>(delBlocks, movingBlocks, newBlocks, blocksForDelete.Item2);
        }

        private void BrainDeleteChecking()
        {
            for (int i = 0; i < this.blocks[0].Count; i++)
            {
                if (this.blocks[0][i].Type == BlockType.Brain)
                {

                    this.currentBrainCount++;
                    int row = 0;
                    while (row < this.blocks.Count)
                    {
                        int nextRow = row + 1;
                        if (nextRow < this.blocks.Count)
                            this.blocks[row][i].Type = this.blocks[nextRow][i].Type;
                        else
                        {
                            Block newBlock;
                            if (HasOtherBrain(this.blocks[0][i]))
                                newBlock = this.GenerateBlock(true);
                            else
                                newBlock = new Block(BlockType.Brain);
                            this.blocks[row][i].Type = newBlock.Type;
                        }
                        //movingBlocks.Add (new Block (this.blocks [row] [i].Type, this.blocks [row] [i].Position));
                        row++;
                    }
                }
            }
        }

        private bool HasOtherBrain(Block brain)
        {
            foreach (List<Block> row in this.blocks)
            {
                List<Block> brains = row.FindAll(delegate (Block b)
                {
                    return b.Type == BlockType.Brain;
                });
                if (brains.Count > 0)
                {
                    if (brains.Contains(brain) && brains.Count == 1)
                        continue;
                    return true;
                }
            }
            return false;
        }

        public void UseWeapon(Weapon weapon, Block block)
        {
            try
            {
                Block existBlock = this.blocks[block.Position.RowIndex][block.Position.ColumnIndex];
                //Del handler = this.GenerateBlock(true);
                //weapon.Use (existBlock, this.blocks, handler);
            }
            catch
            {
            }
        }

        protected List<Block> GetNeighbors(Block block)
        {
            List<Block> neighbors = new List<Block>();
            int row = block.Position.RowIndex;
            int column = block.Position.ColumnIndex;
            if (row - 1 >= 0)
                neighbors.Add(this.blocks[row - 1][column]);
            if (row + 1 < this.blocks.Count)
                neighbors.Add(this.blocks[row + 1][column]);
            if (column - 1 >= 0)
                neighbors.Add(this.blocks[row][column - 1]);
            if (column + 1 < this.blocks[row].Count)
                neighbors.Add(this.blocks[row][column + 1]);
            return neighbors;
        }
    }
}

