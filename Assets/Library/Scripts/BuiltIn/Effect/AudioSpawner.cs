using Basis;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AudioSpawner : UniqueMonoBehaviour<AudioSpawner>
{
    public static GameObjectPool<AudioSource> pool = new();

    public static void Return(AudioSource audioSource)
    {
        audioSource.Stop();

        if (audioSource.gameObject.activeSelf)
        {
            audioSource.transform.
                SetParent(GameCoreSettingBase.audioGeneralSetting.container);
            pool.Return(audioSource);
        }
    }

    [Button("生成音效")]
    public static AudioSource Spawn(
        [ValueDropdown("@GameCoreSettingBase.audioGeneralSetting.GetPrefabNameList()")]
        string id, Vector3 pos, Transform parent = null)
    {
        var preset = AudioPreset.GetPrefabStrictly(id);

        var audioSource = pool.Get(() =>
        {
            var gameObject = new GameObject();

            return gameObject.AddComponent<AudioSource>();
        });

        audioSource.clip = preset.audioClip;

        audioSource.name = preset.name;

        audioSource.volume = preset.volume;

        audioSource.loop = false;

        var container = parent == null ? 
            GameCoreSettingBase.audioGeneralSetting.container : 
            parent;

        audioSource.transform.SetParent(container);

        audioSource.transform.position = pos;

        if (preset.autoCheckStop)
        {
            _ = CheckStop(audioSource);
        }

        audioSource.Play();

        return audioSource;
    }

    private static async UniTaskVoid CheckStop(AudioSource audioSource)
    {
        await UniTask.WaitUntil(() => audioSource.isPlaying == false);

        Return(audioSource);
    }
}
