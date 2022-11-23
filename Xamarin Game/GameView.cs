using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xamarin_Game
{
    internal class GameView : SurfaceView, ISurfaceHolderCallback
    {
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private ISurfaceHolder surfaceHolder;

        private bool isRunning;
        private int displayX, displayY;
        private int score = 0;

        private Paint scorePaint = new Paint();
        private Background background;
        private Hero hero;
        private List<Bird> birds = new List<Bird>();
        private List<Stone> stones = new List<Stone>();
        private readonly float screenRefrashRate;
        private readonly float refreshRateObjectsSpeedRatio;

        private const int BirdsMaxCount = 4;
        private const int DefaultTargetFPS = 120;

        public GameView(Context context) : base(context)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            var metrics = Resources.DisplayMetrics;
            displayX = metrics.WidthPixels;
            displayY = metrics.HeightPixels;

            surfaceHolder = Holder;
            surfaceHolder.AddCallback(this);

            background = new Background(context);

            for (int i = 0; i < BirdsMaxCount; i++)
            {
                birds.Add(new Bird(context, i));
            }

            hero = new Hero(context);

            scorePaint.TextSize = 30;
            scorePaint.Color = Color.Red;

            screenRefrashRate = ((MainActivity)context).WindowManager.DefaultDisplay.RefreshRate;
            refreshRateObjectsSpeedRatio = DefaultTargetFPS / screenRefrashRate;
        }

        public void Run()
        {
            const double oneSecondInMills = 1000.0;
            const double msPerUpdate = oneSecondInMills / DefaultTargetFPS;
            double previous = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            double lag = 0.0;

            while (isRunning)
            {
                double current = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                double elapsed = current - previous;
                previous = current;
                lag += elapsed;
                //ProcessInput();
                const int MAX_SKIPPED_FRAMES = 5;
                int skippedFrames = 0;
                while (lag >= msPerUpdate && skippedFrames < MAX_SKIPPED_FRAMES)
                {
                    Update();
                    lag -= msPerUpdate;
                    skippedFrames++;
                }
                Render(lag / msPerUpdate);
            }
        }

        public void Update()
        {
            List<Bird> birdsToBeRemoved = new List<Bird>();
            List<Stone> stonesToBeRemoved = new List<Stone>();

            for (int i = 0; i < birds.Count; i++)
            {
                Bird bird = birds.ElementAt(i);
                bird.MoveObject();

                if (stones.Count > 0)
                {
                    for (int j = 0; j < stones.Count; j++)
                    {
                        Stone stone = stones.ElementAt(j);
                        if (Rect.Intersects(stone.GetColisionShape(), bird.GetColisionShape()))
                        {
                            score++;
                            birdsToBeRemoved.Add(bird);
                            stonesToBeRemoved.Add(stone);
                        }
                    }
                }
            }

            hero.MoveObject();

            if (stones.Count > 0)
            {
                for (int i = 0; i < stones.Count; i++)
                {
                    Stone stone = stones.ElementAt(i);
                    stone.MoveObject();
                    if (stone.Y + Height < 0)
                    {
                        stonesToBeRemoved.Add(stone);
                    }
                }
            }

            for (int i = 0; i < birdsToBeRemoved.Count; i++)
            {
                if (birds.Count > 0)
                {
                    birds.Remove(birdsToBeRemoved.ElementAt(i));
                }
            }

            for (int i = 0; i < stonesToBeRemoved.Count; i++)
            {
                if (stones.Count > 0)
                {
                    stones.Remove(stonesToBeRemoved.ElementAt(i));
                }
            }
        }

        private void Render(double interpolate = 1)
        {
            if (!surfaceHolder.Surface.IsValid)
            {
                return;
            }

            Canvas canvas = GetCanvas();

            RenderObject(canvas, background);

            for (int i = 0; i < birds.Count; i++)
            {
                Bird bird = birds.ElementAt(i);
                RenderObject(canvas, bird);
            }

            RenderObject(canvas, hero);

            if (stones.Count > 0)
            {
                for (int i = 0; i < stones.Count; i++)
                {
                    Stone stone = stones.ElementAt(i);
                    // Interpolate is used to render object at place where player expects to see object, 
                    // but not, where the object actually is!
                    RenderInterpolatedObject(canvas, stone, interpolate: interpolate);
                }
            }

            canvas.DrawText(score.ToString(), 5, 35, scorePaint);

            surfaceHolder.UnlockCanvasAndPost(canvas);
        }

        private Canvas GetCanvas()
        {
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                return surfaceHolder.LockHardwareCanvas();
            }
            else
            {
                return surfaceHolder.LockCanvas();
            }
        }

        private void RenderObject(Canvas canvas, GameObject gameObject, Paint paint = null)
        {
            canvas.DrawBitmap(gameObject.Bitmap, (float)gameObject.X, (float)gameObject.Y, paint);
        }

        private void RenderInterpolatedObject(Canvas canvas, GameObject gameObject, Paint paint = null, double interpolate = 1)
        {
            canvas.DrawBitmap(gameObject.Bitmap, (float)(gameObject.X), (float)(gameObject.Y - (gameObject.Speed * interpolate)), paint);
        }

        private void GenerateBirds()
        {
            while (isRunning)
            {
                if (birds.Count < BirdsMaxCount)
                {
                    birds.Add(new Bird(Context, new Random().Next(0, 4)));
                }
                Thread.Sleep(2500);
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (e.ActionMasked == MotionEventActions.Down)
            {
                if (e.GetX() > 0 & e.GetX() < displayX / 3)
                {
                    hero.IsMovingLeft = true;
                    hero.IsMovingRight = false;
                }
                else if (e.GetX() > (displayX / 3 * 2) & e.GetX() < displayX)
                {
                    hero.IsMovingLeft = false;
                    hero.IsMovingRight = true;
                }
                else
                {
                    Stone stone = new Stone(Context, hero);
                    stones.Add(stone);
                }
            }
            else if (e.ActionMasked == MotionEventActions.Up)
            {
                hero.IsMovingLeft = false;
                hero.IsMovingRight = false;
            }
            return true;
        }

        public void Resume()
        {
            isRunning = true;
            Task gameLoopTask = Task.Run(() =>
            {
                Run();
            }, cancellationToken);

            Task birdsGenerationTask = Task.Run(() =>
            {
                GenerateBirds();
            }, cancellationToken);
        }

        public void Pause()
        {
            isRunning = false;
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        { }

        public void SurfaceCreated(ISurfaceHolder holder)
        { }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        { Pause(); }
    }
}