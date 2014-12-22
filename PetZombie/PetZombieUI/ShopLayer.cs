using System;
using CocosSharp;

namespace PetZombieUI
{
    public class ShopLayer : CCLayerColor
    {
        private const float scaleRatio = 0.15f;
        private const float marginPortion = 0.1f;
        private float margin = Resolution.DesignResolution.Width*marginPortion/2;

        private CCSize iconSize;
        private CCSprite background;
        private CCNode toolbar;

        private CCSprite backButton;

        private CCSprite backForWeapon1, backForWeapon2, backForWeapon3;

        private CCEventListenerTouchOneByOne listener;

        public ShopLayer()
        {
            iconSize = new CCSize(Resolution.DesignResolution.Width*scaleRatio, Resolution.DesignResolution.Width*scaleRatio);

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            //listener.OnTouchEnded = OnTouchEnded;
            //listener.OnTouchMoved = OnTouchMoved;

            AddBackground();
            AddToolbar();
            AddBackButton();
            AddBackForWeapon();
            AddWeaponItem();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            background.Position = VisibleBoundsWorldspace.Center;
            toolbar.Position = new CCPoint(margin + 0.5f*iconSize.Width, Resolution.DesignResolution.Height - margin - 0.5f*iconSize.Height);

            var width = Resolution.DesignResolution.Width*1.1f;
            //var scale = petZombie.ContentSize.Width / width;
            //var height = petZombie.ContentSize.Height / scale;


            //petZombie.AnchorPoint = new CCPoint(1, 0);
            //petZombie.ScaleTo(new CCSize(width, height));
            /*petZombie.Position = new CCPoint(Resolution.DesignResolution.Width + petZombie.ScaledContentSize.Width*0.2f, 
                -petZombie.ScaledContentSize.Height*0.3f);
                */
        }

        public static CCScene ShopLayerScene(CCWindow mainWindow)
        {
            var scene = new CCScene(mainWindow);
            var layer = new ShopLayer();

            scene.AddChild(layer);

            return scene;
        }

        private bool OnTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            if (GetWorldRectangle(backButton).ContainsPoint(touch.Location))
            {
                //var scaleDown = new CCScaleBy(0.1f, 0.8f);
                //backButton.RunAction(scaleDown);

                Director.ReplaceScene(GameMenuLayer.GameMenuLayerScene(Window));
                return true;
            }
            return false;
        }

        private CCRect GetWorldRectangle(CCSprite sprite)
        {
            var x = sprite.PositionWorldspace.X - iconSize.Width*sprite.AnchorPoint.X;
            var y = sprite.PositionWorldspace.Y - iconSize.Height*sprite.AnchorPoint.Y;

            return new CCRect(x, y, iconSize.Width, iconSize.Height);
        }

        private void AddBackground()
        {
            background = new CCSprite("Images/shop_background");

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

            var patronsIcon = new CCSprite("Images/patrons");
            var bombIcon = new CCSprite("Images/bomb_bar");
            var soporificIcon = new CCSprite("Images/soporific_bar");
            var moneyIcon = new CCSprite("Images/money");

            patronsIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            bombIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            soporificIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            moneyIcon.ScaleTo(new CCSize(iconSize.Width*0.75f, iconSize.Height*0.75f));

            bombIcon.Position = new CCPoint(0, margin*0.8f);
            patronsIcon.Position = new CCPoint((Resolution.DesignResolution.Width/2 - 2*margin)/2, margin*0.7f);
            soporificIcon.Position = new CCPoint(Resolution.DesignResolution.Width/2 - 2*margin, margin*0.8f);
            moneyIcon.Position = new CCPoint(Resolution.DesignResolution.Width - 2*moneyIcon.ScaledContentSize.Width - 3.5f*margin, margin*0.8f);
            //bombIcon.Position = new CCPoint(Resolution.DesignResolution.Width - 2*margin - bombIcon.ScaledContentSize.Width, 0);

            toolbar.AddChild(patronsIcon);
            toolbar.AddChild(bombIcon);
            toolbar.AddChild(soporificIcon);
            toolbar.AddChild(moneyIcon);

            AddChild(toolbar);
        }
            
        private void AddBackButton()
        {
            float scale = 0.17f;
            var backButtonSize = new CCSize(Resolution.DesignResolution.Width*scale, Resolution.DesignResolution.Width*scale);
            backButton = new CCSprite("Images/back_arrow");
            backButton.ScaleTo(new CCSize(backButtonSize.Width, backButtonSize.Height+5));
            backButton.Position = new CCPoint(backButton.ScaledContentSize.Width - margin - margin/2, backButton.ScaledContentSize.Height - margin);

            AddChild(backButton);
        }

