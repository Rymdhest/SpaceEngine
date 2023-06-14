
using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;
using SpaceEngine.Util;
using Noise;
using SpaceEngine.Entity_Component_System;
using OpenTK.Graphics.OpenGL;
namespace SpaceEngine.Modelling
{
    internal class TerrainChunk : Component
    {
        private float spaceBetweenVertices;
        private float[,] heightsLocalGridSpace;
        private Vector2 positionChunkGlobalWorld;
        private int resolution;
        private float worldSize;
        private int normalHeightMap = -1;
        private static OpenSimplexNoise noise = new OpenSimplexNoise(4499954);
        public TerrainChunk(Vector2 position, float WorldSize, int resolution)
        {
            this.resolution = resolution;
            this.worldSize = WorldSize;
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
        public void generateTextureMaps()
        {
            int res = resolution;
            var pixels = new float[4 * res * res];
            for (int z = 0; z < res; z++)
            {
                for (int x = 0; x < res; x++)
                {   
                    int i = z* res + x;
                    Vector3 normal = calculateVertexNormal(x, z);
                    pixels[i * 4 + 0] = normal.X;
                    pixels[i * 4 + 1] = normal.Y;
                    pixels[i * 4 + 2] = normal.Z;
                    pixels[i * 4 + 3] = heightsLocalGridSpace[x,z];
                }
            }
            normalHeightMap = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, normalHeightMap);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba16f, res, res, 0, PixelFormat.Rgba, PixelType.Float, pixels);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (float)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (float)TextureWrapMode.ClampToEdge);
        }
        public int getNormalHeightMap()
        {
            return normalHeightMap;
        }
        public float getWorldSize()
        {
            return worldSize;
        }
        public Vector3 getNormalFlatAt(Vector2 position)
        {
            Vector3 normal;
            float d = spaceBetweenVertices;
            Vector2i grid = fromGlobalWorldToLocalGrid(position);
            float xCoord = (position.X % d) / d;
            float zCoord = (position.Y % d) / d;

            if (xCoord < 0) xCoord += 1;
            if (zCoord < 0) zCoord += 1;

            d = 1f;
            if (xCoord <= (1 - zCoord))
            {
                Vector3 v1 = new Vector3(0, heightsLocalGridSpace[grid.X, grid.Y], 0);
                Vector3 v2 = new Vector3(d, heightsLocalGridSpace[grid.X + 1, grid.Y], 0);
                Vector3 v3 = new Vector3(0, heightsLocalGridSpace[grid.X, grid.Y + 1], d);
                normal = MyMath.calculateFaceNormal(v1, v2, v3);
            }
            else
            {
                Vector3 v1 = new Vector3(d, heightsLocalGridSpace[grid.X + 1, grid.Y], 0);
                Vector3 v2 = new Vector3(d, heightsLocalGridSpace[grid.X + 1, grid.Y + 1], d);
                Vector3 v3 = new Vector3(0, heightsLocalGridSpace[grid.X, grid.Y + 1], d);
                normal = MyMath.calculateFaceNormal(v1, v2, v3);
            }


            return normal;
        }

        public override void cleanUp()
        {
            base.cleanUp();
            if (normalHeightMap != -1)
            GL.DeleteTexture(normalHeightMap);
        }
            public float getPolygonHeightAt(Vector2 position)
        {
            float height;
            float d = spaceBetweenVertices;
            Vector2i grid = fromGlobalWorldToLocalGrid(position);
            float xCoord = ( position.X % d)/d;
            float zCoord = (position.Y % d)/d;

            if (xCoord <0) xCoord += 1;
            if (zCoord < 0) zCoord += 1;

            d = 1f;
            if (xCoord <= (1 - zCoord))
            {
                Vector3 v1 = new Vector3(0, heightsLocalGridSpace[grid.X, grid.Y], 0);
                Vector3 v2 = new Vector3(d, heightsLocalGridSpace[grid.X + 1, grid.Y], 0);
                Vector3 v3 = new Vector3(0, heightsLocalGridSpace[grid.X, grid.Y + 1], d);
                height = MyMath.barryCentric(v1, v2, v3, new Vector2(xCoord, zCoord));
            } else
            {
                Vector3 v1 = new Vector3(d, heightsLocalGridSpace[grid.X+1, grid.Y], 0);
                Vector3 v2 = new Vector3(d, heightsLocalGridSpace[grid.X+1, grid.Y+1], d);
                Vector3 v3 = new Vector3(0, heightsLocalGridSpace[grid.X, grid.Y + 1], d);
                height = MyMath.barryCentric(v1, v2, v3, new Vector2(xCoord, zCoord));
            }


            return height;
        }
        public Vector2i fromGlobalWorldToLocalGrid(Vector2 world)
        {
            Vector2 localWorld = world-positionChunkGlobalWorld;
            Vector2i localGrid = (Vector2i)(localWorld / spaceBetweenVertices);
            return localGrid;
        }

        public RawModel generateRawModel(MasterRenderer.Pipeline pipeline)
        {
            
            int totalVertices = resolution * resolution;
            float[] positions = new float[totalVertices * 3];
            float[] colors = new float[totalVertices * 3];
            float[] materials = new float[totalVertices * 3];
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

                    float specularity;
                    float bloom = 0.05f;
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
                    specularity = MyMath.lerp(1f - MyMath.clamp01(normal.Y), 0f, 1f);
                    if (positionLocalWorld.Y <= 1.85f && normal.Y > 0.65f)
                    {
                        color = sandColor;
                    }
                    if (positionLocalWorld.Y <= 0.0f)
                    {
                        color = waterColor;
                        specularity = 1.0f;
                        bloom = 0.2f;
                    }
                    colors[vertexPointer * 3] = color.X;
                    colors[vertexPointer * 3 + 1] = color.Y;
                    colors[vertexPointer * 3 + 2] = color.Z;


                    materials[vertexPointer * 3] = specularity;
                    materials[vertexPointer * 3 + 1] = bloom;
                    materials[vertexPointer * 3 + 2] = 0f;

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
            RawModel terrainModel = new RawModel(positions, colors, materials, normals, indices, pipeline);
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

            float dessertHill = (1f - Math.Abs(octavedNoise(x, z, 0.0055f, 1, 5))) * (float)Math.Pow(1f-mountainess, 1.2) * 15.3f;

            value += mountain+0.2f+ dessertHill;

            if (value < 0f) value *= 0.1f;

            return value;
        }
    }
}
