
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using SpaceEngine.Util;
using OpenTK.Windowing.Common;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Entity_Component_System.Components;

namespace SpaceEngine.RenderEngine
{
    internal class MasterRenderer
    {
        
        private ShaderProgram simpleShader = new ShaderProgram("Simple_Vertex", "Simple_Fragment");
        private Matrix4 projectionMatrix;
        private float fieldOfView;
        private float near = 0.1f;
        private float far = 100f;
        private Vector3 sunPosition = new Vector3(-30000f, 3000f, -30000f);
        private ScreenQuadRenderer screenQuadRenderer;
        private GeometryPassRenderer geometryPassRenderer;
        private DeferredLightPassRenderer deferredLightPassRenderer;
        public MasterRenderer() {
            fieldOfView = MyMath.PI/2f;

            screenQuadRenderer = new ScreenQuadRenderer();
            geometryPassRenderer = new GeometryPassRenderer();
            deferredLightPassRenderer= new DeferredLightPassRenderer();
            updateProjectionMatrix();
        }
        private void updateProjectionMatrix()
        {

            float aspect = (float)WindowHandler.resolution.X / (float)WindowHandler.resolution.Y;
            float y_scale = (float)((1f / Math.Tan((fieldOfView / 2f))));
            float x_scale = y_scale / aspect;
            float frustum_length = far - near;
            
            projectionMatrix = Matrix4.Identity;
            projectionMatrix.M11 = x_scale;
            projectionMatrix.M22 = y_scale;
            projectionMatrix.M33 = -((far + near) / frustum_length);
            projectionMatrix.M34 = -1f;
            projectionMatrix.M43 = -((2 * near * far) / frustum_length);
            //projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(fieldOfView, aspect, near, far);
        }

        public void prepareFrame()
        {


        }

        public void finishFrame()
        {



            WindowHandler.getWindow().SwapBuffers();
        }
   
        public void render(List<Entity> modelEntities, Matrix4 viewMatrix, Vector3 viewPosition, List<Entity> pointLights)
        {
            prepareFrame();
            geometryPassRenderer.render(modelEntities, viewMatrix, projectionMatrix);
            
            deferredLightPassRenderer.render(geometryPassRenderer.gBuffer, sunPosition, viewMatrix,projectionMatrix, pointLights);
            simpleShader.bind();
            simpleShader.loadUniformInt("blitTexture", 0);
            screenQuadRenderer.renderTextureToScreen(deferredLightPassRenderer.quadRenderer.getLastOutputTexture());
            simpleShader.stop();
            finishFrame();
        }
        public void update(float delta)
        {

        }
        public void onResize(ResizeEventArgs eventArgs)
        {
            GL.Viewport(0, 0, WindowHandler.resolution.X, WindowHandler.resolution.Y);
            updateProjectionMatrix();
        }
    }
}
