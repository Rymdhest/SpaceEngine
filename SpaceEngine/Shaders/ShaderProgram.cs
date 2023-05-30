
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace SpaceEngine.Shaders
{
    internal class ShaderProgram
    {
        private int programID;
        private int vertexShaderID;
        private int fragmentShaderID;
        private int geometryShaderID;

        private Dictionary<string, int> uniforms;
        public ShaderProgram(string vertexFile, string fragmentFile)
        {
            vertexShaderID = loadShader(vertexFile, ShaderType.VertexShader);
            fragmentShaderID = loadShader(fragmentFile, ShaderType.FragmentShader);
            programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);
            GL.LinkProgram(programID);
            GL.ValidateProgram(programID);

            uniforms = new Dictionary<string, int>();
            extractAllUniformsToDictionary(vertexFile);
            extractAllUniformsToDictionary(fragmentFile);

        }
        public ShaderProgram(string vertexFile, string fragmentFile, string geometryFile)
        {
            vertexShaderID = loadShader(vertexFile, ShaderType.VertexShader);
            fragmentShaderID = loadShader(fragmentFile, ShaderType.FragmentShader);
            geometryShaderID = loadShader(geometryFile, ShaderType.GeometryShader);
            programID = GL.CreateProgram();
            GL.AttachShader(programID, vertexShaderID);
            GL.AttachShader(programID, fragmentShaderID);
            GL.AttachShader(programID, geometryShaderID);
            GL.LinkProgram(programID);
            GL.ValidateProgram(programID);

            uniforms = new Dictionary<string, int>();
            extractAllUniformsToDictionary(vertexFile);
            extractAllUniformsToDictionary(fragmentFile);
            extractAllUniformsToDictionary(geometryFile);

        }

        public void loadUniformFloat(string variableName, float value)
        {
            GL.Uniform1(uniforms[variableName], value);
        }
        public void loadUniformVector2f(string variableName, Vector2 value)
        {
            GL.Uniform2(uniforms[variableName], value);
        }
        public void loadUniformVector3f(string variableName, Vector3 value)
        {
            GL.Uniform3(uniforms[variableName], value);
        }
        public void loadUniformVector4f(string variableName, Vector4 value)
        {
            GL.Uniform4(uniforms[variableName], value);
        }
        public void loadUniformMatrix4f(string variableName, Matrix4 value)
        {
            GL.UniformMatrix4(uniforms[variableName],true, ref value);
        }

        private int loadShader(string name, ShaderType type)
        {
            string fullPath = "../../../Shaders/"+ name+".glsl";
            int shaderID = GL.CreateShader(type);

            GL.ShaderSource(shaderID, File.ReadAllText(fullPath));
            GL.CompileShader(shaderID);

            GL.GetShader(shaderID, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True){
                string infoLog = GL.GetShaderInfoLog(shaderID);
                Console.WriteLine($"Could not compile shader ({name}).\n\n{infoLog}");
            }

            return shaderID;
        }

        private void extractAllUniformsToDictionary(string name)
        {

            string fullPath = "../../../Shaders/" + name + ".glsl";
            foreach(string line in File.ReadLines(fullPath))
            {
                string[] words = line.Split(" ");
                if (words.Length == 3) {
                    if (words[0] =="uniform")
                    {
                        string variableName = words[2].Remove(words[2].Length-1, 1);
                        if (!uniforms.ContainsKey(variableName))
                        {
                            int location = GL.GetUniformLocation(programID, variableName);
                            if (location == -1) Console.WriteLine("Something went wrong getting uniform for "+variableName+" in "+name);
                            {
                                
                            }
                            uniforms.Add(variableName, location);
                        }

                    }
                }
            }
        }

        public void bind()
        {
            GL.UseProgram(programID);
        }
        public void stop()
        {
            GL.UseProgram(0);
        }
        public void cleanUp()
        {
            GL.DetachShader(programID, vertexShaderID);
            GL.DetachShader(programID, fragmentShaderID);
            GL.DetachShader(programID, geometryShaderID);
            GL.DeleteShader(vertexShaderID);
            GL.DeleteShader(fragmentShaderID);
            GL.DeleteShader(geometryShaderID);
            GL.DeleteProgram(programID);
        }
    }
}
