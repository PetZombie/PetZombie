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
        private bool isTouchEnded;

        // Func to resume listeners when calling from the code 
        // where listeners aer paused while actions are running.
        private CCCallFunc resumeListeners;

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

            resumeListeners = new CCCallFunc(() => ResumeListeners(true));

            listener = new CCEventListenerTouchOneByOne();
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            listener.OnTouchMoved = OnTouchMoved;

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
            // We need to be not able to handle any touches while handling particular one.
            if (currentTouchedBlock == null)
            {
                isTouchEnded = false;

                // Finding the block that appropriates to user touch loccation on screen.
                currentTouchedBlock = game.FindBlockAt(touch.Location);

                if (currentTouchedBlock != null)
                {
                    // The action that will be run on the touched block sprite.
                    var scaleDown = new CCScaleBy(0.1f, 0.8f);

                    currentTouchedBlock.Sprite.RunAction(scaleDown);

                    return true;
                }
            }

            return false;
        }

        private void OnTouchMoved(CCTouch touch, CCEvent ccevent)
        {
            // If no block touched there's no reason to handle the touch.
            // OnTouchMoved event is called always while display is being touched,
            // so we need to handle it once user has swiped display.
            // This is enough for our porpouse to having the direction user has swiped display.
            // isCurrentTouchedBlockMoved mark stricts that situation going on right way
            // for avoiding other touches' side effects.
            if (currentTouchedBlock != null && !isCurrentTouchedBlockMoved)
            {
                // See GetPriorityDirection method.
                var priorityDirection = GetPriorityDirection(currentTouchedBlock, touch.Delta);
                var position = currentTouchedBlock.Sprite.Position + priorityDirection;
                var replacedBlock = game.GetReplacedBlock(currentTouchedBlock, position);

                if (replacedBlock != null)
                {
                    // Detecting whether toched block and replaced block are replaceable to each other.
                    if (game.AbilityToReplace(currentTouchedBlock, replacedBlock))
                    {
                        // For desire right replace action we set the replaced block's ZOrder up
                        // so that the touched block moves under that one.
                        replacedBlock.Sprite.ZOrder++;

                        // Remember the touched block's initial position for replace action purposes.
                        var previousPosition = currentTouchedBlock.Sprite.Position;

                        var moveTo1 = new CCMoveTo(0.2f, replacedBlock.Sprite.Position);
                        var moveTo2 = new CCMoveTo(0.2f, previousPosition);

                        // Pause listeners to avoid any touches when actions is running.
                        PauseListeners(true);

                        var tuple = game.ReplaceBlocks(currentTouchedBlock, replacedBlock);

                        // If there's "Tree In Row!".
                        if (tuple != null)
                        {
                            var removeBlocks = new CCCallFunc(() => RemoveBlocks(tuple.Item1));
                            var moveBlocks = new CCCallFunc(() => MoveBlocks(tuple.Item2));
                            var action = new CCSequence(moveTo2, removeBlocks, moveBlocks, resumeListeners);

                            currentTouchedBlock.Sprite.RunAction(moveTo1);
                            replacedBlock.Sprite.RunAction(action);
                        }
                        else
                        {
                            var action1 = new CCSequence(moveTo1, moveTo2);
                            var action2 = new CCSequence(moveTo2, moveTo1, resumeListeners);

                            currentTouchedBlock.Sprite.RunAction(action1);
                            replacedBlock.Sprite.RunAction(action2);
                        }

                        // Return back to the previous value taking count of that
                        // the same block can be swipped several times.
                        replacedBlock.Sprite.ZOrder--;
                    }

                    isCurrentTouchedBlockMoved = true;

                    // OnTouchEnded can be not called if user ends their touch before animation 
                    // that pause listeners is complete and it's not resume them again.
                    // In this case we need to call the event handler manually
                    // to perform all things we want to complete.
                    if (!isTouchEnded)
                        OnTouchEnded(touch, ccevent);
                }
            }
        }

        private void OnTouchEnded(CCTouch touch, CCEvent ccevent)
        {
            // Like with OnTouchedMoved we don't need to handle the touch end
            // if ther's no block has been touched.
            if (currentTouchedBlock != null)
            {
                var scale = currentTouchedBlock.Size.Width / 
                    currentTouchedBlock.Sprite.ScaledContentSize.Width;
                var scaleUp = new CCScaleBy(0.2f, scale);

                currentTouchedBlock.Sprite.RunAction(scaleUp);

                // Some necessary resets.
                currentTouchedBlock = null;
                isCurrentTouchedBlockMoved = false;
                isTouchEnded = true;
            }
        }

        #endregion

        private void MoveBlocks(List<Block> blocks)
        {
            CCMoveTo moveTo;
            CCSequence action;

            PauseListeners(true);

            foreach (var block in blocks)
            {
                moveTo = new CCMoveTo(0.1f, new CCPoint(block.Sprite.Position.X, 
                    block.Sprite.Position.Y - block.Size.Height));
                action = new CCSequence(moveTo, resumeListeners);

                block.Sprite.RunAction(action);
            }
        }

        private void RemoveBlocks(List<Block> blocks)
        {
            foreach (var block in blocks)
            {
                blockGrid.RemoveChild(block.Sprite);
            }
        }

        private CCPoint GetPriorityDirection(Block block, CCPoint delta)
        {
            float directionValue;
            var absX = Math.Abs(delta.X);
            var absY = Math.Abs(delta.Y);

            if (absX > absY)
            {
                if (delta.X > 0)
                    directionValue = block.Size.Width + 1;
                else
                    directionValue = -block.Size.Width - 1;

                return new CCPoint(directionValue, 0);
            }
            else if (absY > absX)
            {
                if (delta.Y > 0)
                    directionValue = block.Size.Height + 1;
                else
                    directionValue = -block.Size.Height - 1;

                return new CCPoint(0, directionValue);
            }

            return new CCPoint();
        }

        #region Sprite elements adding
            
        private void AddBackground()
        {
            background = new CCSprite("Images/new background");

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

        #endregion
    }
}

