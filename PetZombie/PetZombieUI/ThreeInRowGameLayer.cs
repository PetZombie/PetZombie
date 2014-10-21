//using System;
using System.Collections.Generic;
using CocosSharp;

namespace PetZombieUI
{
    public class ThreeInRowGameLayer : CCLayerColor
    {
        #region Fields

        private PetZombie.ThreeInRowGame game;

        // Margin fields.
        private const float marginPortion = 0.1f;
        private float freeSpace = Resolution.DesignResolution.Width * marginPortion;

        // Block greed fields.
        private CCNode blockGrid;
        private float blockGridWidth;
        private float blockGridMargin;

        private List<Block> blocks;

        // Block fields.
        private float blockWidth;
        private CCSize blockSize;

        // 
        private int rowsCount;
        private int columnsCount;

        // Animation fields.
        private CCScaleBy scaleDown;

        // Current proceccing block fields.
        private Block currentTouchedBlock;
        private bool isCurrentTouchBlockMoved;

        // Touch fields.
        CCEventListenerTouchOneByOne listener;

        #endregion

        private ThreeInRowGameLayer(int rowsCount, int columnsCount) : base()
        {
            this.rowsCount = rowsCount;
            this.columnsCount = columnsCount;

            blockGridWidth = Resolution.DesignResolution.Width - freeSpace;
            blockWidth = blockGridWidth / 6;
            blockSize = new CCSize(blockWidth, blockWidth);
            blockGridMargin = freeSpace / 2;

            blocks = new List<Block>(rowsCount*columnsCount);

            game = new PetZombie.ThreeInRowGame(rowsCount, columnsCount);

            scaleDown = new CCScaleBy(0.1f, 0.9f);

            listener = new CCEventListenerTouchOneByOne();
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            listener.OnTouchMoved = OnTouchMoved;

            Color = CCColor3B.Gray;
            Opacity = 255;

            AddBackground();
            AddBlockGrid();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;
        }

        public static CCScene ThreeInRowGameLayerScene(CCWindow mainWindow)
        {
            var scene = new CCScene(mainWindow);
            var layer = new ThreeInRowGameLayer(9, 6);

            scene.AddChild(layer);

            return scene;
        }

        #region Touch handlers

        private bool OnTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            //var sprite = ccevent.CurrentTarget as CCSprite;
            //var location = touch.Location;
            //CCRect rect = new CCRect(sprite.PositionX, sprite.PositionY, sprite.ScaledContentSize.Width, sprite.ScaledContentSize.Height);

            currentTouchedBlock = FindBlockAt(touch.Location);

            /*var sprite = ccevent.CurrentTarget as CCSprite;
            var locationOnNode = sprite.ConvertToWorldspace(touch.Location);
            var rect = new CCRect(0, 0, blockSize.Width, blockSize.Height);*/

            if (currentTouchedBlock != null)
            {
                //var action = new CCScaleBy(0.1f, scale);

                PauseListeners(true);



                //if (sprite.ScaledContentSize == blockSize)
                currentTouchedBlock.Sprite.RunAction(scaleDown);

                ResumeListeners(true);

                return true;
            }

            return false;
        }

        private void OnTouchMoved(CCTouch touch, CCEvent ccevent)
        {
            if (!isCurrentTouchBlockMoved)
            {
                //var sprite = ccevent.CurrentTarget as CCSprite;
                var delta = new CCPoint(blockWidth, blockWidth);

                var action1 = new CCMoveBy(0.1f, delta);

                //var updatedBlocks = game.MoveBlocks(currentTouchedBlock, new Block("", new CCPoint(), new CCSize()));

                currentTouchedBlock.Sprite.RunAction(action1);
                currentTouchedBlock.Sprite.RunAction(scaleDown.Reverse());

                isCurrentTouchBlockMoved = true;
            }
        }

        private void OnTouchEnded(CCTouch touch, CCEvent ccevent)
        {
            if (!isCurrentTouchBlockMoved)
            {
                currentTouchedBlock.Sprite.RunAction(scaleDown.Reverse());
            }
            else isCurrentTouchBlockMoved = false;

            currentTouchedBlock = null;
        }

        #endregion
            
        private void AddBackground()
        {
            var background = new CCSprite();

            AddChild(background);
        }

        private void AddBlockGrid()
        {
            blockGrid = new CCNode();
            blockGrid.Position = new CCPoint(blockGridMargin, blockGridMargin);

            Block block;

            for (var i = 0; i < columnsCount; i++)
            {
                for (var j = 0; j < rowsCount; j++)
                {
                    var x = blockWidth * 0.5f + i * blockWidth;
                    var y = blockWidth * 0.5f + j * blockWidth;

                    block = CreateBlock(new CCPoint(x, y));
                    blocks.Add(block);
                    blockGrid.AddChild(block.Sprite);

                    AddEventListener(listener.Copy(), block.Sprite);
                }
            }

            AddChild(blockGrid);
        }

        private Block CreateBlock(CCPoint point)
        {
            var randomNumber = CCRandom.Next(0, 6);
            string fileName = "";

            switch (randomNumber)
            {
                case 0:
                    fileName = "Images/blue_ellipse_block";
                    break;
                case 1:
                    fileName = "Images/green_ellipse_block";
                    break;
                case 2:
                    fileName = "Images/orange_ellipse_block";
                    break;
                case 3:
                    fileName = "Images/red_ellipse_block";
                    break;
                case 4:
                    fileName = "Images/violet_ellipse_block";
                    break;
                case 5:
                    fileName = "Images/zombie_block";
                    break;
            }

            var block = new Block(fileName, point, blockSize);

            /*var block = new CCSprite(fileName);

            block.AnchorPoint = new CCPoint(0.5f, 0.5f);
            block.Position = point;
            block.ScaleTo(blockSize);*/

            return block;
        }

        public Block FindBlockAt(CCPoint point)
        {
            var foundBlock = blocks.Find(
                block => 
            {
                if (block.Rectangle.ContainsPoint(point))
                    return true;

                return false;
            }
            );

            return foundBlock;
        }
    }
}

