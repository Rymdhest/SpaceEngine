using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.RenderEngine;
using System.Diagnostics;

namespace SpaceEngine.Modelling
{
    internal class TerrainManager
    {
        private Dictionary<Vector3i, Entity> chunkEntities = new Dictionary<Vector3i, Entity>();
        private static float chunkSizeWorld = 200f;
        private static float viewDistanceWorld = 4000;
        private static Object threadLock = new object();
        private static Queue<Vector3i> buildQueue = new Queue<Vector3i>();
        private static bool working = false;
        private static Dictionary<Vector3i, ChunkConstructionData> finishedBuildList = new Dictionary<Vector3i, ChunkConstructionData>();
        private static Stopwatch watch = new Stopwatch();

        private struct ChunkConstructionData
        {
            public TerrainChunk component;
            public RawModel rawModel;
            public Vector3 worldPosition;
        }

        public TerrainManager()
        {

        }

        public float getNoiseHeightAt(Vector2 position)
        {
            return TerrainChunk.noiseFunction(position.X, position.Y);
        }

        public void cleanUp()
        {
            foreach (Entity entity in chunkEntities.Values)
            {
                entity.cleanUp();
            }
            chunkEntities.Clear();

            foreach (ChunkConstructionData data in finishedBuildList.Values)
            {
                data.component.cleanUp();
                data.rawModel.cleanUp();
            }
            finishedBuildList.Clear();

            buildQueue.Clear();
        }

        private static void buildNextChunk(Object obj)
        {

            while (buildQueue.Count > 0)
            {           
                Vector3i desiredChunk;
                lock (threadLock)
                {
                    if (buildQueue.Count == 0) break;
                    desiredChunk = buildQueue.Dequeue();
                }
                Vector3 worldPosition = new Vector3(desiredChunk.X, 0f, desiredChunk.Y) * chunkSizeWorld;
                TerrainChunk chunk = new TerrainChunk(worldPosition.Xz, chunkSizeWorld, desiredChunk.Z + 1);
                RawModel rawModel = chunk.generateRawModel(MasterRenderer.Pipeline.FLAT_SHADING);
                ChunkConstructionData data = new ChunkConstructionData();
                data.component = chunk;
                data.rawModel = rawModel;
                data.worldPosition = worldPosition;
                lock (threadLock)
                {
                    if (finishedBuildList.ContainsKey(desiredChunk))
                    {
                        Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " IS TRYING TO ADD "+desiredChunk.ToString() + " BUT IT ALREADY EXCISTS");
                    } else
                    {
                        finishedBuildList.Add(desiredChunk, data);
                    }
                    
                }
            }
            lock (threadLock)
            {
                if (buildQueue.Count == 0)
                {
                    working = false;
                }
            }
            Console.WriteLine("finished worker thread "+ Thread.CurrentThread.ManagedThreadId);
        }

        public void update(Vector3 viewPosition)
        {
            TerrainManager.chunkSizeWorld = 200f;
            TerrainManager.viewDistanceWorld = 3500;






            List<Vector3i> desiresChunkSpacePositions = new List<Vector3i>();
            float chunkZ = (viewPosition.Z / chunkSizeWorld);
            if (chunkZ < 0) chunkZ -= 1;
            float chunkX = (viewPosition.X / chunkSizeWorld);
            if (chunkX < 0) chunkX -= 1;
            Vector2i viewPosChunkSpace = new Vector2i((int)(chunkX), (int)(chunkZ));

            int viewDistanceChunkSpace =(int) (viewDistanceWorld / chunkSizeWorld);
            int maxDetail = 256;
            int minDetail = 2;
            for (int z = -viewDistanceChunkSpace; z<= viewDistanceChunkSpace; z++)
            {
                for (int x = -viewDistanceChunkSpace; x <= viewDistanceChunkSpace; x++)
                {
                    int offsetZ = 0;
                    int offsetX = 0;
                    int detailDampener = Math.Max(Math.Abs(x+ offsetX), Math.Abs(z+ offsetZ));
                    detailDampener -= 0;
                    if (detailDampener < 0) detailDampener = 0;
                    detailDampener = detailDampener / 2;
                    int detail = (int)(maxDetail / (Math.Pow(2, detailDampener)));
                    if (detail < minDetail) 
                    {
                        detail = minDetail;
                    }
                    desiresChunkSpacePositions.Add(new Vector3i(x+ viewPosChunkSpace.X, z+ viewPosChunkSpace.Y, detail));
                }
            }
            List<Vector3i> chunksToRemove;
            List<Vector3i> chunksToAdd;










            if (finishedBuildList.Count > 0)
            {
                watch.Restart();
                lock (threadLock)
                {
                    watch.Stop();
                    if (watch.ElapsedMilliseconds >= 1) Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId + " had to wait " + watch.Elapsed.Milliseconds + " milliseconds");


                    foreach (KeyValuePair<Vector3i, ChunkConstructionData> data in finishedBuildList)
                    {
                        if (!chunkEntities.ContainsKey(data.Key))
                        {
                            Entity terrainChunkEnity = new Entity();
                            terrainChunkEnity.addComponent(data.Value.component);
                            terrainChunkEnity.addComponent(glLoader.loadToVAO(data.Value.rawModel));
                            terrainChunkEnity.addComponent(new Transformation(data.Value.worldPosition, new Vector3(0f, 0f, 0f), 1.0f));
                            //Console.WriteLine("adding " + data.Key.ToString());
                            chunkEntities.Add(data.Key, terrainChunkEnity);
                        }
                    }
                }
            }
            if (buildQueue.Count == 0)
            {
                watch.Restart();
                lock (threadLock)
                {
                    watch.Stop();
                    if (watch.ElapsedMilliseconds >= 1) Console.WriteLine("Thread " + Thread.CurrentThread.ManagedThreadId + " had to wait " + watch.Elapsed.Milliseconds + " milliseconds");
                    chunksToRemove = chunkEntities.Keys.ToList().Except(desiresChunkSpacePositions).ToList();
                    chunksToAdd = desiresChunkSpacePositions.Except(chunkEntities.Keys.ToList()).ToList();
                    chunksToAdd = chunksToAdd.Except(buildQueue).ToList();
                    chunksToAdd = chunksToAdd.Except(finishedBuildList.Keys).ToList();
                    if (chunksToAdd.Count > 0)
                    {
                        Console.WriteLine("adding: " + chunksToAdd.Count + " chunks to build queue");
                    }
                    foreach (Vector3i add in chunksToAdd)
                    {
                        buildQueue.Enqueue(add);
                    }

                    finishedBuildList.Clear();

                    int removedChunks = 0;
                    foreach (Vector3i removeChunk in chunksToRemove)
                    {
                        bool remove = true;

                        foreach (Vector3i buildQueueChunk in buildQueue)
                        {
                            if (buildQueueChunk.Xy == removeChunk.Xy)
                            {
                                remove = false;
                                break;
                            }
                        }

                        if (remove)
                        {
                            chunkEntities[removeChunk].cleanUp();
                            chunkEntities.Remove(removeChunk);
                            removedChunks++;
                        }
                    }

                    if (removedChunks > 0)
                    {
                        Console.WriteLine("removed: " + removedChunks + " chunks");
                    }
                }
            }
            //buildNextChunk();
            if (buildQueue.Count > 0 && !working)
            {
                lock(threadLock)
                {
                    working = true;
                }
                
                for (int i = 0; i<3; i++)
                {
                    Thread thread = new Thread(buildNextChunk);
                    Console.WriteLine("adding worker thread " + thread.ManagedThreadId);
                    thread.Start();

                    
                }

            }
        }
    }
}
