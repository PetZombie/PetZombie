using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using CocosSharp;

namespace PetZombieAndroid
{
	[Activity (Label = "PetZombieAndroid", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

            var application = new CCApplication();
            application.ApplicationDelegate = new PetZombie.PetZombieApplicationDelegate();
            SetContentView(application.AndroidContentView);
            application.StartGame();
		}
	}
}


