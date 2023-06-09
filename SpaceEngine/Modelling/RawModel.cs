
using SpaceEngine.RenderEngine;
using OpenTK.Mathematics;
using System.Transactions;
using SpaceEngine.Util;

namespace SpaceEngine.Modelling
{
    internal class RawModel
    {
        public float[] positions;
        public float[] colors;
        public float[] normals;
        public int[] indices;
        public MasterRenderer.Pipeline pipeline;

        public RawModel()
        : this(new float[0], new float[0], new float[0], new int[0], MasterRenderer.Pipeline.FLAT_SHADING)
        {

        }
        public RawModel(List<Vector3> positions, List<Vector3> colours, List<int> indices, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.FLAT_SHADING)
            : this(fromVector3ListToArray(positions), fromVector3ListToArray(colours), fromIntListToArray(indices), MasterRenderer.Pipeline.FLAT_SHADING)
        {

        }
        public RawModel(float[] positions, float[] colors, int[] indices, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.FLAT_SHADING)
            : this(positions, colors, new float[0], indices, pipeline)
        {

        }
        public RawModel(float[] positions, float[] colors, float[] normals, int[] indices, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.SMOOTH_SHADING)
        {
            this.positions = positions;
            this.colors = colors;
            this.normals = normals;
            this.indices = indices;
            this.pipeline = pipeline;
        }
        public void flatRandomness(float amount)
        {
            flatRandomness(new Vector3(amount));
        }
        public void flatRandomness(Vector3 amount)
        {
            Random rand = new Random();
            for (int i = 0; i < positions.Length; i += 3)
            {
                positions[i] += MyMath.rngMinusPlus() * amount.X;
                positions[i + 1] += MyMath.rngMinusPlus() * amount.Y;
                positions[i + 2] += MyMath.rngMinusPlus() * amount.Z;
            }
        }


        public void scaleRandomness(float amount)
        {
            scaleRandomness(new Vector3(amount));
        }
        public void scaleRandomness(Vector3 amount)
        {
            Random rand = new Random();
            for (int i = 0; i < positions.Length; i += 3)
            {
                positions[i] *= 1f+ MyMath.rngMinusPlus() * amount.X;
                positions[i + 1] *= 1f+ MyMath.rngMinusPlus() * amount.Y;
                positions[i + 2] *= 1f+ MyMath.rngMinusPlus() * amount.Z;
            }
        }
        private static float[] fromVector3ListToArray(List<Vector3> list)
        {
            float[] array = new float[list.Count * 3];

            for (int i = 0; i < list.Count; i++)
            {
                array[i * 3] = list[i].X;
                array[i * 3 + 1] = list[i].Y;
                array[i * 3 + 2] = list[i].Z;
            }
            return array;
        }
        private static int[] fromIntListToArray(List<int> list)
        {
            int[] array = new int[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                array[i] = list[i];
            }
            return array;
        }
        public void cleanUp()
        {

        }

        public void translate(Vector3 translation)
        {
            for (int i = 0; i < positions.Length; i += 3)
            {
                positions[i] += translation.X;
                positions[i + 1] += translation.Y;
                positions[i + 2] += translation.Z;
            }
        }
        public void merge(RawModel other)
        {
            float[] newPositions = new float[positions.Length+other.positions.Length];
            float[] newColours = new float[colors.Length + other.colors.Length];
            float[] newnNrmals = new float[normals.Length + other.normals.Length];
            int[] newIndices = new int[indices.Length + other.indices.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                newPositions[i] = positions[i];
                newColours[i] = colors[i];
            }
            for (int i = 0; i < normals.Length; i++)
            {
                newnNrmals[i] = normals[i];
            }
            for (int i = 0; i < indices.Length; i++)
            {
                newIndices[i] = indices[i];
            }
            for (int i = 0; i < other.positions.Length; i++)
            {
                newPositions[positions.Length + i] = other.positions[i];
                newColours[positions.Length + i] = other.colors[i];
            }
            for (int i = 0; i < other.normals.Length; i++)
            {
                newnNrmals[normals.Length + i] = other.normals[i];
            }
            for (int i = 0; i < other.indices.Length; i++)
            {
                newIndices[indices.Length + i] = other.indices[i] + positions.Length / 3;
            }
            positions = newPositions;
            colors = newColours;
            normals = newnNrmals;
            indices = newIndices;
        }
    }
}
