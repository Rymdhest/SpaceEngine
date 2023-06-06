

using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System.Components;
using static System.Formats.Asn1.AsnWriter;

namespace SpaceEngine.Util
{
    internal class MyMath
    {
        public static readonly float PI = (float)Math.PI;

        public static float sin(float x)
        {
            return (float)Math.Sin(x);
        }
        public static float cos(float x)
        {
            return (float)Math.Cos(x);
        }
        public static Matrix4 createTransformationMatrix(Transformation transformation)
        {
            return createTransformationMatrix(transformation.position, transformation.rotation, transformation.scale);
        }
            public static Matrix4 createTransformationMatrix(Vector3 position, Vector3 rotation, float scale)
        {

            Matrix4 matrix = Matrix4.Identity;
            matrix = matrix * createRotationMatrix(rotation);
            matrix = matrix * Matrix4.CreateTranslation(position);
            matrix = matrix * Matrix4.CreateScale(scale);
            return matrix;
        }
        public static Matrix4 createViewMatrix(Transformation transformation)
        {
            return createViewMatrix(transformation.position, transformation.rotation);
        }
            public static Matrix4 createViewMatrix(Vector3 position, Vector3 rotation)
        {

            Matrix4 matrix = Matrix4.Identity;
            matrix = matrix * Matrix4.CreateTranslation(-position);
            matrix = matrix * createRotationMatrix(rotation);
            return matrix;
        }
        public static Matrix4 createRotationMatrix(Vector3 rotation)
        {

            Matrix4 matrix = Matrix4.Identity;
            matrix = matrix * Matrix4.CreateRotationZ(rotation.Z);
            matrix = matrix * Matrix4.CreateRotationY(rotation.Y);
            matrix = matrix * Matrix4.CreateRotationX(rotation.X);
            return matrix;
        }
        public static float clamp(float number, float min, float max)
        {
            if (number < min) return min;
            if (number > max) return max;
            return number;
        }
        public static float clamp01(float number)
        {
            return clamp(number, 0.0f, 1.0f);
        }
        public static float lerp(float amount, float left, float right)
        {
            return (1.0f - amount) * left + amount * right;
        }
        public static Vector3 calculateFaceNormal(Vector3 v3, Vector3 v2, Vector3 v1)
        {
            float aX, aY, aZ, bX, bY, bZ;

            aX = v2.X - v1.X;
            aY = v2.Y - v1.Y;
            aZ = v2.Z - v1.Z;

            bX = v3.X - v1.X;
            bY = v3.Y - v1.Y;
            bZ = v3.Z - v1.Z;

            return new Vector3((aY * bZ) - (aZ * bY), (aZ * bX) - (aX * bZ), (aX * bY) - (aY * bX));
        }
    }
}
