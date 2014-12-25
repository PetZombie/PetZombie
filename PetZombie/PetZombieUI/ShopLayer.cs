using System;
using CocosSharp;

namespace PetZombieUI
{
    public class ShopLayer : CCLayerColor
    {
        #region Fields

        private const float scaleRatio = 0.15f;
        private const float marginPortion = 0.1f;
        private float margin = Resolution.DesignResolution.Width * marginPortion / 2;
        private CCSize iconSize;
        private CCSprite background;
        private CCNode toolbar;
        private CCSprite backButton;
        private CCSprite buySoporificButton;
        private CCSprite buyBombButton;
        private CCSprite buyPatronsButton;
        CCSprite patronsIcon;
        CCSprite bombIcon;
        CCSprite soporificIcon;
        CCSprite moneyIcon;
        private CCSprite backForWeapon1, backForWeapon2, backForWeapon3;
        private CCEventListenerTouchOneByOne listener;
        PetZombie.Shop shop;
        PetZombie.User user;
        CCSprite currentPressedSprite;
        CCMenu menuToolbar;

        #endregion

        public ShopLayer(PetZombie.User user)
        {
            this.user = user;
            if (this.user == null)
                this.user = new PetZombie.User(3, 2, new PetZombie.ZombiePet("Brad"), 100);
            shop = new PetZombie.Shop(this.user);
            iconSize = new CCSize(Resolution.DesignResolution.Width * scaleRatio, Resolution.DesignResolution.Width * scaleRatio);

            listener = new CCEventListenerTouchOneByOne();
            AddEventListener(listener, this);
            listener.IsSwallowTouches = true;
            listener.OnTouchBegan = OnTouchBegan;
            listener.OnTouchEnded = OnTouchEnded;
            //listener.OnTouchMoved = OnTouchMoved;

            AddBackground();
            AddToolbar();
            AddToolBarLabels();
            AddBackButton();
            AddBackForWeapon();
            AddSoporificItem();
            AddBombItem();
            AddPatronsItem();
            CheckButtonAble();
            AddWeaponCostLabels();
        }

        protected override void AddedToScene()
        {
            base.AddedToScene();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            background.Position = VisibleBoundsWorldspace.Center;
            toolbar.Position = new CCPoint(margin + 0.5f * iconSize.Width, Resolution.DesignResolution.Height - margin - 0.5f * iconSize.Height);

        }

        public static CCScene ShopLayerScene(CCWindow mainWindow, PetZombie.User user = null)
        {
            var scene = new CCScene(mainWindow);
            var layer = new ShopLayer(user);

            scene.AddChild(layer);

            return scene;
        }

        float scaled;

        private bool OnTouchBegan(CCTouch touch, CCEvent ccevent)
        {
            if (GetWorldRectangle(backButton).ContainsPoint(touch.Location))
            {
                Director.ReplaceScene(ThreeInRowGameLayer.ThreeInRowGameLayerScene(Window));
                //Director.ReplaceScene(GameMenuLayer.GameMenuLayerScene(Window));
                return true;
            }

            if (buySoporificButton.Tag == 1 && GetWorldRectangle(buySoporificButton).ContainsPoint(touch.Location))
            {
                currentPressedSprite = buySoporificButton;
                //scaled = buySoporificButton.ScaledContentSize.Width;
                //var scaleDown = new CCScaleBy(0.1f, 0.8f);
                //buySoporificButton.RunAction(scaleDown);

                shop.Buy(new PetZombie.Soporific());
                this.user = shop.User;
                return true;
            }

            if (buyBombButton.Tag == 1 &&GetWorldRectangle(buyBombButton).ContainsPoint(touch.Location))
            {
                currentPressedSprite = buyBombButton;
                //scaled = buyBombButton.ScaledContentSize.Width;
                //var scaleDown = new CCScaleBy(0.1f, 0.8f);
                //buyBombButton.RunAction(scaleDown);

                shop.Buy(new PetZombie.Bomb());
                this.user = shop.User;
                return true;
            }

            if (buyPatronsButton.Tag == 1 && GetWorldRectangle(buyPatronsButton).ContainsPoint(touch.Location))
            {
                currentPressedSprite = buyPatronsButton;
                //scaled = buyPatronsButton.ScaledContentSize.Width;
                //var scaleDown = new CCScaleBy(0.1f, 0.8f);
                //buyPatronsButton.RunAction(scaleDown);

                shop.Buy(new PetZombie.Gun());
                this.user = shop.User;
                return true;
            }

            return false;
        }

