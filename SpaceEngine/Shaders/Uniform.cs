
namespace SpaceEngine.Shaders
{
    internal class Uniform
    {
        private string variableName;
        private int uniformLocation;
        private int type;

        public Uniform(string variableName, int uniformLocation, int type)
        {
            this.variableName = variableName;
            this.uniformLocation = uniformLocation;
            this.type = type;
        }
        public string getVariableName()
        {
            return variableName;
        }
        public int getType()
        {
            return type;
        }
        public int getUniformLocation()
        {
            return uniformLocation;
        }
    }
}
