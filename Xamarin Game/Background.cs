using Android.Content;
using Android.Graphics;

namespace Xamarin_Game
{
    class Background : GameObject
    {
        public Background(Context context) : base(context)
        {
            Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.forest);
            Width = Metrics.WidthPixels;
            Height = Metrics.HeightPixels;
            Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

            X = 0;
            Y = 0;
        }
    }
}