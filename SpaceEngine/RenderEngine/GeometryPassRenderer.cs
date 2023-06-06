﻿using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Util;
using OpenTK.Graphics.OpenGL;
using SpaceEngine.Shaders;

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
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        }
        public void render(List<Entity> flatShadeEntities, List<Entity> smoothShadeEntities, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {

            prepareFrame(viewMatrix, projectionMatrix);

            flatShader.bind();
            flatShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
            flatShader.loadUniformMatrix4f("viewMatrix", viewMatrix);
            foreach (Entity modelEntity in flatShadeEntities)
            {
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(modelEntity.getComponent<Transformation>());
                Model model = modelEntity.getComponent<Model>();
                flatShader.loadUniformMatrix4f("TransformationMatrix", transformationMatrix);
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }
            flatShader.stop();



            smoothShader.bind();
            smoothShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
            smoothShader.loadUniformMatrix4f("viewMatrix", viewMatrix);
            foreach (Entity modelEntity in smoothShadeEntities)
            {
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(modelEntity.getComponent<Transformation>());
                Model model = modelEntity.getComponent<Model>();
                smoothShader.loadUniformMatrix4f("TransformationMatrix", transformationMatrix);
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }
            smoothShader.stop();


            //GL.BindBuffer(BufferTarget.ArrayBuffer, model.getIndexBuffer());
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            finishFrame();
        }
        private void finishFrame()
        {
            GL.BindVertexArray(0);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);

        }
    }
}
