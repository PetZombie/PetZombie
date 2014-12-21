using System;
using CocosSharp;

namespace PetZombieUI
{
    public class LevelsLayer : CCLayerColor
    {
        CCSprite background;

        public LevelsLayer()
        {
            AddBackground();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            background.Position = VisibleBoundsWorldspace.Center;
        }

        public static CCScene LevelsLayerScene(CCWindow mainWindow)
        {
            var scene = new CCScene(mainWindow);
            var layer = new LevelsLayer();

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
    }
}

