using Basis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrailSpawnConfig : BaseConfigClass
{
    [LabelText("拖尾效果")]
    public PrefabIDSetter<RegisteredTrail, TrailSpawnerGeneralSetting> trailID = new();

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        trailID ??= new();
    }
}

public static class TrailSpawner
{
    public static Dictionary<string, GameObjectPool<TrailRenderer>> allPools = new();
    public static Dictionary<TrailRenderer, string> allTrailIDs = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReturnTrailToDefaultContainer(TrailRenderer trail)
    {
        trail.transform.SetParent(GameCoreSettingBase.trailSpawnerGeneralSetting.container);
    }

    /// <summary>
    /// 回收粒子
    /// </summary>
    /// <param name="trail"></param>
    public static void ReturnTrail(TrailRenderer trail)
    {
        if (trail.gameObject.activeSelf)
        {
            var id = allTrailIDs[trail];
            var pool = allPools[id];

            trail.transform.SetParent(GameCoreSettingBase.trailSpawnerGeneralSetting.container);
            pool.Return(trail);
        }
    }

    /// <summary>
    /// 生成粒子
    /// 如果父Transform为Null，则为位置参数为world space position，如若不然，则是local position
    /// </summary>
    /// <param name="id">粒子ID</param>
    /// <param name="pos">位置</param>
    /// <param name="parent">父Transform</param>
    /// <returns></returns>
    public static TrailRenderer SpawnTrail(string id, Vector3 pos, Transform parent = null)
    {
        var registeredTrail = GameCoreSettingBase.trailSpawnerGeneralSetting.GetPrefabStrictly(id);

        GameObjectPool<TrailRenderer> pool;

        if (allPools.ContainsKey(id) == false)
        {
            pool = new();
            allPools[id] = pool;
        }
        else
        {
            pool = allPools[id];
        }

        var container = parent == null ? GameCoreSettingBase.trailSpawnerGeneralSetting.container : parent;

        var newTrail = pool.Get(registeredTrail.trailPrefab, container);

        allTrailIDs[newTrail] = id;

        if (parent == null)
        {
            newTrail.transform.position = pos;
        }
        else
        {
            newTrail.transform.localPosition = pos;
        }

        1.DelayFrameAction(() =>
        {
            newTrail.Clear();
        });

        return newTrail;
    }
}
