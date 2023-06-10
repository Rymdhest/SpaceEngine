
using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;
using System.Reflection.Emit;
using System;

namespace SpaceEngine.Modelling
{
    internal class MeshGenerator
    {

        public static RawModel generateCylinder(List<Vector3> rings, int polygonsPerRing, Vector3 color)
        {
            float PI = MathF.PI;
            List<Vector3> positions = new List<Vector3>();
            List<Vector3> colors = new List<Vector3>();
            List<int> indices = new List<int>();

            for (int ring = 0; ring<rings.Count; ring++)
            {
                for(int detail = 0; detail<polygonsPerRing; detail++)
                {
                    float x1 = MathF.Sin(2f * PI * ((float)detail / polygonsPerRing)) * rings[ring].X;
                    float z1 = MathF.Cos(2f * PI * ((float)detail / polygonsPerRing)) * rings[ring].Z;
                    float y1 = rings[ring].Y;
                    Vector3 p1 = new Vector3(x1, y1, z1);

                    positions.Add(p1);
                    colors.Add(color);

                    if (ring < rings.Count-1)
                    {
                        indices.Add((ring + 1) * polygonsPerRing + (detail + 0) % polygonsPerRing);
                        indices.Add((ring + 0) * polygonsPerRing + (detail + 0) % polygonsPerRing);
                        indices.Add((ring + 0) * polygonsPerRing + (detail + 1) % polygonsPerRing);

                        indices.Add((ring + 1) * polygonsPerRing + (detail + 0) % polygonsPerRing);
                        indices.Add((ring + 0) * polygonsPerRing + (detail + 1) % polygonsPerRing);
                        indices.Add((ring + 1) * polygonsPerRing + (detail + 1) % polygonsPerRing);
                    }
                }
            }
            return new RawModel(positions, colors, indices, MasterRenderer.Pipeline.FLAT_SHADING);
        }

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

            int[] indices = {0,1,3, 3,1,2, //top
                        0,4,1, 1,4,5,  //front
                        1,5,6, 2,1,6,  // right
                        6,7,2, 3,2,7,  //back
                        3,7,4, 0,3,4,  //left
                        6,5,7, 7,5,4};  //bot

            return new RawModel(positions, colours, indices);
        }

