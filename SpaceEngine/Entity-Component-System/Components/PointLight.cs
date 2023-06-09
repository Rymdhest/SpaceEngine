using OpenTK.Mathematics;
using SpaceEngine.Modelling;
using SpaceEngine.RenderEngine;
using System.Drawing;

namespace SpaceEngine.Entity_Component_System.Components
{
    class PointLight : Component
    {
        public Vector3 attenuation { get; set; }
        public Vector3 color { get; set; }
        public float radius { get; set; }
        public Model lightVolumeModel { get; set; }
        public PointLight(Vector3 color, Vector3 attenuation)
        {
            this.color = color;
            this.attenuation = attenuation;
            float constant = attenuation.X;
            float linear = attenuation.Y;
            float quadratic = attenuation.Z;
            float lightMax = (float)Math.Max((float)Math.Max(color.X, color.Y), color.Z);
            radius = (-linear + (float)Math.Sqrt(linear * linear - 4 * quadratic * (constant - (256.0f / 1.5f) * lightMax))) / (2f * quadratic);
            lightVolumeModel = glLoader.loadToVAO(MeshGenerator.generateIcosahedron(new Vector3(radius), new Vector3(0f, 0f, 1.0f), MasterRenderer.Pipeline.OTHER));
            EntityManager.pointLightSystem.addMember(this);
        }
    }
}
