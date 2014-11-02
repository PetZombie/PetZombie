using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class Operation
    {
        public Operation()
        {
        }

        public static List<List<Block>> DeleteBlock(List<Tuple<List<Block>,int>> blocksForDelete, List<List<Block>> blocks,
            ThreeInRowGame.BlockGenerator GenerateBlock, ThreeInRowGame game, ThreeInRowGame.DeleteEventHandler DeleteEvent)
        {
            List<Block> delBlocks = new List<Block>();
            List<Block> prevMovBlocks = new List<Block>();
            List<Block> movingBlocks = new List<Block>();
            List<Block> newBlocks = new List<Block>();
            List<Block> initPositionsOfNewBlocks = new List<Block>();

            bool firstly = true;

            foreach (Tuple<List<Block>,int> oneSet in blocksForDelete)
            {
                foreach (Block block in oneSet.Item1)
                {
                    delBlocks.Add(new Block(block));
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
                                prevMovBlocks.Add(new Block(blocks[row + oneSet.Item2][column]));
                                blocks[row][column].Type = blocks[nextRow][column].Type;
                                movingBlocks.Add(new Block(blocks[row][column]));
                            }
                            else
                            {
                                Block newBlock = GenerateBlock(true);
                                blocks[row][column].Type = newBlock.Type;
                                initPositionsOfNewBlocks.Add(new Block(new Position(blocks.Count, column)));
                                newBlocks.Add(new Block(blocks[row][column]));
                            }
                            row++;
                        }
                    }
                }

            }

            BlocksDeletingEventArgs e = new BlocksDeletingEventArgs(delBlocks, prevMovBlocks, movingBlocks, newBlocks, initPositionsOfNewBlocks);

            DeleteEvent(game, e);

            return blocks;
        }
    }
}

