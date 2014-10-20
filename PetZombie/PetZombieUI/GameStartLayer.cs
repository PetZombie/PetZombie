//using System;
using CocosSharp;

namespace PetZombieUI
{
	public class GameStartLayer : CCLayerColor
	{
        private const float marginPortion = 0.1f;
        private float freeSpace = Resolution.DesignResolution.Width * marginPortion;
        private float blockGridWidth;
        private float blockWidth;
        private CCSize blockSize;
        private float blockGridMargin;

        private int rowsCount;
        private int columnsCount;

        CCEventListenerTouchOneByOne listener;

        private GameStartLayer() : base()
		{
            blockGridWidth = Resolution.DesignResolution.Width - freeSpace;
            blockWidth = blockGridWidth / 6;
            blockSize = new CCSize(blockWidth, blockWidth);
            blockGridMargin = freeSpace / 2;

            rowsCount = 9;
            columnsCount = 6;

            var game = new PetZombie.ThreeInRowGame(rowsCount, columnsCount);

            listener = new CCEventListenerTouchOneByOne();
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;

            Color = CCColor3B.Gray;
			Opacity = 255;

            AddBackground();
            AddBlocks();
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;
		}

		public static CCScene GameStartLayerScene(CCWindow mainWindow)
		{
			var scene = new CCScene(mainWindow);
			var layer = new GameStartLayer();

			scene.AddChild(layer);

			return scene;
		}

        private bool OnTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            var target = ccevent.CurrentTarget as CCSprite;
            var location = target.ConvertToWorldspace(touch.Location);

            //var size = 

                /*if (ccevent.CurrentTarget.)
            {

            }*/

            return false;
        }

        private void AddBackground()
        {
            var background = new CCSprite();
            AddChild(background);
        }

        private void AddBlocks()
        {
            var blockGrid = new CCNode();
            CCSprite block;

            for (var i = 0; i < columnsCount; i++)
            {
                for (var j = 0; j < rowsCount; j++)
                {
                    var x = i * blockWidth;
                    var y = j * blockWidth;
                    block = CreateBlock(new CCPoint(x, y));
                    blockGrid.AddChild(block);

                    //AddEventListener(listener.Copy(), block);
                }
            }

            blockGrid.Position = new CCPoint(blockGridMargin, blockGridMargin);

            AddChild(blockGrid);
        }

        private CCSprite CreateBlock(CCPoint point)
        {
            var randomNumber = CCRandom.Next(0, 6);
            string fileName = "";

            switch (randomNumber)
            {
                case 0:
                    fileName = "Images/green-block";
                    break;
                case 1:
                    fileName = "Images/red-block";
                    break;
                case 2:
                    fileName = "Images/blue-block";
                    break;
                case 3:
                    fileName = "Images/violet-block";
                    break;
                case 4:
                    fileName = "Images/orange-block";
                    break;
                case 5:
                    fileName = "Images/zombie-block";
                    break;
            }

            var block = new CCSprite(fileName);

            block.AnchorPoint = CCPoint.Zero;
            block.Position = point;
            block.ScaleTo(blockSize);

            return block;
        }
	}
}

