

using OpenTK.Graphics.OpenGL;
using SpaceEngine.Entity_Component_System;

namespace SpaceEngine.RenderEngine
{
    internal class Model : Component
    {
        private int vaoID;
        private int[] VBOS;
        private int vertexCount;
        private MasterRenderer.Pipeline pipeline;

        public Model(int vaoID, int[] VBOS, int vertexCount, MasterRenderer.Pipeline pipeline)
        {
            this.vaoID = vaoID;
            this.VBOS = VBOS;
            this.vertexCount = vertexCount;
            this.pipeline = pipeline;
        }

        public int getVAOID()
        {
            return vaoID;
        }

        public MasterRenderer.Pipeline getPipeline()
        {
            return pipeline;
        }

        public int getVertexCount()
        {
            return vertexCount;
        }

        public override void cleanUp()
        {
            GL.DeleteVertexArray(vaoID);
            for (int i = 0; i<VBOS.Length; i++)
            {
                GL.DeleteBuffer(VBOS[i]);
            }
        }
    }
}
