﻿using OpenTK.Windowing.GraphicsLibraryFramework;
using SpaceEngine.Core;
using OpenTK.Mathematics;

namespace SpaceEngine.Entity_Component_System.Components
{
    internal class InputMove : Component
    {
        public override void update(float delta)
        {
            Transformation transformation = owner.getComponent<Transformation>();
            base.update(delta);
            float moveAmount = 150f * delta;
            float turnAmount = 2.5f * delta;
            if (InputHandler.isKeyDown(Keys.W))
            {
                transformation.move(new Vector3(0f, 0f, -moveAmount));
            }
            if (InputHandler.isKeyDown(Keys.S))
            {
                transformation.move(new Vector3(0f, 0f, moveAmount));
            }
            if (InputHandler.isKeyDown(Keys.Q))
            {
                transformation.translate(new Vector3(0f, -moveAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.E))
            {
                transformation.translate(new Vector3(0f, moveAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.A))
            {
                transformation.addRotation(new Vector3(0f, -turnAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.D))
            {
                transformation.addRotation(new Vector3(0f, turnAmount, 0f));
            }
            if (InputHandler.isKeyDown(Keys.R))
            {
                transformation.addRotation(new Vector3(-turnAmount, 0f, 0f));
            }
            if (InputHandler.isKeyDown(Keys.F))
            {
                transformation.addRotation(new Vector3(turnAmount, 0f, 0f));
            }
        }
    }
}
