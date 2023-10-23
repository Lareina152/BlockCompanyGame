using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ParticleSpawnerTestBench : MonoBehaviour
{
    [ValueDropdown("@GameCoreSettingBase.particleSpawnerGeneralSetting.GetPrefabNameList()")]
    public string id;
    public Vector3 pos;

    [Button(nameof(Spawn))]
    public void Spawn()
    {
        ParticleSpawner.SpawnParticle(id, pos);
    }

    public Sprite sprite;

    [Button(nameof(SpawnPixelDestroy))]
    public void SpawnPixelDestroy()
    {
        ParticleSpawner.SpawnPixelDestroyParticle(pos, sprite);
    }

}
