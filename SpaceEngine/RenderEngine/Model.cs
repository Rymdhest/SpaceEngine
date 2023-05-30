

using OpenTK.Graphics.OpenGL;

namespace SpaceEngine.RenderEngine
{
    internal class Model
    {
        private int vaoID;
        private int[] VBOS;
        private int vertexCount;

        public Model(int vaoID, int[] VBOS, int vertexCount)
        {
            this.vaoID = vaoID;
            this.VBOS = VBOS;
            this.vertexCount = vertexCount;
        }

        public int getVAOID()
        {
            return vaoID;
        }

        public int getVertexCount()
        {
            return vertexCount;
        }

        public void cleanUp()
        {
            GL.DeleteVertexArray(vaoID);
            for (int i = 0; i<VBOS.Length; i++)
            {
                GL.DeleteBuffer(VBOS[i]);
            }
        }
    }
}
