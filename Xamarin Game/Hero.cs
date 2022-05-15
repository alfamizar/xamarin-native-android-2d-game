using Android.Content;
using Android.Graphics;
using Debug = System.Diagnostics.Debug;

namespace Xamarin_Game
{
    internal class Hero : GameObject
    {
        private bool isMoveLeft;
        private bool isMoveRight;

        public Hero(Context context) : base(context)
        {
            Bitmap = BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.slingshot2);
            Width = Metrics.WidthPixels / 14;
            Height = Width * Bitmap.Height / Bitmap.Width;
            Bitmap = Bitmap.CreateScaledBitmap(Bitmap, Width, Height, true);

            X = (DisplayX - Width) / 2;
            Y = DisplayY - Height;
            // (Metrics.WidthPixels / 1920f) - screen ration to make speed the same on all devices.
            Speed = (int)(6 * DisplayWidthSizeEqualizer);
        }

        public override void MoveObject()
        {
            //Debug.WriteLine("Hero Moved");
            if (isMoveLeft & !isMoveRight)
            {
                Debug.WriteLine("Hero Moved left");
                X -= Speed;
                if (X <= 0)
                {
                    Debug.WriteLine("Hero set x = 0");
                    X = 0;
                }
            }
            else if (!IsMoveLeft & IsMoveRight)
            {
                Debug.WriteLine("Hero Moved right");
                X += Speed;
                if ((X + Width) > DisplayX)
                {
                    Debug.WriteLine("Hero set x = DispalyX - Width");
                    X = DisplayX - Width;
                }
            }
        }

        public bool IsMoveLeft { get => isMoveLeft; set => isMoveLeft = value; }
        public bool IsMoveRight { get => isMoveRight; set => isMoveRight = value; }
    }
}