using System;
using CocosSharp;

namespace PetZombieUI
{
    public class GameMenuLayer : CCLayerColor
    {
        private const float scaleRatio = 0.2f;
        private const float marginPortion = 0.1f;
        private float margin = Resolution.DesignResolution.Width*marginPortion/2;

        private CCSize iconSize;
        private CCSprite background;
        private CCNode toolbar;

        private CCEventListenerTouchOneByOne listener;

        public GameMenuLayer()
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
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            background.Position = VisibleBoundsWorldspace.Center;
            toolbar.Position = new CCPoint(margin, Resolution.DesignResolution.Height - margin);
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
            Window.DefaultDirector.ReplaceScene (ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window));
            return true;
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
            heartIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            toolbar.AddChild(heartIcon);

            AddChild(toolbar);
        }
    }
}

