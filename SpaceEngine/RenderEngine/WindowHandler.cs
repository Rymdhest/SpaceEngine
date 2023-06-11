
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
namespace SpaceEngine.RenderEngine
{
    internal class WindowHandler
    {
        private string title = "SpaceEngine";
        public static GameWindow? gameWindow = null;

        private Stopwatch frameStopWatch = new Stopwatch();
        private Stopwatch secondStopWatch = new Stopwatch();
        private float delta = 0f;
        private int framesLastSecond = 0;
        private int framesCurrentSecond = 0;
        public static Vector2i resolution;

        public WindowHandler(Vector2i resolution)
        {
            GameWindowSettings gws = GameWindowSettings.Default;
            NativeWindowSettings nws = NativeWindowSettings.Default;
            WindowHandler.resolution = resolution;
            nws.API = ContextAPI.OpenGL;
            //nws.APIVersion = Version.Parse("3.3");
            nws.AutoLoadBindings = true;
            nws.Title = title;
            nws.Size = resolution;
            nws.Location = new Vector2i(100, 100);

            gws.RenderFrequency = 60;
            gws.UpdateFrequency = 60;

            gameWindow = new GameWindow(gws, nws);

            secondStopWatch.Start();
            frameStopWatch.Start();
        }
        public static GameWindow getWindow()
        {
            return gameWindow;
        }
        public void update(float delta)
        {
            this.delta = (float)frameStopWatch.Elapsed.TotalSeconds;
            frameStopWatch.Restart();


            if (secondStopWatch.Elapsed.TotalMilliseconds >= 1000.0)
            {
                framesLastSecond = framesCurrentSecond;
                framesCurrentSecond = 0;
                gameWindow.Title = title + " " + framesLastSecond + " FPS";
                secondStopWatch.Restart();

            }

            framesCurrentSecond++;
            
        }
        public void onResize(ResizeEventArgs eventArgs)
        {
            resolution.X = eventArgs.Width;
            resolution.Y = eventArgs.Height;
        }
        public float getDelta()
        {
            return delta;
        }
        public static void setMouseGrabbed(bool setTo)
        {
            if (setTo)
            {
                gameWindow.CursorState = CursorState.Grabbed;
            } else
            {
                gameWindow.CursorState = CursorState.Normal;
            }
            
        }
    }

}
