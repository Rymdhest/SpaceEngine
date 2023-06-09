using SpaceEngine.Shaders;

namespace SpaceEngine.RenderEngine
{
    internal class PostProcessingRenderer
    {

        private ShaderProgram FXAAShader = new ShaderProgram("Simple_Vertex", "FXAA_Fragment");

        public PostProcessingRenderer()
        {
            FXAAShader.bind();
            FXAAShader.loadUniformInt("l_tex", 0);
            FXAAShader.unBind();
        }

        public void doPostProcessing(ScreenQuadRenderer renderer, FrameBuffer gBuffer)
        {
            applyFXAA(renderer);
        }

        private void applyFXAA(ScreenQuadRenderer renderer)
        {
            FXAAShader.bind();
            FXAAShader.loadUniformVector2f("win_size", WindowHandler.resolution);
            renderer.renderTextureToNextFrameBuffer(renderer.getLastOutputTexture());
            FXAAShader.unBind();
        }
    }
}
