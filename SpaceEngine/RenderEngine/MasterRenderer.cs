
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using SpaceEngine.Util;
using SpaceEngine.GameWorld;
using System.Reflection;
using OpenTK.Windowing.Common;

namespace SpaceEngine.RenderEngine
{
    internal class MasterRenderer
    {
        private ShaderProgram basicShader = new ShaderProgram("Vertex_Shader", "Fragment_Shader", "Geometry_Shader");
        private Matrix4 projectionMatrix;
        private float fieldOfView;
        private float near = 0.1f;
        private float far = 100f;
        private Vector2i resolution;
        public MasterRenderer(Vector2i resolution) {
            fieldOfView = MyMath.PI/2f;
            this.resolution = resolution;
            updateProjectionMatrix();
        }
        private void updateProjectionMatrix()
        {

            float aspect = (float)resolution.X / (float)resolution.Y;
            float y_scale = (float)((1f / Math.Tan((fieldOfView / 2f))));
            float x_scale = y_scale / aspect;
            float frustum_length = far - near;
            
            projectionMatrix = Matrix4.Identity;
            projectionMatrix.M11 = x_scale;
            projectionMatrix.M22 = y_scale;
            projectionMatrix.M33 = -((far + near) / frustum_length);
            projectionMatrix.M34 = -1f;
            projectionMatrix.M43 = -((2 * near * far) / frustum_length);
            //projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspect, near, far);
        }

        public void prepareFrame(Camera camera)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);

            basicShader.bind();
            basicShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
            basicShader.loadUniformMatrix4f("viewMatrix", camera.createViewMatrix());

        }

        public void finishFrame()
        {
            GL.BindVertexArray(0);

            basicShader.stop();
            WindowHandler.getWindow().SwapBuffers();
        }

        public void render(List<ModelEntity> modelEntities, Camera camera)
        {
            prepareFrame(camera);

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
        public void onResize(ResizeEventArgs eventArgs)
        {
            resolution.X = eventArgs.Width;
            resolution.Y = eventArgs.Height;

            GL.Viewport(0, 0, resolution.X, resolution.Y);
            updateProjectionMatrix();
        }
    }
}
