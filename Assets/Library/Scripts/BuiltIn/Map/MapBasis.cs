using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

using Basis;
using Basis.GameItem;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace MapBasis
{
    public enum AxisDirectionType
    {
        /// <summary>
        /// x轴
        /// </summary>
        [LabelText("沿X轴")]
        X,
        /// <summary>
        /// y轴
        /// </summary>
        [LabelText("沿Y轴")]
        Y,
        /// <summary>
        /// z轴
        /// </summary>
        [LabelText("沿Z轴")]
        Z
    }

    public enum MapType
    {
        /// <summary>
        /// 矩形地图
        /// </summary>
        [LabelText("矩形地图")]
        Rect,
        /// <summary>
        /// 底边是平的六边形地图
        /// </summary>
        [LabelText("平底边六边形地图")]
        FlatHexagon,
        /// <summary>
        /// 底边是尖的六边形地图
        /// </summary>
        [LabelText("尖底边六边形地图")]
        PointyHexagon,

    }

    [Serializable]
    public class MapConfiguration : 
        GamePrefabCoreBundle<MapConfiguration, MapCoreGeneralSetting>.GameItemPrefab
    {
        //地图种类
        [LabelText("地图类型")]
        public MapType mapType;

        /// <summary>
        /// 地图在X轴上是否为无限长度
        /// </summary>
        [LabelText("是否在X轴上无限")]
        public bool isInfinityInX;
        /// <summary>
        /// 地图在Y轴上是否为无限长度
        /// </summary>
        [LabelText("是否在Y轴上无限")]
        public bool isInfinityInY;
        /// <summary>
        /// 地图在Z轴上是否为无限长度
        /// </summary>
        [LabelText("是否在Z轴上无限")]
        public bool isInfinityInZ;

        /// <summary>
        /// 区块大小
        /// </summary>
        [LabelText("区块大小")]
        public Vector3Int chunkSize;

        public int chunkWidth => chunkSize.x;

        public int chunkLength => chunkSize.z;

        public int chunkHeight => chunkSize.y;

        /// <summary>
        /// 有限地图专用，固定长宽高，单位为区块
        /// </summary>
        [LabelText("固定大小")]
        public Vector3Int fixedSize;

        public int fixedWidth => fixedSize.x;

        public int fixedLength => fixedSize.z;

        public int fixedHeight => fixedSize.y;

        //有限地图专用，固定长宽高，单位为瓦片
        public int totalWidth => fixedWidth * chunkWidth;

        public int totalLength => fixedLength * chunkLength;

        public int totalHeight => fixedHeight * chunkHeight;

        /// <summary>
        /// 区间加载半径
        /// </summary>
        [LabelText("区间加载半径")]
        public Vector3 chunkLoadingRadius;

        [LabelText("区间一次添加的最大数量")]
        public int onceChunkAddedMaxAmount;

        [LabelText("区间卸载半径")]
        public Vector3 chunkDeleteRadius;

        [LabelText("可视的面")]
        public FaceType visibleFaces = FaceType.All;
    }


    public sealed class MapBundle<M, C, T>
        where M : MapBundle<M, C, T>.IMapData, new()
        where C : MapBundle<M, C, T>.IChunkData, new()
        where T : MapBundle<M, C, T>.ITileData, new()
    {
        #region Data

        public abstract class IMapData
        {
            public Map map { get; set; }

            public virtual void OnLoad() { }

            public virtual void OnClear() {}


        }

        public abstract class IChunkData
        {
            public Chunk chunk { get; set; }
            public Map map => chunk.originMap;

            public virtual void OnLoad() { }

            public virtual void OnClear() {}
        }

        public abstract class ITileData
        {
            public Map map => tile.originMap;
            public Chunk chunk => tile.originChunk;
            public Tile tile { get; set; }

            public virtual void OnLoad() { }

            public virtual void OnClear() {}
        }

        #endregion
        
        [HideReferenceObjectPicker]
        [HideDuplicateReferenceBox]
        [HideInEditorMode]
        [ShowInInspector]
        [LabelText("通用地图核心")]
        public sealed class Map : IEnumerable<Chunk>
        {
            [LabelText("地图基本配置")]
            public MapConfiguration config;

            [LabelText("地图数据")]
            [HideReferenceObjectPicker]
            public M data = new();

            //存区块
            [LabelText("所有区块")]
            [ShowInInspector]
            private readonly Dictionary<Vector3Int, Chunk> chunkDictXYZ = new();

            private readonly Dictionary<Vector2Int, HashSet<Chunk>> chunkDictXY = new();
            private readonly Dictionary<Vector2Int, HashSet<Chunk>> chunkDictYZ = new();
            private readonly Dictionary<Vector2Int, HashSet<Chunk>> chunkDictXZ = new();

            private readonly Dictionary<int, HashSet<Chunk>> chunkDictX = new();
            private readonly Dictionary<int, HashSet<Chunk>> chunkDictY = new();
            private readonly Dictionary<int, HashSet<Chunk>> chunkDictZ = new();

            [LabelText("区块数量")]
            [ShowInInspector]
            public int chunksCount { get; private set; }

            [LabelText("区块生成的起点坐标")]
            [ReadOnly, ShowInInspector]
            private HashSet<Vector3Int> originChunkPositions = new();

            [LabelText("x轴基向量")]
            public Vector3 xBase;
            [LabelText("y轴基向量")]
            public Vector3 yBase;
            [LabelText("z轴基向量")]
            public Vector3 zBase;

            [LabelText("偏移向量")]
            public Vector3 offset = Vector3.zero;

            [LabelText("地图内Tile总数")]
            [ShowInInspector]
            public int tilesCount => chunkDictXYZ.Count * config.chunkWidth * config.chunkLength * config.chunkHeight;

            [FoldoutGroup("坐标范围"), LabelText("最小Tile坐标x")]
            public int minTileX = int.MaxValue;
            [FoldoutGroup("坐标范围"), LabelText("最小Tile坐标y")]
            public int minTileY = int.MaxValue;
            [FoldoutGroup("坐标范围"), LabelText("最小Tile坐标z")]
            public int minTileZ = int.MaxValue;

            [FoldoutGroup("坐标范围"), LabelText("最大Tile坐标x")]
            public int maxTileX = int.MinValue;
            [FoldoutGroup("坐标范围"), LabelText("最大Tile坐标y")]
            public int maxTileY = int.MinValue;
            [FoldoutGroup("坐标范围"), LabelText("最大Tile坐标z")]
            public int maxTileZ = int.MinValue;

            [FoldoutGroup("坐标范围"), LabelText("最小Chunk坐标x")]
            public int minChunkX = int.MaxValue;
            [FoldoutGroup("坐标范围"), LabelText("最小Chunk坐标y")]
            public int minChunkY = int.MaxValue;
            [FoldoutGroup("坐标范围"), LabelText("最小Chunk坐标z")]
            public int minChunkZ = int.MaxValue;

            [FoldoutGroup("坐标范围"), LabelText("最大Chunk坐标x")]
            public int maxChunkX = int.MinValue;
            [FoldoutGroup("坐标范围"), LabelText("最大Chunk坐标y")]
            public int maxChunkY = int.MinValue;
            [FoldoutGroup("坐标范围"), LabelText("最大Chunk坐标z")]
            public int maxChunkZ = int.MinValue;

            public delegate void ChunkEvent(Chunk chunk);
            public delegate void TileEvent(Chunk chunk, Tile tile);

            public event ChunkEvent OnChunkCreateStart;
            public event ChunkEvent OnChunkCreateEnd;
            public event Action<Chunk, FaceType, Chunk> OnNearChunkCreate;
            public event ChunkEvent OnChunkDeleteStart;
            public event ChunkEvent OnChunkDeleteEnd;
            public event Action<Chunk, FaceType, Chunk> OnNearChunkDelete;

            public event TileEvent OnTileCreate;
            public event TileEvent OnTileDelete;

            public event TileEvent OnTileUpdateWhenAddedChunk;
            public event TileEvent OnTileUpdateWhenDeleteChunk;

            #region Initialization

            public Map(string configID) :
                this(GameCoreSettingBase.mapCoreGeneralSetting.GetPrefabStrictly(configID))
            {

            }

            public Map(MapConfiguration newConfig)
            {
                if (newConfig == null)
                {
                    Note.note.Error($"{nameof(newConfig)}参数为Null");
                    return;
                }

                config = newConfig;

                if (config.isInfinityInX == false && config.fixedWidth <= 0)
                {
                    Note.note.Error("X轴非无限地图不可X轴上的固定长度小于等于0");
                }
                if (config.isInfinityInZ == false && config.fixedLength <= 0)
                {
                    Note.note.Error("Z轴非无限地图不可Z轴上的固定长度小于等于0");
                }
                if (config.isInfinityInY == false && config.fixedHeight <= 0)
                {
                    Note.note.Error("Y轴非无限地图不可Y轴上的固定长度小于等于0");
                }

                if (config.chunkWidth <= 0 || config.chunkLength <= 0 || config.chunkHeight <= 0)
                {
                    Note.note.Error("地图区块大小参数不可小于1");
                }

                if (config.chunkLoadingRadius.AnyNumberBelowOrEqual(0))
                {
                    Note.note.Error("任意轴上的区间加载半径不得小于1");
                }

                if (config.onceChunkAddedMaxAmount <= 0)
                {
                    Note.note.Error("一次添加的区块最大数量不能小于等于0");
                }

                data.map = this;

                data.OnLoad();
            }

            #endregion

            public void Clear()
            {
                ForeachChunk(chunk =>
                {
                    chunk.ForeachTile(tile =>
                    {
                        tile.data.OnClear();
                    });

                    chunk.data.OnClear();
                });

                data.OnClear();
            }

            #region Dynamic Map

            public void RemoveOriginChunkPos(params Vector3Int[] chunkPoses)
            {
                foreach (var chunkPos in chunkPoses)
                {
                    originChunkPositions.Remove(chunkPos);
                }
            }

            public void AddOriginChunkPos(params Vector3Int[] newOriginChunkPositions)
            {
                foreach (Vector3Int newOriginChunkPos in newOriginChunkPositions)
                {
                    int newOriginChunkX = newOriginChunkPos.x;
                    int newOriginChunkY = newOriginChunkPos.y;
                    int newOriginChunkZ = newOriginChunkPos.z;

                    if (config.isInfinityInX == false)
                    {
                        newOriginChunkX = Mathf.Clamp(newOriginChunkX, 0, config.fixedWidth - 1);
                    }
                    if (config.isInfinityInZ == false)
                    {
                        newOriginChunkZ = Mathf.Clamp(newOriginChunkZ, 0, config.fixedLength - 1);
                    }
                    if (config.isInfinityInY == false)
                    {
                        newOriginChunkY = Mathf.Clamp(newOriginChunkY, 0, config.fixedHeight - 1);
                    }

                    Vector3Int newModifiedOriginChunkPos = new(newOriginChunkX, newOriginChunkY, newOriginChunkZ);

                    originChunkPositions.Add(newModifiedOriginChunkPos);
                }
            }

            public void AddOriginChunkPos(Tile tile)
            {
                AddOriginChunkPos(tile.originChunk.pos);
            }

            public void AddRandomOriginChunkPos(Vector3Int randomLimitation = default(Vector3Int))
            {
                Vector3Int randomChunkPos = Vector3Int.zero;

                if (randomLimitation == default(Vector3Int))
                {
                    randomLimitation = new Vector3Int(100, 100, 100);
                }

                if (config.isInfinityInX == false)
                {
                    randomChunkPos.x = UnityEngine.Random.Range(0, config.fixedWidth);
                }
                else
                {
                    randomChunkPos.x = UnityEngine.Random.Range(-randomLimitation.x, randomLimitation.x + 1);
                }

                if (config.isInfinityInZ == false)
                {
                    randomChunkPos.z = UnityEngine.Random.Range(0, config.fixedLength);
                }
                else
                {
                    randomChunkPos.z = UnityEngine.Random.Range(-randomLimitation.z, randomLimitation.z + 1);
                }

                if (config.isInfinityInY == false)
                {
                    randomChunkPos.y = UnityEngine.Random.Range(0, config.fixedHeight);
                }
                else
                {
                    randomChunkPos.y = UnityEngine.Random.Range(-randomLimitation.y, randomLimitation.y + 1);
                }

                Note.note.Log($"添加随机德其实区块加载坐标为：{randomChunkPos}");

                AddOriginChunkPos(randomChunkPos);

            }

            /// <summary>
            /// 以originChunk坐标为起点更新区块列表
            /// </summary>
            /// <returns></returns>
            public async UniTask<int> DynamicLoadingChunks()
            {
                var originChunkPositions = this.originChunkPositions.ToArray();

                Note.note.Log($"开始动态加载区块，加载半径为:{config.chunkLoadingRadius}, " +
                         $"加载中心为：{originChunkPositions.ToString(",")}");

                int chunkCreationCount = 0;

                foreach (Vector3Int originChunkPos in originChunkPositions)
                {
                    List<Vector3Int> preupdateList = new();
                    List<Vector3Int> updatedList = new();

                    preupdateList.Add(originChunkPos);

                    Ellipsoid area = new Ellipsoid(originChunkPos, config.chunkLoadingRadius);

                    while (true)
                    {
                        if (chunkCreationCount >= config.onceChunkAddedMaxAmount)
                        {
                            break;
                        }

                        if (preupdateList.Count <= 0)
                        {
                            break;
                        }

                        List<Vector3Int> newPreupdateList = new();
                        foreach (Vector3Int preupdatePos in preupdateList)
                        {
                            if (ContainsChunk(preupdatePos) == false)
                            {
                                Chunk newChunk = CreateChunk(preupdatePos);

                                if (newChunk != null)
                                {
                                    chunkCreationCount++;
                                }
                                else
                                {
                                    Note.note.Warning($"尝试添加坐标为:{preupdatePos}新区块失败");
                                    continue;
                                }
                            }

                            foreach (Vector3Int nearPos in GetNearPositions(MapType.Rect, preupdatePos))
                            {
                                if (newPreupdateList.Contains(nearPos))
                                {
                                    continue;
                                }
                                if (updatedList.Contains(nearPos))
                                {
                                    continue;
                                }

                                if (config.isInfinityInX == false)
                                {
                                    if (new RangeInteger(0, config.fixedWidth - 1).Contains(nearPos.x) == false)
                                    {
                                        continue;
                                    }
                                }
                                if (config.isInfinityInZ == false)
                                {
                                    if (new RangeInteger(0, config.fixedLength - 1).Contains(nearPos.z) == false)
                                    {
                                        continue;
                                    }
                                }
                                if (config.isInfinityInY == false)
                                {
                                    if (new RangeInteger(0, config.fixedHeight - 1).Contains(nearPos.y) == false)
                                    {
                                        continue;
                                    }
                                }

                                if (area.Contains(nearPos) == false)
                                {
                                    continue;
                                }

                                newPreupdateList.Add(nearPos);
                            }

                            await UniTask.NextFrame();
                        }

                        updatedList.AddRange(preupdateList);
                        preupdateList.Clear();
                        preupdateList.AddRange(newPreupdateList);
                        newPreupdateList.Clear();


                    }
                }

                Note.note.Log($"一共新添加了{chunkCreationCount}个区块");

                return chunkCreationCount;
            }

            public async UniTask<int> DynamicDeleteChunks()
            {
                Note.note.Log($"开始动态删除区块，加载半径为:{config.chunkDeleteRadius}");

                int chunkDeleteCount = 0;

                var chunkPosesNeedToDelete = chunkDictXYZ.Keys.ToHashSet();

                var areas = originChunkPositions.ToArray().Select(
                    originChunkPos => new Ellipsoid(originChunkPos, config.chunkDeleteRadius)).
                    ToList();

                chunkPosesNeedToDelete.RemoveWhere(
                    pos => areas.Any(area => area.Contains(pos)));

                foreach (var pos in chunkPosesNeedToDelete)
                {
                    DeleteChunk(pos);
                    chunkDeleteCount++;
                    await UniTask.NextFrame();
                }

                Note.note.Log($"一共删除了{chunkDeleteCount}个区块");

                return chunkDeleteCount;
            }

            #endregion

            #region Chunk&Tile Operation

            public IEnumerator<Chunk> GetEnumerator()
            {
                return chunkDictXYZ.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #region Create Chunk

            public IReadOnlyList<Chunk> CreateAllChunks()
            {
                if (config.isInfinityInX || config.isInfinityInY || config.isInfinityInZ)
                {
                    Note.note.Warning("无限地图不能用CreateAllChunks");
                    return null;
                }

                var chunks = new List<Chunk>();

                for (int i = 0; i < config.fixedWidth; i++)
                {
                    for (int j = 0; j < config.fixedHeight; j++)
                    {
                        for (int k = 0; k < config.fixedLength; k++)
                        {
                            Chunk newChunk = CreateChunk(new Vector3Int(i, j, k));

                            if (newChunk != default)
                            {
                                if (config.isDebugging)
                                {
                                    Note.note.Warning($"创建区块:({i}, {j}, {k})");
                                }
                                chunks.Add(newChunk);
                            }
                            else
                            {
                                Note.note.Warning($"创建区块失败，坐标为:({i}, {j}, {k})");
                            }
                        }
                    }
                }

                return chunks;
            }

            //往区块队列里添加新区块
            public Chunk CreateChunk(Vector3Int chunkPos)
            {

                if (chunkDictXYZ.TryGetValue(chunkPos, out var existedChunk))
                {
                    Note.note.Warning($"坐标为{chunkPos}的chunk已经创建过");
                    return existedChunk;
                }

                if (config.isInfinityInX == false)
                {
                    if (chunkDictX.ContainsKey(chunkPos.x) == false && 
                        chunkDictX.Count >= config.fixedWidth)
                    {
                        Note.note.Warning($"将要创建的区块:{chunkPos}的X坐标" +
                                     $"超过了地图固定宽度:{config.fixedWidth}");
                        return default;
                    }
                }
                if (config.isInfinityInY == false)
                {
                    if (chunkDictY.ContainsKey(chunkPos.y) == false && 
                        chunkDictY.Count >= config.fixedHeight)
                    {
                        Note.note.Warning($"将要创建的区块:{chunkPos}的Y坐标" +
                                     $"超过了地图固定高度:{config.fixedHeight}");
                        return default;
                    }
                }
                if (config.isInfinityInZ == false)
                {
                    if (chunkDictZ.ContainsKey(chunkPos.z) == false && 
                        chunkDictZ.Count >= config.fixedLength)
                    {
                        Note.note.Warning($"将要创建的区块:{chunkPos}的Z坐标" +
                                     $"超过了地图固定长度:{config.fixedLength}");
                        return default;
                    }
                }

                Note.note.Log($"开始创建区块:{chunkPos}");

                Chunk newChunk = new(this, chunkPos);

                chunkDictXYZ[chunkPos] = newChunk;

                if (chunkDictXY.ContainsKey(chunkPos.XY()) == false)
                {
                    chunkDictXY[chunkPos.XY()] = new();
                }

                chunkDictXY[chunkPos.XY()].Add(newChunk);

                if (chunkDictXZ.ContainsKey(chunkPos.XZ()) == false)
                {
                    chunkDictXZ[chunkPos.XZ()] = new();
                }

                chunkDictXZ[chunkPos.XZ()].Add(newChunk);

                if (chunkDictYZ.ContainsKey(chunkPos.YZ()) == false)
                {
                    chunkDictYZ[chunkPos.YZ()] = new();
                }

                chunkDictYZ[chunkPos.YZ()].Add(newChunk);

                if (chunkDictX.ContainsKey(chunkPos.x) == false)
                {
                    chunkDictX[chunkPos.x] = new();
                }

                chunkDictX[chunkPos.x].Add(newChunk);

                if (chunkDictY.ContainsKey(chunkPos.y) == false)
                {
                    chunkDictY[chunkPos.y] = new();
                }

                chunkDictY[chunkPos.y].Add(newChunk);

                if (chunkDictZ.ContainsKey(chunkPos.z) == false)
                {
                    chunkDictZ[chunkPos.z] = new();
                }

                chunkDictZ[chunkPos.z].Add(newChunk);


                if (chunkPos.x < minChunkX)
                {
                    minChunkX = chunkPos.x;
                }
                if (chunkPos.y < minChunkY)
                {
                    minChunkY = chunkPos.y;
                }
                if (chunkPos.z < minChunkZ)
                {
                    minChunkZ = chunkPos.z;
                }
                if (chunkPos.x > maxChunkX)
                {
                    maxChunkX = chunkPos.x;
                }
                if (chunkPos.y > maxChunkY)
                {
                    maxChunkY = chunkPos.y;
                }
                if (chunkPos.z > maxChunkZ)
                {
                    maxChunkZ = chunkPos.z;
                }

                if (newChunk.minTileX < minTileX)
                {
                    minTileX = newChunk.minTileX;
                }
                if (newChunk.minTileY < minTileY)
                {
                    minTileY = newChunk.minTileY;
                }
                if (newChunk.minTileZ < minTileZ)
                {
                    minTileZ = newChunk.minTileZ;
                }
                if (newChunk.maxTileX > maxTileX)
                {
                    maxTileX = newChunk.maxTileX;
                }
                if (newChunk.maxTileY > maxTileY)
                {
                    maxTileY = newChunk.maxTileY;
                }
                if (newChunk.maxTileZ > maxTileZ)
                {
                    maxTileZ = newChunk.maxTileZ;
                }

                chunksCount++;

                OnChunkCreateStart?.Invoke(newChunk);

                if (OnTileCreate != null)
                {
                    newChunk.ForeachTile((tile) => OnTileCreate?.Invoke(newChunk, tile));
                }

                foreach (var tile in newChunk.tilesNeedToUpdate)
                {
                    OnTileUpdateWhenAddedChunk?.Invoke(newChunk, tile);
                }
                newChunk.tilesNeedToUpdate.Clear();

                OnChunkCreateEnd?.Invoke(newChunk);

                foreach (var (face, nearChunk) in newChunk.GetNearChunks())
                {
                    OnNearChunkCreate?.Invoke(nearChunk, face.Reverse(), newChunk);
                }

                return newChunk;
            }

            #endregion

            #region Delete Chunk

            public void DeleteChunk(Vector3Int chunkPos)
            {
                if (chunkDictXYZ.TryGetValue(chunkPos, out var chunk) == false)
                {
                    return;
                }

                foreach (var tile in chunk.GetAllTiles())
                {
                    tile.data.OnClear();
                }

                chunk.data.OnClear();

                chunkDictXYZ.Remove(chunkPos);

                var xyList = chunkDictXY[chunkPos.XY()];
                var xzList = chunkDictXZ[chunkPos.XZ()];
                var yzList = chunkDictYZ[chunkPos.YZ()];

                var xList = chunkDictX[chunkPos.x];
                var yList = chunkDictY[chunkPos.y];
                var zList = chunkDictZ[chunkPos.z];

                xyList.Remove(chunk);
                xzList.Remove(chunk);
                yzList.Remove(chunk);

                xList.Remove(chunk);
                yList.Remove(chunk);
                zList.Remove(chunk);

                if (xyList.Count == 0)
                {
                    chunkDictXY.Remove(chunkPos.XY());
                }

                if (xzList.Count == 0)
                {
                    chunkDictXZ.Remove(chunkPos.XZ());
                }

                if (yzList.Count == 0)
                {
                    chunkDictYZ.Remove(chunkPos.YZ());
                }

                if (xList.Count == 0)
                {
                    chunkDictX.Remove(chunkPos.x);
                }

                if (yList.Count == 0)
                {
                    chunkDictY.Remove(chunkPos.y);
                }

                if (zList.Count == 0)
                {
                    chunkDictZ.Remove(chunkPos.z);
                }

                minChunkX = chunkDictX.Keys.Min();
                minChunkY = chunkDictY.Keys.Min();
                minChunkZ = chunkDictZ.Keys.Min();

                maxChunkX = chunkDictX.Keys.Max();
                maxChunkY = chunkDictY.Keys.Max();
                maxChunkZ = chunkDictZ.Keys.Max();

                minTileX = chunkDictX.Values.Select(chunkList => 
                    chunkList.First().minTileX).Min();
                minTileY = chunkDictY.Values.Select(chunkList => 
                    chunkList.First().minTileY).Min();
                minTileZ = chunkDictZ.Values.Select(chunkList => 
                    chunkList.First().minTileZ).Min();

                maxTileX = chunkDictX.Values.Select(chunkList => 
                    chunkList.First().maxTileX).Max();
                maxTileY = chunkDictY.Values.Select(chunkList => 
                    chunkList.First().maxTileY).Max();
                maxTileZ = chunkDictZ.Values.Select(chunkList => 
                    chunkList.First().maxTileZ).Max();

                chunksCount--;

                OnChunkDeleteStart?.Invoke(chunk);

                foreach (var (_, nearChunk) in chunk.GetNearChunks())
                {
                    nearChunk.Update();

                    foreach (var tile in nearChunk.tilesNeedToUpdate)
                    {
                        OnTileUpdateWhenDeleteChunk?.Invoke(chunk, tile);
                    }
                }

                if (OnTileDelete != null)
                {
                    chunk.ForeachTile(tile => OnTileDelete?.Invoke(chunk, tile));
                }

                OnChunkDeleteEnd?.Invoke(chunk);

                foreach (var (face, nearChunk) in chunk.GetNearChunks())
                {
                    OnNearChunkDelete?.Invoke(nearChunk, face.Reverse(), chunk);
                }
            }

            #endregion

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsChunk(Vector3Int chunkPos)
            {
                return chunkDictXYZ.ContainsKey(chunkPos);
            }

            /// <summary>
            /// 获取区块
            /// </summary>
            /// <param name="chunkPos">区块坐标</param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Button("获取区块")]
            public Chunk GetChunk(Vector3Int chunkPos)
            {
                return chunkDictXYZ.TryGetValue(chunkPos, out var chunk) ? chunk : default;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetChunkByXY(Vector2Int chunkPosXY)
            {
                return chunkDictXY.TryGetValue(chunkPosXY, out var chunks) ? chunks.First() : null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IEnumerable<Chunk> GetChunksByXY(Vector2Int chunkPosXY)
            {
                if (chunkDictXY.TryGetValue(chunkPosXY, out var chunks))
                {
                    foreach (var chunk in chunks)
                    {
                        yield return chunk;
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetChunkByXZ(Vector2Int chunkPosXZ)
            {
                return chunkDictXZ.TryGetValue(chunkPosXZ, out var chunks) ? chunks.First() : null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IEnumerable<Chunk> GetChunksByXZ(Vector2Int chunkPosXZ)
            {
                if (chunkDictXZ.TryGetValue(chunkPosXZ, out var chunks))
                {
                    foreach (var chunk in chunks)
                    {
                        yield return chunk;
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetChunkByYZ(Vector2Int chunkPosYZ)
            {
                return chunkDictYZ.TryGetValue(chunkPosYZ, out var chunks) ? chunks.First() : null;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IEnumerable<Chunk> GetChunksByYZ(Vector2Int chunkPosYZ)
            {
                if (chunkDictYZ.TryGetValue(chunkPosYZ, out var chunks))
                {
                    foreach (var chunk in chunks)
                    {
                        yield return chunk;
                    }
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetChunk(Vector3Int chunkPos, out Chunk chunk)
            {
                return chunkDictXYZ.TryGetValue(chunkPos, out chunk);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IEnumerable<Chunk> GetAllChunks()
            {
                return chunkDictXYZ.Values;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3Int GetChunkPosByTilePos(Vector3Int tilePos)
            {
                return tilePos.Divide(config.chunkSize);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetChunkByTilePos(Vector3Int tilePos)
            {
                return GetChunk(tilePos.Divide(config.chunkSize));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetChunkByTilePosXY(Vector2Int tilePosXY)
            {
                return GetChunkByXY(tilePosXY.Divide(config.chunkSize.XY()));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetChunkByTilePosXZ(Vector2Int tilePosXZ)
            {
                return GetChunkByXZ(tilePosXZ.Divide(config.chunkSize.XZ()));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetChunkByTilePosYZ(Vector2Int tilePosYZ)
            {
                return GetChunkByYZ(tilePosYZ.Divide(config.chunkSize.YZ()));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Chunk GetRandomChunk()
            {
                return chunkDictXYZ.ChooseValue();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ForeachTile(Action<Tile> func)
            {
                foreach (var (_, value) in chunkDictXYZ)
                {
                    value.ForeachTile(func);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ForeachChunk(Action<Chunk> func)
            {
                foreach (var kvp in chunkDictXYZ)
                {
                    func(kvp.Value);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Button("获取瓦片")]
            public Tile GetTile(Vector3Int tilePos)
            {
                Chunk chunkBelonged = GetChunkByTilePos(tilePos);

                return chunkBelonged?.GetTile(tilePos);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tile GetTileStrictly(Vector3Int tilePos)
            {
                var tile = GetTile(tilePos);

                if (tile == null)
                {
                    Note.note.Error($"tilePos:{tilePos}越界");
                }

                return tile;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetTile(Vector3Int tilePos, out Tile tile)
            {
                Chunk chunkBelonged = GetChunkByTilePos(tilePos);
                if (chunkBelonged == default(Chunk))
                {
                    tile = default;
                    return false;
                }

                tile = chunkBelonged.GetTile(tilePos);
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3Int GetTilePosInChunk(Vector3Int tilePos)
            {
                return tilePos.Modulo(config.chunkSize);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2Int GetTileXYInChunk(Vector2Int tilePosXY)
            {
                return tilePosXY.Modulo(config.chunkSize.XY());
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2Int GetTileXZInChunk(Vector2Int tilePosXZ)
            {
                return tilePosXZ.Modulo(config.chunkSize.XZ());
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2Int GetTileYZInChunk(Vector2Int tilePosYZ)
            {
                return tilePosYZ.Modulo(config.chunkSize.YZ());
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tile GetZMaxTile(Vector2Int posXY)
            {
                return GetTile(posXY.InsertAsZ(maxTileZ));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TryGetZMaxTile(Vector2Int posXY, out Tile tile)
            {
                return TryGetTile(posXY.InsertAsZ(maxTileZ), out tile);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IEnumerable<Tile> GetBoxOfTiles(Vector3Int from, Vector3Int to)
            {
                foreach (var pos in from.GetCubeOfPoints(to))
                {
                    var tile = GetTile(pos);

                    if (tile == null)
                    {
                        Note.note.Log($"在GetBoxOfTiles中试图获取坐标为:{pos}的Tile但是为Null");
                        continue;
                    }
                    yield return tile;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tile GetRandomTile()
            {
                return GetRandomChunk().GetRandomTile();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsArea(Vector3Int start, Vector3Int end)
            {
                foreach (var chunkPos in MathFunc.ModuloRange(start, 
                             end, config.chunkSize, true))
                {
                    if (ContainsChunk(chunkPos) == false)
                    {
                        return false;
                    }
                }

                return true;
            }

            #endregion

            #region FindWay

            public List<Tile> GetNearTiles(Tile pivotTile, Func<Tile, bool> judgeFunc)
            {
                List<Tile> result = new List<Tile>();

                foreach (Vector3Int pos in GetNearPositions(config.mapType, pivotTile.pos))
                {

                    Tile tile = GetTile(pos);
                    if (tile == null)
                    {
                        continue;
                    }

                    if (judgeFunc(tile))
                    {
                        result.Add(tile);
                    }
                }

                return result;
            }


            //获取相邻的坐标
            public static List<Vector3Int> GetNearPositions(MapType type, Vector3Int pivot)
            {

                List<Vector3Int> result = new List<Vector3Int>();

                switch (type)
                {

                    case MapType.FlatHexagon:
                    case MapType.PointyHexagon:
                        result.Add(new Vector3Int(pivot.x + 1, pivot.y, pivot.z));
                        result.Add(new Vector3Int(pivot.x, pivot.y, pivot.z + 1));
                        result.Add(new Vector3Int(pivot.x - 1, pivot.y, pivot.z));
                        result.Add(new Vector3Int(pivot.x, pivot.y, pivot.z - 1));
                        result.Add(new Vector3Int(pivot.x + 1, pivot.y, pivot.z - 1));
                        result.Add(new Vector3Int(pivot.x - 1, pivot.y, pivot.z + 1));
                        result.Add(new Vector3Int(pivot.x, pivot.y + 1, pivot.z));
                        result.Add(new Vector3Int(pivot.x, pivot.y - 1, pivot.z));
                        break;

                    case MapType.Rect:
                        result.Add(new Vector3Int(pivot.x + 1, pivot.y, pivot.z));
                        result.Add(new Vector3Int(pivot.x, pivot.y, pivot.z + 1));
                        result.Add(new Vector3Int(pivot.x - 1, pivot.y, pivot.z));
                        result.Add(new Vector3Int(pivot.x, pivot.y, pivot.z - 1));
                        result.Add(new Vector3Int(pivot.x, pivot.y + 1, pivot.z));
                        result.Add(new Vector3Int(pivot.x, pivot.y - 1, pivot.z));
                        break;
                }

                return result;
            }

            private class SearchingNode : IComparable<SearchingNode>
            {
                public SearchingNode parent;
                public Tile tile;
                public int totalCost;
                public int weight;

                public SearchingNode(Tile tile, SearchingNode parent, int totalCost, int weight)
                {
                    this.tile = tile;
                    this.parent = parent;
                    this.totalCost = totalCost;
                    this.weight = weight;
                }

                public SearchingNode(Tile tile, int weight) : this(tile, null, 0, weight) { }

                public int CompareTo(SearchingNode b)
                {
                    return weight.CompareTo(b.weight);
                }
            }

            public List<Tile> FindWayAStar(Tile origin, Tile destination)
            {
                return FindWayAStar(origin, destination, (_, _) => 1);
            }

            public List<Tile> FindWayAStar(Tile origin, Tile destination, Func<Tile, Tile, int> caculateCost)
            {

                var result = new List<Tile>();
                SearchingNode endNode = null;

                var allCosts = new BinaryHeap<SearchingNode>(HeapType.MinHeap);
                // SortedList<int,SearchingNode> allCosts = new SortedList<int,SearchingNode>();
                var existingPos = new List<Vector3Int>();

                allCosts.Push(new SearchingNode(origin, GetDistance(origin, destination)));
                existingPos.Add(origin.pos);

                while (true)
                {
                    if (allCosts.Count <= 0)
                    {
                        break;
                    }

                    var node = allCosts.PopRoot();

                    if (node.tile.pos == destination.pos)
                    {
                        endNode = node;
                        break;
                    }

                    foreach (var tile in node.tile.GetNearTiles())
                    {

                        if (existingPos.Contains(tile.pos))
                        {
                            continue;
                        }

                        var newCost = node.totalCost + caculateCost(node.tile, tile);
                        var weight = newCost + GetDistance(tile, destination);

                        allCosts.Push(new SearchingNode(tile, node, newCost, weight));
                        existingPos.Add(tile.pos);
                    }


                }

                if (endNode != null)
                {
                    var backNode = endNode;

                    result.Add(backNode.tile);

                    while (true)
                    {
                        if (backNode.parent == null)
                        {
                            break;
                        }
                        result.Add(backNode.parent.tile);
                        backNode = backNode.parent;
                    }
                }

                result.Reverse();

                return result;
            }

            public List<Tile> FindWayBFS(Tile origin, Tile destination)
            {
                return FindWayBFS(origin, destination, (_, _) => 1);
            }

            public List<Tile> FindWayBFS(Tile origin, Tile destination, Func<Tile, Tile, int> caculateCost)
            {

                var openList = new BinaryHeap<SearchingNode>(HeapType.MinHeap);
                var existedTiles = new List<Tile>();

                openList.Push(new SearchingNode(origin, 0));
                existedTiles.Add(origin);

                SearchingNode nodeToFind = null;

                var count = 0;
                while (true)
                {
                    SearchingNode nodeToProcess = null;

                    if (openList.Count <= 0)
                    {
                        break;
                    }
                    else
                    {
                        nodeToProcess = openList.PopRoot();
                    }

                    if (nodeToProcess.tile == destination)
                    {
                        nodeToFind = nodeToProcess;
                        break;
                    }

                    foreach (var tile in nodeToProcess.tile.GetNearTiles())
                    {

                        if (!existedTiles.Contains(tile))
                        {
                            var newCost = nodeToProcess.totalCost + caculateCost(nodeToProcess.tile, tile);
                            openList.Push(new SearchingNode(tile, nodeToProcess, newCost, newCost));

                            existedTiles.Add(tile);
                        }

                    }

                    count++;
                }

                var result = new List<Tile>();

                if (nodeToFind != null)
                {
                    var backNode = nodeToFind;

                    result.Add(backNode.tile);

                    while (true)
                    {
                        if (backNode.parent == null)
                        {
                            break;
                        }
                        result.Add(backNode.parent.tile);
                        backNode = backNode.parent;
                    }
                }

                result.Reverse();

                return result;
            }

            public int GetDistance(Tile a, Tile b)
            {
                int relX = a.x - b.x;
                int relY = a.y - b.y;
                int relZ = a.z - b.z;

                switch (config.mapType)
                {

                    case MapType.FlatHexagon:
                    case MapType.PointyHexagon:

                        if ((relX >= 0 && relY >= 0) || (relX <= 0 && relY <= 0))
                        {
                            return Mathf.Abs(relX) + Mathf.Abs(relY) + Mathf.Abs(relZ);
                        }
                        else
                        {
                            return Mathf.Max(Mathf.Abs(relX), Mathf.Abs(relY)) + Mathf.Abs(relZ);
                        }

                    case MapType.Rect:

                        return Mathf.Abs(relX) + Mathf.Abs(relY) + Mathf.Abs(relZ);

                }

                return 0;
            }

            #endregion

            #region RealCoordinate

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 GetRealPosition(Vector3Int tilePos)
            {
                return offset + tilePos.x * xBase + tilePos.y * yBase + tilePos.z * zBase;
            }

            /// <summary>
            /// 仅适用于固定大小地图且为矩形地图
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 GetRealSize()
            {
                Vector3 tileSize = new Vector3(xBase.x, yBase.y, zBase.z);

                Vector3 chunkSize = Vector3.Scale(config.chunkSize, tileSize);

                return Vector3.Scale(config.fixedSize, chunkSize);
            }

            /// <summary>
            /// 设置底部平的六边形地图基失
            /// </summary>
            /// <param name="realWidth"></param>
            /// <param name="realLength"></param>
            /// <param name="bottomWidth"></param>
            /// <param name="realHeight"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetHexBottomFlatBaseVector(float realWidth, float realLength, float bottomWidth, float realHeight)
            {
                this.xBase = new Vector3((bottomWidth + realWidth) / 2, 0, realLength / 2);
                this.zBase = new Vector3(0, 0, realLength);
                this.yBase = new Vector3(0, realHeight, 0);

                // this.xBase = new Vector3((bottomWidth + realWidth) / 2, realLength / 2, 0);
                // this.yBase = new Vector3(0, realLength, 0);
                // this.zBase = new Vector3(0, 0, realHeight);
            }

            /// <summary>
            /// 设置尖顶朝上的六边形地图基失
            /// </summary>
            /// <param name="realWidth"></param>
            /// <param name="realLength"></param>
            /// <param name="sidesHeight"></param>
            /// <param name="realHeight"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetHexTopArrisBaseVector(float realWidth, float realLength, float sidesHeight, float realHeight)
            {
                this.xBase = new Vector3(realWidth, 0, 0);
                this.zBase = new Vector3(realWidth / 2, 0, (realLength + sidesHeight) / 2);
                this.yBase = new Vector3(0, realHeight, 0);

                // this.xBase = new Vector3(realWidth, 0, 0);
                // this.yBase = new Vector3(realWidth / 2, (realLength + sidesHeight) / 2, 0);
                // this.zBase = new Vector3(0, 0, realHeight);
            }

            /// <summary>
            /// 设置四边形地图的基失
            /// </summary>
            /// <param name="realWidth"></param>
            /// <param name="realLength"></param>
            /// <param name="realHeight"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetRectBaseVector(float realWidth, float realLength, float realHeight)
            {
                this.xBase = new Vector3(realWidth, 0, 0);
                this.zBase = new Vector3(0, 0, realLength);
                this.yBase = new Vector3(0, realHeight, 0);

                // this.xBase = new Vector3(realWidth, 0, 0);
                // this.yBase = new Vector3(0, realLength, 0);
                // this.zBase = new Vector3(0, 0, realHeight);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetRectBaseVector(Vector3 cubeSize)
            {
                SetRectBaseVector(cubeSize.x, cubeSize.z, cubeSize.y);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetOffsetVector(Vector3 offset)
            {
                this.offset = offset;
            }

            #endregion
        }

        [HideReferenceObjectPicker]
        [HideDuplicateReferenceBox]
        [HideInEditorMode]
        public sealed class Chunk : IEnumerable<Tile>
        {

            /// <summary>
            /// 区块坐标x
            /// </summary>
            public int x => pos.x;
            /// <summary>
            /// 区块坐标y
            /// </summary>
            public int y => pos.y;
            /// <summary>
            /// 区块坐标z
            /// </summary>
            public int z => pos.z;
            /// <summary>
            /// 区块坐标
            /// </summary>
            [LabelText("区块坐标")]
            [Title("@pos.ToString()")]
            public Vector3Int pos;
            /// <summary>
            /// 区块第一个Tile的坐标
            /// </summary>
            [LabelText("区块第一个Tile的坐标")]
            public Vector3Int firstTilePos;

            /// <summary>
            /// 区块最小的Tile的x坐标
            /// </summary>
            public int minTileX => firstTilePos.x;
            /// <summary>
            /// 区块最小的Tile的y坐标
            /// </summary>
            public int minTileY => firstTilePos.y;
            /// <summary>
            /// 区块最小的Tile的z坐标
            /// </summary>
            public int minTileZ => firstTilePos.z;

            /// <summary>
            /// 区块最大的Tile的x坐标
            /// </summary>
            public int maxTileX => firstTilePos.x + width - 1;
            /// <summary>
            /// 区块最大的Tile的y坐标
            /// </summary>
            public int maxTileY => firstTilePos.y + height - 1;
            /// <summary>
            /// 区块最大的Tile的z坐标
            /// </summary>
            public int maxTileZ => firstTilePos.z + length - 1;

            /// <summary>
            /// 区块最小的Tile的xy坐标
            /// </summary>
            public Vector2Int minTileXY => new(minTileX, minTileY);
            /// <summary>
            /// 区块最小的Tile的xz坐标
            /// </summary>
            public Vector2Int minTileXZ => new(minTileX, minTileZ);
            /// <summary>
            /// 区块最小的Tile的yz坐标
            /// </summary>
            public Vector2Int minTileYZ => new(minTileY, minTileZ);

            /// <summary>
            /// 区块最大的Tile的xy坐标
            /// </summary>
            public Vector2Int maxTileXY => new(maxTileX, maxTileY);
            /// <summary>
            /// 区块最大的Tile的xz坐标
            /// </summary>
            public Vector2Int maxTileXZ => new(maxTileX, maxTileZ);
            /// <summary>
            /// 区块最大的Tile的yz坐标
            /// </summary>
            public Vector2Int maxTileYZ => new(maxTileY, maxTileZ);

            /// <summary>
            /// 区块宽度
            /// </summary>
            public int width => size.x;
            /// <summary>
            /// 区块高度
            /// </summary>
            public int height => size.y;
            /// <summary>
            /// 区块长度
            /// </summary>
            public int length => size.z;
            /// <summary>
            /// 区块大小尺寸
            /// </summary>
            [LabelText("区块大小")]
            public Vector3Int size;

            [LabelText("区块数据"), HideReferenceObjectPicker]
            public C data = new C();

            [NonSerialized]
            private Tile[,,] tiles;

            [LabelText("所属地图")]
            public Map originMap;

            [LabelText("区块内Tile总数")]
            public int tilesCount;

            [LabelText("是否在世界边界")]
            public bool isInWorldEdge = true;

            /// <summary>
            /// pos with offset (1, 0, 0)
            /// </summary>
            [LabelText("右区块(1, 0, 0)")]
            public Chunk right;
            /// <summary>
            /// pos with offset (-1, 0, 0)
            /// </summary>
            [LabelText("左区块(-1, 0, 0)")]
            public Chunk left;
            /// <summary>
            /// pos with offset (0, 1, 0)
            /// </summary>
            [LabelText("上区块(0, 1, 0))")]
            public Chunk up;
            /// <summary>
            /// pos with offset (0, -1, 0)
            /// </summary>
            [LabelText("下区块(0, -1, 0)")]
            public Chunk down;
            /// <summary>
            /// pos with offset (0, 0, 1)
            /// </summary>
            [LabelText("前区块(0, 0, 1)")]
            public Chunk forward;
            /// <summary>
            /// pos with offset (0, 0, -1)
            /// </summary>
            [LabelText("后区块(0, 0, -1)")]
            public Chunk back;

            public delegate void TileEvent(Chunk chunk, Tile tile);

            public List<Tile> tilesNeedToUpdate = new();

            public IEnumerable<Vector2Int> xyPlane
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            yield return new(firstTilePos.x + i, firstTilePos.y + j);
                        }
                    }
                }
            }

            public IEnumerable<Vector2Int> xzPlane
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    for (int i = 0; i < width; i++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            yield return new(firstTilePos.x + i, firstTilePos.z + k);
                        }
                    }
                }
            }

            public IEnumerable<Vector2Int> yzPlane
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            yield return new(firstTilePos.y + j, firstTilePos.z + k);
                        }
                    }
                }
            }

            public IEnumerable<int> xRange
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    for (int i = 0; i < width; i++)
                    {
                        yield return firstTilePos.x + i;
                    }
                }
            }

            public IEnumerable<int> yRange
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    for (int j = 0; j < height; j++)
                    {
                        yield return firstTilePos.y + j;
                    }
                }
            }

            public IEnumerable<int> zRange
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get
                {
                    for (int k = 0; k < length; k++)
                    {
                        yield return firstTilePos.z + k;
                    }
                }
            }

            public Chunk(Map originMap, Vector3Int chunkPos)
            {
                this.originMap = originMap;

                this.size = originMap.config.chunkSize;

                if (size.AnyNumberBelowOrEqual(0))
                {
                    Note.note.Error($"区块大小不可小于1,当前值为{size}");
                }

                this.tilesCount = size.Products();

                this.pos = chunkPos;

                this.tiles = new Tile[width, height, length];

                size.GetCubeOfPoints().Examine(pos =>
                {
                    tiles.Set(pos, 
                        new(this, GetTilePos(pos), pos));
                });

                ForeachTileInFace(tile =>
                {
                    tile.isInChunkFace = true;
                });
                ForeachTileInFace(originMap.config.visibleFaces, tile =>
                {
                    tile.isInWorldFace = true;
                });
                ForeachTileInEdge(tile => tile.isInChunkEdge = true);

                foreach (Tile tile in this.tiles)
                {
                    tile.forward = GetTile(tile.posInChunk + Vector3Int.forward);
                    tile.back = GetTile(tile.posInChunk + Vector3Int.back);
                    tile.up = GetTile(tile.posInChunk + Vector3Int.up);
                    tile.down = GetTile(tile.posInChunk + Vector3Int.down);
                    tile.right = GetTile(tile.posInChunk + Vector3Int.right);
                    tile.left = GetTile(tile.posInChunk + Vector3Int.left);
                }

                this.firstTilePos = GetTilePos(new Vector3Int(0, 0, 0));

                Update();

                data.chunk = this;
                data.OnLoad();
            }

            #region GetTile

            public IEnumerable<Tile> GetAllTiles()
            {
                return tiles.Cast<Tile>();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tile GetTile(Vector3Int tilePos)
            {
                Vector3Int tilePosAfterModified = tilePos.Modulo(size);

                return tiles[tilePosAfterModified.x, tilePosAfterModified.y, tilePosAfterModified.z];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [Button("获取瓦片")]
            public Tile GetTileByPosInChunk(Vector3Int posInChunk)
            {
                if (posInChunk.x >= size.x)
                {
                    Note.note.Error($"posInChunk的x坐标越界，x为 {posInChunk.x} 而Chunk的size.x 为 {size.x}");
                }

                if (posInChunk.y >= size.y)
                {
                    Note.note.Error($"posInChunk的y坐标越界，y为 {posInChunk.y} 而Chunk的size.y 为 {size.y}");
                }

                if (posInChunk.z >= size.z)
                {
                    Note.note.Error($"posInChunk的z坐标越界，z为 {posInChunk.z} 而Chunk的size.z 为 {size.z}");
                }

                return tiles[posInChunk.x, posInChunk.y, posInChunk.z];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tile GetRandomTile()
            {
                return tiles.Get(size.RandomRange());
            }

            #endregion

            #region GetChunk

            public Chunk GetChunk(FaceType faceType)
            {
                return faceType switch
                {
                    FaceType.Right => right,
                    FaceType.Left => left,
                    FaceType.Up => up,
                    FaceType.Down => down,
                    FaceType.Forward => forward,
                    FaceType.Back => back,
                    _ => throw new ArgumentOutOfRangeException(nameof(faceType), faceType, null)
                };
            }

            public IEnumerable<(FaceType, Chunk)> GetNearChunks()
            {
                if (right != null)
                {
                    yield return (FaceType.Right, right);
                }

                if (left != null)
                {
                    yield return (FaceType.Left, left);
                }

                if (up != null)
                {
                    yield return (FaceType.Up, up);
                }

                if (down != null)
                {
                    yield return (FaceType.Down, down);
                }

                if (forward != null)
                {
                    yield return (FaceType.Forward, forward);
                }

                if (back != null)
                {
                    yield return (FaceType.Back, back);
                }
            }

            #endregion

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3Int GetTilePos(Vector3Int tileRelativePos)
            {
                return Vector3Int.Scale(pos, size) + tileRelativePos;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 GetRealPosition()
            {
                return originMap.GetRealPosition(firstTilePos);
            }

            #region Traversal

            public void ForeachNearChunks(Action<Chunk> func)
            {
                if (forward != default(Chunk))
                {
                    func(forward);
                }

                if (back != default(Chunk))
                {
                    func(back);
                }

                if (up != default(Chunk))
                {
                    func(up);
                }

                if (down != default(Chunk))
                {
                    func(down);
                }

                if (right != default(Chunk))
                {
                    func(right);
                }

                if (left != default(Chunk))
                {
                    func(left);
                }
            }

            public void ForeachTile(Action<Tile> func)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            func(tiles[i, j, k]);
                        }
                    }
                }
            }

            public void ForeachTile(Action<Chunk, Tile> func)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            func(this, tiles[i, j, k]);
                        }
                    }
                }
            }

            public IEnumerator<Tile> GetEnumerator()
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        for (int k = 0; k < length; k++)
                        {
                            yield return tiles[i, j, k];
                        }
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void ForeachInnerTile(Action<Tile> action)
            {
                size.GetInnerPointsOfCube().Examine(pos => action(tiles.Get(pos)));
            }

            public void ForeachTileInFace(Action<Tile> action)
            {
                size.GetAllFacePointsOfCube().Examine(pos => action(tiles.Get(pos)));
            }

            public void ForeachTileInFace(FaceType faceType, Action<Tile> action)
            {
                size.GetFacePointsOfCube(faceType).Examine(pos => action(tiles.Get(pos)));
            }

            public void ForeachTileInEdge(Action<Tile> action)
            {
                size.GetAllEdgePointsOfCube().Examine(pos => action(tiles.Get(pos)));
            }

            // public void UpdateEdgeTiles() {
            //     int chunksCount = 0;

            //     ForeachTileInEdge((Tile tile) => {
            //         tile.UpdateNearTiles();

            //         chunksCount++;
            //     });

            //     if (chunksCount < 6) {
            //         isInWorldFace = true;
            //     } else {
            //         isInWorldFace = false;
            //     }
            // }

            #endregion


            public void Update()
            {
                int nearChunkCount = 0;

                FaceType validFaces = 0;

                //forward
                var newForward = originMap.GetChunk(pos + Vector3Int.forward);
                if (forward != newForward)
                {
                    if (forward != default)
                    {
                        forward.back = this;
                        nearChunkCount++;

                        ForeachTileInFace(FaceType.Forward, tile =>
                        {
                            tile.forward = 
                                forward.tiles[tile.posInChunk.x, tile.posInChunk.y, 0];
                            tile.forward.back = tile;

                            tile.isInWorldFace = false;
                            tile.forward.isInWorldFace = false;

                            tile.nearChunks.Add(forward);
                            tile.forward.nearChunks.Add(this);

                            tilesNeedToUpdate.Add(tile.forward);
                        });

                        validFaces |= FaceType.Forward;
                    }
                    else
                    {
                        ForeachTileInFace(FaceType.Forward, tile =>
                        {
                            tile.forward = default;
                            if (originMap.config.visibleFaces.HasFlag(FaceType.Forward))
                                tile.isInWorldFace = true;

                            tilesNeedToUpdate.Add(tile);
                        });
                    }
                }
                

                //back
                var newBack = originMap.GetChunk(pos + Vector3Int.back);
                if (back != newBack)
                {
                    if (back != default)
                    {
                        back.forward = this;
                        nearChunkCount++;

                        ForeachTileInFace(FaceType.Back, tile =>
                        {
                            tile.back = 
                                back.tiles[tile.posInChunk.x, tile.posInChunk.y, size.z - 1];
                            tile.back.forward = tile;

                            tile.isInWorldFace = false;
                            tile.back.isInWorldFace = false;

                            tile.nearChunks.Add(back);
                            tile.back.nearChunks.Add(this);

                            tilesNeedToUpdate.Add(tile.back);
                        });

                        validFaces |= FaceType.Back;
                    }
                    else
                    {
                        ForeachTileInFace(FaceType.Back, tile =>
                        {
                            tile.back = default;
                            if (originMap.config.visibleFaces.HasFlag(FaceType.Back))
                                tile.isInWorldFace = true;

                            tilesNeedToUpdate.Add(tile);
                        });
                    }
                }

                //up
                var newUp = originMap.GetChunk(pos + Vector3Int.up);
                if (up != newUp)
                {
                    if (up != default)
                    {
                        up.down = this;
                        nearChunkCount++;

                        ForeachTileInFace(FaceType.Up, tile =>
                        {
                            tile.up = 
                                up.tiles[tile.posInChunk.x, 0, tile.posInChunk.z];
                            tile.up.down = tile;

                            tile.isInWorldFace = false;
                            tile.up.isInWorldFace = false;

                            tile.nearChunks.Add(up);
                            tile.up.nearChunks.Add(this);

                            tilesNeedToUpdate.Add(tile.up);
                        });

                        validFaces |= FaceType.Up;
                    }
                    else
                    {
                        ForeachTileInFace(FaceType.Up, tile =>
                        {
                            tile.up = default;
                            if (originMap.config.visibleFaces.HasFlag(FaceType.Up))
                                tile.isInWorldFace = true;

                            tilesNeedToUpdate.Add(tile);
                        });
                    }
                }

                //down
                var newDown = originMap.GetChunk(pos + Vector3Int.down);
                if (down != newDown)
                {
                    if (down != default)
                    {
                        down.up = this;
                        nearChunkCount++;

                        ForeachTileInFace(FaceType.Down, tile =>
                        {
                            tile.down = 
                                down.tiles[tile.posInChunk.x, size.y - 1, tile.posInChunk.z];
                            tile.down.up = tile;

                            tile.isInWorldFace = false;
                            tile.down.isInWorldFace = false;

                            tile.nearChunks.Add(down);
                            tile.down.nearChunks.Add(this);

                            tilesNeedToUpdate.Add(tile.down);
                        });

                        validFaces |= FaceType.Down;
                    }
                    else
                    {
                        ForeachTileInFace(FaceType.Down, tile =>
                        {
                            tile.down = default;
                            if (originMap.config.visibleFaces.HasFlag(FaceType.Down))
                                tile.isInWorldFace = true;

                            tilesNeedToUpdate.Add(tile);
                        });
                    }
                }

                //right
                var newRight = originMap.GetChunk(pos + Vector3Int.right);
                if (right != newRight)
                {
                    if (right != default)
                    {
                        right.left = this;
                        nearChunkCount++;

                        ForeachTileInFace(FaceType.Right, tile =>
                        {
                            tile.right = 
                                right.tiles[0, tile.posInChunk.y, tile.posInChunk.z];
                            tile.right.left = tile;

                            tile.isInWorldFace = false;
                            tile.right.isInWorldFace = false;

                            tile.nearChunks.Add(right);
                            tile.right.nearChunks.Add(this);

                            tilesNeedToUpdate.Add(tile.right);
                        });

                        validFaces |= FaceType.Right;
                    }
                    else
                    {
                        ForeachTileInFace(FaceType.Right, tile =>
                        {
                            tile.right = default;
                            if (originMap.config.visibleFaces.HasFlag(FaceType.Right))
                                tile.isInWorldFace = true;

                            tilesNeedToUpdate.Add(tile);
                        });
                    }
                }

                //left
                var newLeft = originMap.GetChunk(pos + Vector3Int.left);
                if (left != newLeft)
                {
                    if (left != default)
                    {
                        left.right = this;
                        nearChunkCount++;

                        ForeachTileInFace(FaceType.Left, tile =>
                        {
                            tile.left = 
                                left.tiles[size.x - 1, tile.posInChunk.y, tile.posInChunk.z];
                            tile.left.right = tile;

                            tile.isInWorldFace = false;
                            tile.left.isInWorldFace = false;

                            tile.nearChunks.Add(left);
                            tile.left.nearChunks.Add(this);

                            tilesNeedToUpdate.Add(tile.left);
                        });

                        validFaces |= FaceType.Left;
                    }
                    else
                    {
                        ForeachTileInFace(FaceType.Left, tile =>
                        {
                            tile.left = default;
                            if (originMap.config.visibleFaces.HasFlag(FaceType.Left))
                                tile.isInWorldFace = true;

                            tilesNeedToUpdate.Add(tile);
                        });
                    }
                }

                if (validFaces.HasFlag(originMap.config.visibleFaces))
                {
                    isInWorldEdge = false;
                }

                static void Temp(Tile tile)
                {
                    if (tile.forward == default(Tile) || tile.back == default(Tile) || tile.up == default(Tile) || tile.down == default(Tile) || tile.right == default(Tile) || tile.left == default(Tile))
                    {
                        tile.isInWorldFace = true;
                    }
                    else
                    {
                        tile.isInWorldFace = false;
                    }
                }

                ForeachTileInEdge(Temp);
                ForeachNearChunks(chunk =>
                {
                    chunk.ForeachTileInEdge(Temp);
                });


            }
        }

        [HideReferenceObjectPicker]
        [HideDuplicateReferenceBox]
        [HideInEditorMode]
        public sealed class Tile
        {
            public int x => pos.x;
            public int y => pos.y;
            public int z => pos.z;
            public Vector2Int xy => pos.XY();
            public Vector2Int xz => pos.XZ();
            public Vector2Int yz => pos.YZ();
            [LabelText("全局坐标")]
            public Vector3Int pos;

            public float xF => pos.x;
            public float yF => pos.y;
            public float zF => pos.z;
            public Vector2 xyF => pos.XY();
            public Vector2 xzF => pos.XZ();
            public Vector2 yzF => pos.YZ();
            public Vector3 posF => pos;

            public int xInChunk => posInChunk.x;
            public int yInChunk => posInChunk.y;
            public int zInChunk => posInChunk.z;
            public Vector2Int xyInChunk => posInChunk.XY();
            public Vector2Int xzInChunk => posInChunk.XZ();
            public Vector2Int yzInChunk => posInChunk.YZ();
            [LabelText("区块内坐标")]
            public Vector3Int posInChunk;


            public float xInChunkF => posInChunk.x;
            public float yInChunkF => posInChunk.y;
            public float zInChunkF => posInChunk.z;
            public Vector2 xyInChunkF => posInChunk.XY();
            public Vector2 xzInChunkF => posInChunk.XZ();
            public Vector2 yzInChunkF => posInChunk.YZ();
            public Vector3 posInChunkF => posInChunk;

            [LabelText("是否在区块边界棱上")]
            public bool isInChunkEdge = false;
            [LabelText("是否在区块边界面上")]
            public bool isInChunkFace = false;
            [LabelText("是否在世界边界面上")]
            public bool isInWorldFace = false;

            [LabelText("周围的区块")]
            public List<Chunk> nearChunks = new();

            /// <summary>
            /// pos with offset (1, 0, 0)
            /// </summary>
            [LabelText("右(1, 0, 0)")]
            public Tile right;
            /// <summary>
            /// pos with offset (-1, 0, 0)
            /// </summary>
            [LabelText("左(-1, 0, 0)")]
            public Tile left;
            /// <summary>
            /// pos with offset (0, 1, 0)
            /// </summary>
            [LabelText("上(0, 1, 0)")]
            public Tile up;
            /// <summary>
            /// pos with offset (0, -1, 0)
            /// </summary>
            [LabelText("下(0, -1, 0)")]
            public Tile down;
            /// <summary>
            /// pos with offset (0, 0, 1)
            /// </summary>
            [LabelText("前(0, 0, 1)")]
            public Tile forward;
            /// <summary>
            /// pos with offset (0, 0, -1)
            /// </summary>
            [LabelText("后(0, 0, -1)")]
            public Tile back;


            public Map originMap => originChunk.originMap;

            [LabelText("所属于的区块")]
            public Chunk originChunk;

            [LabelText("Tile数据"), HideReferenceObjectPicker]
            public T data = new();

            [LabelText("在区块里的遍历Index")]
            public int foreachIndexInChunk;

            public Tile(Chunk originChunk, Vector3Int tilePos, Vector3Int posInChunk)
            {

                this.originChunk = originChunk;

                this.pos = tilePos;
                this.posInChunk = posInChunk;

                this.foreachIndexInChunk = 
                    zInChunk + 
                    yInChunk * originChunk.length + 
                    xInChunk * originChunk.length * originChunk.height;

                data.tile = this;

                data.OnLoad();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Tile GetTile(FaceType faceType)
            {
                return faceType switch
                {
                    FaceType.Right => right,
                    FaceType.Left => left,
                    FaceType.Up => up,
                    FaceType.Down => down,
                    FaceType.Forward => forward,
                    FaceType.Back => back,
                    _ => throw new ArgumentOutOfRangeException(nameof(faceType), faceType, null)
                };
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetTile(FaceType faceType, Tile newTile)
            {
                switch (faceType)
                {
                    case FaceType.Right:
                        right = newTile;
                        break;
                    case FaceType.Left:
                        left = newTile;
                        break;
                    case FaceType.Up:
                        up = newTile;
                        break;
                    case FaceType.Down:
                        down = newTile;
                        break;
                    case FaceType.Forward:
                        forward = newTile;
                        break;
                    case FaceType.Back:
                        back = newTile;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(faceType), 
                            faceType, null);
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 GetRealPosition()
            {
                return originMap.GetRealPosition(pos);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 GetRealPositionInChunk()
            {
                return originMap.GetRealPosition(posInChunk);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public List<Tile> GetNearTiles(Func<Tile, bool> judgeFunc)
            {
                if (originMap == null)
                {
                    Note.note.Error("没有初始化该Tile所在地图");
                    return null;
                }
                return originMap.GetNearTiles(this, judgeFunc);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public List<Tile> GetNearTiles()
            {
                return GetNearTiles(_ => true);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void ForeachNearChunks(Action<Chunk> func)
            {
                foreach (Chunk chunk in nearChunks)
                {
                    func(chunk);
                }
            }


        }
    }



}