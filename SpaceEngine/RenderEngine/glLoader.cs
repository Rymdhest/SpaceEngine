

using OpenTK.Graphics.OpenGL;
using SpaceEngine.Modelling;
using static SpaceEngine.RenderEngine.MasterRenderer;

namespace SpaceEngine.RenderEngine
{
    internal class glLoader
    {
        public static Model loadToVAO(RawModel rawModel)
        {
            if (rawModel.pipeline == Pipeline.FLAT_SHADING)
            {
                return loadToVAO(rawModel.positions, rawModel.colors,rawModel.materials, rawModel.indices);
            } else if (rawModel.pipeline == Pipeline.SMOOTH_SHADING)
            {
                return loadToVAO(rawModel.positions, rawModel.colors, rawModel.materials, rawModel.normals,rawModel.indices);
            }
            else if (rawModel.pipeline == Pipeline.POST_GEOMETRY)
            {
                return loadToVAO(rawModel.positions, rawModel.colors, rawModel.materials, rawModel.indices, rawModel.pipeline);
            }
            else
            {
                return loadToVAO(rawModel.positions, rawModel.indices, 3, rawModel.pipeline);
            }
            
        }
        public static Model loadToVAO(float[] positions, float[] colors, float[] materials, int[] indices, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.FLAT_SHADING)
        {
            int vaoID = createVAO();
            int[] VBOS = new int[4];
            VBOS[3] = bindIndicesBuffer(indices);

            VBOS[0] = storeDataInAttributeList(0, 3, positions);
            VBOS[1] = storeDataInAttributeList(1, 3, colors);
            VBOS[2] = storeDataInAttributeList(2, 3, materials);
            unbindVAO();
            return new Model(vaoID, VBOS, indices.Length, pipeline);
        }

        public static Model loadToVAO(float[] positions, float[] colors, float[] materials, float[] normals, int[] indices, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.SMOOTH_SHADING)
        {
            int vaoID = createVAO();
            int[] VBOS = new int[5];
            VBOS[4] = bindIndicesBuffer(indices);

            VBOS[0] = storeDataInAttributeList(0, 3, positions);
            VBOS[1] = storeDataInAttributeList(1, 3, colors);
            VBOS[2] = storeDataInAttributeList(2, 3, materials);
            VBOS[3] = storeDataInAttributeList(3, 3, normals);
            unbindVAO();
            return new Model(vaoID, VBOS, indices.Length, pipeline);
        }

        public static Model loadToVAO(float[] positions, int[] indices, int dimensions, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.OTHER)
        {
            int vaoID = createVAO();
            int[] VBOS = new int[2];
            VBOS[1] = bindIndicesBuffer(indices);
            VBOS[0] = storeDataInAttributeList(0, dimensions, positions);
            unbindVAO();
            return new Model(vaoID, VBOS, indices.Length, pipeline);
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
