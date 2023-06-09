using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SpaceEngine.Core
{
    internal class InputHandler
    {
        private static List<Keys> heldDownKeys = new List<Keys>();
        private static List<Keys> clickedKeys = new List<Keys>();


        private static Vector2 mouseDelta = new Vector2();
        public InputHandler() {
        }
        public void update(float delta)
        {
            clickedKeys.Clear();
            mouseDelta.X = 0f;
            mouseDelta.Y = 0f;
        }

        public static Vector2 getMoouseDelta()
        {
            return mouseDelta;
        }

        public void MouseMove(MouseMoveEventArgs eventArgs)
        {
            mouseDelta += eventArgs.Delta;
        }

        public void keyDown(KeyboardKeyEventArgs eventArgs)
        {
            clickedKeys.Add(eventArgs.Key);
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
        public static Boolean isKeyClicked(Keys key) {
            if (clickedKeys.Contains(key)) return true;
            else return false;
        }
    }
}
