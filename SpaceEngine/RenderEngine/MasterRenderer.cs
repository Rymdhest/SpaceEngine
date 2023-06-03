
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using SpaceEngine.Util;
using OpenTK.Windowing.Common;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Entity_Component_System.Components;

namespace SpaceEngine.RenderEngine
{
    internal class MasterRenderer
    {
        private ShaderProgram flatShader = new ShaderProgram("Flat_Shade_Vertex", "Flat_Shade_Fragment", "Flat_Shade_Geometry");
        private ShaderProgram simpleShader = new ShaderProgram("Simple_Vertex", "Simple_Fragment");
        private Matrix4 projectionMatrix;
        private float fieldOfView;
        private float near = 0.1f;
        private float far = 100f;
        public FrameBuffer gBuffer;
        private ScreenQuadRenderer screenQuadRenderer;
        public MasterRenderer() {
            fieldOfView = MyMath.PI/2f;
            FrameBufferSettings gBufferSettings = new FrameBufferSettings();
            gBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment0));
            gBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment1));
            gBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment2));

            DepthAttachmentSettings depthSettings = new DepthAttachmentSettings();
            depthSettings.isTexture = true;
            gBufferSettings.depthAttachmentSettings = depthSettings;
            gBuffer = new FrameBuffer(gBufferSettings);
            screenQuadRenderer = new ScreenQuadRenderer();
            updateProjectionMatrix();
        }
        private void updateProjectionMatrix()
        {

            float aspect = (float)WindowHandler.resolution.X / (float)WindowHandler.resolution.Y;
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

        public void prepareFrame(Matrix4 viewMatrix, Vector3 viewPosition)
        {
            
            gBuffer.bind();
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            


            flatShader.bind();
            flatShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
            flatShader.loadUniformMatrix4f("viewMatrix", viewMatrix);
            flatShader.loadUniformVector3f("cameraPos", viewPosition);

        }

        public void finishFrame()
        {
            GL.BindVertexArray(0);
            flatShader.stop();

            simpleShader.bind();
            simpleShader.loadUniformInt("blitTexture", 0);
            screenQuadRenderer.renderTextureToNextFrameBuffer(gBuffer.getRenderAttachment(0));
            screenQuadRenderer.renderTextureToScreen(screenQuadRenderer.getLastOutputTexture());
            simpleShader.stop();

            WindowHandler.getWindow().SwapBuffers();
            
        }

        public void render(List<Entity> modelEntities, Matrix4 viewMatrix, Vector3 viewPosition)
        {
            
            prepareFrame(viewMatrix, viewPosition);

            foreach (Entity modelEntity in modelEntities)
            {
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(modelEntity.getComponent<Transformation>());
                Model model = modelEntity.getComponent<Model>();
                flatShader.loadUniformMatrix4f("TransformationMatrix", transformationMatrix);
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);
                
            }
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            //GL.BindBuffer(BufferTarget.ArrayBuffer, model.getIndexBuffer());
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            finishFrame();
        }
        public void update(float delta)
        {

        }
        public void onResize(ResizeEventArgs eventArgs)
        {
            GL.Viewport(0, 0, WindowHandler.resolution.X, WindowHandler.resolution.Y);
            updateProjectionMatrix();
        }
    }
}
