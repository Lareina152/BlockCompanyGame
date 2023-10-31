using Basis;
using ItemBasis;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Sirenix.Serialization;
using UnityEngine;

#if FISHNET

using FishNet;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Serializing;

#endif

namespace Basis.GameItem
{
    public class
        SimpleGameItemBundle<TPrefab, TGeneralSetting, TInstance> : GamePrefabCoreBundle<TPrefab, TGeneralSetting>
        where TPrefab : SimpleGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItemPrefab, new()
        where TGeneralSetting : SimpleGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItemGeneralSetting
        where TInstance : SimpleGameItemBundle<TPrefab, TGeneralSetting, TInstance>.GameItem, new()
    {
        public const string BIND_INSTANCE_TYPE_NAME = "bindInstanceType";

        [HideReferenceObjectPicker]
        [Serializable]
        [HideInEditorMode]
        [HideDuplicateReferenceBox]
        public abstract class GameItem
        {
            public const string NULL_ID = GamePrefabCoreBundle<TPrefab, TGeneralSetting>.NULL_ID;

            public static Dictionary<string, Type> extendedTypes = new();

            public static Dictionary<Type, Type> bindInstanceTypes = new();

            public static bool isInitialized = false;

            /// <summary>
            /// 由哪个预制体生成的此实例
            /// </summary>
            [LabelText("预制体")] [ShowInInspector] protected TPrefab origin;

            /// <summary>
            /// 实例的ID，与预制体的ID一一对应
            /// </summary>
            [LabelText("ID")]
            [ShowInInspector, DisplayAsString]
            public string id { get; private set; }

            [LabelText("唯一种类")] [ShowInInspector] public GameType uniqueGameType { get; private set; }

            /// <summary>
            /// 是否在调试此ID对应的预制体或实例
            /// </summary>
            public bool isDebugging => origin.isDebugging;

            public StringTranslation name => origin.name;

            protected TGeneralSetting generalSetting => GameItemPrefab.GetStaticGeneralSetting();

            #region FishNet Properties

#if FISHNET

            public bool isServer
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => InstanceFinder.IsServer;
            }

            public bool isClient
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => InstanceFinder.IsClient;
            }

            public bool isHost
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => InstanceFinder.IsHost;
            }

            public bool isServerOnly
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => InstanceFinder.IsServerOnly;
            }

            public bool isClientOnly
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => InstanceFinder.IsClientOnly;
            }

            public ServerManager serverManager => InstanceFinder.ServerManager;
            public ClientManager clientManager => InstanceFinder.ClientManager;

#endif

            #endregion

            /// <summary>
            /// 判断两者是否一样
            /// </summary>
            /// <param name="other">另一个</param>
            /// <returns></returns>
            public virtual bool IsSameAs(TInstance other)
            {
                return id == other.id;
            }

            #region Net Serialization

#if FISHNET

            /// <summary>
            /// 在网络上如何传输，当在此实例被写进byte流时调用
            /// </summary>
            /// <param name="writer"></param>
            protected virtual void OnWrite(Writer writer)
            {

            }

            /// <summary>
            /// 在网络上如何传输，当在此实例被从byte流中读出时调用
            /// </summary>
            /// <param name="reader"></param>
            protected virtual void OnRead(Reader reader)
            {

            }

            /// <summary>
            /// Fishnet的网络byte流写入
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="gameItem"></param>
            public static void Write(Writer writer, GameItem gameItem)
            {
                if (gameItem == null)
                {
                    writer.WriteString(NULL_ID);
                }
                else
                {
                    writer.WriteString(gameItem.id);

                    gameItem.OnWrite(writer);
                }
            }

            /// <summary>
            /// Fishnet的网络byte流读出
            /// </summary>
            /// <param name="reader"></param>
            /// <returns></returns>
            public static TInstance Read(Reader reader)
            {
                var id = reader.ReadString();

                if (id == NULL_ID)
                {
                    return null;
                }

                var gameItem = Create(id);

                gameItem.OnRead(reader);

                return gameItem;
            }

