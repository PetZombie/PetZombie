//using System;
using System.Collections.Generic;
using CocosSharp;

namespace PetZombieUI
{
    public class ThreeInRowGameLayer : CCLayerColor
    {
        #region Fields

        private ThreeInRowGame game;

        // Margin fields.
        private const float marginPortion = 0.1f;
        private float freeSpace = Resolution.DesignResolution.Width * marginPortion;

        // Block greed fields.
        private CCNode blockGrid;
        private float blockGridWidth;
        private float blockGridMargin;

        // Block fields.
        private float blockWidth;
        private CCSize blockSize;

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
            blockGridWidth = Resolution.DesignResolution.Width - freeSpace;
            blockWidth = blockGridWidth / 6;
            blockSize = new CCSize(blockWidth, blockWidth);
            blockGridMargin = freeSpace / 2;

            game = new ThreeInRowGame(rowsCount, columnsCount, blockSize);

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
            if (!isCurrentTouchBlockMoved && currentTouchedBlock != null)
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
            if (currentTouchedBlock != null)
            {
                if (!isCurrentTouchBlockMoved)
                {
                    currentTouchedBlock.Sprite.RunAction(scaleDown.Reverse());
                }
                else
                    isCurrentTouchBlockMoved = false;

                currentTouchedBlock = null;
            }
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

            //Block block;

            foreach (var block in game.Blocks)
            {
                blockGrid.AddChild(block.Sprite);
                AddEventListener(listener.Copy(), block.Sprite);
            }

            AddChild(blockGrid);
        }

        public Block FindBlockAt(CCPoint point)
        {
            var foundBlock = game.Blocks.Find(
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

