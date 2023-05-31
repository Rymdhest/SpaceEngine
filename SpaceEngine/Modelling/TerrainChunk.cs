using SpaceEngine.GameWorld;
using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;
using SpaceEngine.Util;
using Noise;

namespace SpaceEngine.Modelling
{
    internal class TerrainChunk : ModelEntity
    {
        private float spaceBetweenVertices;
        public TerrainChunk(Vector3 position, float WorldSize, int verticesPerRow) : base(position, new Vector3(0f), null)
        {
            base.setModel(generateModel(verticesPerRow, WorldSize));
        }
        private Model generateModel(int verticesPerRow, float WorldSize)
        {
            int totalVertices = verticesPerRow * verticesPerRow;
            float[] positions = new float[totalVertices * 3];
            float[] colors = new float[totalVertices * 3];
            int[] indices = new int[6 * (verticesPerRow - 1) * (verticesPerRow - 1)];

            int vertexPointer = 0;


            for (int z = 0; z < verticesPerRow; z++)
            {
                for (int x = 0; x < verticesPerRow; x++)
                {
                    float worldX = (float)x / ((float)verticesPerRow - 1) * WorldSize;
                    float worldZ = (float)z / ((float)verticesPerRow - 1) * WorldSize;
                    float worldY = noiseFunction(worldX, worldZ);
                    positions[vertexPointer * 3] = worldX;
                    positions[vertexPointer * 3 + 1] = worldY;
                    positions[vertexPointer * 3 + 2] = worldZ;

                    colors[vertexPointer * 3] = 0f;
                    colors[vertexPointer * 3 + 1] = MyMath.clamp01(worldY/ 10);
                    colors[vertexPointer * 3 + 2] = 1f-MyMath.clamp01(worldY/ 10);


                    vertexPointer++;
                }
            }

            int pointer = 0;
            for (int gz = 0; gz < verticesPerRow - 1; gz++)
            {
                for (int gx = 0; gx < verticesPerRow - 1; gx++)
                {
                    int topLeft = (gz * verticesPerRow) + gx;
                    int topRight = topLeft + 1;
                    int bottomLeft = ((gz + 1) * verticesPerRow) + gx;
                    int bottomRight = bottomLeft + 1;
                    indices[pointer++] = topLeft;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = topRight;
                    indices[pointer++] = topRight;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = bottomRight;
                }
            }

            return glLoader.loadToVAO(new RawModel(positions, colors, indices));
        }
        private float noiseFunction(float x, float z)
        {
            float frequency = 0.05f;
            float magnitude = 7f;
            int octaves =26;
            OpenSimplexNoise noise = new OpenSimplexNoise(1337);
            float value = 0f;
            for (int i = 0; i<octaves; i++)
            {
                value += (float)Math.Pow(noise.Evaluate(x * frequency, z * frequency), 1)* magnitude;
                frequency *= 2f;
                magnitude *= 0.5f;


            }

            frequency = 0.05f;
            magnitude = 10f;
            octaves = 4;
            for (int i = 0; i < octaves; i++)
            {
                value += magnitude*0.5f - Math.Abs(noise.Evaluate(x * frequency, z * frequency)) * magnitude;
                frequency *= 2f;
                magnitude *= 0.5f;


            }
            frequency = 0.04f;
            magnitude = 0.5f;
            octaves = 1;
            for (int i = 0; i < octaves; i++)
            {
                value *= 0.5f+(float)Math.Pow(noise.Evaluate(x * frequency, z * frequency), 1) * magnitude;
                frequency *= 2f;
                magnitude *= 0.5f;


            }

            frequency = 0.02f;
            magnitude = 1.5f;
            octaves = 1;
            for (int i = 0; i < octaves; i++)
            {
                value *= 1.5f + (float)Math.Pow(noise.Evaluate(x * frequency, z * frequency), 1) * magnitude;
                frequency *= 2f;
                magnitude *= 0.5f;


            }

            if (value < 0f) value = 0f;

            return value;
        }
    }
}
