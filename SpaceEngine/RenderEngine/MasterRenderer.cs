
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using SpaceEngine.Util;
using SpaceEngine.GameWorld;
using System.Reflection;

namespace SpaceEngine.RenderEngine
{
    internal class MasterRenderer
    {
        private ShaderProgram basicShader = new ShaderProgram("Vertex_Shader", "Fragment_Shader", "Geometry_Shader");
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
            basicShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);

        }

        public void finishFrame()
        {
            GL.BindVertexArray(0);

            basicShader.stop();
            WindowHandler.getWindow().SwapBuffers();
        }

        public void render(List<ModelEntity> modelEntities)
        {
            prepareFrame();

            foreach (ModelEntity modelEntity in modelEntities)
            {

                basicShader.loadUniformMatrix4f("TransformationMatrix", modelEntity.createTransformationMatrix());
                GL.BindVertexArray(modelEntity.GetModel().getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                GL.DrawElements(PrimitiveType.Triangles, modelEntity.GetModel().getVertexCount(), DrawElementsType.UnsignedInt, 0);
                
            }
            //GL.BindBuffer(BufferTarget.ArrayBuffer, model.getIndexBuffer());
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            finishFrame();
        }
        public void update(float delta)
        {

        }
    }
}
