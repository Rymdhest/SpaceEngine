

using OpenTK.Mathematics;
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
        public static Matrix4 createTransformationMatrix(Vector3 position, Vector3 rotation, float scale)
        {

            Matrix4 matrix = Matrix4.Identity;
            matrix = matrix * createRotationMatrix(rotation);
            matrix = matrix * Matrix4.CreateTranslation(position);
            matrix = matrix * Matrix4.CreateScale(scale);
            return matrix;
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
    }
}
