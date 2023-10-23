using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

public class ParticleSpawnConfig : BaseConfigClass
{
    [LabelText("粒子效果")]
    public PrefabIDSetter<RegisteredParticle, ParticleSpawnerGeneralSetting> particleID = new();

    protected override void OnInspectorInit()
    {
        base.OnInspectorInit();

        particleID ??= new();
    }
}

public static class ParticleSpawner
{
    public static Dictionary<string, GameObjectPool<ParticleSystem>> allPools = new();
    public static Dictionary<ParticleSystem, string> allParticleIDs = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ReturnParticleToDefaultContainer(ParticleSystem particle)
    {
        particle.transform.SetParent(GameCoreSettingBase.particleSpawnerGeneralSetting.container);
    }

    /// <summary>
    /// 回收粒子
    /// </summary>
    /// <param name="particle"></param>
    public static void ReturnParticle(ParticleSystem particle)
    {
        if (particle.gameObject.activeSelf)
        {
            var id = allParticleIDs[particle];
            var pool = allPools[id];

            particle.transform.SetParent(GameCoreSettingBase.particleSpawnerGeneralSetting.container);
            pool.Return(particle);
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
    public static ParticleSystem SpawnParticle(string id, Vector3 pos, Transform parent = null)
    {
        var registeredParticle = GameCoreSettingBase.particleSpawnerGeneralSetting.GetPrefabStrictly(id);

        GameObjectPool<ParticleSystem> pool;

        if (allPools.ContainsKey(id) == false)
        {
            pool = new();
            allPools[id] = pool;
        }
        else
        {
            pool = allPools[id];
        }

        var container = parent == null ? GameCoreSettingBase.particleSpawnerGeneralSetting.container : parent;

        var newParticleSystem = pool.Get(registeredParticle.particlePrefab, container);

        allParticleIDs[newParticleSystem] = id;

        if (parent == null)
        {
            newParticleSystem.transform.position = pos;
        }
        else
        {
            newParticleSystem.transform.localPosition = pos;
        }

        newParticleSystem.Clear();
        newParticleSystem.Stop();

        1.DelayFrameAction(() =>
        {
            newParticleSystem.Play();
        });

        if (registeredParticle.enableDurationLimitation)
        {
            registeredParticle.duration.GetValue().DelayAction(() =>
            {
                ReturnParticle(newParticleSystem);
            });
        }

        return newParticleSystem;
    }

    public static ParticleSystem SpawnPixelDestroyParticle(Vector3 pos, Sprite sprite)
    {
        var newParticleSystem = SpawnParticle("pixel_destroy", pos);
        newParticleSystem.SetSampleTexture(sprite);

        return newParticleSystem;
    }

    public static ParticleSystem SpawnPixelExplosionParticle(Vector3 pos, Sprite sprite)
    {
        var newParticleSystem = SpawnParticle("pixel_explosion", pos);
        newParticleSystem.SetSampleTexture(sprite);

        return newParticleSystem;
    }
}
