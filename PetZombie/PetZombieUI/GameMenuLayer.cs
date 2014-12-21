using System;
using CocosSharp;

namespace PetZombieUI
{
    public class GameMenuLayer : CCLayerColor
    {
        private const float scaleRatio = 0.18f;
        private const float marginPortion = 0.1f;
        private float margin = Resolution.DesignResolution.Width*marginPortion/2;

        private CCSize iconSize;
        private CCSprite background;
        private CCNode toolbar;

        private CCEventListenerTouchOneByOne listener;

        private CCSprite petZombie;

        public GameMenuLayer()
        {
            iconSize = new CCSize(Resolution.DesignResolution.Width*scaleRatio, Resolution.DesignResolution.Width*scaleRatio);

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            //listener.OnTouchEnded = OnTouchEnded;
            //listener.OnTouchMoved = OnTouchMoved;

            petZombie = new CCSprite("Images/pet_zombie");

            AddBackground();
            AddToolbar();
            AddPetZombie();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            background.Position = VisibleBoundsWorldspace.Center;
            toolbar.Position = new CCPoint(margin + 0.5f*iconSize.Width, Resolution.DesignResolution.Height - margin - 0.5f*iconSize.Height);

            var width = Resolution.DesignResolution.Width*1.1f;
            var scale = petZombie.ContentSize.Width / width;
            var height = petZombie.ContentSize.Height / scale;


            petZombie.AnchorPoint = new CCPoint(1, 0);
            petZombie.ScaleTo(new CCSize(width, height));
            petZombie.Position = new CCPoint(Resolution.DesignResolution.Width + petZombie.ScaledContentSize.Width*0.2f, 
                -petZombie.ScaledContentSize.Height*0.3f);
        }

        public static CCScene GameMenuLayerScene(CCWindow mainWindow)
        {
            var scene = new CCScene(mainWindow);
            var layer = new GameMenuLayer();

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


            Window.DefaultDirector.ReplaceScene(ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window)); //ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window));
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
            var timerIcon = new CCSprite("Images/timer's_back");
            var brainIcon = new CCSprite("Images/brain_bar");

            heartIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            timerIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            brainIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));

            timerIcon.Position = new CCPoint(0, -iconSize.Height);

            brainIcon.Position = new CCPoint(Resolution.DesignResolution.Width - 2*margin - brainIcon.ScaledContentSize.Width, 0);

            toolbar.AddChild(heartIcon);
            toolbar.AddChild(timerIcon);
            toolbar.AddChild(brainIcon);

            AddChild(toolbar);
        }

        private void AddPetZombie()
        {
            AddChild(petZombie);
        }
    }
}

