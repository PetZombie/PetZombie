using System;
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

        // Current proceccing block fields.
        private Block currentTouchedBlock;

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

            listener = new CCEventListenerTouchOneByOne();
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            //listener.OnTouchMoved = OnTouchMoved;

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
            if (currentTouchedBlock == null)
            {
                currentTouchedBlock = game.FindBlockAt(touch.Location);

                if (currentTouchedBlock != null)
                {
                    var scaleDown = new CCScaleBy(0.1f, 0.8f);
                    currentTouchedBlock.Sprite.RunAction(scaleDown);

                    return true;
                }
            }

            return false;
        }

        private void OnTouchMoved(CCTouch touch, CCEvent ccevent)
        {
            if (currentTouchedBlock != null)
            {
                var moveToPosition = currentTouchedBlock.Sprite.Position + touch.Delta;
                var replacedBlock = game.GetReplacedBlock(currentTouchedBlock, moveToPosition);

                if (replacedBlock != null)
                {
                    var moveTo = new CCMoveTo(0.1f, replacedBlock.Sprite.Position);

                    currentTouchedBlock.Sprite.RunAction(moveTo);
                }

                /*var delta = new CCPoint(blockWidth, blockWidth);
                var action1 = new CCMoveBy(0.1f, delta);*/

                //currentTouchedBlock.Sprite.RunAction(action1);
                //currentTouchedBlock.Sprite.RunAction(scaleDown.Reverse());

            }
        }

        private void OnTouchEnded(CCTouch touch, CCEvent ccevent)
        {
            if (currentTouchedBlock != null)
            {
                    var scale = currentTouchedBlock.Size.Width / currentTouchedBlock.Sprite.ScaledContentSize.Width;
                    var scaleUp = new CCScaleBy(0.1f, scale);

                    currentTouchedBlock.Sprite.RunAction(scaleUp);

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


    }
}

