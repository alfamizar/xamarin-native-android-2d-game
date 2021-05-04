using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;

namespace Xamarin_Game
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class MainActivity : AppCompatActivity
    {

        GameView gameView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource

            this.Window.AddFlags(Android.Views.WindowManagerFlags.Fullscreen);


            gameView = new GameView(this);

            SetContentView(gameView);
        }

        protected override void OnResume()
        {
            base.OnResume();
            if (gameView != null)
            {
                gameView.Resume();
            }
        }

        protected override void OnPause()
        {
            base.OnPause();
            if (gameView != null)
            {
                gameView.Pause();
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}