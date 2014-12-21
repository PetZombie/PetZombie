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

        private CCEventListenerTouchOneByOne listener;

        public ShopLayer()
        {
            iconSize = new CCSize(Resolution.DesignResolution.Width*scaleRatio, Resolution.DesignResolution.Width*scaleRatio);

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
            listener.IsSwallowTouches = true;
            //listener.OnTouchBegan = OnTouchBegan;
            //listener.OnTouchEnded = OnTouchEnded;
            //listener.OnTouchMoved = OnTouchMoved;

            AddBackground();
            AddToolbar();
            AddBackButton();
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
            /*foreach (var sprite in toolbar)
            {
                if (GetWorldRectangle(sprite))
                {

                }
            }*/


            //Window.DefaultDirector.ReplaceScene (ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window));
            //return true;
            return true;
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

            patronsIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            bombIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            soporificIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));

            bombIcon.Position = new CCPoint(0, margin);

            patronsIcon.Position = new CCPoint((Resolution.DesignResolution.Width/2 - margin)/2, margin);

            soporificIcon.Position = new CCPoint(Resolution.DesignResolution.Width/2 - margin, margin);
            //bombIcon.Position = new CCPoint(Resolution.DesignResolution.Width - 2*margin - bombIcon.ScaledContentSize.Width, 0);

            toolbar.AddChild(patronsIcon);
            toolbar.AddChild(bombIcon);
            toolbar.AddChild(soporificIcon);

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
    }
}

