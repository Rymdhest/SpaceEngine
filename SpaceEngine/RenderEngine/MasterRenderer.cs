
using OpenTK.Mathematics;
using SpaceEngine.Shaders;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Core;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Modelling;

namespace SpaceEngine.RenderEngine
{
    internal class MasterRenderer
    {
        public enum Pipeline {FLAT_SHADING, SMOOTH_SHADING, POST_GEOMETRY, OTHER};
        public static ShaderProgram simpleShader = new ShaderProgram("Simple_Vertex", "Simple_Fragment");
        private Matrix4 projectionMatrix;
        private float fieldOfView;
        private float near = 0.1f;
        private float far = 1000f;
        private ScreenQuadRenderer screenQuadRenderer;
        private GeometryPassRenderer geometryPassRenderer;
        private DeferredLightPassRenderer deferredLightPassRenderer;
        private PostProcessingRenderer postProcessingRenderer;
        private PostGeometryRenderer postGeometryRenderer;
        public MasterRenderer() {
            fieldOfView = MathF.PI/2f;

            screenQuadRenderer = new ScreenQuadRenderer();
            geometryPassRenderer = new GeometryPassRenderer();
            deferredLightPassRenderer= new DeferredLightPassRenderer();
            postProcessingRenderer= new PostProcessingRenderer();
            postGeometryRenderer = new PostGeometryRenderer();
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
   
        public void render(Matrix4 viewMatrix, Vector3 viewPosition, Entity sunEntity, ComponentSystem pointLights, TerrainManager terrainManager)
        {
            prepareFrame();
            geometryPassRenderer.render(EntityManager.flatShadingSystem, EntityManager.smoothShadingSystem, viewMatrix, projectionMatrix, terrainManager, viewPosition);
            deferredLightPassRenderer.render(screenQuadRenderer, geometryPassRenderer.gBuffer, sunEntity, viewMatrix,projectionMatrix, pointLights);
            postProcessingRenderer.doPostProcessing(screenQuadRenderer, geometryPassRenderer.gBuffer, sunEntity, viewPosition, viewMatrix);
            postGeometryRenderer.render(EntityManager.postGeometrySystem, viewMatrix, projectionMatrix);
            simpleShader.bind();
            simpleShader.loadUniformInt("blitTexture", 0);
            screenQuadRenderer.renderTextureToScreen(screenQuadRenderer.getLastOutputTexture());
            //screenQuadRenderer.renderTextureToScreen(geometryPassRenderer.gBuffer.getRenderAttachment(2));
            simpleShader.unBind();
            finishFrame();
        }
        public void update(float delta)
        {

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
