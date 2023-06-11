using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Shaders;
using SpaceEngine.Util;
using OpenTK.Graphics.OpenGL;

namespace SpaceEngine.RenderEngine
{
    internal class PostGeometryRenderer
    {

        private ShaderProgram postGeometryShader = new ShaderProgram("Post_Geometry_Vertex", "Post_Geometry_Fragment");
        public void render(ComponentSystem postGeometrySystem, Matrix4 viewMatrix, Matrix4 projectionMatrix)
        {
            postGeometryShader.cleanUp();
            postGeometryShader = new ShaderProgram("Post_Geometry_Vertex", "Post_Geometry_Fragment");
            prepareFrame();
            postGeometryShader.bind();
            foreach (Model model in postGeometrySystem.getMembers())
            {
                Matrix4 transformationMatrix = MyMath.createTransformationMatrix(model.owner.getComponent<Transformation>());
                Matrix4 modelViewMatrix = transformationMatrix * viewMatrix;
                postGeometryShader.loadUniformMatrix4f("projectionMatrix", projectionMatrix);
                postGeometryShader.loadUniformMatrix4f("modelViewMatrix", modelViewMatrix);
                postGeometryShader.loadUniformMatrix4f("modelViewProjectionMatrix", modelViewMatrix * projectionMatrix);
                Vector3 pos = model.owner.getComponent<Transformation>().position;
                postGeometryShader.loadUniformVector4f("modelWorldPosition", (new Vector4(pos.X, pos.Y, pos.Z, 1.0f)* viewMatrix* projectionMatrix));
                postGeometryShader.loadUniformVector2f("screenResolution", WindowHandler.resolution);
                //postGeometryShader.loadUniformMatrix4f("normalModelViewMatrix", Matrix4.Transpose(Matrix4.Invert(modelViewMatrix)));
                GL.BindVertexArray(model.getVAOID());
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
                //GL.EnableVertexAttribArray(3);

                GL.DrawElements(PrimitiveType.Triangles, model.getVertexCount(), DrawElementsType.UnsignedInt, 0);

            }
            postGeometryShader.unBind();
            finishFrame();
        }
        private void prepareFrame()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthMask(true);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Blend);
        }
        private void finishFrame()
        {
            GL.BindVertexArray(0);
            GL.DisableVertexAttribArray(0);
            GL.DisableVertexAttribArray(1);
            GL.DisableVertexAttribArray(2);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.Blend);
            //GL.DisableVertexAttribArray(3);
        }
    }
}