        private void OnTouchEnded(CCTouch touch, CCEvent ccevent)
        {

            if (currentPressedSprite != null)
            {
                //var scale = scaled / currentPressedSprite.ScaledContentSize.Width;
                //var scaleUp = new CCScaleBy(0.1f, scale);

                //currentPressedSprite.RunAction(scaleUp);
                currentPressedSprite = null;
                CheckButtonAble();
                UpdateLabelText();
            }
        }

        private void UpdateLabelText()
        {
            CCMenuItemLabel money = menuToolbar.GetChildByTag(0) as CCMenuItemLabel;
            money.Label.Text = user.Money.ToString();

            CCMenuItemLabel soporific = menuToolbar.GetChildByTag(1) as CCMenuItemLabel;
            soporific.Label.Text = user.Weapon[0].Count.ToString();

            CCMenuItemLabel bomb = menuToolbar.GetChildByTag(2) as CCMenuItemLabel;
            bomb.Label.Text = user.Weapon[1].Count.ToString();

            CCMenuItemLabel patrons = menuToolbar.GetChildByTag(3) as CCMenuItemLabel;
            patrons.Label.Text = user.Weapon[2].Count.ToString();
        }

        private CCRect GetWorldRectangle(CCSprite sprite)
        {
            var x = sprite.PositionWorldspace.X - iconSize.Width * sprite.AnchorPoint.X;
            var y = sprite.PositionWorldspace.Y - iconSize.Height * sprite.AnchorPoint.Y;

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

            patronsIcon = new CCSprite("Images/patrons");
            bombIcon = new CCSprite("Images/bomb_bar");
            soporificIcon = new CCSprite("Images/soporific_bar");
            moneyIcon = new CCSprite("Images/money");

            patronsIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            bombIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            soporificIcon.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            moneyIcon.ScaleTo(new CCSize(iconSize.Width * 0.75f, iconSize.Height * 0.75f));

            bombIcon.Position = new CCPoint(0, margin * 0.8f);
            patronsIcon.Position = new CCPoint((Resolution.DesignResolution.Width / 2 - 2 * margin) / 2, margin * 0.7f);
            soporificIcon.Position = new CCPoint(Resolution.DesignResolution.Width / 2 - 1.8f * margin, margin * 0.8f);
            moneyIcon.Position = new CCPoint(Resolution.DesignResolution.Width - 2 * moneyIcon.ScaledContentSize.Width - 3.5f * margin, margin * 0.8f);
            //bombIcon.Position = new CCPoint(Resolution.DesignResolution.Width - 2*margin - bombIcon.ScaledContentSize.Width, 0);

            toolbar.AddChild(patronsIcon);
            toolbar.AddChild(bombIcon);
            toolbar.AddChild(soporificIcon);
            toolbar.AddChild(moneyIcon);

            AddChild(toolbar);

        }

        private void AddToolBarLabels()
        {
            CCLabel moneyLabel = new CCLabel(user.Money.ToString(), "arial", 50);
            moneyLabel.Position = new CCPoint(Resolution.DesignResolution.Width - 3.7f * margin, Resolution.DesignResolution.Height - 2.7f * margin);

            CCLabel soporificCountLabel = new CCLabel(user.Weapon[0].Count.ToString(), "arial", 50);
            soporificCountLabel.Position = new CCPoint(soporificIcon.Position.X + 3.8f * margin, Resolution.DesignResolution.Height - 3.55f * margin);

            CCLabel bombCountLabel = new CCLabel(user.Weapon[1].Count.ToString(), "arial", 50);
            bombCountLabel.Position = new CCPoint(bombIcon.Position.X + 3.8f * margin, Resolution.DesignResolution.Height - 3.55f * margin);

            CCLabel patronsCountLabel = new CCLabel(user.Weapon[2].Count.ToString(), "arial", 50);
            patronsCountLabel.Position = new CCPoint(patronsIcon.Position.X + 4.2f * margin, Resolution.DesignResolution.Height - 3.55f * margin);

            CCMenuItemLabel menuLabel1 = new CCMenuItemLabel(moneyLabel);
            menuLabel1.Tag = 0;
            CCMenuItemLabel menuLabel2 = new CCMenuItemLabel(soporificCountLabel);
            menuLabel2.Tag = 1;
            CCMenuItemLabel menuLabel3 = new CCMenuItemLabel(bombCountLabel);
            menuLabel3.Tag = 2;
            CCMenuItemLabel menuLabel4 = new CCMenuItemLabel(patronsCountLabel);
            menuLabel4.Tag = 3;
            menuToolbar = new CCMenu(menuLabel1, menuLabel2, menuLabel3, menuLabel4);
            AddChild(menuToolbar);
        }

