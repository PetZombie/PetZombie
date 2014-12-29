using System;
using System.Collections.Generic;
using CocosSharp;

namespace PetZombieUI
{
    public class LevelsLayer : CCLayerColor
    {
        private CCSprite background;
        private CCSize iconSize;
        private const float scaleRatio = 0.18f;
        private const float marginPortion = 0.1f;
        private float margin = Resolution.DesignResolution.Width*marginPortion/2;

        private CCEventListenerTouchOneByOne listener;

        private List<CCSprite> levels;
        private CCSprite shopIcon;
        private CCSprite homeIcon;
        private CCSprite heartIcon;
        private CCSprite timerIcon;

        PetZombie.User user;

        public LevelsLayer(PetZombie.User user)
        {
            this.user = user;
            iconSize = new CCSize(Resolution.DesignResolution.Width*scaleRatio, Resolution.DesignResolution.Width*scaleRatio);

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            //listener.OnTouchEnded = OnTouchEnded;
            //listener.OnTouchMoved = OnTouchMoved;

            AddBackground();
            AddLevelsButtons();
            AddOtherButtons();
        }

        private bool OnTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            foreach (var level in levels)
                if (GetWorldRectangle(level).ContainsPoint(touch.Location))
                {
                    if (levels.IndexOf(level) + 1 > user.LastLevel + 1)
                      return true;
                    if (user.LivesCount <= 0)
                    {
                        Director.ReplaceScene(GameMenuLayer.GameMenuLayerScene(Window, user));
                        return true;
                    }
                    Director.ReplaceScene(ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window, levels.IndexOf(level)+1, user));
                    return true;
                }

            if (GetWorldRectangle(homeIcon).ContainsPoint(touch.Location))
            {
                Director.ReplaceScene(GameMenuLayer.GameMenuLayerScene(Window, user));
                return true;
            }
            if (GetWorldRectangle(shopIcon).ContainsPoint(touch.Location))
            {
                Director.ReplaceScene(ShopLayer.ShopLayerScene(Window, user));
                return true;
            }

