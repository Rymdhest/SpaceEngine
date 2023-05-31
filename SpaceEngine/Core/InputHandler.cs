using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceEngine.Core
{
    internal class InputHandler
    {
        private static List<Keys> heldDownKeys = new List<Keys>();
        public InputHandler() {
        }
        public void update(float delta)
        {

        }
        public void keyDown(KeyboardKeyEventArgs eventArgs)
        {
            if (!heldDownKeys.Contains(eventArgs.Key))
            {
                heldDownKeys.Add(eventArgs.Key);
            }
        }
        public void keyUp(KeyboardKeyEventArgs eventArgs)
        {
            if (heldDownKeys.Contains(eventArgs.Key))
            {
                heldDownKeys.Remove(eventArgs.Key);
            }
        }
        public static Boolean isKeyDown(Keys key)
        {
            if (heldDownKeys.Contains(key)) return true;
            else return false;
        }
    }
}
