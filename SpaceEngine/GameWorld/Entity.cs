
using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;
using SpaceEngine.Util;

namespace SpaceEngine.GameWorld
{
    internal class Entity
    {
        private Vector3 position;
        private Vector3 rotation;
        private float scale;


        public Entity(Vector3 postion, Vector3 rotation, float scale = 1.0f)
        {
            this.position = postion;
            this.rotation = rotation;
            this.scale = scale;
        }

        public void update(float delta)
        {
            rotation.X += 1.14f*delta;
            rotation.Z += 0.514f * delta;
            position.X = MyMath.sin(Engine.EngineClock);
            position.Y = MyMath.cos(Engine.EngineClock);
        }

        public Matrix4 createTransformationMatrix()
        {

            Matrix4 matrix = Matrix4.Identity;
            matrix = matrix * Matrix4.CreateRotationX(rotation.X);
            matrix = matrix * Matrix4.CreateRotationY(rotation.Y);
            matrix = matrix * Matrix4.CreateRotationZ(rotation.Z);
            matrix = matrix * Matrix4.CreateTranslation(position);
            matrix = matrix * Matrix4.CreateScale(scale);
            return matrix;
        }
    }
}
