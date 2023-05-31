
using OpenTK.Mathematics;
using SpaceEngine.Core;
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

        public virtual void update(float delta)
        {

        }

        public Matrix4 createTransformationMatrix()
        {
            return MyMath.createTransformationMatrix(position, rotation, scale);
        }

        public void setPosition(Vector3 newPos)
        {
            position = newPos;
        }

        public void translate(Vector3 translation)
        {
            position += translation;
        }
        public void translate(Vector4 translation)
        {
            translate(new Vector3(translation.X, translation.Y, translation.Z));
        }
        public void addRotation(Vector3 rotationAdd)
        {
            rotation += rotationAdd;
        }
        public Vector3 getPosition()
        {
            return position;
        }
        public Vector3 getRotation()
        {
            return rotation;
        }
        public float getScale()
        {
            return scale;
        }
    }
}
