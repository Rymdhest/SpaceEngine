using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Modelling;
using SpaceEngine.Shaders;
using SpaceEngine.Util;
using OpenTK.Graphics.OpenGL;

namespace SpaceEngine.RenderEngine
{
    internal class ShadowRenderer
    {
        private FrameBuffer shadowFrameBuffer;
        private ShaderProgram shadowShader = new ShaderProgram("Shadow_Vertex", "Shadow_Fragment");
        private ShadowBox shadowBox;
        public static float shadowDistance = 500f;
        public Vector2i shadowMapResolution = new Vector2i(512, 512);
        public Matrix4 lightViewMatrix;
        public Matrix4 lightProjectionMatrix;

        public ShadowRenderer()
        {
            FrameBufferSettings settings = new FrameBufferSettings(shadowMapResolution);
            DepthAttachmentSettings depthAttachmentSettings = new DepthAttachmentSettings();
            depthAttachmentSettings.isTexture = true;
            settings.depthAttachmentSettings = depthAttachmentSettings;
            shadowFrameBuffer = new FrameBuffer(settings);
            lightViewMatrix = Matrix4.Identity;
            shadowBox = new ShadowBox(lightViewMatrix);
        }

        public void render(ComponentSystem flatShadeEntities, ComponentSystem smoothShadeEntities, TerrainManager terrainManager, Vector3 lightDirection, Entity camera, Matrix4 viewTest, Matrix4 projTest)
        {

            //shadowBox.update(camera);
            //updateLightViewMatrix(-lightDirection, shadowBox.getCenter());
            //updateOrthoProjectionMatrix(shadowBox.getSize());

            updateOrthoProjectionMatrix(new Vector3(40, 40, 60));
            updateLightViewMatrix(-lightDirection, new Vector3(-15f, 0f, 0f));

            shadowFrameBuffer.bind();
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(8f, 1f);
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            GL.Clear(ClearBufferMask.DepthBufferBit);


            shadowShader.bind();
            foreach (Model model in flatShadeEntities.getMembers())
            {
                glModel glModel = model.getModel();
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(model.owner.getComponent<Transformation>());
                shadowShader.loadUniformMatrix4f("modelViewProjectionMatrix", transformationMatrix * lightViewMatrix * lightProjectionMatrix);
                GL.BindVertexArray(glModel.getVAOID());
                GL.EnableVertexAttribArray(0);

                GL.DrawElements(PrimitiveType.Triangles, glModel.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }

            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.Enable(EnableCap.CullFace);
            shadowShader.unBind();
        }

        private Matrix4 updateLightViewMatrix(Vector3 direction, Vector3 center)
        {
            direction.Normalize();
            center *= -1f;
            //lightViewMatrix =  Matrix4.Identity;
            lightViewMatrix.M11 = 1f;
            lightViewMatrix.M12 = 0f;
            lightViewMatrix.M13 = 0f;
            lightViewMatrix.M14 = 0f;

            lightViewMatrix.M21 = 0f;
            lightViewMatrix.M22 = 1f;
            lightViewMatrix.M23 = 0f;
            lightViewMatrix.M24 = 0f;

            lightViewMatrix.M31 = 0f;
            lightViewMatrix.M32 = 0f;
            lightViewMatrix.M33 = 1f;
            lightViewMatrix.M34 = 0f;

            lightViewMatrix.M41 = 0f;
            lightViewMatrix.M42 = 0f;
            lightViewMatrix.M43 = 0f;
            lightViewMatrix.M44 = 1f;

            float pitch = MathF.Acos((direction.Xz).Length);
            lightViewMatrix *= Matrix4.CreateRotationX(pitch);

            float yaw = MathF.Atan(direction.X / direction.Z);

            yaw = direction.Z > 0 ? yaw - MathF.PI : yaw;

            lightViewMatrix *= Matrix4.CreateTranslation(center);
            lightViewMatrix *= Matrix4.CreateRotationY(-yaw);
            return lightViewMatrix;
        }

        private void updateOrthoProjectionMatrix(Vector3 size)
        {
            lightProjectionMatrix = Matrix4.Identity;
            lightProjectionMatrix.M11 = 2f / size.X;
            lightProjectionMatrix.M22 = 2f / size.Y;
            lightProjectionMatrix.M33 = -2f / size.Z;
            lightProjectionMatrix.M44 = 1;
            //lightProjectionMatrix = Matrix4.CreateOrthographic(size.X, size.Y, 0, -size.Z);
            //lightProjectionMatrix = Matrix4.CreateOrthographic(100, 100, 0, 100);

        }
        public int getDepthTexture()
        {
            return shadowFrameBuffer.getDepthAttachment();
        }

    }
}
