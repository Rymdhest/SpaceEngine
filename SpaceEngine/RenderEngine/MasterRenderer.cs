
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using SpaceEngine.Util;

namespace SpaceEngine.RenderEngine
{
    internal class MasterRenderer
    {
        private ShaderProgram basicShader = new ShaderProgram("Vertex_Shader", "Fragment_Shader");
        private Matrix4 projectionMatrix;
        private float fieldOfView;
        private float near = 0.1f;
        private float far = 100f;
        public MasterRenderer() {
            fieldOfView = MyMath.PI/2f;
            updateProjectionMatrix();
        }
        private void updateProjectionMatrix()
        {
            int width = WindowHandler.getWindow().Size.X;
            int height = WindowHandler.getWindow().Size.X;
            float aspect = (float)width / (float)height;
            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspect, near, far);
        }

        public void prepareFrame()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            basicShader.bind();
            
        }

        public void finishFrame()
        {
            basicShader.stop();
            WindowHandler.getWindow().SwapBuffers();
        }

        public void render()
        {
            prepareFrame();
            float z = 0f;
            float[] vertices = new float[] { -1.0f, -1.0f, z,
                                             1.0f, -1.0f, z ,
                                             0.0f, 1.0f, z };
            float[] colors = new float[] {  1.0f, 0.0f, 0.0f,
                                             0.0f, 1.0f, 0.0f ,
                                             0.0f, 0.0f, 1.0f };
            int[] indices = new int[] { 0, 1, 2 };
            Matrix4 TransformationMatrix = Matrix4.CreateTranslation(1, 1, -2);

            Model model = Loader.loadToVAO(vertices, colors, indices);
            basicShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
            basicShader.loadUniformMatrix4f("TransformationMatrix", TransformationMatrix);
            basicShader.loadUniformFloat("saturation", 3f);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, model.getIndexBuffer());
            GL.BindVertexArray(model.getVAOID());
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            finishFrame();
        }
        public void update()
        {

        }
    }
}
