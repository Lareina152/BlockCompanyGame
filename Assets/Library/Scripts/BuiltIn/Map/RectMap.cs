using Basis;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RectMap
{
    [Serializable]
    public struct Config
    {
        [LabelText("容器"), Required]
        public Transform container;
        [LabelText("地板Tilemap"), Required]
        public Tilemap floorTilemap;
        [LabelText("地板装饰Tilemap"), Required]
        public Tilemap floorDecorationTilemap;
        [LabelText("物品Tilemap"), Required]
        public Tilemap objectTilemap;
    }

    public Config config { get; private set; }

    public Grid grid => config.container.GetComponent<Grid>();

    public RectMap(Config config)
    {
        if (config.container == null)
        {
            Note.note.Error("config.container不能为Null");
        }
        if (config.container.GetComponent<Grid>() == null)
        {
            Note.note.Error("config.container必须要有Grid组件");
        }

        if (config.floorTilemap == null)
        {
            Note.note.Error("config.floorTilemap不能为Null");
        }
        if (config.floorTilemap.GetComponent<TilemapRenderer>() == null)
        {
            Note.note.Error("config.floorTilemap必须要有TilemapRenderer组件");
        }

        if (config.floorDecorationTilemap == null)
        {
            Note.note.Error("config.floorDecorationTilemap不能为Null");
        }
        if (config.floorDecorationTilemap.GetComponent<TilemapRenderer>() == null)
        {
            Note.note.Error("config.floorDecorationTilemap必须要有TilemapRenderer组件");
        }

        if (config.objectTilemap == null)
        {
            Note.note.Error("config.objectTilemap不能为Null");
        }
        if (config.objectTilemap.GetComponent<TilemapRenderer>() == null)
        {
            Note.note.Error("config.objectTilemap必须要有TilemapRenderer组件");
        }

        this.config = config;
    }

    public void SetFloor(Vector3Int pos, TileBase tileBase)
    {

    }
}
