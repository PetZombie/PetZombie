﻿using System;
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

        private CCNode toolbar;

        // Block fields.
        private float blockWidth;
        private CCSize blockSize;

        // Current proceccing block fields.
        private Block currentTouchedBlock;
        private bool isCurrentTouchedBlockMoved;
        private bool isTouchEnded;

        private Block replacedBlock;
        private CCPoint previousPosition;

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
            game.Delete += OnDelete;

            resumeListeners = new CCCallFunc(() => ResumeListeners(true));

            listener = new CCEventListenerTouchOneByOne();
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            listener.OnTouchMoved = OnTouchMoved;

            AddBackground();
            AddBlockGrid();
            AddToolbar();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            background.Position = VisibleBoundsWorldspace.Center;
            toolbar.Position = new CCPoint(blockGridMargin, Resolution.DesignResolution.Height - blockGridMargin - blockSize.Height);

            //ApperBlockGrid();
        }

        public void ApperBlockGrid()
        {
            var moveTo1 = new CCMoveTo(1.0f, new CCPoint(blockGridMargin, blockGridMargin + blockSize.Height/2));
            var moveTo2 = new CCMoveTo(0.2f, new CCPoint(blockGridMargin, blockGridMargin));

            blockGrid.RunAction(new CCSequence(moveTo1, moveTo2));
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
                replacedBlock = game.GetReplacedBlock(currentTouchedBlock, position);

                if (replacedBlock != null)
                {
                    // Detecting whether toched block and replaced block are replaceable to each other.
                    if (game.AbilityToReplace(currentTouchedBlock, replacedBlock))
                    {
                        // For desire right replace action we set the replaced block's ZOrder up
                        // so that the touched block moves under that one.
                        replacedBlock.Sprite.ZOrder++;

                        // Remember the touched block's initial position for replace action purposes.
                        previousPosition = currentTouchedBlock.Sprite.Position;

                        var moveTo1 = new CCMoveTo(0.2f, replacedBlock.Sprite.Position);
                        var moveTo2 = new CCMoveTo(0.2f, previousPosition);

                        // Pause listeners to avoid any touches when actions is running.
                        PauseListeners(true);

                        var isThreeInRow = game.ReplaceBlocks(currentTouchedBlock, replacedBlock);

                        // If there's "Tree In Row!".
                        if (isThreeInRow)
                        {
                            //var removeBlocks = new CCCallFunc(() => RemoveBlocks(tuple.Item1));
                            //var moveBlocks = new CCCallFunc(() => MoveBlocks(tuple.Item2, tuple.Item3, tuple.Item4));
                            //var action = new CCSequence(moveTo2, removeBlocks, moveBlocks, resumeListeners);

                            //currentTouchedBlock.Sprite.RunAction(moveTo1);
                            //replacedBlock.Sprite.RunAction(action);

                            //game.UpdateBlocks();
                        }
                        else
                        {/*
                            var action1 = new CCSequence(moveTo1, moveTo2);
                            var action2 = new CCSequence(moveTo2, moveTo1, resumeListeners);

                            currentTouchedBlock.Sprite.RunAction(action1);
                            replacedBlock.Sprite.RunAction(action2);*/
                            ResumeListeners(true);
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
                var scaleUp = new CCScaleBy(0.1f, scale);

                currentTouchedBlock.Sprite.RunAction(scaleUp);

                // Some necessary resets.
                currentTouchedBlock = null;
                isCurrentTouchedBlockMoved = false;
                isTouchEnded = true;
            }
        }

        #endregion

        private void OnDelete(object sender, PetZombie.BlocksDeletingEventArgs args)
        {
            var moveTo1 = new CCMoveTo(0.2f, replacedBlock.Sprite.Position);
            var moveTo2 = new CCMoveTo(0.2f, previousPosition);

            var removeBlocks = new CCCallFunc(() => RemoveBlocks(args.DelBlocks));
            removeBlocks.Duration = 0.2f;
            var moveBlocks = new CCCallFunc(() => MoveBlocks(args.PrevMovBlocks, args.CurMovBlocks, args.NewBlocks, args.InitPositionsOfNewBlocks));
            moveBlocks.Duration = 0.3f;
            var updateBlockGrid = new CCCallFunc(() => UpdateBlockGrid());
            var action = new CCSequence(moveTo2, removeBlocks, moveBlocks, updateBlockGrid, resumeListeners);

            currentTouchedBlock.Sprite.RunAction(moveTo1);
            replacedBlock.Sprite.RunAction(action);

        }

        private void MoveBlocks(List<PetZombie.Block> prevMovingBlocks, 
            List<PetZombie.Block> currentMovingBlocks, 
            List<PetZombie.Block> newBlocks, List<PetZombie.Block> initPositionsOfNewBlocks)
        {
            CCMoveTo moveTo;

            for (var i = 0; i < prevMovingBlocks.Count; i++)
            {
                moveTo = new CCMoveTo(0.2f, Block.GetPosition(currentMovingBlocks[i], blockSize));


                var sprite = FindBlockSprite(prevMovingBlocks[i]);
                if (sprite != null)
                    sprite.RunAction(moveTo);
            }

            for (var i = 0; i < initPositionsOfNewBlocks.Count; i++)
            {
                var newBlock = new Block(initPositionsOfNewBlocks[i], blockSize);

                game.Blocks.Add(newBlock);
                blockGrid.AddChild(newBlock.Sprite);
                AddEventListener(listener.Copy(), newBlock.Sprite);

                moveTo = new CCMoveTo(0.2f, Block.GetPosition(initPositionsOfNewBlocks[i], blockSize));

                FindBlockSprite(initPositionsOfNewBlocks[i]).RunAction(moveTo);
            }
        }

        private void RemoveBlocks(List<PetZombie.Block> blocks)
        {
            foreach (var block in blocks)
            {
                blockGrid.RemoveChild(FindBlockSprite(block));
            }
        }

        private void UpdateBlockGrid()
        {
            game.UpdateBlocks();
            RemoveChild(blockGrid);
            AddBlockGrid();
        }

        private CCNode FindBlockSprite(PetZombie.Block block)
        {
            foreach (var sprite in blockGrid.Children)
            {
                if (sprite.Position == Block.GetPosition(block, blockSize))
                    return sprite;
            }

            return null;
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

        private void AddToolbar()
        {
            toolbar = new CCNode();

            var toolbarItems = new CCSprite[]
            { 
                new CCSprite("Images/star_icon"),
                new CCSprite("Images/trace"),
                new CCSprite("Images/brain_icon"),
                new CCSprite("Images/soporific_icon"),
                new CCSprite("Images/aim_icon"),
                new CCSprite("Images/bomb_icon")
            };

            for (var i = 0; i < toolbarItems.Length; i++)
            {
                //toolbarItems[i].ScaleTo(blockSize);
                toolbarItems[i].Position = new CCPoint(i*blockWidth, 0);
                toolbarItems[i].AnchorPoint = CCPoint.Zero;
                toolbar.AddChild(toolbarItems[i]);
            }

            AddChild(toolbar);
        }

        #endregion
    }
}

