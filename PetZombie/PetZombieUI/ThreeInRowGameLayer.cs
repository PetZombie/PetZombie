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
        private CCEventListenerKeyboard keyboard;

        private CCSprite background;

        private CCLayerColor darkBackgroundLayer;
        private CCSprite popUpWindow;

        private bool isGameEnded;

        private CCSize popUpWindowSize;

        private CCSprite retryButton;
        private CCSprite backButton;
        private CCSprite nextButton;
        private CCSprite resumeButton;

        PetZombie.User user;
        int level;
        int rowsCount, columnsCount;

        CCLabel pointsLabel, stepsLabel, brainsLabel, soporificLabel, gunLabel, bombLabel;

        CCSprite tempWeapon;
        Weapon currentWeapon;
        bool touched;

        bool isPaused = false;
        #endregion

        private ThreeInRowGameLayer(int rowsCount, int columnsCount, int level, PetZombie.User user) : base()
        {
            this.rowsCount = rowsCount;
            this.columnsCount = columnsCount;
            this.level = level;
            this.user = user;
            //if (this.user == null)
            //  this.user = new PetZombie.User(3, 2, new PetZombie.ZombiePet("Brad"), 100);
            freeSpace = Resolution.DesignResolution.Width * marginPortion;
            blockGridWidth = Resolution.DesignResolution.Width - freeSpace;
            blockWidth = blockGridWidth / 6;
            blockSize = new CCSize(blockWidth, blockWidth);
            blockGridMargin = freeSpace / 2;

            game = new ThreeInRowGame(rowsCount, columnsCount, blockSize, level, user);
            game.Delete += OnDelete;
            game.EndGame += OnEndGame;

            //resumeListeners = new CCCallFunc(() => ResumeListeners(true));

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            listener.OnTouchMoved = OnTouchMoved;

            keyboard = new CCEventListenerKeyboard();
            AddEventListener(keyboard);
            //keyboard.OnKeyPressed = OnKeyPressed;

            darkBackgroundLayer = new CCLayerColor();
            popUpWindow = new CCSprite("Images/window_background");
            //popUpWindow.AddChild();
            popUpWindowSize = popUpWindow.ScaledContentSize;

            isGameEnded = false;

            AddBackground();
            AddBlockGrid();
            AddToolbar();
            AddLabels();
        }

        private void AddLabels()
        {
            pointsLabel = new CCLabel(game.Points.ToString(), "arial", 50);
            pointsLabel.Color = new CCColor3B(0, 0, 0);
            pointsLabel.Position = new CCPoint(90, Resolution.DesignResolution.Height - 180);
            AddChild(pointsLabel);

            stepsLabel = new CCLabel(game.StepsCount.ToString(), "arial", 50);
            stepsLabel.Color = new CCColor3B(0, 0, 0);
            stepsLabel.Position = new CCPoint(200, Resolution.DesignResolution.Height - 180);
            AddChild(stepsLabel);

            brainsLabel = new CCLabel(game.BrainCount.ToString()+"/"+game.target, "arial", 50);
            brainsLabel.Color = new CCColor3B(0, 0, 0);
            brainsLabel.Position = new CCPoint(310, Resolution.DesignResolution.Height - 180);
            AddChild(brainsLabel);

            soporificLabel = new CCLabel(game.user.Weapon[0].Count.ToString(), "arial", 50);
            soporificLabel.Color = new CCColor3B(0, 0, 0);
            soporificLabel.Position = new CCPoint(420, Resolution.DesignResolution.Height - 180);
            AddChild(soporificLabel);

            gunLabel = new CCLabel(game.user.Weapon[2].Count.ToString(), "arial", 50);
            gunLabel.Color = new CCColor3B(0, 0, 0);
            gunLabel.Position = new CCPoint(530, Resolution.DesignResolution.Height - 180);
            AddChild(gunLabel);

            bombLabel = new CCLabel(game.user.Weapon[1].Count.ToString(), "arial", 50);
            bombLabel.Color = new CCColor3B(0, 0, 0);
            bombLabel.Position = new CCPoint(630, Resolution.DesignResolution.Height - 180);
            AddChild(bombLabel);
        }

        private void UpdateLabels()
        {
            pointsLabel.Text = game.Points.ToString();
            stepsLabel.Text = game.StepsCount.ToString();
            brainsLabel.Text = game.BrainCount.ToString() + "/" + game.target;

            soporificLabel.Text = game.user.Weapon[0].Count.ToString();
            bombLabel.Text = game.user.Weapon[1].Count.ToString();
            gunLabel.Text = game.user.Weapon[2].Count.ToString();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            background.Position = VisibleBoundsWorldspace.Center;
            toolbar.Position = new CCPoint(blockGridMargin, Resolution.DesignResolution.Height - blockGridMargin - blockSize.Height);
            popUpWindow.Position = VisibleBoundsWorldspace.Center;

            var width = Resolution.DesignResolution.Width * 0.7f;
            var scale = popUpWindow.ContentSize.Width / width;
            var height = popUpWindow.ContentSize.Height / scale;

            darkBackgroundLayer.Color = CCColor3B.Black;
            darkBackgroundLayer.Opacity = 100;
            popUpWindow.ScaleTo(new CCSize(width, height));
            popUpWindow.Opacity = 200;
        }

        public static CCScene ThreeInRowGameLayerScene(CCWindow mainWindow,int level, PetZombie.User user)
        {
            var scene = new CCScene(mainWindow);
            var layer = new ThreeInRowGameLayer(9, 6,level, user);

            scene.AddChild(layer);

            return scene;
        }

        private int DetermWeaponCount(Weapon w)
        {
            foreach (PetZombie.Weapon uw in game.user.Weapon)
            {
                if (uw is PetZombie.Soporific && w is PetZombie.Soporific)
                    return uw.Count;
                if (uw is PetZombie.Bomb && w is PetZombie.Bomb)
                    return uw.Count;
                if (uw is PetZombie.Gun && w is PetZombie.Gun)
                    return uw.Count;
            }
            return 0;
        }

        #region Touch handlers

        /*private void OnKeyPressed(CCEventKeyboard keyboard)
        {
            if (!isPaused && keyboard.Keys == CCKeys.Back)
            {
                AddChild(darkBackgroundLayer);
                AddChild(popUpWindow);

                var marginFactor = 0.1f;

                backButton = new CCSprite("Images/back2");
                retryButton = new CCSprite("Images/retry2");
                resumeButton = new CCSprite("Images/continue2");

                var stringBackground1 = new CCSprite("Images/string_background");
                var stringBackground2 = new CCSprite("Images/string_background");
                var stringBackground3 = new CCSprite("Images/string_background");

                var brain = new CCSprite("Images/brain_small");
                var star = new CCSprite("Images/star_small");
                var money = new CCSprite("Images/money_small");

                brain.Position = new CCPoint(popUpWindowSize.Width * marginFactor*1.5f, brain.ScaledContentSize.Height * 0.5f);
                star.Position = new CCPoint(popUpWindowSize.Width*marginFactor*1.5f, star.ScaledContentSize.Height * 0.5f);
                money.Position = new CCPoint(popUpWindowSize.Width*marginFactor*1.5f, money.ScaledContentSize.Height * 0.5f);

                stringBackground3.AddChild(brain);
                stringBackground2.AddChild(star);
                stringBackground1.AddChild(money);

                var stringBackgroundSize = stringBackground1.ScaledContentSize;

                backButton.Position = new CCPoint(popUpWindowSize.Width*marginFactor + backButton.ScaledContentSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor + retryButton.ScaledContentSize.Width*0.5f);
                retryButton.Position = new CCPoint(popUpWindowSize.Width*0.5f,
                    popUpWindowSize.Width*marginFactor + retryButton.ScaledContentSize.Width*0.5f);
                resumeButton.Position = new CCPoint(popUpWindowSize.Width - 
                    popUpWindowSize.Width*marginFactor - resumeButton.ScaledContentSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor + resumeButton.ScaledContentSize.Width*0.5f);

                stringBackground1.Position = new CCPoint(popUpWindowSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor*2 + backButton.ScaledContentSize.Height +
                    stringBackground1.ScaledContentSize.Height*0.5f);

                stringBackground2.Position = new CCPoint(popUpWindowSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor*3 + backButton.ScaledContentSize.Height + 
                    stringBackground1.ScaledContentSize.Height);

                stringBackground3.Position = new CCPoint(popUpWindowSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor*4 + backButton.ScaledContentSize.Height + 
                    stringBackground1.ScaledContentSize.Height*1.5f);

                popUpWindow.AddChild(stringBackground1);
                popUpWindow.AddChild(stringBackground2);
                popUpWindow.AddChild(stringBackground3);

                popUpWindow.AddChild(resumeButton);
                popUpWindow.AddChild(backButton);
                popUpWindow.AddChild(retryButton);

                CCLabel messageLabel3 = new CCLabel("Y o u   c o l l e c t e  d :", "arial", 100);
                messageLabel3.Position = new CCPoint(popUpWindowSize.Width * 0.62f, popUpWindowSize.Height * 0.68f);// - 2*star.ScaledContentSize.Height);
                popUpWindow.AddChild(messageLabel3);

                string p = "";
                foreach(char c in game.Points.ToString())
                    p+=c + "  ";
                CCLabel pointsEndLabel = new CCLabel(p, "arial", 100);
                pointsEndLabel.Position = new CCPoint(popUpWindowSize.Width*0.73f, popUpWindowSize.Height*0.305f);
                popUpWindow.AddChild(pointsEndLabel);

                CCLabel brainsEndLabel = new CCLabel(game.BrainCount.ToString(), "arial", 100);
                brainsEndLabel.Position = new CCPoint(popUpWindowSize.Width*0.73f, popUpWindowSize.Height*0.43f);
                popUpWindow.AddChild(brainsEndLabel);

                string m = "";
                foreach(char c in game.Gold.ToString())
                    m += c + "  ";
                CCLabel moneyEndLabel = new CCLabel(m, "arial", 100);
                moneyEndLabel.Position = new CCPoint(popUpWindowSize.Width*0.73f, popUpWindowSize.Height*0.17f);
                popUpWindow.AddChild(moneyEndLabel);

                isPaused = true;
            }
            else
            {
                RemoveChild(darkBackgroundLayer);
                RemoveChild(popUpWindow);
                isPaused = false;
            }
        }
*/

        private bool OnTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            if (isPaused && !isGameEnded)
            {
                if (GetWorldRectangle(retryButton).ContainsPoint(touch.Location))
                {
                    Director.ReplaceScene(ThreeInRowGameLayerScene(Window, game.Level, game.user));
                }
                else if (GetWorldRectangle(backButton).ContainsPoint(touch.Location))
                {
                    Director.ReplaceScene(LevelsLayer.LevelsLayerScene(Window, game.user));
                }
                else if (GetWorldRectangle(resumeButton).ContainsPoint(touch.Location))
                {
                    RemoveChild(darkBackgroundLayer);
                    RemoveChild(popUpWindow);
                    isPaused = false;
                }
                isPaused = false;
                return true;
            }


            if (isGameEnded)
            {
                if (GetWorldRectangle(retryButton).ContainsPoint(touch.Location))
                {
                    Director.ReplaceScene(ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window, game.Level, game.user));
                }
                else
                {
                    if (GetWorldRectangle(backButton).ContainsPoint(touch.Location))
                        Director.ReplaceScene(LevelsLayer.LevelsLayerScene(Window, game.user));
                    else if (GetWorldRectangle(nextButton).ContainsPoint(touch.Location))
                    {
                        Director.ReplaceScene(ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window, game.Level+1, game.user));
                        // RemoveChild(darkBackgroundLayer);
                        //RemoveChild(popUpWindow);
                        //game = new ThreeInRowGame(rowsCount, columnsCount, blockSize, level+1, user);
                        //UpdateBlockGrid();
                    }
                }
            }

            // We need to be not able to handle any touches while handling particular one.
            if (currentTouchedBlock == null && !isGameEnded)
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
                            if (DetermWeaponCount(weapon) > 0)
                            {
                                tempWeapon = new CCSprite(weapon.Sprite.Texture);
                                tempWeapon.ScaleTo(weapon.Sprite.ScaledContentSize);
                                tempWeapon.Position = weapon.Sprite.Position + toolbar.Position;
                                weapon.Sprite.ZOrder++;

                                AddChild(tempWeapon);
                                currentWeapon = weapon;
                                touched = true;

                                /*

                                var weaponSprite = new CCSprite(weapon.Sprite.Texture);
                                weaponSprite.ScaleTo(weapon.Sprite.ScaledContentSize);
                                weaponSprite.Position = weapon.Sprite.Position;

                                toolbar.AddChild(weaponSprite);
                                weapon.Sprite.ZOrder++;
                                currentTouchedWeapon = weapon;
                                */
                            }

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
            else if (touched)
            {
                tempWeapon.Position += touch.Delta;
                //currentTouchedWeapon.Sprite.Position += touch.Delta;
            }
            UpdateLabels();
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
            else if (touched)
            {
                touched = false;
                var block = game.FindBlockAt(tempWeapon.PositionWorldspace);

                if (block != null)
                {
                    game.UseWeapon(currentWeapon, block);
                }
                RemoveChild(tempWeapon);
                currentWeapon = null;
            }
            UpdateLabels();
        }

        #endregion

        private bool _moveTo1Completed;
        private bool _moveTo2Completed;

        private void OnDelete(object sender, PetZombie.BlocksDeletingEventArgs args)
        {
            if (currentTouchedBlock != null)
            {
                _moveTo1Completed = false;
                _moveTo2Completed = false;

                var moveTo1 = new CCMoveTo(0.2f, replacedBlock.Sprite.Position);
                var moveTo2 = new CCMoveTo(0.2f, previousPosition);

                currentTouchedBlock.Sprite.RunAction(new CCSequence(moveTo1, new CCCallFunc(() => OnMove1Completed(args))));
                replacedBlock.Sprite.RunAction(new CCSequence(moveTo2, new CCCallFunc(() => OnMove2Completed(args))));

                /*var removeBlocks = new CCCallFunc(() => RemoveBlocks(args.DelBlocks));
                removeBlocks.Duration = 0.2f;
                var moveBlocks = new CCCallFunc(() => MoveBlocks(args.PrevMovBlocks, args.CurMovBlocks));
                moveBlocks.Duration = 0.5f;
                var updateBlockGrid = new CCCallFunc(() => UpdateBlockGrid());
                updateBlockGrid.Duration = 0.1f;
                var nextDelete = new CCCallFunc(() => MyNextDelete());
                nextDelete.Duration = 0.1f;
                var action = new CCSequence(moveTo2, removeBlocks, moveBlocks, updateBlockGrid, nextDelete);

                currentTouchedBlock.Sprite.RunAction(moveTo1);
                replacedBlock.Sprite.RunAction(action);*/
            }
            else
            {
                _moveTo1Completed = false;
                _moveTo2Completed = false;

                //var moveTo1 = new CCMoveTo(0.2f, replacedBlock.Sprite.Position);
                //var moveTo2 = new CCMoveTo(0.2f, previousPosition);

                RunAction(new CCSequence(new CCCallFunc(() => OnMove1Completed(args))));
                RunAction(new CCSequence(new CCCallFunc(() => OnMove2Completed(args))));

                /*var removeBlocks = new CCCallFunc(() => RemoveBlocks(args.DelBlocks));
                removeBlocks.Duration = 0.2f;
                var moveBlocks = new CCCallFunc(() => MoveBlocks(args.PrevMovBlocks, args.CurMovBlocks));
                moveBlocks.Duration = 0.5f;
                var updateBlockGrid = new CCCallFunc(() => UpdateBlockGrid());
                updateBlockGrid.Duration = 0.1f;
                var nextDelete = new CCCallFunc(() => MyNextDelete());
                nextDelete.Duration = 0.1f;
                var action = new CCSequence(removeBlocks, moveBlocks, updateBlockGrid, nextDelete);

                RunAction(action);*/
            }
            UpdateLabels();
        }

        private void OnEndGame(object sender, PetZombie.EndGameEventArgs args)
        {
            RemoveChild(darkBackgroundLayer);
            RemoveChild(popUpWindow);

            this.user = game.user;
            UpdateLabels();
            var marginFactor = 0.1f;

            AddChild(darkBackgroundLayer);
            AddChild(popUpWindow);

            isGameEnded = true;

            var stringBackground1 = new CCSprite("Images/string_background");
            var stringBackground2 = new CCSprite("Images/string_background");
            var stringBackground3 = new CCSprite("Images/string_background");

            backButton = new CCSprite("Images/back2");
            retryButton = new CCSprite("Images/retry2");
            nextButton = new CCSprite("Images/next2");

            var brain = new CCSprite("Images/brain_small");
            var star = new CCSprite("Images/star_small");
            var money = new CCSprite("Images/money_small");

            //backButton.ScaleTo(blockSize);
            //retryButton.ScaleTo(blockSize);

            brain.Position = new CCPoint(popUpWindowSize.Width * marginFactor*1.5f, brain.ScaledContentSize.Height * 0.5f);
            star.Position = new CCPoint(popUpWindowSize.Width*marginFactor*1.5f, star.ScaledContentSize.Height * 0.5f);
            money.Position = new CCPoint(popUpWindowSize.Width*marginFactor*1.5f, money.ScaledContentSize.Height * 0.5f);

            stringBackground3.AddChild(brain);
            stringBackground2.AddChild(star);
            stringBackground1.AddChild(money);

            backButton.Position = new CCPoint(popUpWindowSize.Width*marginFactor + backButton.ScaledContentSize.Width*0.5f, 
                popUpWindowSize.Width*marginFactor + retryButton.ScaledContentSize.Width*0.5f);

            var stringBackgroundSize = stringBackground1.ScaledContentSize;

            //args.win = true;

            if (args.win)
            {
                CCLabel messageLabel = new CCLabel("L e v e l   i s", "arial", 100);
                messageLabel.Position = new CCPoint(popUpWindowSize.Width/2, popUpWindowSize.Height*0.75f);
                popUpWindow.AddChild(messageLabel);

                CCLabel messageLabel2 = new CCLabel("c o  m  p l e t e d", "arial", 100);
                messageLabel2.Position = new CCPoint(popUpWindowSize.Width*0.55f, popUpWindowSize.Height*0.75f - star.ScaledContentSize.Height*0.7f);
                popUpWindow.AddChild(messageLabel2);

                CCLabel messageLabel3 = new CCLabel("Y o u   c o l l e c t e  d :", "arial", 100);
                messageLabel3.Position = new CCPoint(popUpWindowSize.Width*0.62f, popUpWindowSize.Height*0.75f - 2*star.ScaledContentSize.Height*0.7f);
                popUpWindow.AddChild(messageLabel3);

                                retryButton.Position = new CCPoint(popUpWindowSize.Width*0.5f,
                    popUpWindowSize.Width*marginFactor + retryButton.ScaledContentSize.Width*0.5f);
                nextButton.Position = new CCPoint(popUpWindowSize.Width - 
                    popUpWindowSize.Width*marginFactor - nextButton.ScaledContentSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor + nextButton.ScaledContentSize.Width*0.5f);

                stringBackground1.Position = new CCPoint(popUpWindowSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor*2 + backButton.ScaledContentSize.Height +
                    stringBackground1.ScaledContentSize.Height*0.5f);

                stringBackground2.Position = new CCPoint(popUpWindowSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor*3 + backButton.ScaledContentSize.Height + 
                    stringBackground1.ScaledContentSize.Height);

                stringBackground3.Position = new CCPoint(popUpWindowSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor*4 + backButton.ScaledContentSize.Height + 
                    stringBackground1.ScaledContentSize.Height*1.5f);

                popUpWindow.AddChild(stringBackground1);
                popUpWindow.AddChild(stringBackground2);
                popUpWindow.AddChild(stringBackground3);

                string p = "";
                foreach(char c in game.Points.ToString())
                    p+=c + "  ";
                CCLabel pointsEndLabel = new CCLabel(p, "arial", 100);
                pointsEndLabel.Position = new CCPoint(popUpWindowSize.Width*0.73f, popUpWindowSize.Height*0.305f);
                popUpWindow.AddChild(pointsEndLabel);

                CCLabel brainsEndLabel = new CCLabel(game.BrainCount.ToString(), "arial", 100);
                brainsEndLabel.Position = new CCPoint(popUpWindowSize.Width*0.73f, popUpWindowSize.Height*0.43f);
                popUpWindow.AddChild(brainsEndLabel);

                string m = "";
                foreach(char c in game.Gold.ToString())
                    m += c + "  ";
                CCLabel moneyEndLabel = new CCLabel(m, "arial", 100);
                moneyEndLabel.Position = new CCPoint(popUpWindowSize.Width*0.73f, popUpWindowSize.Height*0.17f);
                popUpWindow.AddChild(moneyEndLabel);

                popUpWindow.AddChild(nextButton);
            }
            else
            {
                CCLabel message1Label = new CCLabel("L e v e l   i s n ' t", "arial", 150);
                message1Label.Position = new CCPoint(popUpWindowSize.Width*0.6f, popUpWindowSize.Height*0.52f);
                popUpWindow.AddChild(message1Label);

                CCLabel message2Label = new CCLabel("c o  m  p l e t e d", "arial", 150);
                message2Label.Position = new CCPoint(popUpWindowSize.Width*0.6f, popUpWindowSize.Height*0.5f - star.ScaledContentSize.Height*1.5f);
                popUpWindow.AddChild(message2Label);

                retryButton.Position = new CCPoint(popUpWindowSize.Width - popUpWindowSize.Width*marginFactor - retryButton.ScaledContentSize.Width*0.5f, 
                    popUpWindowSize.Width*marginFactor + retryButton.ScaledContentSize.Width*0.5f);
            }

            popUpWindow.AddChild(backButton);
            popUpWindow.AddChild(retryButton);
        }

        private void OnMove1Completed(PetZombie.BlocksDeletingEventArgs args)
        {
            _moveTo1Completed = true;
            if (_moveTo2Completed)
                RemoveBlocks(args);
        }

        private void OnMove2Completed(PetZombie.BlocksDeletingEventArgs args)
        {
            _moveTo2Completed = true;
            if (_moveTo1Completed)
                RemoveBlocks(args);
        }

        private void MyNextDelete()
        {
            game.NextDelete();
        }

        private void MoveBlocks(PetZombie.BlocksDeletingEventArgs args)
        {
            CCMoveTo moveTo;
            CCMoveTo moveto2;

            var movedBlocksCount = 0;

            for (var i = 0; i < args.PrevMovBlocks.Count; i++)
            {
                var sprite = FindBlockSprite(args.PrevMovBlocks[i]);

                if (sprite != null)
                {
                    blockGrid.RemoveChild(sprite);
                    game.RemoveBlock(args.PrevMovBlocks[i]);
                }
            }

            for (var i = 0; i < args.PrevMovBlocks.Count; i++)
            {
                var position = Block.GetPosition(args.CurMovBlocks[i], blockSize);

                moveTo = new CCMoveTo(0.2f, position);
                moveto2 = new CCMoveTo(0.1f, new CCPoint(position.X, position.Y + blockSize.Height/4));
                var action = new CCSequence(moveTo, moveto2, moveTo);

                var block = new Block(args.PrevMovBlocks[i], blockSize);

                blockGrid.AddChild(block.Sprite);
                //game.AddBlock(currentMovingBlocks[i]);
                //AddEventListener(listener.Copy(), block.Sprite);

                block.Sprite.RunAction(action);

                movedBlocksCount++;

                if (movedBlocksCount == args.CurMovBlocks.Count)
                    UpdateBlockGrid();
            }
        }

        private void RemoveBlocks(PetZombie.BlocksDeletingEventArgs args)
        {
            var scaleBy = new CCScaleBy(0.2f, 0.5f);
            var removedBlocksCount = 0;

            foreach (var block in args.DelBlocks)
            {
                var sprite = FindBlockSprite(block);
                var remove = new CCCallFunc(() =>
                {
                    blockGrid.RemoveChild(sprite);
                    removedBlocksCount++;

                    if (removedBlocksCount == args.DelBlocks.Count)
                    {
                        MoveBlocks(args);
                    }

                    //game.RemoveBlock(block);
                });
                sprite.RunAction(new CCSequence(scaleBy, remove));
            }
            if (args.DelBlocks.Count == 0)
                UpdateBlockGrid();
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
            NextDelete();
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

            //var toolbarItemCountLabels = new List<CCLabelTtf>();
           //            toolbarItemCountLabels.Add(pointCountLabel);
//            toolbarItemCountLabels.Add(stepCountLabel);
//            toolbarItemCountLabels.Add(brainCountLabel);
//            toolbarItemCountLabels.Add(soporificCountLabel);
//            toolbarItemCountLabels.Add(gunCountLabel);
//            toolbarItemCountLabels.Add(bombCountLabel);

            for (var i = 0; i < toolbarItems.Length; i++)
            {
                toolbarItems[i].ScaleTo(blockSize);
                toolbarItems[i].Position = new CCPoint(i*blockWidth + blockSize.Width/2, blockSize.Height/2);
                //menu.Children[i].Position = new CCPoint(i*blockWidth + blockSize.Width/2 + 5, blockSize.Height/2 - 5);
                //toolbarItems[i].AnchorPoint = CCPoint.Zero;


                toolbar.AddChild(toolbarItems[i]);
                //AddChild(toolbarItemCountLabels[i]);
            }

            AddChild(toolbar);
        }

        private CCRect GetWorldRectangle(CCSprite sprite)
        {
            var x = sprite.PositionWorldspace.X - sprite.ScaledContentSize.Width*sprite.AnchorPoint.X;
            var y = sprite.PositionWorldspace.Y - sprite.ScaledContentSize.Height*sprite.AnchorPoint.Y;

            return new CCRect(x, y, sprite.ScaledContentSize.Width, sprite.ScaledContentSize.Height);
        }

        #endregion
    }
}

