using OpenTK.Mathematics;
using SpaceEngine.RenderEngine;

namespace SpaceEngine.Modelling
{
    internal class ModelGenerator
    {
        public static glModel unitSphere = glLoader.loadToVAO(MeshGenerator.generateIcosahedron(new Vector3(1), new Vector3(0), MasterRenderer.Pipeline.OTHER));
        public static glModel tree = glLoader.loadToVAO(generateTree());
        public static RawModel generateTree()
        {

            Vector3 trunkColor = new Vector3 (0.55f, 0.39f, 0.18f);
            Vector3 leafColor = new Vector3(0.49f, 0.54f, 0.24f);

            float trunkRadius = 0.6f;
            float trunkHeight = 6.4f;
            Vector3 leafRadius = new Vector3(2f, 3.5f, 2f);



            RawModel model = new RawModel();

            List<Vector3> trunkLayers = new List<Vector3>() { 
            new Vector3(trunkRadius, 0f, trunkRadius*2f),
            new Vector3(trunkRadius, trunkHeight*0.33f, trunkRadius*0.9f),
            new Vector3(trunkRadius, trunkHeight*0.66f, trunkRadius*0.8f),
            new Vector3(trunkRadius, trunkHeight, trunkRadius*0.7f)};

            RawModel trunk = MeshGenerator.generateCylinder(trunkLayers, 7, trunkColor);

            trunk.scaleRandomness(new Vector3(0.2f, 0f, 0.2f));
            model.merge(trunk);

            RawModel leaf = MeshGenerator.generateIcosahedron(leafRadius, leafColor, RenderEngine.MasterRenderer.Pipeline.FLAT_SHADING);
            leaf.translate(new Vector3(0, trunkHeight+leafRadius.Y*0.85f, 0));

            leaf.flatRandomness(0.3f);
            model.merge(leaf);
            return model;
        }
    }
}