#endif

            #endregion

            #region Clone

            /// <summary>
            /// 当被复制时调用
            /// </summary>
            /// <param name="other"></param>
            protected virtual void OnClone(TInstance other)
            {

            }

            /// <summary>
            /// 获得复制
            /// </summary>
            /// <returns></returns>
            public TInstance GetClone()
            {
                var clone = Create(id);

                clone.OnClone(this as TInstance);

                return clone;
            }

            #endregion

            #region String

            protected virtual string OnGetString()
            {
                return "";
            }

            public override string ToString()
            {
                var extraString = OnGetString();
                if (extraString.IsNullOrEmptyAfterTrim())
                {
                    return $"[{GetType()}:id:{id}]";
                }

                return $"[{GetType()}:id:{id},{extraString}]";

            }

            #endregion

            #region Game Type

            public bool HasAnyGameType(IEnumerable<string> typeIDs)
            {
                return typeIDs.Any(HasGameType);
            }

            public bool HasAllGameTypes(IEnumerable<string> typeIDs)
            {
                return typeIDs.All(HasGameType);
            }

            public bool HasGameType(string typeID)
            {
                return origin.gameTypesDict.ContainsKey(typeID);
            }

            public bool TryGetGameType(string typeID, out GameType gameType)
            {
                return origin.gameTypesDict.TryGetValue(typeID, out gameType);
            }

            public GameType GetGameType(string typeID)
            {
                if (origin.gameTypesDict.TryGetValue(typeID, out var gameType))
                {
                    return gameType;
                }

                Note.note.Warning($"找不到ID为{typeID}的GameType");
                return null;
            }

            public IEnumerable<string> GetAllGameTypeIDs()
            {
                return origin.gameTypesDict.Keys;
            }

            #endregion

            #region Origin

            /// <summary>
            /// 获得此实例的预制体
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            protected T GetOrigin<T>() where T : TPrefab, new()
            {
                return origin as T;
            }

            /// <summary>
            /// 获得此实例的预制体，如果获取失败则报错
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            protected T GetOriginStrictly<T>() where T : TPrefab, new()
            {
                var newOrigin = origin as T;

                if (newOrigin == null)
                {
                    Note.note.Error($"{this}不是{typeof(T)}");
                }

                return newOrigin;
            }

            #endregion

            #region Extensions

            public static Type GetExtendedGameItemType(string id)
            {
                if (id.IsNullOrEmptyAfterTrim())
                {
                    return null;
                }

                return extendedTypes.TryGetValue(id, out var type) ? type : null;
            }

            public static Type GetBindInstanceType(Type prefabType)
            {
                return bindInstanceTypes.TryGetValue(prefabType, out var type) ? type : null;
            }

            public static void RefreshExtendedGameItems()
            {
                var generalSetting = GameCoreSettingBase.FindGeneralSetting<GameItemGeneralSetting>();

                if (generalSetting == null)
                {
                    Note.note.Warning($"找不到{typeof(GameItem)}对应的通用设置");
                    return;
                }

                extendedTypes.Clear();

                foreach (var type in typeof(GameItem).FindDerivedClasses(false, false))
                {
                    string id = string.Empty;

                    var instance = type.CreateInstance();

                    PropertyInfo registeredIDPropertyInfo = type.GetPropertyByName(REGISTERED_ID_NAME);

                    if (registeredIDPropertyInfo != null)
                    {
                        if (registeredIDPropertyInfo.PropertyType == typeof(string))
                        {
                            id = (string)registeredIDPropertyInfo.GetValue(instance);
                        }
                        else
                        {
                            Note.note.Warning(
                                $"{REGISTERED_ID_NAME}的Type需要为string,而不是{registeredIDPropertyInfo.PropertyType}," +
                                $"来自Type为{type}的扩展");
                        }
                    }

                    if (id.IsNullOrEmptyAfterTrim())
                    {
                        FieldInfo registeredIDFieldInfo = type.GetFieldByName(REGISTERED_ID_NAME);

                        if (registeredIDFieldInfo != null)
                        {
                            if (registeredIDFieldInfo.FieldType == typeof(string))
                            {
                                id = (string)registeredIDFieldInfo.GetValue(instance);
                            }
                            else
                            {
                                Note.note.Warning(
                                    $"{REGISTERED_ID_NAME}的Type需要为string,而不是{registeredIDFieldInfo.FieldType}," +
                                    $"来自Type为{type}的扩展");
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (id.IsNullOrEmptyAfterTrim())
                    {
                        continue;
                    }

                    if (generalSetting.ContainsPrefabID(id) == false)
                    {
                        Note.note.Warning($"通用设置里不包含此ID:{id}的预制体，无法注册扩展实例");
                        continue;
                    }

                    if (extendedTypes.ContainsKey(id))
                    {
                        Note.note.Warning($"重复注册扩展实例id:{id}，原先注册的扩展实例被覆盖");
                    }

                    extendedTypes[id] = type;
                }

                bindInstanceTypes.Clear();

                foreach (var type in GetDerivedPrefabsWithoutRegisteredID())
                {
                    var instance = type.CreateInstance();

                    Type bindInstanceType = null;

                    var bindInstanceTypeFieldInfo = type.GetFieldByName(BIND_INSTANCE_TYPE_NAME);

                    if (bindInstanceTypeFieldInfo != null)
                    {
                        //Debug.LogWarning(type);

                        if (bindInstanceTypeFieldInfo.FieldType == typeof(Type))
                        {
                            bindInstanceType = bindInstanceTypeFieldInfo.GetValue(instance) as Type;
                        }
                        else
                        {
                            Note.note.Warning(
                                $"{BIND_INSTANCE_TYPE_NAME}的Type需要为Type,而不是{bindInstanceTypeFieldInfo.FieldType}," +
                                $"来自Type为{type}的扩展");
                        }
                    }

                    if (bindInstanceType == null)
                    {
                        var bindInstanceTypePropertyInfo = type.GetPropertyByName(BIND_INSTANCE_TYPE_NAME);

                        if (bindInstanceTypePropertyInfo != null)
                        {
                            if (bindInstanceTypePropertyInfo.PropertyType == typeof(Type))
                            {
                                bindInstanceType = bindInstanceTypePropertyInfo.GetValue(instance) as Type;
                            }
                            else
                            {
                                Note.note.Warning(
                                    $"{BIND_INSTANCE_TYPE_NAME}的Type需要为Type,而不是{bindInstanceTypePropertyInfo.PropertyType}," +
                                    $"来自Type为{type}的扩展");
                            }
                        }
                    }

                    if (bindInstanceType == null)
                    {
                        continue;
                    }

                    if (bindInstanceTypes.ContainsKey(type))
                    {
                        Note.note.Warning($"{nameof(bindInstanceTypes)}已经对PrefabType:{type}进行过绑定，将会覆盖旧的绑定，新绑定为" +
                                     $"InstanceType：{bindInstanceType}");
                    }

                    bindInstanceTypes[type] = bindInstanceType;
                }
            }

            #endregion

            #region Create

            /// <summary>
            /// 当被创建时调用
            /// </summary>
            protected virtual void OnCreate()
            {
                if (GameItemPrefab.gameTypeAbandoned == false)
                {
                    uniqueGameType = origin.uniqueGameType;
                }
            }

            /// <summary>
            /// 创建新实例，会自动转换成注册的Class
            /// </summary>
            /// <param name="id">需要创建的实例ID</param>
            /// <returns></returns>
            public static TInstance Create(string id)
            {
                TInstance newGameItem;

                var prefab = GameItemPrefab.GetPrefabStrictly(id);

                if (extendedTypes.TryGetValue(id, out var extendedType))
                {
                    newGameItem = (TInstance)Activator.CreateInstance(extendedType);
                }
                else
                {
                    if (bindInstanceTypes.TryGetValue(prefab.GetType(), out var bindType))
                    {
                        newGameItem = (TInstance)Activator.CreateInstance(bindType);
                    }
                    else
                    {
                        newGameItem = new TInstance();
                    }
                }

                newGameItem.id = id;
                newGameItem.origin = prefab;
                newGameItem.OnCreate();

                return newGameItem;
            }

            public static T Create<T>(string id) where T : TInstance
            {
                var result = Create(id) as T;

                Note.note.AssertIsNotNull(result, nameof(result));

                return result;
            }

            public static TInstance CreateRandom()
            {
                var prefab = GameItemPrefab.GetRandomPrefab();

                return Create(prefab.id);
            }

            public static T CreateRandom<T>() where T : TInstance
            {
                var prefabs = GameItemPrefab.GetAllPrefabs();

                var ids = new List<string>();

                foreach (var prefab in prefabs)
                {
                    if (prefab.actualInstanceType.IsAssignableFrom(typeof(T)))
                    {
                        ids.Add(prefab.id);
                    }
                }

                return Create<T>(ids.Choose());
            }

            #endregion

            public static void Init()
            {
                if (isInitialized)
                {
                    return;
                }

                Note.note.Log($"正在加载{typeof(TPrefab)}的实例扩展和绑定");

                RefreshExtendedGameItems();

                Note.note.Log($"一共加载了{extendedTypes.Count}个自定义实例扩展类");

                Note.note.Log($"一共加载了{bindInstanceTypes.Count}个默认实例绑定类");

                Note.note.Log($"{typeof(TPrefab)}的实例扩展和绑定加载结束");

                isInitialized = true;
            }
        }


        //public class Package<ThisItem> : IEnumerable where ThisItem : GameItem
        //{
        //    protected List<ThisItem> allItems = new();

        //    public void Add(ThisItem item)
        //    {
        //        allItems.Add(item);
        //    }

        //    public void Remove(ThisItem item)
        //    {
        //        allItems.Remove(item);
        //    }

        //    public IEnumerator GetEnumerator()
        //    {
        //        return allItems.GetEnumerator();
        //    }
        //}

        [JsonObject(MemberSerialization.OptIn)]
        public new abstract class GameItemPrefab : GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemPrefab
        {
            [LabelText("扩展实例类"), FoldoutGroup(EXTENDED_CLASS_CATEGORY)]
            [ShowInInspector]
            [EnableGUI]
            [HideIfNull]
            [PropertyOrder(-5000)]
            public Type extendedGameItemType => GameItem.GetExtendedGameItemType(id);

            [LabelText("绑定实例类"), FoldoutGroup(EXTENDED_CLASS_CATEGORY)]
            [ShowInInspector]
            [EnableGUI]
            [HideIfNull]
            [PropertyOrder(-4000)]
            public Type bindGameItemType => GameItem.GetBindInstanceType(GetType());

            public Type actualInstanceType
            {
                get
                {
                    if (extendedGameItemType != null)
                    {
                        return extendedGameItemType;
                    }

                    if (bindGameItemType != null)
                    {
                        return bindGameItemType;
                    }

                    return typeof(TInstance);
                }
            }

            #region GUI

#if UNITY_EDITOR
            [Button("打开实例脚本"), HorizontalGroup(OPEN_SCRIPT_BUTTON_HORIZONTAL_GROUP)]
            private void OpenInstanceScript()
            {
                actualInstanceType.OpenScriptOfType();
            }
#endif

#endregion

            #region Create

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TInstance CreateInstance()
            {
                return GameItem.Create(id);
            }

            #endregion

            #region PrefabObjectCreate

            //protected bool isCreatePrefabObjectOn => ((GameItemGeneralSetting)generalSetting).isCreatePrefabObjectOn;

            //private string briefCardPrefabIntro => $"{prefabName}预制体是用来自定义更高级的功能，比如自定义{prefabName}功能的脚本";
            //private string wholeCardPrefabIntro => briefCardPrefabIntro + "\n" +
            //                                       $"如果没有选择{prefabName}预制体，则使用通用{prefabName}预制体";


            //public GameObject prefabObject
            //{
            //    get
            //    {
            //        if (customPrefabCreator == null)
            //        {
            //            return ((GameItemGeneralSetting)generalSetting).universalPrefabObject;
            //        }
            //        else if (customPrefabCreator.customPrefab == null)
            //        {
            //            return ((GameItemGeneralSetting)generalSetting).universalPrefabObject;
            //        }
            //        else
            //        {
            //            return customPrefabCreator.customPrefab;
            //        }
            //    }
            //}

            //[Title("预制体")]
            //[CustomPrefabCreatorSetting(
            //    "@generalSetting.universalPrefabObject",
            //    "@generalSetting.prefabObjectDirectoryRelativePath",
            //    "@id",
            //    DisplayName = "@prefabName",
            //    AfterGenerate = "AfterGenerateCustomPrefabObject"
            //)]
            //[ShowIf("isCreatePrefabObjectOn")]
            //[SerializeField]
            //[HideLabel]
            //[PropertyOrder(899)]
            //protected CustomPrefabCreator customPrefabCreator = new();

            //protected virtual void AfterGenerateCustomPrefabObject(bool isSuccess, GameObject newPrefab)
            //{
            //    note.Log("233");
            //}

            #endregion
        }

        public new abstract class GameItemGeneralSetting :
            GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemGeneralSetting
        {
            [LabelText("扩展实例类"), TitleGroup(EXTENDED_TYPES_SETTING_CATEGORY)]
            [ReadOnly]
            [EnableGUI]
            [ShowInInspector]
            public Dictionary<string, Type> extendedTypes => GameItem.extendedTypes;

            [LabelText("绑定实例类"), TitleGroup(EXTENDED_TYPES_SETTING_CATEGORY)]
            [ReadOnly]
            [EnableGUI]
            [ShowInInspector]
            public Dictionary<Type, Type> bindInstanceTypes => GameItem.bindInstanceTypes;

            #region PrefabObjectCreate

            //public virtual bool isCreatePrefabObjectOn => false;

            //[Title("预制体")]
            //[LabelText(@"@""通用"" + prefabName + ""预制体"""), GUIColor(0.4f, 0.8f, 1)]
            //[Required, AssetsOnly, SerializeField, Space(10), PropertyOrder(998)]
            //[ShowIf("isCreatePrefabObjectOn")]
            //private GameObject _universalPrefabObject;

            //public GameObject universalPrefabObject
            //{
            //    get
            //    {
            //        if (isCreatePrefabObjectOn == false)
            //        {
            //            note.Error($"没有开启创建{prefabName}预制体的功能，请勿访问通用{prefabName}预制体");
            //        }

            //        return _universalPrefabObject;
            //    }
            //}

            //[LabelText(@"@prefabName + ""预制体保存的文件夹"""), GUIColor(0.4f, 0.8f, 1)]
            //[FolderPath, PropertyOrder(999)]
            //[ShowIf("isCreatePrefabObjectOn")]
            //public string prefabObjectDirectoryRelativePath = "Assets/Resources/Prefab";

            #endregion

            #region GUI

            protected override void OnInspectorInit()
            {
                base.OnInspectorInit();

                GameItem.RefreshExtendedGameItems();
            }

            #endregion

            protected override void OnPreInit()
            {
                base.OnPreInit();

                GameItem.Init();
            }

            //protected override void OnInit()
            //{
            //    base.OnInit();

            //    //if (isCreatePrefabObjectOn == true)
            //    //{
            //    //    if (universalPrefabObject == null)
            //    //    {
            //    //        note.Error($"{prefabName}通用配置文件没有设置{prefabName}通用预制体");
            //    //    }
            //    //}
            //}


        }
    }
}