            return false;
        }

        private CCRect GetWorldRectangle(CCSprite sprite)
        {
            var x = sprite.PositionWorldspace.X - sprite.ScaledContentSize.Width*sprite.AnchorPoint.X;
            var y = sprite.PositionWorldspace.Y - sprite.ScaledContentSize.Height*sprite.AnchorPoint.Y;

            return new CCRect(x, y, sprite.ScaledContentSize.Width, sprite.ScaledContentSize.Height);
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            background.Position = VisibleBoundsWorldspace.Center;
        }

        public static CCScene LevelsLayerScene(CCWindow mainWindow, PetZombie.User user)
        {
            var scene = new CCScene(mainWindow);
            var layer = new LevelsLayer(user);

            scene.AddChild(layer);

            return scene;
        }

        private void AddBackground()
        {
            background = new CCSprite("Images/level's_background");

            var scaleX = background.ContentSize.Width / Resolution.DesignResolution.Width;
            var scaleY = background.ContentSize.Height / Resolution.DesignResolution.Height;

            var scaleValue = Math.Min(scaleX, scaleY);

            var screenWidth = background.ContentSize.Width / scaleValue;
            var screenHeight = background.ContentSize.Height / scaleValue;

            background.ScaleTo(new CCSize(screenWidth, screenHeight));

            AddChild(background);
        }

        private void AddLevelsButtons()
        {
            levels = new List<CCSprite>();

            var level1 = new CCSprite("Images/1tab");
            var level2 = new CCSprite("Images/2tab");
            var level3 = new CCSprite("Images/3tab");
            var level4 = new CCSprite("Images/4tab");
            var level5 = new CCSprite("Images/5tab");
            var level6 = new CCSprite("Images/6tab");
            var level7 = new CCSprite("Images/7tab");
            var level8 = new CCSprite("Images/8tab");
            var level9 = new CCSprite("Images/9tab");
            var level10 = new CCSprite("Images/10tab");

            level1.Position = new CCPoint(Resolution.DesignResolution.Width-230, Resolution.DesignResolution.Height - 850 - Resolution.DesignResolution.Height*0.15f);
            level2.Position = new CCPoint(Resolution.DesignResolution.Width-360, Resolution.DesignResolution.Height - 790 - Resolution.DesignResolution.Height*0.15f);
            level3.Position = new CCPoint(Resolution.DesignResolution.Width-550, Resolution.DesignResolution.Height - 670 - Resolution.DesignResolution.Height*0.15f);
            level4.Position = new CCPoint(Resolution.DesignResolution.Width-490, Resolution.DesignResolution.Height - 460 - Resolution.DesignResolution.Height*0.15f);
            level5.Position = new CCPoint(Resolution.DesignResolution.Width-280, Resolution.DesignResolution.Height - 438 - Resolution.DesignResolution.Height*0.15f);
            level6.Position = new CCPoint(Resolution.DesignResolution.Width-120, Resolution.DesignResolution.Height - 500 - Resolution.DesignResolution.Height*0.15f);
            level7.Position = new CCPoint(Resolution.DesignResolution.Width-100, Resolution.DesignResolution.Height - 350 - Resolution.DesignResolution.Height*0.15f);
            level8.Position = new CCPoint(Resolution.DesignResolution.Width-290, Resolution.DesignResolution.Height - 280 - Resolution.DesignResolution.Height*0.15f);
            level9.Position = new CCPoint(Resolution.DesignResolution.Width-450, Resolution.DesignResolution.Height - 120 - Resolution.DesignResolution.Height*0.15f);
            level10.Position = new CCPoint(Resolution.DesignResolution.Width-200, Resolution.DesignResolution.Height - 80 - Resolution.DesignResolution.Height*0.15f);

            levels.Add(level1);
            levels.Add(level2);
            levels.Add(level3);
            levels.Add(level4);
            levels.Add(level5);
            levels.Add(level6);
            levels.Add(level7);
            levels.Add(level8);
            levels.Add(level9);
            levels.Add(level10);

            for (int l = 0; l < user.LastLevel + 1; l++)
                levels[l].Texture = new CCTexture2D("Images/"+(l+1).ToString()+"tablight");// = new CCSprite("Images/"+(l+1).ToString()+"tablight");

            foreach (var level in levels)
                level.ScaleTo(iconSize);

            foreach (var level in levels)
                AddChild(level);
        }

        private void AddOtherButtons()
        {
            shopIcon = new CCSprite("Images/shop_button");
            homeIcon = new CCSprite("Images/home");
            heartIcon = new CCSprite("Images/heart");
            timerIcon = new CCSprite("Images/timer's_back");

            shopIcon.ScaleTo(iconSize);
            homeIcon.ScaleTo(iconSize);
            heartIcon.ScaleTo(iconSize);
            timerIcon.ScaleTo(new CCSize(iconSize.Width, timerIcon.ContentSize.Height/(timerIcon.ContentSize.Width/iconSize.Width)));

            shopIcon.Position = new CCPoint(margin + homeIcon.ScaledContentSize.Width/2, 
                margin + homeIcon.ScaledContentSize.Height/2);

            homeIcon.Position = new CCPoint(Resolution.DesignResolution.Width - margin - shopIcon.ScaledContentSize.Width/2, 
                margin + homeIcon.ScaledContentSize.Height/2);

            heartIcon.Position = new CCPoint(margin + homeIcon.ScaledContentSize.Width/2, 
                Resolution.DesignResolution.Height - margin - heartIcon.ScaledContentSize.Height/2);

            timerIcon.Position = new CCPoint(margin + homeIcon.ScaledContentSize.Width/2, 
                heartIcon.Position.Y - timerIcon.ScaledContentSize.Height*1.5f);

            AddChild(shopIcon);
            AddChild(homeIcon);
            //AddChild(heartIcon);
            //AddChild(timerIcon);
        }
    }
}