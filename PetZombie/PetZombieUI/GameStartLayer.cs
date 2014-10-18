//using System;
using CocosSharp;

namespace PetZombieUI
{
	public class GameStartLayer : CCLayerColor
	{
		public GameStartLayer() : base()
		{
			var touchListener = new CCEventListenerTouchAllAtOnce();
			//touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene(GameLayer.GameScene(Window));

			AddEventListener(touchListener, this);

			Color = CCColor3B.Red;
			Opacity = 255;

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

        private void AddBlock()
        {
            var block = new CCSprite("green-block");
            block.Position = VisibleBoundsWorldspace.Center;
            AddChild(block);
        }
	}
}

