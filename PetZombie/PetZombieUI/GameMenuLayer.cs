using System;
using CocosSharp;
using System.Threading;

namespace PetZombieUI
{
    public class GameMenuLayer : CCLayerColor
    {
        #region Field

        private const float scaleRatio = 0.18f;
        private const float marginPortion = 0.1f;
        private float margin = Resolution.DesignResolution.Width * marginPortion / 2;
        private CCSize iconSize;
        private CCSprite background;
        private CCNode toolbar;
        private CCProgressTimer progressBar;
        private CCSprite progressBarBorder;
        private CCEventListenerTouchOneByOne listener;
        private CCSprite petZombie;
        CCSprite brainIcon;
        CCSprite tempBrainIcon;
        CCSprite shopIcon;
        CCSprite levelIcon;
        CCSprite timerBackIcon;

        CCSprite exitButton;

        CCLabel timer;
        CCLabel livesLabel;
        CCLabel brainsLabel;
        AutoResetEvent autoEvent;
        TimerCallback tcb;
        Timer stateTimer;
        AutoResetEvent autoEventSatiety;
        TimerCallback tcbSatiety;
        Timer satietyTimer;
        private bool isBrainTouched = false;
        PetZombie.User user;
        PetZombie.IDataService data;

        private CCEventListenerKeyboard keyboard;

        #endregion

        public GameMenuLayer(PetZombie.User user)
        {
            data = PetZombie.DataServiceFactory.DataService();
            if (user == null)
            {
                this.user = data.Read();
                if (this.user == null)
                {
                this.user = new PetZombie.User(10, 3, new PetZombie.ZombiePet("Fred", 100), 50);
                data.Write(this.user);
                }
            }
            else
                this.user = user;

            iconSize = new CCSize(Resolution.DesignResolution.Width * scaleRatio, Resolution.DesignResolution.Width * scaleRatio);

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            listener.OnTouchMoved = OnTouchMoved;

            keyboard = new CCEventListenerKeyboard();
            AddEventListener(keyboard);
            keyboard.OnKeyPressed = OnKeyPressed;

            petZombie = new CCSprite("Images/pet_zombie");

            AddBackground();
            AddPetZombie();
            AddToolbar();
            AddProgressBar();
            AddTimer();

            AddExitButton();

            autoEvent = new AutoResetEvent(false);
            tcb = UpdateTime;
            stateTimer = new Timer(tcb, autoEvent, 1000, 1000);

            autoEventSatiety = new AutoResetEvent(false);
            tcbSatiety = UpdateSatiety;
            satietyTimer = new Timer(tcbSatiety, autoEventSatiety, 1000, 1000);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            background.Position = VisibleBoundsWorldspace.Center;
            toolbar.Position = new CCPoint(margin + 0.5f * iconSize.Width, Resolution.DesignResolution.Height - margin - 0.5f * iconSize.Height);

            var width = Resolution.DesignResolution.Width * 1.1f;
            var scale = petZombie.ContentSize.Width / width;
            var height = petZombie.ContentSize.Height / scale;


            petZombie.AnchorPoint = new CCPoint(1, 0);
            petZombie.ScaleTo(new CCSize(width, height));
            petZombie.Position = new CCPoint(Resolution.DesignResolution.Width + petZombie.ScaledContentSize.Width * 0.2f, 
                -petZombie.ScaledContentSize.Height * 0.3f);
        }

        public static CCScene GameMenuLayerScene(CCWindow mainWindow, PetZombie.User user = null)
        {
            var scene = new CCScene(mainWindow);
            var layer = new GameMenuLayer(user);

            scene.AddChild(layer);

            return scene;
        }

        #region Touch handlers

        private void OnKeyPressed(CCEventKeyboard keyboard)
        {
            data.Write(user);
            System.Environment.Exit(0);
        }

