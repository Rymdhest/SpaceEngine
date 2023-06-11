

using OpenTK.Graphics.OpenGL;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.Entity_Component_System.Components;
using System.Transactions;

namespace SpaceEngine.RenderEngine
{
    internal class Model : Component
    {
        private int vaoID;
        private int[] VBOS;
        private int vertexCount;
        private MasterRenderer.Pipeline pipeline;

        public Model(int vaoID, int[] VBOS, int vertexCount, MasterRenderer.Pipeline pipeline)
        {
            this.vaoID = vaoID;
            this.VBOS = VBOS;
            this.vertexCount = vertexCount;
            this.pipeline = pipeline;

            if (pipeline == MasterRenderer.Pipeline.FLAT_SHADING)
            {
                EntityManager.flatShadingSystem.addMember(this);
            }
            else if (pipeline == MasterRenderer.Pipeline.SMOOTH_SHADING)
            {
                EntityManager.smoothShadingSystem.addMember(this);
            }
            else if (pipeline == MasterRenderer.Pipeline.POST_GEOMETRY)
            {
                EntityManager.postGeometrySystem.addMember(this);
            }
        }

        public int getVAOID()
        {
            return vaoID;
        }

        public MasterRenderer.Pipeline getPipeline()
        {
            return pipeline;
        }

        public int getVertexCount()
        {
            return vertexCount;
        }

        public override void cleanUp()
        {
            base.cleanUp();
            GL.DeleteVertexArray(vaoID);
            for (int i = 0; i<VBOS.Length; i++)
            {
                GL.DeleteBuffer(VBOS[i]);
            }
        }
    }
}
