using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class ThreeInRowGame : IGame
    {
        #region Fields

        List<List<Block>> blocks;
        public int target;
        int stepsCount;
        //List<Weapon> weapons;
        Random random;
        int points, level, currentBrainCount, gold;
        int blockPoints, brainPoints, zombiePoints, stepPoints;
        public User user;
        IDataService data;

        #endregion

        #region Delegates

        public delegate void DeleteEventHandler(object sender,BlocksDeletingEventArgs e);

        public event DeleteEventHandler Delete;

        public delegate void EndGameEventHandler(object sender,EndGameEventArgs e);

        public event EndGameEventHandler EndGame;

        public delegate Block BlockGenerator(bool brain,int rowIndex = 0,int columnIndex = 0,bool brainInBank = false);

        #endregion

        #region Properties

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

        public int Gold
        {
            get { return this.gold; }
        }

        public int StepsCount
        {
            get { return this.stepsCount; }
        }

        public int BrainCount
        {
            get { return this.currentBrainCount; }
        }

        #endregion

        public ThreeInRowGame(int rowsCount, int columnsCount, int level, User user)
        {
            data = DataServiceFactory.DataService();
            //data.Write(new User(2, 3, new ZombiePet("Fred", 50), 2));
            //this.user = data.Read();
            this.user = user;//new User(2, 3, new ZombiePet("Fred", 50), 2);
            this.random = new Random();
            this.level = level;
            do
            {
                this.blocks = new List<List<Block>>();
                for (int i = 0; i < rowsCount; i++)
                {
                    List<Block> row = new List<Block>();
                    for (int j = 0; j < columnsCount; j++)
                    {
                        row.Add(GenerateBlock(false, false, i, j));
                    }
                    this.blocks.Add(row);
                }

                if (this.level == 3 || this.level == 6 || this.level == 9)
                    this.GenerateBrain(true);
                else
                    this.GenerateBrain();
                if (this.level > 3)
                {
                    int k = 0;
                    for (int i = 0; i < blocks[0].Count; i++)
                    {
                        blocks[0][i].Cage = true;
                        if (this.level > 4 && k < blocks[0].Count)
                        {
                            blocks[1][k].Cage = true;
                            k += 2;
                        }
                    }
                }
                if (this.level > 1)
                    this.GenerateZombie();
                if (this.level > 4)
                    GenerateZombie();
                if (this.level > 9)
                    GenerateZombie();
            } while (this.CheckDelete().Count > 0 || !this.CheckBrainAndZombieAreNotNear());

            this.target = 2+level/3;//target;
            this.stepsCount = 20 + level;//steps;

            this.gold = 0;

            this.points = 0;
            this.currentBrainCount = 0;

            this.blockPoints = 10;
            this.zombiePoints = 50;
            this.brainPoints = 70;
            this.stepPoints = 10;
        }

        /// <summary>
        /// Генерация нового блока случайного типа, исключая зомби-блок.
        /// </summary>
        /// <param name="brain">Может сгенерироваться мозг или нет</param>
        /// <param name="rowIndex">Позиция блока в строке. По умолчанию равна 0</param>
        /// <param name="columnIndex">Позиция блока в столбце. По умолчанию равна 0</param>
        /// <returns>Блок с типом, случайно выбранным из перечисления BlockType, исключая Zombie</returns>
        private Block GenerateBlock(bool brain, bool brainIBank = false, int rowIndex = 0, int columnIndex = 0)
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
                return (b.Type == BlockType.Brain || b.Type == BlockType.BrainInBank || b.Type == BlockType.BrainInCrackedBank);
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
            List<Tuple<List<Block>, int>> delBlocks = this.CheckDelete(true);

            if (delBlocks.Count > 0)
            {
                this.DeleteBlocks(new List<Tuple<List<Block>, int>>(delBlocks));
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
            if (block1.Cage || block2.Cage)
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
                        this.blocks[b.Position.RowIndex][b.Position.ColumnIndex] = new Block(BlockType.BrainInCrackedBank, b.Position, b.Cage);
                    else if (b.Type == BlockType.BrainInCrackedBank)
                        this.blocks[b.Position.RowIndex][b.Position.ColumnIndex] = new Block(BlockType.Brain, b.Position, b.Cage);
                }
            }
        }

        private List<Tuple<List<Block>, int>> CheckDelete(bool nextDelete=false)
        {
            List<Tuple<List<Block>, int>> delBlocks = new List<Tuple<List<Block>, int>>();
            int rowsCount = this.blocks.Count;
            int columnsCount;
            bool hasRow = false, hasColumn = false, hasBlockOtherBrain = nextDelete;
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
                        hasBlockOtherBrain = true;
                    }
                    if (!hasRow && tmpRow.Count > 2)
                    {
                        this.CrashBank(tmpRow);
                        delBlocks.Add(new Tuple<List<Block>, int>(tmpRow, 1));
                        hasRow = true;
                        hasBlockOtherBrain = true;
                    }
                    if (i == 0 && this.blocks[i][j].Type == BlockType.Brain)
                    {
                        List<Block> brainForDelete = new List<Block>();
                        brainForDelete.Add(this.blocks[i][j]);
                        delBlocks.Add(new Tuple<List<Block>, int>(brainForDelete, 1));
                    }
                }
            }

            var zombieEated = ZombieEatBrain();
            if (zombieEated.Count > 0)
            {
                delBlocks.AddRange(zombieEated);
                hasBlockOtherBrain = true;
            }

            if (hasBlockOtherBrain)
                return delBlocks;
            else
                return new List<Tuple<List<Block>, int>>();
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

        //bool useWeapon = false;

        private void DeleteBlocks(List<Tuple<List<Block>, int>> blocksForDelete, bool deleteZombie = false)
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
                    {
                        if (block.Type == BlockType.Brain)
                            points += brainPoints;
                        else
                            points += blockPoints;
                    }

                    if (block.Type == BlockType.Brain && block.Position.RowIndex == 0)
                        currentBrainCount++;

                    if (this.blocks[block.Position.RowIndex][block.Position.ColumnIndex].Cage)
                    {
                        this.blocks[block.Position.RowIndex][block.Position.ColumnIndex] = new Block(this.blocks[block.Position.RowIndex][block.Position.ColumnIndex].Type, this.blocks[block.Position.RowIndex][block.Position.ColumnIndex].Position, false);
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
                                    {
                                        if (level == 3 || level == 5)
                                            newBlock = this.GenerateBlock(true, true);
                                        else
                                            newBlock = this.GenerateBlock(true);
                                    }
                                    else
                                    {
                                        if (level == 3 || level == 5)
                                            newBlock = new Block(BlockType.BrainInBank);
                                        else
                                            newBlock = new Block(BlockType.Brain);
                                    }
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
            //useWeapon = false;
            DeleteEventHandler handler = Delete;
            if (handler != null)
                handler(this, e);
        }

        private int HasOtherBrain(Block brain)
        {
            int brainCount = 0;
            foreach (List<Block> row in this.blocks)
            {
                List<Block> brains = row.FindAll(delegate (Block b)
                {
                    return (b.Type == BlockType.Brain || b.Type == BlockType.BrainInCrackedBank || b.Type == BlockType.BrainInBank);
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

        private List<Tuple<List<Block>, int>> ZombieEatBrain()
        {
            List<Tuple<List<Block>, int>> allDelBlocks = new List<Tuple<List<Block>, int>>();

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

                    if (row > 0 && this.blocks[row - 1][column].Type == BlockType.Brain)
                    {
                        List<Block> brain = new List<Block>();
                        brain.Add(new Block(this.blocks[row - 1][column]));
                        Tuple<List<Block>, int> delBlocks = new Tuple<List<Block>, int>(brain, 1);
                        allDelBlocks.Add(delBlocks);
                    }
                    if (column > 0 && this.blocks[row][column - 1].Type == BlockType.Brain)
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

                    if (row + 1 < blocks.Count && this.blocks[row + 1][column].Type == BlockType.Brain)
                    {
                        List<Block> brain = new List<Block>();
                        brain.Add(new Block(this.blocks[row + 1][column]));
                        Tuple<List<Block>, int> delBlocks = new Tuple<List<Block>, int>(brain, 1);
                        allDelBlocks.Add(delBlocks);
                    }
                }
            }
            return allDelBlocks;
        }

        public void UseWeapon(Weapon weapon, Block block)
        {
            //useWeapon = true;
            if (weapon is Soporific)
            {
                Soporific soporific = weapon as Soporific;
                this.blocks = new List<List<Block>>(soporific.GetAsleepZombieInBlocks(block, this.blocks));
                user.Weapon[0].Count--;
            }
            else
            {
                if (weapon is Bomb)
                    user.Weapon[1].Count--;
                else
                {
                    if (weapon is Gun)
                        user.Weapon[2].Count--;
                }
            }
            var delBlocks = weapon.Use(block, blocks.Count, blocks[0].Count);
            bool hasZombie = false;
            foreach (var oneSet in delBlocks)
            {
                Block z = oneSet.Item1.Find(delegate(Block tb)
                {
                    return blocks[tb.Position.RowIndex][tb.Position.ColumnIndex].Type == BlockType.Zombie;
                });
                if (z != null)
                    hasZombie = true;
            }
            DeleteBlocks(delBlocks, hasZombie);
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
            if (currentBrainCount >= target)
            {
                points += stepPoints * stepsCount;
                gold = points / 20;
                user.Money += gold;
                user.BrainsCount += this.BrainCount;
                user.LastLevel = this.Level;
                EndGameEventArgs e = new EndGameEventArgs(true, this.user);

                data.Write(this.user);
                EndGameEventHandler handler = EndGame;
                if (handler != null)
                    handler(this, e);
            }
            else
            {
                if (stepsCount == 0)
                {
                    if (this.user.LivesCount > 0)
                        this.user.LivesCount--;
                    EndGameEventArgs e = new EndGameEventArgs(false, this.user);
                    data.Write(this.user);
                    EndGameEventHandler handler = EndGame;
                    if (handler != null)
                        handler(this, e);
                }
            }
        }
    }
}