        private bool OnTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            if (GetWorldRectangle(exitButton).ContainsPoint(touch.Location))
            {
                data.Write(user);
                //this.Dispose();
                //Application.ExitGame();
                System.Environment.Exit(0);
                return true;
            }
            if (GetWorldRectangle(shopIcon).ContainsPoint(touch.Location))
            {
                user.timer = fullTime;
                user.time = DateTime.UtcNow.ToString();
                //data.Write(user);
                fullTime = "";
                stateTimer.Dispose();
                satietyTimer.Dispose();
                Director.ReplaceScene(ShopLayer.ShopLayerScene(Window, user));
                return true;
            }
            else if (GetWorldRectangle(levelIcon).ContainsPoint(touch.Location))
            {
                user.timer = fullTime;
                user.time = DateTime.UtcNow.ToString();
                //data.Write(user);
                fullTime = "";
                stateTimer.Dispose();
                satietyTimer.Dispose();
                Director.ReplaceScene(LevelsLayer.LevelsLayerScene(Window, user));
                return true;
            }
            else if (GetWorldRectangle(brainIcon).ContainsPoint(touch.Location))
            {
                if (user.CanFeed())
                {
                    isBrainTouched = true;

                    tempBrainIcon = new CCSprite(brainIcon.Texture);
                    tempBrainIcon.ScaleTo(brainIcon.ScaledContentSize);
                    tempBrainIcon.Position = toolbar.Position + brainIcon.Position;
                    brainIcon.ZOrder++;

                    AddChild(tempBrainIcon);
                }
                return true;
            }



            return false;
        }

        private void OnTouchMoved(CCTouch touch, CCEvent ccevent)
        {
            if (isBrainTouched)
            {
                tempBrainIcon.Position += touch.Delta;
            }
        }

        private void OnTouchEnded(CCTouch touch, CCEvent ccevent)
        {
            if (isBrainTouched)
            {
                if (GetWorldRectangle(petZombie).ContainsPoint(tempBrainIcon.PositionWorldspace))
                {
                    user.FeedZombie();
                    UpdateLabels();
                }
                RemoveChild(tempBrainIcon);
                isBrainTouched = false;
                UpdateSatiety(new object());
            }
        }

        #endregion

        private CCRect GetWorldRectangle(CCSprite sprite)
        {
            var x = sprite.PositionWorldspace.X - sprite.ScaledContentSize.Width * sprite.AnchorPoint.X;
            var y = sprite.PositionWorldspace.Y - sprite.ScaledContentSize.Height * sprite.AnchorPoint.Y;

            return new CCRect(x, y, sprite.ScaledContentSize.Width, sprite.ScaledContentSize.Height);
        }

        #region Adding UI elements

        private void AddBackground()
        {
            background = new CCSprite("Images/room background");

            var scaleX = background.ContentSize.Width / Resolution.DesignResolution.Width;
            var scaleY = background.ContentSize.Height / Resolution.DesignResolution.Height;

            var scaleValue = Math.Min(scaleX, scaleY);

            var screenWidth = background.ContentSize.Width / scaleValue;
            var screenHeight = background.ContentSize.Height / scaleValue;

            background.ScaleTo(new CCSize(screenWidth, screenHeight));

            AddChild(background);
        }

