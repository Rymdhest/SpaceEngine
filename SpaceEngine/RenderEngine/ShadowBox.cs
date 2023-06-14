using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Entity_Component_System.Components;

namespace SpaceEngine.RenderEngine
{
    internal class ShadowBox
    {
        private float OFFSET = 20;
        private static Vector4 UP = new Vector4(0, 1, 0, 0);
        private static Vector4 FORWARD = new Vector4(0, 0, -1, 0);


        private Vector3 min;
        private Vector3 max;

        private Vector2 far;
        private Vector2 near;

        private Matrix4 lightViewMatrix;

        public ShadowBox(Matrix4 lightViewMatrix)
        {

            min = new Vector3(11);
            max = new Vector3(55);

            near = new Vector2(64);
            far = new Vector2(12);

            calculateWidthsAndHeights();
            this.lightViewMatrix = lightViewMatrix;
        }

        public void update(Entity Camera)
        {
            calculateWidthsAndHeights();
            Matrix4 rotation = Matrix4.Identity;

            rotation *= Matrix4.CreateRotationY(-Camera.getComponent<Transformation>().rotation.Y);
            rotation *= Matrix4.CreateRotationX(-Camera.getComponent<Transformation>().rotation.X);
            Vector3 forwardVector = (rotation * FORWARD).Xyz;

            Vector3 toFar = new Vector3(forwardVector);
            toFar *= ShadowRenderer.shadowDistance;
            Vector3 toNear = new Vector3(forwardVector);
            toNear *= MasterRenderer.near;
            Vector3 centerNear = toNear + Camera.getComponent<Transformation>().position;
            Vector3 centerFar = toFar + Camera.getComponent<Transformation>().position;

            Vector4[] points = calculateFrustumVertices(rotation, forwardVector, centerNear,
                    centerFar);

            bool first = true;
            foreach (Vector4 point in points)
            {
                if (first)
                {
                    min.X = point.X;
                    max.X = point.X;
                    min.Y = point.Y;
                    max.Y = point.Y;
                    min.Z = point.Z;
                    max.Z = point.Z;
                    first = false;
                    continue;
                }
                if (point.X > max.X)
                {
                    max.X = point.X;
                }
                else if (point.X < min.X)
                {
                    min.X = point.X;
                }
                if (point.Y > max.Y)
                {
                    max.Y = point.Y;
                }
                else if (point.Y < min.Y)
                {
                    min.Y = point.Y;
                }
                if (point.Z > max.Z)
                {
                    max.Z = point.Z;
                }
                else if (point.Z < min.Z)
                {
                    min.Z = point.Z;
                }
            }
            max.Z += OFFSET;
        }
        private Vector4[] calculateFrustumVertices(Matrix4 rotation, Vector3 forwardVector,
        Vector3 centerNear, Vector3 centerFar)
        {
            Vector3 upVector = (rotation * UP).Xyz;
            Vector3 rightVector = Vector3.Cross(forwardVector, upVector);
            Vector3 downVector = -upVector;
            Vector3 leftVector = -rightVector;
            Vector3 farTop = centerFar + (upVector * far.Y);
            Vector3 farBottom = centerFar + (downVector * far.Y);
            Vector3 nearTop = centerNear + (upVector * near.Y);
            Vector3 nearBottom = centerNear + (downVector * near.Y);
            Vector4[] points = new Vector4[8];
            points[0] = calculateLightSpaceFrustumCorner(farTop, rightVector, far.X);
            points[1] = calculateLightSpaceFrustumCorner(farTop, leftVector, far.X);
            points[2] = calculateLightSpaceFrustumCorner(farBottom, rightVector, far.X);
            points[3] = calculateLightSpaceFrustumCorner(farBottom, leftVector, far.X);
            points[4] = calculateLightSpaceFrustumCorner(nearTop, rightVector, near.X);
            points[5] = calculateLightSpaceFrustumCorner(nearTop, leftVector, near.X);
            points[6] = calculateLightSpaceFrustumCorner(nearBottom, rightVector, near.X);
            points[7] = calculateLightSpaceFrustumCorner(nearBottom, leftVector, near.X);
            return points;
        }
        private Vector4 calculateLightSpaceFrustumCorner(Vector3 startPoint, Vector3 direction,
        float width)
        {
            Vector3 point = startPoint + (direction * width);
            Vector4 point4 = new Vector4(point.X, point.Y, point.Z, 1f);
            
            point4 =  lightViewMatrix * point4; 
            return point4;
        }

        public Vector3 getSize()
        {
            return max-min;
        }
        public Vector3 getCenter()
        {
            float x = (min.X + max.X) / 2f;
            float y = (min.Y + max.Y) / 2f;
            float z = (min.Z + max.Z) / 2f;
            Vector4 cen = new Vector4(x, y, z, 1);
            Matrix4 invertedLight = Matrix4.Invert(lightViewMatrix);
            return (invertedLight * cen).Xyz;
        }
        public void calculateWidthsAndHeights()
        {
            float aspectRatio = WindowHandler.resolution.X/ (float)WindowHandler.resolution.Y;
            far.X = ShadowRenderer.shadowDistance * MathF.Tan(MasterRenderer.fieldOfView);
            near.X = MasterRenderer.near * MathF.Tan(MasterRenderer.fieldOfView);
            far.Y = far.X / aspectRatio;
            near.Y = near.X / aspectRatio;
        }
    }
}
