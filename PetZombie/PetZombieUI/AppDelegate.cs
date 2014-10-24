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

			// Set display orientation.
			mainWindow.SupportedDisplayOrientations = CCDisplayOrientation.Portrait;

            // Set common resolution that will be used for all scenes.
            Resolution.DesignResolution = mainWindow.DesignResolutionSize;

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

