//using System;
using CocosSharp;

namespace PetZombieUI
{
	public class GameStartLayer : CCLayerColor
	{
        private const float marginPortion = 0.1f;

        private GameStartLayer() : base()
		{
			var touchListener = new CCEventListenerTouchAllAtOnce();
			//touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene(GameLayer.GameScene(Window));

			AddEventListener(touchListener, this);

            Color = CCColor3B.Gray;
			Opacity = 255;

            AddBackground();
            AddBlock();
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            /*var label = new CCLabelTtf("Tap Screen to Go Bananas!", "arial", 22)
			{
				Position = VisibleBoundsWorldspace.Center,
				Color = CCColor3B.Green,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle,
				Dimensions = ContentSize
			};

			AddChild(label);*/
		}

		public static CCScene GameStartLayerScene(CCWindow mainWindow)
		{
			var scene = new CCScene(mainWindow);
			var layer = new GameStartLayer();

			scene.AddChild(layer);

			return scene;
		}

        private void AddBackground()
        {
            var background = new CCSprite();
            AddChild(background);
        }

        private void AddBlocks()
        {

        }

        private void AddBlock()
        {
            var block = new CCSprite("Images/green-block");
            block.AnchorPoint = CCPoint.Zero;
            AddChild(block);
        }
	}
}

