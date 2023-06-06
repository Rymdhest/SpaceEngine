
using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;
using SpaceEngine.Util;
using Noise;

namespace SpaceEngine.Modelling
{
    internal class TerrainChunk 
    {
        private float spaceBetweenVertices;
        private float[,] heightsLocalGridSpace;
        int resolution;
        public TerrainChunk(Vector2 position, float WorldSize, int resolution)
        {
            this.resolution = resolution;
            heightsLocalGridSpace = new float[resolution, resolution];
            spaceBetweenVertices = WorldSize / resolution;
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    heightsLocalGridSpace[x,z] = noiseFunction(position.X+x* spaceBetweenVertices, position.Y+z* spaceBetweenVertices);
                }
            }
        }
        public Model generateModel()
        {
            
            int totalVertices = resolution * resolution;
            float[] positions = new float[totalVertices * 3];
            float[] colors = new float[totalVertices * 3];
            float[] normals = new float[totalVertices * 3];
            int[] indices = new int[6 * (resolution - 1) * (resolution - 1)];

            int vertexPointer = 0;


            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float localWorldX = x* spaceBetweenVertices;
                    float localWorldZ = z* spaceBetweenVertices;
                    float localWorldY = heightsLocalGridSpace[x,z];
                    positions[vertexPointer * 3] = localWorldX;
                    positions[vertexPointer * 3 + 1] = localWorldY;
                    positions[vertexPointer * 3 + 2] = localWorldZ;

                    Vector3 normal = calculateVertexNormal(x, z);
                    normals[vertexPointer * 3] = normal.X;
                    normals[vertexPointer * 3 + 1] = normal.Y;
                    normals[vertexPointer * 3 + 2] = normal.Z;

                    colors[vertexPointer * 3] = 0f;
                    colors[vertexPointer * 3 + 1] = MyMath.clamp01(localWorldY / 10);
                    colors[vertexPointer * 3 + 2] = 1f-MyMath.clamp01(localWorldY / 10);


                    vertexPointer++;
                }
            }

            int pointer = 0;
            for (int gz = 0; gz < resolution - 1; gz++)
            {
                for (int gx = 0; gx < resolution - 1; gx++)
                {
                    int topLeft = (gz * resolution) + gx;
                    int topRight = topLeft + 1;
                    int bottomLeft = ((gz + 1) * resolution) + gx;
                    int bottomRight = bottomLeft + 1;
                    indices[pointer++] = topLeft;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = topRight;
                    indices[pointer++] = topRight;
                    indices[pointer++] = bottomLeft;
                    indices[pointer++] = bottomRight;
                }
            }

            return glLoader.loadToVAO(positions, colors, normals, indices);
        }
        public Vector3 getLocalWorldPositionFromGridSpace(int x, int z)
        {
            float localWorldX = x * spaceBetweenVertices;
            float localWorldZ = z * spaceBetweenVertices;
            float localWorldY = heightsLocalGridSpace[x, z];
            return new Vector3(localWorldX, localWorldY, localWorldZ);
        }
        private Vector3 calculateVertexNormal(int x, int z)
        {

            if (x < 1) x = 1;
            if (x > resolution - 2) x = resolution - 2;
            if (z < 1) z = 1;
            if (z > resolution - 2) z = resolution - 2;

            Vector3 vertexNormal = new Vector3(0f, 0f, 0f);
            Vector3[] faceNormals = new Vector3[6];
            Vector3 center = getLocalWorldPositionFromGridSpace(x,z);
            faceNormals[0] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x-1, z), getLocalWorldPositionFromGridSpace(x-1, z-1));
            faceNormals[1] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x - 1, z - 1), getLocalWorldPositionFromGridSpace(x, z - 1));
            faceNormals[2] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x, z - 1), getLocalWorldPositionFromGridSpace(x+1, z));
            faceNormals[3] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x+1, z), getLocalWorldPositionFromGridSpace(x+1, z+1));
            faceNormals[4] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x+1, z+1), getLocalWorldPositionFromGridSpace(x, z+1));
            faceNormals[5] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x, z+1), getLocalWorldPositionFromGridSpace(x-1, z));

            for (int i = 0; i < 6; i++)
            {
                vertexNormal += faceNormals[i];
            }
            vertexNormal = vertexNormal / 6.0f;
            vertexNormal.Normalize();
            return vertexNormal;
        }

        private static float noiseFunction(float x, float z)
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
