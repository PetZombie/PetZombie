//using System;
using CocosSharp;
using CocosDenshion;

namespace PetZombieUI
{
	public class AppDelegate : CCApplicationDelegate
	{
		public override void ApplicationDidFinishLaunching(CCApplication application, CCWindow mainWindow)
		{
			application.PreferMultiSampling = false;
            application.ContentSearchPaths.Add("Content");

            Resolution.DesignResolution = mainWindow.DesignResolutionSize;
            //mainWindow.SetDesignResolutionSize(480, 800, CCSceneResolutionPolicy.FixedHeight);

			// Set display orientation.
			mainWindow.SupportedDisplayOrientations = CCDisplayOrientation.Portrait;

			// Initialize the scene.
            var scene = ThreeInRowGameLayer.ThreeInRowGameLayerScene(mainWindow);

			// Run the initial scene.
			mainWindow.RunWithScene(scene);
		}

		public override void ApplicationDidEnterBackground(CCApplication application)
		{
			// Stop all of the animation actions that are running.
			application.Paused = true;

			// If SimpleAudioEngine is used, the music must be paused.
			CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic();
		}

		public override void ApplicationWillEnterForeground(CCApplication application)
		{
			application.Paused = false;
			CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic();
		}
	}
}

