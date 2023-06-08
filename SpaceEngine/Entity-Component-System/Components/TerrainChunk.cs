
using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;
using SpaceEngine.Util;
using Noise;
using SpaceEngine.Entity_Component_System;

namespace SpaceEngine.Modelling
{
    internal class TerrainChunk : Component
    {
        private float spaceBetweenVertices;
        private float[,] heightsLocalGridSpace;
        private Vector2 positionChunkGlobalWorld;
        int resolution;
        private static OpenSimplexNoise noise = new OpenSimplexNoise(14);
        public TerrainChunk(Vector2 position, float WorldSize, int resolution)
        {
            this.resolution = resolution;
            this.positionChunkGlobalWorld = position;
            heightsLocalGridSpace = new float[resolution, resolution];
            spaceBetweenVertices = WorldSize / (resolution-1);
            for (int z = 0; z < resolution; z++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    heightsLocalGridSpace[x,z] = noiseFunction(position.X+x* spaceBetweenVertices, position.Y+z* spaceBetweenVertices);
                }
            }
        }
        public RawModel generateRawModel(MasterRenderer.Pipeline pipeline)
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
                    Vector3 positionLocalWorld = new Vector3(localWorldX, localWorldY, localWorldZ);
                    positions[vertexPointer * 3] = positionLocalWorld.X;
                    positions[vertexPointer * 3 + 1] = positionLocalWorld.Y;
                    positions[vertexPointer * 3 + 2] = positionLocalWorld.Z;


                    Vector3 normal = calculateVertexNormal(x, z);
                    if (pipeline == MasterRenderer.Pipeline.SMOOTH_SHADING)
                    {
                        normals[vertexPointer * 3] = normal.X;
                        normals[vertexPointer * 3 + 1] = normal.Y;
                        normals[vertexPointer * 3 + 2] = normal.Z;
                    }

                    Vector3 grassColor = new Vector3(72 / 255f, 144 / 255f, 48 / 255f);
                    Vector3 snowColor = new Vector3(221 / 255f, 229 / 255f, 244 / 255f);
                    Vector3 dirtColor = new Vector3(144 / 255f, 120 / 255f, 48 / 255f);
                    Vector3 sandColor = new Vector3(236 / 255f, 244 / 255f, 173 / 255f);

                    Vector3 posiyionGlobalWorld = new Vector3(positionChunkGlobalWorld.X+positionLocalWorld.X, positionLocalWorld.Y, positionChunkGlobalWorld.Y+positionLocalWorld.Z);
                    float dirtyness = noise.Evaluate(posiyionGlobalWorld.X*0.002f, posiyionGlobalWorld.Z*0.002f);
                    grassColor = MyMath.lerp(MyMath.clamp01(dirtyness), grassColor, dirtColor);
                    Vector3 groundColor = MyMath.lerp(MyMath.clamp01(positionLocalWorld.Y-100f), grassColor, snowColor);

                    Vector3 rockColor = new Vector3(82 / 255f, 82 / 255f, 82 / 255f);
                    Vector3 waterColor = new Vector3(35 / 255f, 137 / 255f, 218 / 255f);
                    Vector3 color = MyMath.lerp(1f-MyMath.clamp01(normal.Y * normal.Y), groundColor, rockColor);
                    if (positionLocalWorld.Y <= 1.85f && normal.Y > 0.65f)
                    {
                        color = sandColor;
                    }
                    if (positionLocalWorld.Y <= 0.0f)
                    {
                        color = waterColor;
                    }
                    colors[vertexPointer * 3] = color.X;
                    colors[vertexPointer * 3 + 1] = color.Y;
                    colors[vertexPointer * 3 + 2] = color.Z;


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
            RawModel terrainModel = new RawModel(positions, colors, normals, indices, pipeline);
            return terrainModel;
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
            faceNormals[0] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x-1, z), getLocalWorldPositionFromGridSpace(x-1, z+1));
            faceNormals[1] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x - 1, z + 1), getLocalWorldPositionFromGridSpace(x, z + 1));
            faceNormals[2] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x, z + 1), getLocalWorldPositionFromGridSpace(x+1, z));
            faceNormals[3] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x+1, z), getLocalWorldPositionFromGridSpace(x+1, z-1));
            faceNormals[4] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x+1, z-1), getLocalWorldPositionFromGridSpace(x, z-1));
            faceNormals[5] = MyMath.calculateFaceNormal(center, getLocalWorldPositionFromGridSpace(x, z-1), getLocalWorldPositionFromGridSpace(x-1, z));

            for (int i = 0; i < 6; i++)
            {
                vertexNormal += faceNormals[i];
            }
            vertexNormal = vertexNormal / 6.0f;
            vertexNormal.Normalize();
            return vertexNormal;
        }
        public static float octavedNoise(float x, float y, float frequency, float magnitude, int octaves)
        {
            float value = 0;
            float totalMagnitude = 0;
            for (int i = 0; i < octaves; i++)
            {
                totalMagnitude += magnitude;
                value += noise.Evaluate(x * frequency, y * frequency) * magnitude;
                frequency *= 2f;
                magnitude *= 0.5f;
            }
            if (totalMagnitude == 0f) totalMagnitude = 1f;
            return value;
        }

        public static float noiseFunction(float x, float z)
        {
            float value = 0f;

            float mountainess= octavedNoise(x, z, 0.0005f, 1f, 1)*0.5f+0.5f;
            //mountainess = 1f;
            float mountainHeight = 650f;
            float ridge = (1f - Math.Abs(octavedNoise(x, z, 0.0032f, 1, 3)))*(float)Math.Pow(mountainess, 1.5);
            float mountain = ((octavedNoise(x, z, 0.0025f, 1,10))) * (float)Math.Pow(mountainess, 0.82);
            mountain *= ridge;
            mountain *= mountainHeight;

            float dessertHill = (1f - Math.Abs(octavedNoise(x, z, 0.0055f, 1, 8))) * (float)Math.Pow(1f-mountainess, 1.2) * 15.3f;

            value += mountain+0.2f+ dessertHill;

            if (value < 0f) value *= 0.1f;

            return value;
        }
    }
}