        void AddToolbar()
        {
            toolbar = new CCNode();

            var heartIcon = new CCSprite("Images/heart");
            timerBackIcon = new CCSprite("Images/timer's_back");
            brainIcon = new CCSprite("Images/brain_bar");
            shopIcon = new CCSprite("Images/shop_button");
            levelIcon = new CCSprite("Images/levels_button");

            livesLabel = new CCLabel(user.LivesCount.ToString(), "Arial", 50);
            brainsLabel = new CCLabel(user.BrainsCount.ToString(), "Arial", 50);

            heartIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            timerBackIcon.ScaleTo(new CCSize(iconSize.Width, timerBackIcon.ContentSize.Height / (timerBackIcon.ContentSize.Width / iconSize.Width)));
            brainIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            shopIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            levelIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));

            timerBackIcon.Position = new CCPoint(0, -timerBackIcon.ScaledContentSize.Height * 1.5f);
            brainIcon.Position = new CCPoint(Resolution.DesignResolution.Width - 2 * margin - brainIcon.ScaledContentSize.Width, 0);
            shopIcon.Position = new CCPoint(margin + shopIcon.ScaledContentSize.Width * 0.5f, 
                margin + shopIcon.ScaledContentSize.Height * 0.5f);
            levelIcon.Position = new CCPoint(Resolution.DesignResolution.Width - margin - levelIcon.ScaledContentSize.Width * 0.5f,
                margin + levelIcon.ScaledContentSize.Height * 0.5f);

            livesLabel.Position = new CCPoint(2.7f * margin, Resolution.DesignResolution.Height - 2.7f * margin);
            brainsLabel.Position = new CCPoint(Resolution.DesignResolution.Width - 3 * margin, Resolution.DesignResolution.Height - 5.5f * margin);

            toolbar.AddChild(heartIcon);
            toolbar.AddChild(timerBackIcon);
            toolbar.AddChild(brainIcon);


            AddChild(shopIcon);
            AddChild(levelIcon);

            AddChild(toolbar);

            AddChild(livesLabel);
            AddChild(brainsLabel);
        }

        private void AddPetZombie()
        {
            AddChild(petZombie);
        }

        private void AddProgressBar()
        {
            progressBarBorder = new CCSprite("Images/progress_frame");
            progressBarBorder.Position = new CCPoint(Resolution.DesignResolution.Width / 2, Resolution.DesignResolution.Height - 3 * margin);
            //progressBarBorder.Scale = 0.5f;
            AddChild(progressBarBorder);

            progressBar = new CCProgressTimer(new CCSprite("Images/progress"));
            //progressBar.Scale = 0.5f;
            progressBar.Position = new CCPoint(8, 8);
            //progressBar.ContentSize = new CCSize(5,5);
            progressBar.Type = CCProgressTimerType.Bar;
            progressBar.AnchorPoint = new CCPoint(0, 0);
            progressBar.BarChangeRate = new CCPoint(1, 0);
            progressBar.Tag = 1;
            progressBar.Midpoint = new CCPoint(0.0f, 0.1f);

            progressBarBorder.AddChild(progressBar);
            progressBar.Percentage = user.Zombie.Satiety;

        }

        string userTimer;

        private void AddTimer()
        {
            userTimer = user.Timer;
            if (userTimer == "")
                timer = new CCLabel(userTimer, "Arial", 50);
            else
                timer = new CCLabel(DateTime.Parse(userTimer).Minute.ToString() + ":" + DateTime.Parse(userTimer).Second.ToString(), "Arial", 50);

            timer.Position = new CCPoint(Resolution.DesignResolution.Width * 0.15f, Resolution.DesignResolution.Height * 0.83f);
            timer.Color = new CCColor3B(0, 0, 0);
            AddChild(timer);
            fullTime = userTimer;
        }

        #endregion

        string fullTime;

        private void UpdateTime(Object stateInfo)
        {
            if (timer == null || userTimer == "")
            {
                userTimer = user.GetTimer();
                return;
            }
            if (fullTime != "" && DateTime.Parse(fullTime) == DateTime.MinValue)
            {
                user.LivesCount++;
                user.timer = "";
                stateTimer.Dispose();
                timer.Text = "";
                UpdateLabels();
                return;
            }

            if (timer.Text == "")
            {
                fullTime = (DateTime.Parse(userTimer).AddSeconds(-1)).ToString();
                timer.Text = DateTime.Parse(fullTime).Minute.ToString() + ":" + DateTime.Parse(fullTime).Second.ToString();
            }
            else
            {
                fullTime = (DateTime.Parse(fullTime).AddSeconds(-1)).ToString();
                timer.Text = DateTime.Parse(fullTime).Minute.ToString() + ":" + DateTime.Parse(fullTime).Second.ToString();
            }

        }

        private void UpdateSatiety(Object stateInfo)
        {
            if (user.Zombie.Satiety <= 0)
            {
                if (user.LivesCount > 0)
                {
                    user.LivesCount--;
                    user.Zombie.Eat();
                    UpdateLabels();
                }
                else
                    satietyTimer.Dispose();
                return;
            }
            //user.Zombie.Satiety -= 1;
            progressBar.Percentage = user.Zombie.Satiety;
        }

        private void UpdateLabels()
        {
            livesLabel.Text = user.LivesCount.ToString();
            brainsLabel.Text = user.BrainsCount.ToString();
            progressBar.Percentage = user.Zombie.Satiety;
        }

        private void AddExitButton()
        {
            exitButton = new CCSprite("Images/back_arrow.png");
            exitButton.Position = new CCPoint(Resolution.DesignResolution.Width / 2, 2*margin);
            exitButton.Scale = 0.5f;
            AddChild(exitButton);
        }
    }
}

