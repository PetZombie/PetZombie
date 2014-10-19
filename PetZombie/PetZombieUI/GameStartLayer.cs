//using System;
using CocosSharp;

namespace PetZombieUI
{
	public class GameStartLayer : CCLayerColor
	{
        private const float marginPortion = 0.1f;
        private float margin = Resolution.DeviceResolution.Width * marginPortion;
        private float blockGridWidth;
        private float blockWidth;
        private CCSize blockSize;

        private int rowsCount;
        private int columnsCount;

        private GameStartLayer() : base()
		{
            blockGridWidth = Resolution.DeviceResolution.Width - margin;
            blockWidth = blockGridWidth / 6;
            blockSize = new CCSize(blockWidth, blockWidth);

            rowsCount = 9;
            columnsCount = 6;

            //var touchListener = new CCEventListenerTouchAllAtOnce();
            //touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene(GameLayer.GameScene(Window));

            //AddEventListener(touchListener, this);

            var game = new PetZombie.ThreeInRowGame(rowsCount, columnsCount);

            Color = CCColor3B.Gray;
			Opacity = 255;

            AddBackground();
            AddBlocks();
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
            for (var x = margin / 2; x < columnsCount * blockWidth; x += blockWidth)
            {
                for (var y = margin / 2; y < rowsCount * blockWidth; y += blockWidth)
                {
                    AddBlock(new CCPoint(x, y));
                }
            }
        }

        private void AddBlock(CCPoint point)
        {
            var blockSprite = new CCSprite("Images/green-block");
            blockSprite.AnchorPoint = CCPoint.Zero;
            blockSprite.Position = point;
            blockSprite.ScaleTo(blockSize);
            AddChild(blockSprite);
        }
	}
}

