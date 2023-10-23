using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ExtendedTilemap
{
    public class AnimationInfo
    {
        public Rule rule;

        public List<Sprite> sprites;

        public int currentIndex = 0;

        public float timeLeftToNext;

        public bool loop = false;

        public bool enable = true;
    }

    public struct Neighbor
    {
        public ExtRuleTile upperLeft, upper, upperRight;
        public ExtRuleTile left, right;
        public ExtRuleTile lowerLeft, lower, lowerRight;
    }

    [RequireComponent(typeof(Tilemap))]

    public class ExtTilemap : SerializedMonoBehaviour
    {
        public ExtRuleTileGeneralSetting setting => GameCoreSettingBase.extRuleTileGeneralSetting;

        [LabelText("运行时一开始清除地图")]
        public bool clearMapOnAwake = false;

        [LabelText("所有Tiles")]
        public Dictionary<Vector3Int, ExtRuleTile> allRuleTiles = new();

        [LabelText("所有的动画信息")]
        public Dictionary<Vector3Int, AnimationInfo> allAnimationInfos = new();

        private Tilemap _tilemap;

        private Tilemap tilemap
        {
            get
            {
                if (_tilemap == null)
                {
                    _tilemap = GetComponent<Tilemap>();
                }

                return _tilemap;
            }
        }

        private void Awake()
        {
            if (clearMapOnAwake)
            {
                ClearMap();
            }
        }

        #region Animation

        private void Update()
        {
            foreach (var (pos, animationInfo) in allAnimationInfos)
            {
                if (animationInfo.enable == false)
                {
                    continue;
                }

                if (animationInfo.sprites == null || animationInfo.sprites.Count == 0)
                {
                    continue;
                }

                animationInfo.timeLeftToNext -= Time.deltaTime;

                if (animationInfo.timeLeftToNext <= 0)
                {
                    animationInfo.timeLeftToNext = animationInfo.rule.gap;

                    animationInfo.currentIndex++;

                    if (animationInfo.currentIndex >= animationInfo.sprites.Count)
                    {
                        animationInfo.currentIndex = 0;

                        if (animationInfo.loop == false)
                        {
                            animationInfo.enable = false;
                        }
                    }
                }

                var sprite = setting.GetTileBase(animationInfo.sprites[animationInfo.currentIndex]);

                tilemap.SetTile(pos, sprite);
            }
        }

        public void Play(Vector3Int pos, bool loop)
        {
            if (allAnimationInfos.TryGetValue(pos, out var animationInfo))
            {
                animationInfo.enable = true;
                animationInfo.loop = loop;
            }
        }

        public void Replay(Vector3Int pos, bool loop)
        {
            if (allAnimationInfos.TryGetValue(pos, out var animationInfo))
            {
                animationInfo.enable = true;
                animationInfo.loop = loop;
                animationInfo.currentIndex = 0;
            }
        }

        public void Stop(Vector3Int pos)
        {
            if (allAnimationInfos.TryGetValue(pos, out var animationInfo))
            {
                animationInfo.enable = false;
            }
        }

        #endregion

        #region RealPosition

        public Vector3 GetRealPosition(Vector3Int pos)
        {
            return tilemap.CellToWorld(pos);
        }

        public Vector3Int GetTilePosition(Vector3 realPos)
        {
            return tilemap.WorldToCell(realPos);
        }

        public Vector3 GetCellSize()
        {
            return tilemap.cellSize;
        }

        #endregion



        public Sprite GetSprite(Vector3Int pos)
        {
            return tilemap.GetSprite(pos);
        }

        public void SetSprite(Vector3Int pos, Sprite sprite)
        {
            tilemap.SetTile(pos, GameCoreSettingBase.extRuleTileGeneralSetting.GetTileBase(sprite));
        }

        public bool TryGetTile(Vector3Int pos, out ExtRuleTile tile)
        {
            return allRuleTiles.TryGetValue(pos, out tile);
        }

        public ExtRuleTile GetTile(Vector3Int pos)
        {
            return allRuleTiles.TryGetValue(pos, out var extRuleTile) ? extRuleTile : null;
        }

        public bool HasTile(Vector3Int pos)
        {
            return allRuleTiles.ContainsKey(pos);
        }

        private void ForceUpdate(Vector3Int pos)
        {
            if (TryGetTile(pos, out var extRuleTile))
            {
                var id = extRuleTile.id;

                var neighbor = GetNeighbor(pos);

                var rule = setting.GetRule(id, neighbor);

                if (rule == null)
                {
                    allAnimationInfos.Remove(pos);
                    var defaultTileBase = setting.GetDefaultTileBase(id);
                    tilemap.SetTile(pos, defaultTileBase);
                    return;
                }

                if (rule.enableAnimation)
                {
                    if (allAnimationInfos.TryGetValue(pos, out var animationInfo))
                    {
                        if (animationInfo.rule == rule)
                        {
                            return;
                        }
                    }
                    else
                    {
                        animationInfo = new();
                        allAnimationInfos[pos] = animationInfo;
                    }

                    animationInfo.rule = rule;
                    animationInfo.sprites = rule.animationSprites;
                    animationInfo.timeLeftToNext = rule.gap;

                    if (rule.autoPlayOnStart)
                    {
                        animationInfo.enable = true;
                        animationInfo.loop = true;
                    }
                    else
                    {
                        animationInfo.enable = false;
                        animationInfo.loop = false;
                    }
                }
                else
                {
                    allAnimationInfos.Remove(pos);
                    var tileBase = setting.GetTileBase(rule.sprite);
                    tilemap.SetTile(pos, tileBase);
                }
            }
        }

        private void SetTileWithoutUpdate(Vector3Int pos, string id)
        {
            allRuleTiles[pos] = setting.GetPrefabStrictly(id);

            ForceUpdate(pos);
        }

        /// <summary>
        /// 放置瓦片
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="id"></param>
        [Button("放置瓦片", ButtonStyle.Box)]
        public void SetTile([LabelText("坐标")] Vector3Int pos,
            [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")] [HideLabel]
            string id)
        {
            SetTileWithoutUpdate(pos, id);

            foreach (var neighborPos in GetNeighborPoses(pos))
            {
                UpdateTile(neighborPos);
            }
        }

        /// <summary>
        /// 在矩形区域放置特定ID的瓦片
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="id"></param>
        public void SetTiles(Vector3Int startPos, Vector3Int endPos, string id)
        {
            SetTiles(new(startPos, endPos), id);
        }

        /// <summary>
        /// 在矩形区域放置特定ID的瓦片
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="id"></param>
        [Button("在矩形区域放置特定ID的瓦片", ButtonStyle.Box)]
        public void SetTiles([HideLabel] CubeInt cube,
            [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")] [HideLabel]
            string id)
        {
            if (cube == null)
            {
                return;
            }

            foreach (var pos in cube.GetAllPoints())
            {
                SetTile(pos, id);
            }
        }

        /// <summary>
        /// 在矩形区域内放置随机数量的特定ID的瓦片
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="id"></param>
        /// <param name="number"></param>
        [Button("在矩形区域内放置随机数量的特定ID的瓦片", ButtonStyle.Box)]
        public void SetRandomTiles([HideLabel] CubeInt cube,
            [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")] [HideLabel]
            string id,
            [MinValue(1)] int number = 1)
        {
            if (cube == null)
            {
                return;
            }

            foreach (var pos in cube.GetRandomPoints(number))
            {
                SetTile(pos, id);
            }
        }

        /// <summary>
        /// 在矩形区域内放置随机数量的特定ID的瓦片
        /// </summary>
        /// <param name="cube"></param>
        /// <param name="id"></param>
        /// <param name="rate"></param>
        [Button("在矩形区域内随机放置特定面积占比数量的特定ID的瓦片", ButtonStyle.Box)]
        public void SetRandomTiles([HideLabel] CubeInt cube,
            [ValueDropdown("@GameCoreSettingBase.extRuleTileGeneralSetting.GetPrefabNameList()")] [HideLabel]
            string id,
            [PropertyRange(0, 1)] float rate = 0.5f)
        {
            if (cube == null)
            {
                return;
            }

            foreach (var pos in cube.GetRandomPoints(rate))
            {
                SetTile(pos, id);
            }
        }

        /// <summary>
        /// 更新瓦片贴图
        /// </summary>
        /// <param name="pos"></param>
        public void UpdateTile(Vector3Int pos)
        {
            if (allRuleTiles.TryGetValue(pos, out var extRuleTile))
            {
                if (extRuleTile.enableUpdate)
                {
                    ForceUpdate(pos);
                }
            }
            else
            {
                SetTileWithoutUpdate(pos, ExtRuleTileGeneralSetting.EMPTY_TILE_ID);
            }
        }

        /// <summary>
        /// 清除瓦片
        /// </summary>
        /// <param name="pos"></param>
        [Button("清除瓦片", ButtonStyle.Box)]
        public void ClearTile([LabelText("坐标")] Vector3Int pos)
        {
            //if (allRuleTiles.ContainsKey(pos))
            //{
            //    allRuleTiles.Remove(pos);

            //    foreach (var neighborPos in GetNeighborPoses(pos))
            //    {
            //        UpdateTile(neighborPos);
            //    }
            //}

            //tilemap.SetTile(pos, GameCoreSettingBase.extRuleTileGeneralSetting.emptyTileBase);
            SetTile(pos, ExtRuleTileGeneralSetting.EMPTY_TILE_ID);
        }

        /// <summary>
        /// 更新所有瓦片的贴图
        /// </summary>
        [Button("刷新地图", ButtonStyle.Box)]
        public void RefreshMap()
        {
            foreach (var pos in allRuleTiles.Keys)
            {
                UpdateTile(pos);
            }
        }

        /// <summary>
        /// 清空地图
        /// </summary>
        [Button("清空地图")]
        public void ClearMap()
        {
            allRuleTiles.Clear();
            tilemap.ClearAllTiles();
            allAnimationInfos.Clear();
        }

        private Neighbor GetNeighbor(Vector3Int pos)
        {
            return new Neighbor
            {
                upperLeft = GetTile(pos + new Vector3Int(-1, 1, 0)),
                upper = GetTile(pos + new Vector3Int(0, 1, 0)),
                upperRight = GetTile(pos + new Vector3Int(1, 1, 0)),
                left = GetTile(pos + new Vector3Int(-1, 0, 0)),
                right = GetTile(pos + new Vector3Int(1, 0, 0)),
                lowerLeft = GetTile(pos + new Vector3Int(-1, -1, 0)),
                lower = GetTile(pos + new Vector3Int(0, -1, 0)),
                lowerRight = GetTile(pos + new Vector3Int(1, -1, 0))
            };
        }

        private IEnumerable<Vector3Int> GetNeighborPoses(Vector3Int pos)
        {
            return new[]
            {
                pos + new Vector3Int(-1, 1, 0),
                pos + new Vector3Int(0, 1, 0),
                pos + new Vector3Int(1, 1, 0),
                pos + new Vector3Int(-1, 0, 0),
                pos + new Vector3Int(1, 0, 0),
                pos + new Vector3Int(-1, -1, 0),
                pos + new Vector3Int(0, -1, 0),
                pos + new Vector3Int(1, -1, 0)
            };
        }
    }
}
