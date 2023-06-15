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
        private ShaderProgram shadowShader = new ShaderProgram("Shadow_Vertex", "Shadow_Fragment");
        public Matrix4 lightViewMatrix;
        private List<ShadowCascade> cascades = new List<ShadowCascade>();

        public ShadowRenderer()
        {
            lightViewMatrix = Matrix4.Identity;

            cascades.Add(new ShadowCascade(new Vector2i(1024, 1024), 100));
            cascades.Add(new ShadowCascade(new Vector2i(1024, 1024), 200));
            cascades.Add(new ShadowCascade(new Vector2i(1024, 1024), 600));
            cascades.Add(new ShadowCascade(new Vector2i(1024, 1024), 2400));

        }

        public void render(ComponentSystem flatShadeEntities, ComponentSystem smoothShadeEntities, TerrainManager terrainManager, Vector3 lightDirection, Entity camera, Matrix4 viewTest, Matrix4 projTest)
        {

            //Console.WriteLine(lightViewMatrix.ToString());
            //shadowBox.update(camera);
            //updateLightViewMatrix(-lightDirection, shadowBox.getCenter());
            //updateOrthoProjectionMatrix(shadowBox.getSize());

            //updateOrthoProjectionMatrix(new Vector3(160, 160, 160));

            updateLightViewMatrix(-lightDirection, camera.getComponent<Transformation>().position);

            //shadowFrameBuffer.bind();
            GL.DepthMask(true);
            GL.Enable(EnableCap.DepthTest);
            GL.Disable(EnableCap.CullFace);


            GL.Disable(EnableCap.Blend);
            GL.Enable(EnableCap.PolygonOffsetFill);
            GL.PolygonOffset(8f, 1f);
            shadowShader.bind();

            foreach (ShadowCascade cascade in cascades)
            {
                cascade.bindFrameBuffer();
                GL.Clear(ClearBufferMask.DepthBufferBit);
                foreach (Model model in flatShadeEntities.getMembers())
                {
                    glModel glModel = model.getModel();
                    Matrix4 transformationMatrix = MyMath.createTransformationMatrix(model.owner.getComponent<Transformation>());
                    shadowShader.loadUniformMatrix4f("modelViewProjectionMatrix", transformationMatrix * lightViewMatrix * cascade.getProjectionMatrix());
                    GL.BindVertexArray(glModel.getVAOID());
                    GL.EnableVertexAttribArray(0);

                    GL.DrawElements(PrimitiveType.Triangles, glModel.getVertexCount(), DrawElementsType.UnsignedInt, 0);

                }
            }



            GL.Disable(EnableCap.PolygonOffsetFill);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            shadowShader.unBind();
        }

        public List<ShadowCascade> getShadowCascades()
        {
            return cascades;
        }

        public Matrix4 getLightViewMatrix()
        {
            return lightViewMatrix;
        }

        public int getNumberOfCascades()
        {
            return cascades.Count;
        }

        private Matrix4 updateLightViewMatrix(Vector3 direction, Vector3 center)
        {
            direction.Normalize();
            center *= -1f;
            lightViewMatrix =  Matrix4.Identity;

            float rotX = MathF.Acos((direction.Xz).Length);
            float rotY = MathF.Atan(direction.X / direction.Z);
            rotY = direction.Z > 0 ? rotY - MathF.PI : rotY;

            lightViewMatrix *= Matrix4.CreateTranslation(new Vector3(center.X, center.Y, center.Z));
            lightViewMatrix *= Matrix4.CreateRotationX(rotX);
            lightViewMatrix *= Matrix4.CreateRotationY(-rotY);
            return lightViewMatrix;
        }
        /*
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
        */


    }
}
