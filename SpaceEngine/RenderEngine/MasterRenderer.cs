
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Core;

namespace SpaceEngine.RenderEngine
{
    internal class MasterRenderer
    {
        public enum Pipeline {FLAT_SHADING, SMOOTH_SHADING, OTHER};
        public static ShaderProgram simpleShader = new ShaderProgram("Simple_Vertex", "Simple_Fragment");
        private Matrix4 projectionMatrix;
        private float fieldOfView;
        private float near = 0.1f;
        private float far = 1000f;
        private Vector3 sunPosition = new Vector3(0f,0f,0f);
        private ScreenQuadRenderer screenQuadRenderer;
        private GeometryPassRenderer geometryPassRenderer;
        private DeferredLightPassRenderer deferredLightPassRenderer;
        private PostProcessingRenderer postProcessingRenderer;
        public MasterRenderer() {
            fieldOfView = MathF.PI/2f;

            screenQuadRenderer = new ScreenQuadRenderer();
            geometryPassRenderer = new GeometryPassRenderer();
            deferredLightPassRenderer= new DeferredLightPassRenderer();
            postProcessingRenderer= new PostProcessingRenderer();
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
   
        public void render(ComponentSystem flatShadeEntities, ComponentSystem SmoothShadeEntities, Matrix4 viewMatrix, Vector3 viewPosition, ComponentSystem pointLights)
        {
            prepareFrame();
            geometryPassRenderer.render(flatShadeEntities, SmoothShadeEntities, viewMatrix, projectionMatrix);
            deferredLightPassRenderer.render(screenQuadRenderer, geometryPassRenderer.gBuffer, sunPosition, viewMatrix,projectionMatrix, pointLights);
            postProcessingRenderer.doPostProcessing(screenQuadRenderer, geometryPassRenderer.gBuffer, sunPosition, viewPosition, viewMatrix);

            simpleShader.bind();
            simpleShader.loadUniformInt("blitTexture", 0);
            screenQuadRenderer.renderTextureToScreen(screenQuadRenderer.getLastOutputTexture());
            //screenQuadRenderer.renderTextureToScreen(geometryPassRenderer.gBuffer.getRenderAttachment(2));
            simpleShader.unBind();
            finishFrame();
        }
        public void update(float delta)
        {
            float distance = 999999f;
            float speed = 0.5f;
            sunPosition.X = MathF.Sin(Engine.EngineDeltaClock* speed) * distance;
            sunPosition.Y = (MathF.Cos(Engine.EngineDeltaClock* speed)+0.7f) * distance;
            sunPosition.Z = MathF.Cos(Engine.EngineDeltaClock* speed) * distance;
        }
        public void onResize(ResizeEventArgs eventArgs)
        {
            screenQuadRenderer.onResize(eventArgs);
            geometryPassRenderer.onResize(eventArgs);
            postProcessingRenderer.onResize(eventArgs);
            GL.Viewport(0, 0, WindowHandler.resolution.X, WindowHandler.resolution.Y);
            updateProjectionMatrix();
        }
    }
}
