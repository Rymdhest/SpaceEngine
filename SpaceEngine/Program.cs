using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SpaceEngine.RenderEngine;

namespace SpaceEngine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            GameWindowSettings gws = GameWindowSettings.Default;
            NativeWindowSettings nws = NativeWindowSettings.Default;

            nws.API = ContextAPI.OpenGL;
            nws.APIVersion = Version.Parse("4.1");
            nws.AutoLoadBindings = true;
            nws.Title = "SpaceEngine";

            GameWindow gameWindow = new GameWindow(gws, nws);

            float[] vertices = new float[] { -1.0f, -1.0f, 0.0f,
                                             1.0f, -1.0f, 0.0f ,
                                             0.0f, 1.0f, 0.0f };
            float[] colors = new float[] {  1.0f, 0.0f, 0.0f,
                                             0.0f, 1.0f, 0.0f ,
                                             0.0f, 0.0f, 1.0f };
            int[] indices = new int[] {0, 1, 2};

            Model model = Loader.loadToVAO(vertices, colors, indices);

            gameWindow.Load += delegate
            {
                int program = createShader();
                GL.UseProgram(program);
            };

            gameWindow.RenderFrame += delegate (FrameEventArgs eventArgs)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //GL.BindBuffer(BufferTarget.ArrayBuffer, model.getIndexBuffer());
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);
                GL.BindVertexArray(0);
                //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                gameWindow.SwapBuffers();
            };

            gameWindow.UpdateFrame += delegate (FrameEventArgs eventArgs)
            {
                //Console.WriteLine(eventArgs.Time);
            };
            createShader();
            gameWindow.Run();

        }
        public static int createShader()
        {
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(vertexShader, File.ReadAllText("../../../Shaders/Vertex_Shader.glsl"));
            GL.ShaderSource(fragmentShader, File.ReadAllText("../../../Shaders/Fragment_Shader.glsl"));
            GL.CompileShader(vertexShader);
            GL.CompileShader(fragmentShader);

            Console.WriteLine(GL.GetShaderInfoLog(vertexShader));
            Console.WriteLine(GL.GetShaderInfoLog(fragmentShader));

            int shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vertexShader);
            GL.AttachShader(shaderProgram, fragmentShader);
            GL.LinkProgram(shaderProgram);
            GL.DetachShader(shaderProgram, vertexShader);
            GL.DetachShader(shaderProgram, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
            return shaderProgram;
        }
    }
}