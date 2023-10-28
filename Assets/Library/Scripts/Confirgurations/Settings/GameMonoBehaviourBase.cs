using Basis;
using Sirenix.OdinInspector;

#if MIRROR
using Mirror;
#endif

using UnityEngine;

[OnInspectorInit("@this.OnInspectorInit()")]
public class MonoBehaviourBase : MonoBehaviour
{
    protected virtual void OnInspectorInit()
    {

    }
}

[DisallowMultipleComponent]
public class UniqueMonoBehaviour<T> : SerializedMonoBehaviour where T : UniqueMonoBehaviour<T>
{
    public static T instance;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Note.note.Error($"重复添加组件{nameof(T)}");
        }

        instance = (T)this;
    }

    public void _DisplayMySelf()
    {
        Note.note.Log($"{name}");
    }
}

#if MIRROR

public class UniqueNetworkBehaviour<T> : NetworkBehaviour where T : UniqueNetworkBehaviour<T>
{
    public static Note.note Note.note = new("UniqueNetworkBehaviour");

    public static T instance;

    protected virtual void Awake()
    {
        if (instance != null)
        {
            Note.note.Error($"重复添加组件{nameof(T)}");
        }

        instance = (T)this;
    }
}

#endif