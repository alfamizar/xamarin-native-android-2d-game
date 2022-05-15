using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Debug = System.Diagnostics.Debug;

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

        private const int BIRDS_MAX_COUNT = 4;

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

            for (int i = 0; i < BIRDS_MAX_COUNT; i++)
            {
                birds.Add(new Bird(context, i));
            }

            hero = new Hero(context);

            scorePaint.TextSize = 30;
            scorePaint.Color = Color.Red;
        }

        public void Run()
        {
            const double oneSecondInMills = 1000.0;
            const int targetFps = 120;
            const double MS_PER_UPDATE = oneSecondInMills / targetFps;
            double previous = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            double lag = 0.0;

            Stopwatch stopwatch = new Stopwatch();

            while (isRunning)
            {
                double current = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                double elapsed = current - previous;
                previous = current;
                lag += elapsed;
                //ProcessInput();
                int updatesPerRender = 0;
                while (lag >= MS_PER_UPDATE)
                {
                    stopwatch.Reset();
                    stopwatch.Start();
                    Update();
                    stopwatch.Stop();
                    Debug.WriteLine($"Update time is {stopwatch.ElapsedMilliseconds} mills");
                    lag -= MS_PER_UPDATE;
                    updatesPerRender++;
                }
                Debug.WriteLine($"Updates per render: {updatesPerRender}");
                stopwatch.Reset();
                stopwatch.Start();
                Render(lag / MS_PER_UPDATE);
                stopwatch.Stop();
                Debug.WriteLine($"Render time is {stopwatch.ElapsedMilliseconds} mills");
            }
        }

        public void Update()
        {
            //Thread.Sleep(5);
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
                            Debug.WriteLine("Intersected");
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
                Debug.WriteLine("SurfaceHolderIsInvalid");
                return;
            }

            Canvas canvas;
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
            {
                canvas = surfaceHolder.LockHardwareCanvas();
            }
            else
            {
                canvas = surfaceHolder.LockCanvas();
            }
            canvas.DrawBitmap(background.Bitmap, background.X, background.Y, null);

            for (int i = 0; i < birds.Count; i++)
            {
                Bird bird = birds.ElementAt(i);
                canvas.DrawBitmap(bird.Bitmap, (float)(bird.X), bird.Y, null);
            }

            canvas.DrawBitmap(hero.Bitmap, (float)(hero.X), hero.Y, null);

            if (stones.Count > 0)
            {
                for (int i = 0; i < stones.Count; i++)
                {
                    Stone stone = stones.ElementAt(i);
                    // Interpolate is used to render object at place where player expects to see object, 
                    // but not, where the object actually is!
                    canvas.DrawBitmap(stone.Bitmap, (float)(stone.X), (float)(stone.Y - (stone.Speed * interpolate)), null);
                }
            }

            canvas.DrawText(score.ToString(), 5, 35, scorePaint);

            surfaceHolder.UnlockCanvasAndPost(canvas);
        }

        private void GenerateBirds()
        {
            while (isRunning)
            {
                if (birds.Count < BIRDS_MAX_COUNT)
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
                Debug.WriteLine("Action Down");
                if (e.GetX() > 0 & e.GetX() < displayX / 3)
                {
                    Debug.WriteLine("Action Down Left 1/3 Screen");
                    hero.IsMoveLeft = true;
                    hero.IsMoveRight = false;
                }
                else if (e.GetX() > (displayX / 3 * 2) & e.GetX() < displayX)
                {
                    Debug.WriteLine("Action Down Right 1/3 Screen");
                    hero.IsMoveLeft = false;
                    hero.IsMoveRight = true;
                }
                else
                {
                    Stone stone = new Stone(Context, hero);
                    stones.Add(stone);
                }
            }
            else if (e.ActionMasked == MotionEventActions.Up)
            {
                Debug.WriteLine("Action Up");
                hero.IsMoveLeft = false;
                hero.IsMoveRight = false;
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