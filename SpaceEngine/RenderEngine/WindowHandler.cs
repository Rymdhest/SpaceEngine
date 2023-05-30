
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace SpaceEngine.RenderEngine
{
    internal class WindowHandler
    {
        private static GameWindow? gameWindow = null;
        public WindowHandler()
        {
            GameWindowSettings gws = GameWindowSettings.Default;
            NativeWindowSettings nws = NativeWindowSettings.Default;

            nws.API = ContextAPI.OpenGL;
            //nws.APIVersion = Version.Parse("4.1");
            nws.AutoLoadBindings = true;
            nws.Title = "SpaceEngine";
            nws.Size = new Vector2i(1900, 1080);
            nws.Location = new Vector2i(1300, 200);

            gameWindow = new GameWindow(gws, nws);
        }
        public static GameWindow getWindow()
        {
            return gameWindow;
        }
    }
}