        private void AddWeaponCostLabels()
        {
            CCLabel soporificCostLabel = new CCLabel(user.Weapon[0].Cost.ToString(), "arial", 50);
            soporificCostLabel.Position = new CCPoint(Resolution.DesignResolution.Width / 1.9f, Resolution.DesignResolution.Height * 0.72f);

            CCLabel bombCostLabel = new CCLabel(user.Weapon[1].Cost.ToString(), "arial", 50);
            bombCostLabel.Position = new CCPoint(Resolution.DesignResolution.Width / 1.9f, Resolution.DesignResolution.Height * 0.52f);

            CCLabel patronsCostLabel = new CCLabel(user.Weapon[2].Cost.ToString(), "arial", 50);
            patronsCostLabel.Position = new CCPoint(Resolution.DesignResolution.Width / 1.9f, Resolution.DesignResolution.Height * 0.32f);

            CCMenuItemLabel menuLabel1 = new CCMenuItemLabel(soporificCostLabel);
            CCMenuItemLabel menuLabel2 = new CCMenuItemLabel(bombCostLabel);
            CCMenuItemLabel menuLabel3 = new CCMenuItemLabel(patronsCostLabel);
            CCMenu menu = new CCMenu(menuLabel1, menuLabel2, menuLabel3);
            AddChild(menu);
        }

        private void AddBackButton()
        {
            float scale = 0.17f;
            var backButtonSize = new CCSize(Resolution.DesignResolution.Width * scale, Resolution.DesignResolution.Width * scale);
            backButton = new CCSprite("Images/back_arrow");
            backButton.ScaleTo(new CCSize(backButtonSize.Width, backButtonSize.Height + 5));
            backButton.Position = new CCPoint(backButton.ScaledContentSize.Width - margin - margin / 2, backButton.ScaledContentSize.Height - margin);

            AddChild(backButton);
        }

        private void AddBackForWeapon()
        {
            //float scale = 0.17f;
            //var backWeaponSize = new CCSize(Resolution.DesignResolution.Width, Resolution.DesignResolution.Height);
            backForWeapon1 = new CCSprite("Images/back_for_weapon");
            //backForWeapon.ScaledContentSize
            var size = new CCSize(backForWeapon1.ScaledContentSize.Width * 0.5f, backForWeapon1.ScaledContentSize.Height * 0.5f);
            backForWeapon1.ScaleTo(size);
            backForWeapon1.Position = new CCPoint(Resolution.DesignResolution.Width / 2, Resolution.DesignResolution.Height * 0.75f);

            backForWeapon2 = new CCSprite("Images/back_for_weapon");
            //backForWeapon.ScaledContentSize
            backForWeapon2.ScaleTo(size);
            backForWeapon2.Position = new CCPoint(Resolution.DesignResolution.Width / 2, Resolution.DesignResolution.Height * 0.55f);

            backForWeapon3 = new CCSprite("Images/back_for_weapon");
            //backForWeapon.ScaledContentSize
            backForWeapon3.ScaleTo(size);
            backForWeapon3.Position = new CCPoint(Resolution.DesignResolution.Width / 2, Resolution.DesignResolution.Height * 0.35f);

            AddChild(backForWeapon1);
            AddChild(backForWeapon2);
            AddChild(backForWeapon3);
        }

        CCSize buttonSize;

