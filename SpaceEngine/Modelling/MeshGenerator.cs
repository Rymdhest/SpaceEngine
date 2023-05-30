
using OpenTK.Mathematics;

namespace SpaceEngine.Modelling
{
    internal class MeshGenerator
    {
        public static RawModel generateBox(Vector3 min, Vector3 max)
        {
            float[] positions = {
                min.X, max.Y, max.Z,
                max.X, max.Y, max.Z,
                max.X, max.Y, min.Z,
                min.X, max.Y, min.Z,

                min.X, min.Y, max.Z,
                max.X, min.Y, max.Z,
                max.X, min.Y, min.Z,
                min.X, min.Y, min.Z};

            float[] colours = new float[positions.Length];

            for (int i = 0; i < colours.Length; i += 3)
            {
                colours[i] = 1.0f;
                colours[i + 1] = 0.0f;
                colours[i + 2] = 0.0f;
            }

            int[] indices = {0,1,3, 3,1,2,
                        0,4,1, 1,4,5,
                        1,5,6, 2,1,6,
                        6,7,2, 3,2,7,
                        3,7,4, 0,3,4,
                        6,5,7, 7,5,4};

            return new RawModel(positions, colours, indices);
        }
    }
}
