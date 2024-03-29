﻿using Android.App;
using Android.OS;
using Android.Runtime;
using Xamarin.Forms.Platform.Android;

namespace Xamarin_Game
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : FormsAppCompatActivity
    {
        private GameView gameView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource

            Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);

            gameView = new GameView(this);

            SetContentView(gameView);
        }

        protected override void OnResume()
        {
            base.OnResume();
            gameView?.Resume();
        }

        protected override void OnPause()
        {
            base.OnPause();
            gameView?.Pause();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}