
namespace SpaceEngine.Modelling
{
    internal class RawModel
    {
        public float[] positions;
        public float[] colours;
        public int[] indices;

        public RawModel(float[] positions, float[] colours, int[] indices)
        {
            this.positions = positions;
            this.colours = colours; 
            this.indices = indices;
        }
    }
}
