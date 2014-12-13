using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class ThreeInRowGame : IGame
    {
        List<List<Block>> blocks;
        public int target;
        int stepsCount;
        List<Weapon> weapons;
        Random random;
        int points, level, currentBrainCount, gold;
        int blockPoints, brainPoints, zombiePoints, stepPoints;

        public delegate void DeleteEventHandler(object sender,BlocksDeletingEventArgs e);

        public event DeleteEventHandler Delete;

        public delegate void EndGameEventHandler(object sender, EndGameEventArgs e);

        public event EndGameEventHandler EndGame;

        public delegate Block BlockGenerator(bool brain,int rowIndex = 0,int columnIndex = 0, bool brainInBank=false);

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
            IDataService data = DataServiceFactory.DataService();
            data.Write(new User(2, 3, new ZombiePet("Bob"), 1));

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
                if (level > 1)
                    this.GenerateZombie();
                if (level > 4)
                    GenerateZombie();
                this.GenerateBrain();

                if (level > 3)
                {
                    int k = 0;
                    for (int i = 0; i < blocks[0].Count; i++)
                    {
                        blocks[0][i].Cage = true;
                        if (level > 4)
                        {
                            blocks[1][k].Cage = true;
                            k += 2;
                        }
                    }
                }
            } while (this.CheckDelete().Count > 0 || !this.CheckBrainAndZombieAreNotNear());

            this.target = target;
            this.stepsCount = steps;
            this.level = level;
            this.gold = 0;

            this.weapons = new List<Weapon>();
            this.points = 0;
            this.currentBrainCount = 0;

            this.blockPoints = 10;
            this.zombiePoints = 50;
            this.brainPoints = 70;
            this.stepPoints = 10;
            User user = data.Read();
        }

        /// <summary>
        /// Генерация нового блока случайного типа, исключая зомби-блок.
        /// </summary>
        /// <param name="brain">Может сгенерироваться мозг или нет</param>
        /// <param name="rowIndex">Позиция блока в строке. По умолчанию равна 0</param>
        /// <param name="columnIndex">Позиция блока в столбце. По умолчанию равна 0</param>
        /// <returns>Блок с типом, случайно выбранным из перечисления BlockType, исключая Zombie</returns>
        private Block GenerateBlock(bool brain, int rowIndex = 0, int columnIndex = 0, bool brainIBank = false)
        {
            int number;
            if (brain)
                number = random.Next(0, 6);
            else
                number = random.Next(0, 5);
            BlockType type = (BlockType)BlockType.ToObject(typeof(BlockType), number);
            if (brainIBank && type == BlockType.Brain)
                type = BlockType.BrainInBank;
            Position position = new Position(rowIndex, columnIndex);
            Block block = new Block(type, position);
            return block;
        }
        //Генерация одного блока зомби, путем замещения типа случайного блока на поле.
        private void GenerateZombie()
        {
            int randomRow = random.Next(0, this.blocks.Count);
            int randomColumn = random.Next(0, this.blocks[randomRow].Count);
            this.blocks[randomRow][randomColumn] = new ZombieBlock(new Position(randomRow, randomColumn));
        }
        //Генерация одного блока зомби, путем замещения типа случайного блока на поле.
        private void GenerateBrain(bool brainIBank = false)
        {
            int lastRow = this.blocks.Count - 1;
            int randomColumn = random.Next(0, this.blocks[lastRow].Count);
            if (brainIBank)
                this.blocks[lastRow][randomColumn].Type = BlockType.BrainInBank;
            else
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

        /// <summary>
        /// Метод заменяет местами два блока и удаляет блоки, если собралась нудная комбинация.
        /// </summary>
        /// <param name="block1">Первый блок для перемещения</param>
        /// <param name="block2">Второй блок для перемещения</param>
        public bool ReplaceBlocks(Block block1, Block block2)
        {
            this.blocks[block1.Position.RowIndex][block1.Position.ColumnIndex].Type = block2.Type;
            this.blocks[block2.Position.RowIndex][block2.Position.ColumnIndex].Type = block1.Type;

            List<Tuple<List<Block>, int>> delBlocks = this.CheckDelete();

            if (delBlocks.Count > 0)
            {
                this.stepsCount--;
                this.DeleteBlocks(new List<Tuple<List<Block>, int>>(delBlocks));
                this.ZombieEatBrain();
                //this.BrainDeleteChecking();
                return true;
            }
            else
            {
                this.blocks[block1.Position.RowIndex][block1.Position.ColumnIndex].Type = block1.Type;
                this.blocks[block2.Position.RowIndex][block2.Position.ColumnIndex].Type = block2.Type;
                return false;
            }
        }

        public void NextDelete()
        {
            List<Tuple<List<Block>, int>> delBlocks = this.CheckDelete();

            if (delBlocks.Count > 0)
            {
                this.DeleteBlocks(new List<Tuple<List<Block>, int>>(delBlocks));
                this.ZombieEatBrain();
                //this.BrainDeleteChecking();
            }
            else
                CheckEndGame();

        }

        /// <summary>
        /// Проверка на возможность перемещения двух блоков. Учитывается расположение блоков в матрице, а также тип блоков.
        /// </summary>
        /// <param name="block1">Первый блок для перемещения</param>
        /// <param name="block2">Второй блок для перемещения</param>
        /// <returns>Возможно замена местами этих вух блоков или нет</returns>
        public bool AbilityToReplace(Block block1, Block block2)
        {
            if (block1.Type == BlockType.Zombie || block2.Type == BlockType.Zombie)
                return false;
            if (block1.Type == BlockType.BrainInBank || block2.Type == BlockType.BrainInBank)
                return false;
            if (block1.Type == BlockType.BrainInCrackedBank || block2.Type == BlockType.BrainInCrackedBank)
                return false;
            if (Math.Abs(block1.Position.RowIndex - block2.Position.RowIndex) == 1 &&
                (block1.Position.ColumnIndex == block2.Position.ColumnIndex))
                return true;
            if (Math.Abs(block1.Position.ColumnIndex - block2.Position.ColumnIndex) == 1 &&
                (block1.Position.RowIndex == block2.Position.RowIndex))
                return true;
            return false;
        }

        private void CrashBank(List<Block> blocks)
        {
            foreach (Block block in blocks)
            {
                List<Block> neighbors = GetNeighbors(block);
                foreach (Block b in neighbors)
                {
                    if (b.Type == BlockType.BrainInBank)
                        this.blocks[b.Position.RowIndex][b.Position.ColumnIndex].Type = BlockType.BrainInCrackedBank;
                    else if (b.Type == BlockType.BrainInCrackedBank)
                        this.blocks[b.Position.RowIndex][b.Position.ColumnIndex].Type = BlockType.Brain;
                }
            }
        }

        private List<Tuple<List<Block>, int>> CheckDelete()
        {
            List<Tuple<List<Block>, int>> delBlocks = new List<Tuple<List<Block>, int>>();
            int rowsCount = this.blocks.Count;
            int columnsCount;
            bool hasRow = false, hasColumn = false;
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
                            if (this.blocks[k][j].Type != BlockType.Brain && this.blocks[k][j].Type != BlockType.BrainInBank
                                && this.blocks[k][j].Type != BlockType.BrainInCrackedBank)
                                tmpColumn.Add(this.blocks[k][j]);
                        }
                        else
                        {
                            k = rowsCount;
                        }
                        if (l < columnsCount && this.blocks[i][l].Type == this.blocks[i][j].Type)
                        {
                            if (this.blocks[i][j].Type != BlockType.Brain && this.blocks[i][j].Type != BlockType.BrainInBank
                                && this.blocks[i][j].Type != BlockType.BrainInCrackedBank)
                                tmpRow.Add(this.blocks[i][l]);
                        }
                        else
                        {
                            l = columnsCount;
                        }
                        k++;
                        l++;
                    }
                    if (!hasColumn && tmpColumn.Count > 2)
                    {
                        this.CrashBank(tmpColumn);
                        delBlocks.Add(new Tuple<List<Block>, int>(tmpColumn, tmpColumn.Count));
                        hasColumn = true;
                    }
                    if (!hasRow && tmpRow.Count > 2)
                    {
                        this.CrashBank(tmpRow);
                        delBlocks.Add(new Tuple<List<Block>, int>(tmpRow, 1));
                        hasRow = true;
                    }
                    if (i == 0 && this.blocks[i][j].Type == BlockType.Brain)
                    {
                        List<Block> brainForDelete = new List<Block>();
                        brainForDelete.Add(this.blocks[i][j]);
                        delBlocks.Add(new Tuple<List<Block>, int>(brainForDelete, 1));
                    }
                }
            }

            return delBlocks;
        }

        private bool ContainsBlock(List<Block> blocks, Block block)
        {
            foreach (Block b in blocks)
            {
                if (b.Position.RowIndex == block.Position.RowIndex && b.Position.ColumnIndex == block.Position.ColumnIndex)
                    return true;
            }
            return false;
        }

        private void DeleteBlocks(List<Tuple<List<Block>, int>> blocksForDelete, bool deleteZombie=false)
        {
            List<Block> delBlocks = new List<Block>();
            List<Block> prevMovBlocks = new List<Block>();
            List<Block> movingBlocks = new List<Block>();

            bool firstly = true;

            foreach (Tuple<List<Block>,int> oneSet in blocksForDelete)
            {
                foreach (Block block in oneSet.Item1)
                {
                    if (block is ZombieBlock)
                        points += zombiePoints;
                    else
                        points += blockPoints;

                    if (block.Cage)
                    {
                        this.blocks[block.Position.RowIndex][block.Position.ColumnIndex].Cage = false;
                        continue;
                    }
                    else
                    {
                        if (!ContainsBlock(delBlocks, block))
                            delBlocks.Add(new Block(block));
                    }

                    if (oneSet.Item2 == 1 || firstly)
                    {
                        firstly = false;
                        int row = block.Position.RowIndex;
                        int column = block.Position.ColumnIndex;
                        while (row < this.blocks.Count)
                        {
                            if (!deleteZombie && this.blocks[row][column].Type == BlockType.Zombie)
                            {
                                row++;
                                continue;
                            }

                            int nextRow = row + oneSet.Item2;

                            if (!deleteZombie && nextRow < this.blocks.Count && this.blocks[nextRow][column].Type == BlockType.Zombie)
                                nextRow++;

                            if (nextRow < this.blocks.Count)
                            {
                                prevMovBlocks.Add(new Block(this.blocks[nextRow][column]));
                                this.blocks[row][column].Type = this.blocks[nextRow][column].Type;
                                movingBlocks.Add(new Block(this.blocks[row][column]));
                            }
                            else
                            {
                                Block newBlock;
                                int currentBrains = HasOtherBrain(this.blocks[row][column]);
                                if (currentBrains > 3)
                                    newBlock = this.GenerateBlock(false);
                                else
                                {
                                    if (currentBrains > 0)
                                        newBlock = this.GenerateBlock(true);
                                    else
                                        newBlock = new Block(BlockType.Brain);
                                }
                                this.blocks[row][column].Type = newBlock.Type;
                                prevMovBlocks.Add(new Block(newBlock.Type, new Position(this.blocks.Count, column)));
                                movingBlocks.Add(new Block(newBlock.Type, new Position(row, column)));
                            }
                            row++;
                        }
                    }
                }

            }

            BlocksDeletingEventArgs e = new BlocksDeletingEventArgs(delBlocks, prevMovBlocks, movingBlocks);
         
            DeleteEventHandler handler = Delete;
            if (handler != null)
                handler(this, e);
        }

        private bool BrainDeleteChecking()
        {
            List<Block> delBlocks = new List<Block>();
            List<Block> prevMovBlocks = new List<Block>();
            List<Block> movingBlocks = new List<Block>();
            bool hasDelete = false;

            for (int i = 0; i < this.blocks[0].Count; i++)
            {
                if (this.blocks[0][i].Type == BlockType.Brain)
                {
                    hasDelete = true;
                    delBlocks.Add(this.blocks[0][i]);
                    this.currentBrainCount++;
                    this.points += brainPoints;
                    int row = 0;
                    while (row < this.blocks.Count)
                    {
                        int nextRow = row + 1;
                        if (nextRow < this.blocks.Count)
                        {
                            prevMovBlocks.Add(this.blocks[nextRow][i]);
                            this.blocks[row][i].Type = this.blocks[nextRow][i].Type;
                            movingBlocks.Add(this.blocks[row][i]);
                        }
                        else
                        {
                            Block newBlock;
                            int currentBrains = HasOtherBrain(this.blocks[0][i]);
                            if (currentBrains > 3)
                                newBlock = this.GenerateBlock(false);
                            else
                            {
                                if (currentBrains > 0)
                                    newBlock = this.GenerateBlock(true);
                                else
                                    newBlock = new Block(BlockType.Brain);
                            }
                            this.blocks[row][i].Type = newBlock.Type;
                            prevMovBlocks.Add(new Block(new Position(this.blocks.Count, i)));
                            movingBlocks.Add(this.blocks[row][i]);
                        }
                        row++;
                    }
                }
            }
            if (hasDelete)
            {
                BlocksDeletingEventArgs e = new BlocksDeletingEventArgs(delBlocks, prevMovBlocks, movingBlocks);

                DeleteEventHandler handler = Delete;
                if (handler != null)
                    handler(this, e);
            }
            return hasDelete;
        }

        private int HasOtherBrain(Block brain)
        {
            int brainCount = 0;
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
                    brainCount += brains.Count;
                }
            }
            return brainCount;
        }

        private void ZombieEatBrain()
        {
            foreach (List<Block> oneRow in this.blocks)
            {
                List<ZombieBlock> zombies = new List<ZombieBlock>();
                foreach (Block b in oneRow)
                {
                    if (b is ZombieBlock)
                    {
                        ZombieBlock z = b as ZombieBlock;
                        zombies.Add(z);
                    }
                }

                foreach (ZombieBlock zombie in zombies)
                {
                    if (!zombie.CanEat)
                        continue;
                    int row = zombie.Position.RowIndex;
                    int column = zombie.Position.ColumnIndex;
                    List<Tuple<List<Block>, int>> allDelBlocks = new List<Tuple<List<Block>, int>>();

                    if (row-1 > 0 && this.blocks[row - 1][column].Type == BlockType.Brain)
                    {
                        List<Block> brain = new List<Block>();
                        brain.Add(new Block(this.blocks[row - 1][column]));
                        Tuple<List<Block>, int> delBlocks = new Tuple<List<Block>, int>(brain, 1);
                        allDelBlocks.Add(delBlocks);
                    }
                    if (column-1 > 0 && this.blocks[row][column - 1].Type == BlockType.Brain)
                    {
                        List<Block> brain = new List<Block>();
                        brain.Add(new Block(this.blocks[row][column - 1]));
                        Tuple<List<Block>, int> delBlocks = new Tuple<List<Block>, int>(brain, 1);
                        allDelBlocks.Add(delBlocks);
                    }

                    if (column + 1 < blocks[0].Count && this.blocks[row][column + 1].Type == BlockType.Brain)
                    {
                        List<Block> brain = new List<Block>();
                        brain.Add(new Block(this.blocks[row][column + 1]));
                        Tuple<List<Block>, int> delBlocks = new Tuple<List<Block>, int>(brain, 1);
                        allDelBlocks.Add(delBlocks);
                    }

                    if (row+1 < blocks.Count && this.blocks[row + 1][column].Type == BlockType.Brain)
                    {
                        List<Block> brain = new List<Block>();
                        brain.Add(new Block(this.blocks[row + 1][column]));
                        Tuple<List<Block>, int> delBlocks = new Tuple<List<Block>, int>(brain, 1);
                        allDelBlocks.Add(delBlocks);
                    }

                    if (allDelBlocks.Count > 0)
                    {
                        DeleteBlocks(allDelBlocks);
                    }

                }
            }
        }

        public void UseWeapon(Weapon weapon, Block block)
        {
            if (weapon is Soporific)
            {
                Soporific soporific = weapon as Soporific;
                this.blocks = new List<List<Block>>(soporific.GetAsleepZombieInBlocks(block, this.blocks));
            }
            DeleteBlocks(weapon.Use(block, blocks.Count, blocks[0].Count), true);
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

        private void CheckEndGame()
        {
            if (currentBrainCount == target)
            {
                points += stepPoints * stepsCount;
                gold = points / 20;

                EndGameEventArgs e = new EndGameEventArgs(true);

                EndGameEventHandler handler = EndGame;
                if (handler != null)
                    handler(this, e);
            }
            if (stepsCount == 0)
            {
                gold = points / 20;

                EndGameEventArgs e = new EndGameEventArgs(false);

                EndGameEventHandler handler = EndGame;
                if (handler != null)
                    handler(this, e);
            }
        }
    }
}