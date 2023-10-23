#if UNITY_EDITOR
using Basis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapBasicTools : MonoBehaviour
{
    [LabelText("绑定的Tilemap")]
    [SerializeField, Required]
    protected Tilemap bindTilemap;

    protected virtual void Reset()
    {
        bindTilemap = this.FindFirstChildComponent<Tilemap>(true);
    }

    [Button("清除地图", ButtonSizes.Medium)]
    public void ClearMap()
    {
        if (bindTilemap == null)
        {
            return;
        }

        if (EditorUtility.DisplayDialog("TilemapTools", "是否清除地图，此操作不可撤销", "是", "否") == false)
        {
            return;
        }

        bindTilemap.ClearAllTiles();
    }
}
#endif
