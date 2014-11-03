using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class Operation
    {
        public Operation()
        {
        }

        private static bool HasOtherBrain(Block brain, List<List<Block>> blocks)
        {
            foreach (List<Block> row in blocks)
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

        public static List<List<Block>> DeleteBlock(List<Tuple<List<Block>,int>> blocksForDelete, List<List<Block>> blocks,
            ThreeInRowGame.BlockGenerator GenerateBlock, ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent)
        {
            List<Block> delBlocks = new List<Block>();
            List<Block> prevMovBlocks = new List<Block>();
            List<Block> movingBlocks = new List<Block>();

            bool firstly = true;

            foreach (Tuple<List<Block>,int> oneSet in blocksForDelete)
            {
                foreach (Block block in oneSet.Item1)
                {
                    delBlocks.Add(new Block(block));
                    if (block is ZombieBlock)
                        game.Points += game.ZombiePoints;
                    else
                        game.Points += game.BlockPoints;

                    Block newBlock;
                    if (block.Type == BlockType.Brain && !HasOtherBrain(block, blocks))
                        newBlock = new Block(BlockType.Brain);
                    else
                        newBlock = GenerateBlock(true);

                    if (oneSet.Item2 == 1 || firstly)
                    {
                        firstly = false;
                        int row = block.Position.RowIndex;
                        int column = block.Position.ColumnIndex;
                        while (row < blocks.Count)
                        {
                            if (blocks[row][column].Type == BlockType.Zombie)
                            {
                                row++;
                                continue;
                            }

                            int nextRow = row + oneSet.Item2;

                            if (nextRow < blocks.Count && blocks[nextRow][column].Type == BlockType.Zombie)
                                nextRow++;

                            if (nextRow < blocks.Count)
                            {
                                prevMovBlocks.Add(new Block(blocks[nextRow][column]));
                                blocks[row][column].Type = blocks[nextRow][column].Type;
                                movingBlocks.Add(new Block(blocks[row][column]));
                            }
                            else
                            {
                                blocks[row][column].Type = newBlock.Type;
                                prevMovBlocks.Add(new Block(new Position(blocks.Count, column)));
                                movingBlocks.Add(new Block(blocks[row][column]));
                            }
                            row++;
                        }
                    }
                }

            }

            BlocksDeletingEventArgs e = new BlocksDeletingEventArgs(delBlocks, prevMovBlocks, movingBlocks);

            DeleteEvent(game, e);

            return blocks;
        }
    }
}