        private void AddBackForWeapon()
        {
            //float scale = 0.17f;
            //var backWeaponSize = new CCSize(Resolution.DesignResolution.Width, Resolution.DesignResolution.Height);
            backForWeapon1 = new CCSprite("Images/back_for_weapon");
            //backForWeapon.ScaledContentSize
            var size = new CCSize(backForWeapon1.ScaledContentSize.Width*0.5f, backForWeapon1.ScaledContentSize.Height*0.5f);
            backForWeapon1.ScaleTo(size);
            backForWeapon1.Position = new CCPoint(Resolution.DesignResolution.Width/2, Resolution.DesignResolution.Height*0.75f);

            backForWeapon2 = new CCSprite("Images/back_for_weapon");
            //backForWeapon.ScaledContentSize
            backForWeapon2.ScaleTo(size);
            backForWeapon2.Position = new CCPoint(Resolution.DesignResolution.Width/2, Resolution.DesignResolution.Height*0.55f);

            backForWeapon3 = new CCSprite("Images/back_for_weapon");
            //backForWeapon.ScaledContentSize
            backForWeapon3.ScaleTo(size);
            backForWeapon3.Position = new CCPoint(Resolution.DesignResolution.Width/2, Resolution.DesignResolution.Height*0.35f);

            AddChild(backForWeapon1);
            AddChild(backForWeapon2);
            AddChild(backForWeapon3);
        }

        private void AddWeaponItem()
        {
            var soporificItem = new CCSprite("Images/soporific_bar");
            //backForWeapon.ScaledContentSize
            soporificItem.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            soporificItem.Position = new CCPoint(Resolution.DesignResolution.Width/5, Resolution.DesignResolution.Height*0.75f);

            var goldItem = new CCSprite("Images/money");
            //backForWeapon.ScaledContentSize
            goldItem.ScaleTo(new CCSize(iconSize.Width*0.75f, iconSize.Height*0.75f));
            goldItem.Position = new CCPoint(Resolution.DesignResolution.Width/2 - 1.5f*margin, Resolution.DesignResolution.Height*0.75f);

            var buyButton = new CCSprite("Images/buy_button");
            //backForWeapon.ScaledContentSize
            buyButton.ScaleTo(new CCSize(buyButton.ScaledContentSize.Width*0.5f, buyButton.ScaledContentSize.Height*0.5f));
            buyButton.Position = new CCPoint(Resolution.DesignResolution.Width*0.8f, Resolution.DesignResolution.Height*0.75f);

            AddChild(goldItem);
            AddChild(soporificItem);
            AddChild(buyButton);

            var soporificItem2 = new CCSprite("Images/bomb_bar");
            //backForWeapon.ScaledContentSize
            soporificItem2.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            soporificItem2.Position = new CCPoint(Resolution.DesignResolution.Width/5, Resolution.DesignResolution.Height*0.55f);

            var goldItem2 = new CCSprite("Images/money");
            //backForWeapon.ScaledContentSize
            goldItem2.ScaleTo(new CCSize(iconSize.Width*0.75f, iconSize.Height*0.75f));
            goldItem2.Position = new CCPoint(Resolution.DesignResolution.Width/2 - 1.5f*margin, Resolution.DesignResolution.Height*0.55f);

            var buyButton2 = new CCSprite("Images/buy_button");
            //backForWeapon.ScaledContentSize
            buyButton2.ScaleTo(new CCSize(buyButton2.ScaledContentSize.Width*0.5f, buyButton2.ScaledContentSize.Height*0.5f));
            buyButton2.Position = new CCPoint(Resolution.DesignResolution.Width*0.8f, Resolution.DesignResolution.Height*0.55f);
        
            AddChild(goldItem2);
            AddChild(soporificItem2);
            AddChild(buyButton2);

            var soporificItem3 = new CCSprite("Images/patrons");
            //backForWeapon.ScaledContentSize
            soporificItem3.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            soporificItem3.Position = new CCPoint(Resolution.DesignResolution.Width/5, Resolution.DesignResolution.Height*0.35f);

            var goldItem3 = new CCSprite("Images/money");
            //backForWeapon.ScaledContentSize
            goldItem3.ScaleTo(new CCSize(iconSize.Width*0.75f, iconSize.Height*0.75f));
            goldItem3.Position = new CCPoint(Resolution.DesignResolution.Width/2 - 1.5f*margin, Resolution.DesignResolution.Height*0.35f);

            var buyButton3 = new CCSprite("Images/buy_button");
            //backForWeapon.ScaledContentSize
            buyButton3.ScaleTo(new CCSize(buyButton3.ScaledContentSize.Width*0.5f, buyButton3.ScaledContentSize.Height*0.5f));
            buyButton3.Position = new CCPoint(Resolution.DesignResolution.Width*0.8f, Resolution.DesignResolution.Height*0.35f);

            AddChild(goldItem3);
            AddChild(soporificItem3);
            AddChild(buyButton3);
        }
    }
}

