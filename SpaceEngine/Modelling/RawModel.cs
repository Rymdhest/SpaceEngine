
using SpaceEngine.RenderEngine;
using static SpaceEngine.RenderEngine.MasterRenderer;

namespace SpaceEngine.Modelling
{
    internal class RawModel
    {
        public float[] positions;
        public float[] colors;
        public float[]? normals;
        public int[] indices;
        public MasterRenderer.Pipeline pipeline;
        public RawModel(float[] positions, float[] colors, int[] indices, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.FLAT_SHADING)
        {
            this.positions = positions;
            this.colors = colors; 
            this.indices = indices;
            this.normals = null;
            this.pipeline = pipeline;
        }
        public RawModel(float[] positions, float[] colors, float[] normals, int[] indices, MasterRenderer.Pipeline pipeline = MasterRenderer.Pipeline.SMOOTH_SHADING)
        {
            this.positions = positions;
            this.colors = colors;
            this.normals = normals;
            this.indices = indices;
            this.pipeline = pipeline;
        }

        public void cleanUp()
        {

        }
    }
}
