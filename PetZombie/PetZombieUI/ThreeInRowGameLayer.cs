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

        private CCNode toolbar;
        private List<Weapon> weapons;
        private Weapon currentTouchedWeapon;

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
        //private CCCallFunc resumeListeners;

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

            //resumeListeners = new CCCallFunc(() => ResumeListeners(true));

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
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
                else
                {
                    foreach (var weapon in weapons)
                    {
                        if (weapon.WorldRectangle.ContainsPoint(touch.Location))
                        {
                            var weaponSprite = new CCSprite(weapon.Sprite.Texture);
                            weaponSprite.ScaleTo(weapon.Sprite.ScaledContentSize);
                            weaponSprite.Position = weapon.Sprite.Position;

                            toolbar.AddChild(weaponSprite);
                            weapon.Sprite.ZOrder++;
                            currentTouchedWeapon = weapon;

                            return true;
                        }
                    }
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
                        if (!isThreeInRow)
                        {
                            //var removeBlocks = new CCCallFunc(() => RemoveBlocks(tuple.Item1));
                            //var moveBlocks = new CCCallFunc(() => MoveBlocks(tuple.Item2, tuple.Item3, tuple.Item4));
                            //var action = new CCSequence(moveTo2, removeBlocks, moveBlocks, resumeListeners);

                            //currentTouchedBlock.Sprite.RunAction(moveTo1);
                            //replacedBlock.Sprite.RunAction(action);

                            //game.UpdateBlocks();

                            var action1 = new CCSequence(moveTo1, moveTo2);
                            var action2 = new CCSequence(moveTo2, moveTo1, new CCCallFunc(() => ResumeListeners(true)));

                            currentTouchedBlock.Sprite.RunAction(action1);
                            replacedBlock.Sprite.RunAction(action2);
                        }
                        else
                        {
                            //game.NextDelete();
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
            else if (currentTouchedWeapon != null)
            {
                currentTouchedWeapon.Sprite.Position += touch.Delta;
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
            var moveTo2 = new CCMoveTo(0.3f, previousPosition);

            var removeBlocks = new CCCallFunc(() => RemoveBlocks(args.DelBlocks));
            removeBlocks.Duration = 0.2f;
            var moveBlocks = new CCCallFunc(() => MoveBlocks(args.PrevMovBlocks, args.CurMovBlocks));
            moveBlocks.Duration = 0.5f;
            var updateBlockGrid = new CCCallFunc(() => UpdateBlockGrid());
            updateBlockGrid.Duration = 0.1f;
            var nextDelete = new CCCallFunc(() => MyNextDelete());
            nextDelete.Duration = 0.1f;
            var action = new CCSequence(moveTo2, removeBlocks, moveBlocks, updateBlockGrid, nextDelete);

            currentTouchedBlock.Sprite.RunAction(moveTo1);
            replacedBlock.Sprite.RunAction(action);
        }

        private void MyNextDelete()
        {
            game.NextDelete();
        }

        private void MoveBlocks(List<PetZombie.Block> prevMovingBlocks, 
            List<PetZombie.Block> currentMovingBlocks)
        {
            CCMoveTo moveTo;
            CCMoveTo moveto2;

            for (var i = 0; i < prevMovingBlocks.Count; i++)
            {
                var sprite = FindBlockSprite(prevMovingBlocks[i]);

                if (sprite != null)
                {
                    blockGrid.RemoveChild(sprite);
                    game.RemoveBlock(prevMovingBlocks[i]);
                }
            }

            for (var i = 0; i < prevMovingBlocks.Count; i++)
            {
                var position = Block.GetPosition(currentMovingBlocks[i], blockSize);

                moveTo = new CCMoveTo(0.2f, position);
                moveto2 = new CCMoveTo(0.1f, new CCPoint(position.X, position.Y + blockSize.Height/4));
                var action = new CCSequence(moveTo, moveto2, moveTo);

                var block = new Block(prevMovingBlocks[i], blockSize);

                blockGrid.AddChild(block.Sprite);
                //game.AddBlock(currentMovingBlocks[i]);
                //AddEventListener(listener.Copy(), block.Sprite);

                block.Sprite.RunAction(action);
            }
        }

        private void RemoveBlocks(List<PetZombie.Block> blocks)
        {
            var scaleBy = new CCScaleBy(0.15f, 0.5f);

            foreach (var block in blocks)
            {
                var sprite = FindBlockSprite(block);
                var remove = new CCCallFunc(() =>
                {
                    blockGrid.RemoveChild(sprite);
                    //game.RemoveBlock(block);
                });
                sprite.RunAction(new CCSequence(scaleBy, remove));
            }
        }

        private void NextDelete()
        {
            game.NextDelete();
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

            foreach (var block in game.Blocks)
            {
                blockGrid.AddChild(block.Sprite);
            }
                
            AddChild(blockGrid);
        }

        private void AddToolbar()
        {
            var soporific = new Soporific();
            var gun = new Gun();
            var bomb = new Bomb();

            weapons = new List<Weapon>();

            weapons.Add(soporific);
            weapons.Add(gun);
            weapons.Add(bomb);

            toolbar = new CCNode();

            var toolbarItems = new CCSprite[]
            { 
                new CCSprite("Images/star_bar"),
                new CCSprite("Images/trace_bar"),
                new CCSprite("Images/brain_bar"),
                soporific.Sprite,
                gun.Sprite,
                bomb.Sprite
                /*new CCSprite("Images/soporific_bar"),
                new CCSprite("Images/aim_bar"),
                new CCSprite("Images/bomb_bar")*/
            };

            for (var i = 0; i < toolbarItems.Length; i++)
            {
                toolbarItems[i].ScaleTo(blockSize);
                toolbarItems[i].Position = new CCPoint(i*blockWidth + blockSize.Width/2, blockSize.Height/2);
                //toolbarItems[i].AnchorPoint = CCPoint.Zero;

                toolbar.AddChild(toolbarItems[i]);
            }

            AddChild(toolbar);
        }

        #endregion
    }
}

