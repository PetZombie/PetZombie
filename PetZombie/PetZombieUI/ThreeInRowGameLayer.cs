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
        private float freeSpace;

        // Block greed fields.
        private CCNode blockGrid;
        private float blockGridWidth;
        private float blockGridMargin;

        // Block fields.
        private float blockWidth;
        private CCSize blockSize;

        // Current proceccing block fields.
        private Block currentTouchedBlock;
        private bool isCurrentTouchedBlockMoved;
        private bool isReplacedTouchBlockMoved;
        private CCPoint previousPosition;

        // Touch fields.
        CCEventListenerTouchOneByOne listener;

        private CCSprite background;

        #endregion

        private ThreeInRowGameLayer(int rowsCount, int columnsCount) : base()
        {
            freeSpace = Resolution.DesignResolution.Width * marginPortion;
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

            background.Position = VisibleBoundsWorldspace.Center;
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
            if (currentTouchedBlock != null && !isReplacedTouchBlockMoved)
            {
                var priorityDirection = GetPriorityDirection(currentTouchedBlock, touch.Delta);
                var replacedBlock = game.GetReplacedBlock(currentTouchedBlock, 
                    currentTouchedBlock.Sprite.Position + priorityDirection);

                if (replacedBlock != null)
                {


                    if (!isCurrentTouchedBlockMoved)
                    {
                        previousPosition = currentTouchedBlock.Sprite.Position;
                        var moveTo1 = new CCMoveTo(0.1f, replacedBlock.Sprite.Position);
                        currentTouchedBlock.Sprite.RunAction(moveTo1);
                        isCurrentTouchedBlockMoved = true;
                    }
                    else
                    {
                        var moveTo2 = new CCMoveTo(0.1f, previousPosition);
                        replacedBlock.Sprite.RunAction(moveTo2);
                        isReplacedTouchBlockMoved = true;
                    }
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
                    var scale = currentTouchedBlock.Size.Width / 
                        currentTouchedBlock.Sprite.ScaledContentSize.Width;
                    var scaleUp = new CCScaleBy(0.1f, scale);

                    currentTouchedBlock.Sprite.RunAction(scaleUp);

                currentTouchedBlock = null;
                isCurrentTouchedBlockMoved = false;
                isReplacedTouchBlockMoved = false;
            }
        }

        #endregion
        /*
        private CCPoint GetPriorityDirection(Block block, CCPoint delta)
        {
            float additionValue;
            var absX = Math.Abs(delta.X);
            var absY = Math.Abs(delta.Y);

            if (absX > absY)
            {
                if (delta.X > 0)
                    additionValue = block.Size.Width;
                else
                    additionValue = -block.Size.Width;

                return new CCPoint(delta.X + additionValue, 0);
            }
            else if (absY > absX)
            {
                if (delta.Y > 0)
                    additionValue = block.Size.Height;
                else
                    additionValue = -block.Size.Height;

                return new CCPoint(0, delta.Y + additionValue);
            }

            return new CCPoint();
        }*/
            
        private void AddBackground()
        {
            background = new CCSprite("Images/background blur");

            var scaleX = background.ContentSize.Width / Resolution.DesignResolution.Width;
            var scaleY = background.ContentSize.Height / Resolution.DesignResolution.Height;

            var scaleValue = Math.Min(scaleX, scaleY);

            var screenWidth = background.ContentSize.Width / scaleValue;
            var screenHeight = background.ContentSize.Height / scaleValue;

            background.ScaleTo(new CCSize(screenWidth, screenHeight));

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

