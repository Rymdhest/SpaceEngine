using OpenTK.Mathematics;
using SpaceEngine.Entity_Component_System.Components;
using SpaceEngine.Entity_Component_System;
using SpaceEngine.RenderEngine;
using System.Diagnostics;
using System.ComponentModel.Design;
using System.Collections.Generic;

namespace SpaceEngine.Modelling
{
    internal class TerrainManager
    {
        public Dictionary<Vector3i, Entity> chunkEntities = new Dictionary<Vector3i, Entity>();
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
        public float getPolygonHeightAt(Vector2 position)
        {
            List<Entity> chunks = chunkEntities.Where(kv => kv.Key.Xy == fromWorldToChunkSpace(position)).Select(kv => kv.Value).ToList();
            if (chunks.Count > 0) {
                return chunks[0].getComponent<TerrainChunk>().getPolygonHeightAt(position);
            }
            else 
            {
                return TerrainChunk.noiseFunction(position.X, position.Y);
            }
            
        }
        public Vector3 getNormalFlatAt(Vector2 position)
        {
            List<Entity> chunks = chunkEntities.Where(kv => kv.Key.Xy == fromWorldToChunkSpace(position)).Select(kv => kv.Value).ToList();
            if (chunks.Count > 0)
            {
                return chunks[0].getComponent<TerrainChunk>().getNormalFlatAt(position);
            }
            else
            {
                return new Vector3(0f, 1f, 0f);
            }

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

        public Vector2i fromWorldToChunkSpace(Vector2 worldSpace)
        {
            float chunkZ = (worldSpace.Y / chunkSizeWorld);
            if (chunkZ < 0) chunkZ -= 1;
            float chunkX = (worldSpace.X / chunkSizeWorld);
            if (chunkX < 0) chunkX -= 1;
            return new Vector2i((int)(chunkX), (int)(chunkZ));
        }

        public void update(Vector3 viewPosition)
        {
            TerrainManager.chunkSizeWorld = 200f;
            TerrainManager.viewDistanceWorld = 5000;

            List<Vector3i> desiresChunkSpacePositions = new List<Vector3i>();
            Vector2i viewPosChunkSpace = fromWorldToChunkSpace(viewPosition.Xz);

            int viewDistanceChunkSpace =(int) (viewDistanceWorld / chunkSizeWorld);
            int maxDetail = 128;
            int minDetail = 4;
            for (int z = -viewDistanceChunkSpace; z<= viewDistanceChunkSpace; z++)
            {
                for (int x = -viewDistanceChunkSpace; x <= viewDistanceChunkSpace; x++)
                {
                    int offsetZ = 0;
                    int offsetX = 0;
                    int detailDampener = Math.Max(Math.Abs(x+ offsetX), Math.Abs(z+ offsetZ));
                    detailDampener -= 0;
                    if (detailDampener < 0) detailDampener = 0;
                    detailDampener = detailDampener / 4;
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
                            terrainChunkEnity.addComponent(new Model( glLoader.loadToVAO(data.Value.rawModel), MasterRenderer.Pipeline.FLAT_SHADING));
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
                    chunksToAdd.Sort((v1, v2) => (v1.Xy - viewPosChunkSpace).EuclideanLength.CompareTo((v2.Xy - viewPosChunkSpace).EuclideanLength));
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
                
                for (int i = 0; i<4; i++)
                {
                    Thread thread = new Thread(buildNextChunk);
                    Console.WriteLine("adding worker thread " + thread.ManagedThreadId);
                    thread.Start();

                    
                }

            }
        }
    }
}
