//using System;
using CocosSharp;

namespace PetZombieUI
{
    public class GameStartLayer : CCLayerColor
    {
        // Margin fields.
        private const float marginPortion = 0.1f;
        private float freeSpace = Resolution.DesignResolution.Width * marginPortion;

        // Block greed fields.
        private float blockGridWidth;
        private float blockGridMargin;

        // Block fields.
        private float blockWidth;
        private CCSize blockSize;

        // 
        private int rowsCount;
        private int columnsCount;

        // Animation fields.
        private CCScaleBy scaleDown;

        // Touch fields.
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
            listener.OnTouchEnded = OnTouchEnded;
            //listener.OnTouchMoved = OnTouchMoved;

            scaleDown = new CCScaleBy(0.1f, 0.9f);

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
            var sprite = ccevent.CurrentTarget as CCSprite;
            var location = touch.Location;
            CCRect rect = new CCRect(sprite.PositionX, sprite.PositionY, sprite.ScaledContentSize.Width, sprite.ScaledContentSize.Height);

            if (rect.ContainsPoint(location))
            {
                //var action = new CCScaleBy(0.1f, scale);

                PauseListeners(true);

                if (sprite.ScaledContentSize == blockSize)
                    sprite.RunAction(scaleDown);

                ResumeListeners(true);

                return true;
            }

            return false;
        }

        private void OnTouchMoved (CCTouch touch, CCEvent ccevent)
        {
            var sprite = ccevent.CurrentTarget as CCSprite;
            var action = new CCMoveTo(1.0f, touch.Delta);
            sprite.RunAction(action);
        }

        private void OnTouchEnded(CCTouch touch, CCEvent ccevent)
        {
            var sprite = ccevent.CurrentTarget as CCSprite;

            sprite.RunAction(scaleDown.Reverse());
        }

        private void AddBackground()
        {
            var background = new CCSprite();
            AddChild(background);
        }

        private void AddBlocks()
        {
            var blockGrid = new CCNode();
            blockGrid.Position = new CCPoint(blockGridMargin, blockGridMargin);

            CCSprite block;

            for (var i = 0; i < columnsCount; i++)
            {
                for (var j = 0; j < rowsCount; j++)
                {
                    var x = blockWidth * 0.5f + i * blockWidth;
                    var y = blockWidth * 0.5f + j * blockWidth;
                    block = CreateBlock(new CCPoint(x, y));
                    blockGrid.AddChild(block);

                    AddEventListener(listener.Copy(), block);
                }
            }

            AddChild(blockGrid);
        }

        private CCSprite CreateBlock(CCPoint point)
        {
            var randomNumber = CCRandom.Next(0, 6);
            string fileName = "";

            switch (randomNumber)
            {
                case 0:
                    fileName = "Images/blue_ellipse_block";
                    break;
                case 1:
                    fileName = "Images/green_ellipse_block";
                    break;
                case 2:
                    fileName = "Images/orange_ellipse_block";
                    break;
                case 3:
                    fileName = "Images/red_ellipse_block";
                    break;
                case 4:
                    fileName = "Images/violet_ellipse_block";
                    break;
                case 5:
                    fileName = "Images/zombie_block";
                    break;
            }

            var block = new CCSprite(fileName);

            block.AnchorPoint = new CCPoint(0.5f, 0.5f);
            block.Position = point;
            block.ScaleTo(blockSize);

            return block;
        }
    }
}