        public static RawModel generateIcosahedron(Vector3 scale, Vector3 color, MasterRenderer.Pipeline pipeline)
        {
            float[] positions = {
         0.000000f, -1.000000f, 0.000000f,
         0.723607f, -0.447220f, 0.525725f,
         -0.276388f, -0.447220f, 0.850649f,
         -0.894426f, -0.447216f, 0.000000f,
         -0.276388f, -0.447220f, -0.850649f,
         0.723607f, -0.447220f, -0.525725f,
         0.276388f, 0.447220f, 0.850649f,
         -0.723607f, 0.447220f, 0.525725f,
         -0.723607f, 0.447220f, -0.525725f,
         0.276388f, 0.447220f, -0.850649f,
          0.894426f, 0.447216f, 0.000000f,
         0.000000f, 1.000000f, 0.000000f,
         -0.162456f, -0.850654f, 0.499995f,
         0.425323f, -0.850654f, 0.309011f,
         0.262869f, -0.525738f, 0.809012f,
         0.850648f, -0.525736f, 0.000000f,
         0.425323f, -0.850654f, -0.309011f,
         -0.525730f, -0.850652f, 0.000000f,
         -0.688189f, -0.525736f, 0.499997f,
         -0.162456f, -0.850654f, -0.499995f,
         -0.688189f, -0.525736f, -0.499997f,
         0.262869f, -0.525738f, -0.809012f,
         0.951058f, 0.000000f, 0.309013f,
         0.951058f, 0.000000f, -0.309013f,
         0.000000f, 0.000000f, 1.000000f,
         0.587786f, 0.000000f, 0.809017f,
         -0.951058f, 0.000000f, 0.309013f,
         -0.587786f, 0.000000f, 0.809017f,
         -0.587786f, 0.000000f, -0.809017f,
         -0.951058f, 0.000000f, -0.309013f,
         0.587786f, 0.000000f, -0.809017f,
         0.000000f, 0.000000f, -1.000000f,
         0.688189f, 0.525736f, 0.499997f,
         -0.262869f, 0.525738f, 0.809012f,
         -0.850648f, 0.525736f, 0.000000f,
         -0.262869f, 0.525738f, -0.809012f,
         0.688189f, 0.525736f, -0.499997f,
         0.162456f, 0.850654f, 0.499995f,
         0.525730f, 0.850652f, 0.000000f,
         -0.425323f, 0.850654f, 0.309011f,
         -0.425323f, 0.850654f, -0.309011f,
         0.162456f, 0.850654f, -0.499995f,};

            float[] colours = new float[positions.Length];
            int[] indices = {
            1, 14, 13,
                2, 14, 16,
                1, 13, 18,
                1, 18, 20,
                1, 20, 17,
                2, 16, 23,
                3, 15, 25,
                4, 19, 27,
                5, 21, 29,
                6, 22, 31,
                2, 23, 26,
                3, 25, 28,
                4, 27, 30,
                5, 29, 32,
                6, 31, 24,
                7, 33, 38,
                8, 34, 40,
                9, 35, 41,
                10, 36, 42,
                11, 37, 39,
                39, 42, 12,
                39, 37, 42,
                37, 10, 42,
                42, 41, 12,
                42, 36, 41,
                36, 9, 41,
                41, 40, 12,
                41, 35, 40,
                35, 8, 40,
                40, 38, 12,
                40, 34, 38,
                34, 7, 38,
                38, 39, 12,
                38, 33, 39,
                33, 11, 39,
                24, 37, 11,
                24, 31, 37,
                31, 10, 37,
                32, 36, 10,
                32, 29, 36,
                29, 9, 36,
                30, 35, 9,
                30, 27, 35,
                27, 8, 35,
                28, 34, 8,
                28, 25, 34,
                25, 7, 34,
                26, 33, 7,
                26, 23, 33,
                23, 11, 33,
                31, 32, 10,
                31, 22, 32,
                22, 5, 32,
                29, 30, 9,
                29, 21, 30,
                21, 4, 30,
                27, 28, 8,
                27, 19, 28,
                19, 3, 28,
                25, 26, 7,
                25, 15, 26,
                15, 2, 26,
                23, 24, 11,
                23, 16, 24,
                16, 6, 24,
                17, 22, 6,
                17, 20, 22,
                20, 5, 22,
                20, 21, 5,
                20, 18, 21,
                18, 4, 21,
                18, 19, 4,
                18, 13, 19,
                13, 3, 19,
                16, 17, 6,
                16, 14, 17,
                14, 1, 17,
                13, 15, 3,
                13, 14, 15,
                14, 2, 15,
    };
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] -= 1;
            }


            float[] normals = new float[positions.Length];
            if (pipeline == MasterRenderer.Pipeline.SMOOTH_SHADING)
            {
                for (int i = 0; i < normals.Length; i += 3)
                {
                    Vector3 normal = new Vector3(positions[i], positions[i + 1], positions[i + 2]);
                    normal.Normalize();
                    normals[i] = normal.X;
                    normals[i + 1] = normal.Y;
                    normals[i + 2] = normal.Z;
                }
            }


            for (int i = 0; i < positions.Length; i += 3)
            {
                positions[i] *= scale.X;
                positions[i + 1] *= scale.Y;
                positions[i + 2] *= scale.Z;
            }


            for (int i = 0; i < colours.Length; i += 3)
            {
                colours[i] = color.X;
                colours[i + 1] = color.Y;
                colours[i + 2] = color.Z;
            }

            if (pipeline == MasterRenderer.Pipeline.FLAT_SHADING)
            {
                RawModel rawModel = new RawModel(positions, colours, indices);
                return rawModel;
            }
            else if (pipeline == MasterRenderer.Pipeline.SMOOTH_SHADING)
            {
                return new RawModel(positions, colours, normals, indices);
            }
            else
            {
                return new RawModel(positions, colours, indices, pipeline);
            }
            

        }
    }
}
