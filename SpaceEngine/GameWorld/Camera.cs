
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceEngine.Core;
using SpaceEngine.Util;

namespace SpaceEngine.GameWorld
{
    internal class Camera : Entity
    {
        public Camera(Vector3 postion, Vector3 rotation) : base(postion, rotation)
        {
        }
        public override void update(float delta)
        {
            base.update(delta);
            float moveAmount = 5f*delta;
            float turnAmount = 2.5f * delta;
            if (InputHandler.isKeyDown(Keys.W))
            {
                move(new Vector3(0f, 0f, -moveAmount));
            }
            if (InputHandler.isKeyDown(Keys.S))
            {
                move(new Vector3(0f, 0f, moveAmount));
            }
            if (InputHandler.isKeyDown(Keys.Q))
            {
                move(new Vector3(0f, -moveAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.E))
            {
                move(new Vector3(0f, moveAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.A))
            {
                base.addRotation(new Vector3(0f, -turnAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.D))
            {
                base.addRotation(new Vector3(0f, turnAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.R))
            {
                base.addRotation(new Vector3(-turnAmount,0f , 0f));
            }
            if (InputHandler.isKeyDown(Keys.F))
            {
                base.addRotation(new Vector3(turnAmount, 0f, 0f));
            }
        }
        public Matrix4 createViewMatrix()
        {
            return MyMath.createViewMatrix(base.getPosition(), base.getRotation());
        }
        private void move(Vector3 direction)
        {
            Vector4 moveVector = new Vector4(direction.X, direction.Y, direction.Z, 1.0f);
            Matrix4 rotationMatrix = MyMath.createRotationMatrix(base.getRotation());
            moveVector = rotationMatrix* moveVector;
            base.translate(moveVector);
        }
    }
}