        private void AddSoporificItem()
        {
            var soporificItem = new CCSprite("Images/soporific_bar");
            soporificItem.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            soporificItem.Position = new CCPoint(Resolution.DesignResolution.Width / 5, Resolution.DesignResolution.Height * 0.75f);

            var goldItem = new CCSprite("Images/money");
            goldItem.ScaleTo(new CCSize(iconSize.Width * 0.75f, iconSize.Height * 0.75f));
            goldItem.Position = new CCPoint(Resolution.DesignResolution.Width / 2 - 1.5f * margin, Resolution.DesignResolution.Height * 0.75f);

            buySoporificButton = new CCSprite("Images/enable_buy_button");
            buttonSize = new CCSize(buySoporificButton.ScaledContentSize.Width * 0.5f, buySoporificButton.ScaledContentSize.Height * 0.5f);
            buySoporificButton.ScaleTo(buttonSize);
            buySoporificButton.Position = new CCPoint(Resolution.DesignResolution.Width * 0.8f, Resolution.DesignResolution.Height * 0.75f);
            buySoporificButton.Tag = 1;

            AddChild(goldItem);
            AddChild(soporificItem);
            AddChild(buySoporificButton);
        }

        private void AddBombItem()
        {
            var bombItem = new CCSprite("Images/bomb_bar");
            bombItem.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            bombItem.Position = new CCPoint(Resolution.DesignResolution.Width / 5, Resolution.DesignResolution.Height * 0.55f);

            var goldItem2 = new CCSprite("Images/money");
            goldItem2.ScaleTo(new CCSize(iconSize.Width * 0.75f, iconSize.Height * 0.75f));
            goldItem2.Position = new CCPoint(Resolution.DesignResolution.Width / 2 - 1.5f * margin, Resolution.DesignResolution.Height * 0.55f);

            buyBombButton = new CCSprite("Images/enable_buy_button");
            buyBombButton.ScaleTo(new CCSize(buyBombButton.ScaledContentSize.Width * 0.5f, buyBombButton.ScaledContentSize.Height * 0.5f));
            buyBombButton.Position = new CCPoint(Resolution.DesignResolution.Width * 0.8f, Resolution.DesignResolution.Height * 0.55f);
            buyBombButton.Tag = 1;

            AddChild(goldItem2);
            AddChild(bombItem);
            AddChild(buyBombButton);
        }

        private void AddPatronsItem()
        {
            var patronsItem = new CCSprite("Images/patrons");
            patronsItem.ScaleTo(new CCSize(iconSize.Width, iconSize.Height));
            patronsItem.Position = new CCPoint(Resolution.DesignResolution.Width / 5, Resolution.DesignResolution.Height * 0.35f);

            var goldItem3 = new CCSprite("Images/money");
            goldItem3.ScaleTo(new CCSize(iconSize.Width * 0.75f, iconSize.Height * 0.75f));
            goldItem3.Position = new CCPoint(Resolution.DesignResolution.Width / 2 - 1.5f * margin, Resolution.DesignResolution.Height * 0.35f);

            buyPatronsButton = new CCSprite("Images/enable_buy_button");
            buyPatronsButton.ScaleTo(new CCSize(buyPatronsButton.ScaledContentSize.Width * 0.5f, buyPatronsButton.ScaledContentSize.Height * 0.5f));
            buyPatronsButton.Position = new CCPoint(Resolution.DesignResolution.Width * 0.8f, Resolution.DesignResolution.Height * 0.35f);
            buyPatronsButton.Tag = 1;

            AddChild(goldItem3);
            AddChild(patronsItem);
            AddChild(buyPatronsButton);
        }

        private void CheckButtonAble()
        {
            if (user.Money < user.Weapon[0].Cost)
            {
                var button = buySoporificButton;
                RemoveChild(buySoporificButton);
                buySoporificButton = new CCSprite("Images/disable_buy_button");
                buySoporificButton.ScaleTo(button.ScaledContentSize);
                buySoporificButton.Position = new CCPoint(button.Position);
                buySoporificButton.Tag = 0;
                AddChild(buySoporificButton);
            }
            if (user.Money < user.Weapon[1].Cost)
            {
                var button = buyBombButton;
                RemoveChild(buyBombButton);
                buyBombButton = new CCSprite("Images/disable_buy_button");
                buyBombButton.ScaleTo(button.ScaledContentSize);
                buyBombButton.Position = new CCPoint(button.Position);
                buyBombButton.Tag = 0;
                AddChild(buyBombButton);
            }
            if (user.Money < user.Weapon[2].Cost)
            {
                var button = buyPatronsButton;
                RemoveChild(buyPatronsButton);
                buyPatronsButton = new CCSprite("Images/disable_buy_button");
                buyPatronsButton.ScaleTo(button.ScaledContentSize);
                buyPatronsButton.Position = new CCPoint(button.Position);
                buyPatronsButton.Tag = 0;
                AddChild(buyPatronsButton);
            }
        }
    }
}

