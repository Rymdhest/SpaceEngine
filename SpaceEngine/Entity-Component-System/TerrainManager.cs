using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.RenderEngine;
using System.Linq;

namespace SpaceEngine.Modelling
{
    internal class TerrainManager
    {
        private Dictionary<Vector3i, Entity> chunkEntities = new Dictionary<Vector3i, Entity>();
        private float chunkSizeWorld = 10f;
        private float viewDistanceWorld = 60;

        public TerrainManager()
        {

        }

        public void update(Vector3 viewPosition)
        {
            chunkSizeWorld = 500f;
            viewDistanceWorld =4000;


            List<Vector3i> desiresChunkSpacePositions = new List<Vector3i>();
            Vector2i viewPosChunkSpace = new Vector2i((int)(viewPosition.X/chunkSizeWorld-1), (int)(viewPosition.Z/chunkSizeWorld));

            int viewDistanceChunkSpace =(int) (viewDistanceWorld / chunkSizeWorld);
            int maxDetail = 64;
            int minDetail = 2;
            for (int z = -viewDistanceChunkSpace; z<= viewDistanceChunkSpace; z++)
            {
                for (int x = -viewDistanceChunkSpace; x <= viewDistanceChunkSpace; x++)
                {
                    int detailDampener = Math.Max(Math.Abs(x-1), Math.Abs(z));
                    detailDampener -= 1;
                    if (detailDampener < 0) detailDampener = 0;
                    detailDampener = detailDampener / 4;
                    int detail = (int)(maxDetail / (Math.Pow(2, detailDampener)));
                    if (detail < minDetail) 
                    {
                        detail = minDetail;
                    }
                    //Console.WriteLine(detail);
                    desiresChunkSpacePositions.Add(new Vector3i(x+ viewPosChunkSpace.X, z+ viewPosChunkSpace.Y, detail));
                }
            }
            

            List<Vector3i> chunksToRemove = chunkEntities.Keys.Except(desiresChunkSpacePositions).ToList();
            List<Vector3i> chunksToAdd = desiresChunkSpacePositions.Except(chunkEntities.Keys.ToList()).ToList();

            if (chunksToAdd.Count > 0)
            {
                Console.WriteLine("adding: "+chunksToAdd.Count+" chunks");
            }
            if (chunksToRemove.Count > 0)
            {
                Console.WriteLine("removing: " + chunksToRemove.Count + " chunks");
            }

            foreach (Vector3i removeChunk in chunksToRemove)
            {
                chunkEntities[removeChunk].cleanUp();
                chunkEntities.Remove(removeChunk);
            }

            foreach (Vector3i desiredChunk in chunksToAdd)
            {
                Vector3 worldPosition = new Vector3(desiredChunk.X, 0f, desiredChunk.Y) * chunkSizeWorld;
                Entity terrainChunkEnity = new Entity();
                TerrainChunk chunk = new TerrainChunk(worldPosition.Xz, chunkSizeWorld, desiredChunk.Z+1);
                terrainChunkEnity.addComponent(chunk);
                terrainChunkEnity.addComponent(chunk.generateModel(MasterRenderer.Pipeline.FLAT_SHADING));
                terrainChunkEnity.addComponent(new Transformation(worldPosition, new Vector3(0f, 0f, 0f), 1.0f));
                chunkEntities.Add(desiredChunk, terrainChunkEnity);
            }
        }
    }
}
