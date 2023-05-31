
namespace SpaceEngine.Modelling
{
    internal class RawModel
    {
        public float[] positions;
        public float[] colors;
        public int[] indices;

        public RawModel(float[] positions, float[] colors, int[] indices)
        {
            this.positions = positions;
            this.colors = colors; 
            this.indices = indices;
        }
    }
}
