

using OpenTK.Graphics.OpenGL;
using SpaceEngine.Modelling;

namespace SpaceEngine.RenderEngine
{
    internal class glLoader
    {
        public static Model loadToVAO(RawModel rawModel)
        {
            return loadToVAO(rawModel.positions, rawModel.colors, rawModel.indices);
        }
        public static Model loadToVAO(float[] positions, float[] colors, int[] indices)
        {
            int vaoID = createVAO();
            int[] VBOS = new int[3];
            VBOS[2] = bindIndicesBuffer(indices);

            VBOS[0] = storeDataInAttributeList(0, 3, positions);
            VBOS[1] = storeDataInAttributeList(1, 3, colors);
            unbindVAO();
            return new Model(vaoID, VBOS, indices.Length, MasterRenderer.Pipeline.FLAT_SHADING);
        }

        public static Model loadToVAO(float[] positions, float[] colors, float[] normals, int[] indices)
        {
            int vaoID = createVAO();
            int[] VBOS = new int[4];
            VBOS[3] = bindIndicesBuffer(indices);

            VBOS[0] = storeDataInAttributeList(0, 3, positions);
            VBOS[1] = storeDataInAttributeList(1, 3, colors);
            VBOS[2] = storeDataInAttributeList(2, 3, normals);
            unbindVAO();
            return new Model(vaoID, VBOS, indices.Length, MasterRenderer.Pipeline.SMOOTH_SHADING);
        }

        public static Model loadToVAO(float[] positions, int[] indices)
        {
            int vaoID = createVAO();
            int[] VBOS = new int[2];
            VBOS[1] = bindIndicesBuffer(indices);
            VBOS[0] = storeDataInAttributeList(0, 2, positions);
            unbindVAO();
            return new Model(vaoID, VBOS, indices.Length, MasterRenderer.Pipeline.OTHER);
        }

        private static int createVAO()
        {
            int vaoID = GL.GenVertexArray();
            GL.BindVertexArray(vaoID);
            return vaoID;
        }
        private static int bindIndicesBuffer(int[] indices)
        {
            int vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, vboID);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticCopy);
            return vboID;
        }
        private static int storeDataInAttributeList(int attributeNumber, int coordinateSize, float[] data)
        {
            int vboID = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer,vboID);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attributeNumber, coordinateSize, VertexAttribPointerType.Float, false, 0, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vboID;
        }
        private static void unbindVAO()
        {
            GL.BindVertexArray(0);
        }
    }

}
