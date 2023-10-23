using Basis;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace MapBasis
{
    [HideInEditorMode]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [ShowInInspector]
    [LabelText("等距地图核心")]
    public class IsometricMap
    {
        [Serializable]
        public struct Config
        {
            [LabelText("容器")]
            public Transform container;
            [LabelText("Tilemap的预制体")]
            public Tilemap tilemapPrefab;

            [LabelText("透明的TileBase")]
            public TileBase opaqueTileBase;

            [LabelText("高度间隔")]
            public float heightGapRate;
        }

        [LabelText("所有的Tilemaps")]
        [SerializeField, DictionaryDrawerSettings(IsReadOnly = true)]
        private readonly Dictionary<int, Tilemap> allTilemaps = new();

        [LabelText("最小Z坐标")]
        public int minZ = int.MaxValue;

        [LabelText("最大Z坐标")]
        public int maxZ = int.MinValue;

        [LabelText("配置文件")]
        public Config config;

        public IsometricMap(Config newConfig)
        {
            config = newConfig;

            if (config.container.GetComponent<Grid>() == null)
            {
                Note.note.Error("等距地图配置中的Container必须要有Grid组件");
            }

            if (config.tilemapPrefab.GetComponent<TilemapRenderer>() == null)
            {
                Note.note.Error("等距地图配置中的tilemapPrefab必须要有TilemapRenderer组件");
            }

            
        }

        public Tilemap GetTilemap(int z)
        {
            if (allTilemaps.ContainsKey(z))
            {
                return allTilemaps[z];
            }
            else
            {
                return null;
            }
        }

        public void SetTile(Vector3Int pos, TileBase tileBase, bool withCheck = true)
        {
            if (withCheck == true)
            {
                CheckZ(pos.z, createIfNonExistent: true);
            }
            

            allTilemaps[pos.z].SetTile(new(pos.x, pos.y), tileBase);
        }

        public void BoxFill(Vector3Int from, Vector3Int to, TileBase tileBase, bool overwrite = true)
        {
            CubeInt cube = new(from, to);

            foreach (var z in cube.zPoints.GetAllPoints())
            {
                CheckZ(z, createIfNonExistent: true);
            }

            if (overwrite == true)
            {
                foreach (var pos in cube.GetAllPoints())
                {
                    SetTile(pos, tileBase, false);
                }
            }
            else
            {
                foreach (var pos in cube.GetAllPoints())
                {
                    if (HasTile(pos))
                    {
                        SetTile(pos, tileBase, false);
                    }
                }
            }
            
        }

        public void ClearTile(Vector3Int pos)
        {
            if (CheckZ(pos.z) == false)
            {
                return;
            }

            allTilemaps[pos.z].SetTile(new(pos.x, pos.y), config.opaqueTileBase);
        }

        public TileBase GetTile(Vector3Int pos)
        {
            if (CheckZ(pos.z) == false)
            {
                return null;
            }

            return allTilemaps[pos.z].GetTile(new(pos.x, pos.y));
        }

        public bool HasTile(Vector3Int pos)
        {
            if (CheckZ(pos.z) == false)
            {
                return false;
            }

            if (allTilemaps[pos.z].HasTile(pos) == false)
            {
                return false;
            }

            if (allTilemaps[pos.z].GetTile(pos) == config.opaqueTileBase)
            {
                return false;
            }

            return true;
        }

        private bool CheckZ(int z, bool createIfNonExistent = false)
        {
            bool result = true;

            if (!allTilemaps.ContainsKey(z))
            {
                result = false;
                if (createIfNonExistent == false)
                {
                    return result;
                }
            }

            if (result == true && allTilemaps[z] == null)
            {
                Note.note.Warning($"z={z}的Tilemap已注册，但却为null");

                result = false;
                if (createIfNonExistent == false)
                {
                    return result;
                }
            }

            if (result == false)
            {
                if (z < minZ)
                {
                    minZ = z;
                }
                if (z > maxZ)
                {
                    maxZ = z;
                }

                Tilemap newTilemap = Object.Instantiate(config.tilemapPrefab, config.container);
                allTilemaps[z] = newTilemap;
                newTilemap.ClearAllTiles();
                newTilemap.transform.localPosition = new(0, z * newTilemap.cellSize.y * config.heightGapRate, 0);
                newTilemap.transform.localRotation = Quaternion.identity;

                TilemapRenderer newTilemapRenderer = newTilemap.GetComponent<TilemapRenderer>();
                newTilemapRenderer.sortingLayerName = "Landform";
                newTilemapRenderer.sortingOrder = z;

                newTilemap.gameObject.name = $"z={z}";

                foreach (var kvp in allTilemaps)
                {
                    kvp.Value.transform.SetSiblingIndex(kvp.Key - minZ);
                }

                result = true;
            }

            return result;
        }
    }
}

