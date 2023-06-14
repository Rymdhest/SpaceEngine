using SpaceEngine.Modelling;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Util;
using System.Reflection;
using SpaceEngine.Core;

namespace SpaceEngine.RenderEngine
{
    internal class GrassRenderer
    {
        private glModel grassBlade;
        private ShaderProgram grassShader = new ShaderProgram("Grass_Vertex", "Grass_Fragment");
        public GrassRenderer()
        {
            grassShader.bind();
            grassShader.loadUniformInt("normalHeightMap", 0);
            grassShader.unBind();

            float width = 0.1f;
            float height = 1.0f;

            float[] positions = {
            -width, 0f, 0f,
            width, 0f, 0f,
            0, height, 0f};

            int[] indices = {0, 1, 2};

            grassBlade = glLoader.loadToVAO(positions, indices, 3);
        }
        public void updater(float delta)
        {
        }

        public void render(Matrix4 viewMatrix, Matrix4 projectionMatrix, TerrainManager terrainManager, Vector3 cameraPosition)
        {

            grassBlade.cleanUp();
            //grassShader.cleanUp();
            //grassShader = new ShaderProgram("Grass_Vertex", "Grass_Fragment");

            float width = 0.03f;
            float height = 0.9f;

            float[] positions = {
            -width, 0f, 0f,
            width, 0f, 0f,
            -width, height*(1/3f), 0f,
            width, height*(1/3f), 0f,
            -width, height*(2/3f), 0f,
            width, height*(2/3f), 0f,
            0, height, 0f};

            Vector3 normal = new Vector3(0f, 2f, 1.0f);
            normal.Normalize();

            int[] indices = {
            0, 1, 2,
            2, 1, 3,
            2, 3, 4,
            4, 3, 5,
            4, 5, 6};

            grassBlade = glLoader.loadToVAO(positions, indices, 3);
            //grassBlade = glLoader.loadToVAO(ModelGenerator.generateTree());
            GL.Disable(EnableCap.CullFace);


            grassShader.bind();
            grassShader.loadUniformFloat("time", Engine.EngineDeltaClock);
            grassShader.loadUniformVector3f("normal", normal);
            grassShader.loadUniformFloat("bladeHeight", height);

            GL.BindVertexArray(grassBlade.getVAOID());
            GL.EnableVertexAttribArray(0);

            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    TerrainChunk chunk = terrainManager.getTerarinChunkAt(cameraPosition.Xz+new Vector2(x*200f, z*200f));
                    if (chunk == null) return;
                    renderGrassOnChunk(chunk, viewMatrix, projectionMatrix);
                }
            }


            GL.Enable(EnableCap.CullFace);
            grassShader.unBind();
        }

        private void renderGrassOnChunk(TerrainChunk chunk, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            int bladesPerRow = 600;
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, chunk.getNormalHeightMap());
            Matrix4 transformationMatrix = MyMath.createTransformationMatrix(chunk.owner.getComponent<Transformation>());
            Matrix4 modelViewMatrix = transformationMatrix * viewMatrix;
            grassShader.loadUniformMatrix4f("modelMatrix", transformationMatrix);
            grassShader.loadUniformMatrix4f("modelViewMatrix", modelViewMatrix);
            grassShader.loadUniformMatrix4f("modelViewProjectionMatrix", modelViewMatrix * projectionMatrix);
            grassShader.loadUniformMatrix4f("normalModelViewMatrix", Matrix4.Transpose(Matrix4.Invert(modelViewMatrix)));
            grassShader.loadUniformFloat("patchSizeWorld", 200f);
            grassShader.loadUniformFloat("bladesPerRow", bladesPerRow);

            GL.DrawElementsInstanced(PrimitiveType.Triangles, grassBlade.getVertexCount(), DrawElementsType.UnsignedInt, IntPtr.Zero, bladesPerRow * bladesPerRow);

        }

        public void cleanUp()
        {

        }
    }
}
