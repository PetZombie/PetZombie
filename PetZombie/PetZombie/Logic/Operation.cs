using System;
using System.Collections.Generic;

namespace PetZombie
{
    public class Operation
    {
        public Operation()
        {
        }

        public static List<List<Block>> DeleteBlock(Block block, List<List<Block>> blocks, Delegate GenerateBlock)
        {
            int row = block.Position.RowIndex;
            int column = block.Position.ColumnIndex;
            int increment = 1;
            while (row < blocks.Count)
            {
                if (blocks[row][column].Type == BlockType.Zombie)
                {
                    row++;
                    increment--;
                    continue;
                }
                int nextRow = row + increment;
                if (blocks[nextRow][column].Type == BlockType.Zombie)
                {
                    increment++;
                    continue;
                }

                if (nextRow < blocks.Count)
                    blocks[row][column].Type = blocks[nextRow][column].Type;
                else
                {
                    //Block newBlock = GenerateBlock(true);
                    //blocks[row][column].Type = newBlock.Type;
                }
                row++;
            }
            return blocks;
        }
    }
}

