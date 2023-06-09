using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Util;
using OpenTK.Graphics.OpenGL;
using SpaceEngine.Shaders;
using OpenTK.Windowing.Common;

namespace SpaceEngine.RenderEngine
{
    internal class GeometryPassRenderer
    {
        private ShaderProgram flatShader = new ShaderProgram("Flat_Shade_Vertex", "Flat_Shade_Fragment", "Flat_Shade_Geometry");
        private ShaderProgram smoothShader = new ShaderProgram("Smooth_Shade_Vertex", "Smooth_Shade_Fragment");
        public FrameBuffer gBuffer;

        public GeometryPassRenderer()
        {
            FrameBufferSettings gBufferSettings = new FrameBufferSettings();
            gBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment0));
            gBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment1));
            gBufferSettings.drawBuffers.Add(new DrawBufferSettings(FramebufferAttachment.ColorAttachment2));

            DepthAttachmentSettings depthSettings = new DepthAttachmentSettings();
            gBufferSettings.depthAttachmentSettings = depthSettings;
            gBuffer = new FrameBuffer(gBufferSettings);
        }
        private void prepareFrame(Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            gBuffer.bind();
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.Blend);
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
           
        }
        public void render(ComponentSystem flatShadeEntities, ComponentSystem smoothShadeEntities, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {

            prepareFrame(viewMatrix, projectionMatrix);
            
            flatShader.bind();
            foreach (Model model in flatShadeEntities.getMembers())
            {
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(model.owner.getComponent<Transformation>());
                Matrix4 modelViewMatrix = transformationMatrix * viewMatrix;
                smoothShader.loadUniformMatrix4f("modelViewMatrix", modelViewMatrix);
                smoothShader.loadUniformMatrix4f("modelViewProjectionMatrix", modelViewMatrix * projectionMatrix);
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }

            flatShader.unBind();
            


            smoothShader.bind();
            foreach (Model model in smoothShadeEntities.getMembers())
            {
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(model.owner.getComponent<Transformation>());
                Matrix4 modelViewMatrix = transformationMatrix * viewMatrix;
                smoothShader.loadUniformMatrix4f("modelViewMatrix", modelViewMatrix);
                smoothShader.loadUniformMatrix4f("modelViewProjectionMatrix", modelViewMatrix*projectionMatrix);
                smoothShader.loadUniformMatrix4f("normalModelViewMatrix", Matrix4.Transpose(Matrix4.Invert(modelViewMatrix)));
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
                GL.EnableVertexAttribArray(3);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }
            smoothShader.unBind();


            //GL.BindBuffer(BufferTarget.ArrayBuffer, model.getIndexBuffer());
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            finishFrame();
        }
        public void onResize(ResizeEventArgs eventArgs)
        {
            gBuffer.resize(WindowHandler.resolution);
        }
        private void finishFrame()
        {
            GL.BindVertexArray(0);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.DisableVertexAttribArray(3);

        }
    }
}
