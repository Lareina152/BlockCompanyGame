using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using UnityEngine;
using System.Reflection;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using Newtonsoft.Json.Linq;
using ItemBasis;
using Sirenix.Serialization;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif

namespace Basis.GameItem
{
    public interface IPrefabCoreGeneralSetting
    {
        /// <summary>
        /// 随机获取预制体的ID，如果无预制体返回NUll
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetRandomPrefabID();

        /// <summary>
        /// 获取全部预制体的ID
        /// </summary>
        /// <returns>返回所有预制体的ID构成的IEnumerable</returns>
        public IEnumerable<string> GetPrefabIDList();

        /// <summary>
        /// 获取特定ID预制体的名称
        /// 最好仅在Editor模式下调用，因为此方法效率较低
        /// </summary>
        /// <param name="id">预制体ID</param>
        /// <returns></returns>
        public StringTranslation GetPrefabName(string id);

        /// <summary>
        /// 获取所有预制体的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
        /// 比如 [ValueDropdown("@GameSetting.inputSystemGeneralSetting.GetPrefabNameList()")]
        /// </summary>
        /// <returns>返回为ValueDropdownItem构成的IEnumerable</returns>
        public IEnumerable<ValueDropdownItem<string>> GetPrefabNameList();

        /// <summary>
        /// 获取特定Type对应的类或其派生类的预制体的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
        /// 比如 [ValueDropdown("@GameSetting.uiPanelGeneralSetting.GetPrefabNameList(typeof(ContainerUIPreset))")]
        /// </summary>
        /// <param name="specificType">约束预制体的类型</param>
        /// <returns>返回为ValueDropdownItem构成的IEnumerable</returns>
        public IEnumerable<ValueDropdownItem<string>> GetPrefabNameList(Type specificType);

        /// <summary>
        /// 预制体列表是否包含某个特定ID
        /// 最好仅在Editor模式下调用，因为此方法效率较低
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool ContainsID(string id);

        public bool ContainsIDIgnoreActiveState(string id);

        public string GetTypeName(string typeID);

        /// <summary>
        /// 获取所有叶节点类别的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ValueDropdownItem<string>> GetTypeNameList();

        /// <summary>
        /// 获取所有叶节点类别的ID列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetTypeIDList();

        /// <summary>
        /// 获取所有类别的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ValueDropdownItem<string>> GetAllTypeNameList();

        /// <summary>
        /// 获取所有类别的ID列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAllTypeIDList();

        /// <summary>
        /// 是否包含此种类
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public bool ContainsType(string typeID);

        public IEnumerable<ValueDropdownItem<string>> GetPrefabNameListByType(string typeID);
    }

    public class GamePrefabCoreBundle<TPrefab, TGeneralSetting>
        where TPrefab : GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemPrefab
        where TGeneralSetting : GamePrefabCoreBundle<TPrefab, TGeneralSetting>.GameItemGeneralSetting
    {
        public const string REGISTERED_ID_NAME = "registeredID";
        public const string NULL_ID = "null";

        public static IEnumerable<Type> GetDerivedPrefabsWithoutRegisteredID()
        {
            return typeof(TPrefab).FindDerivedClasses(true, false).Where(type =>
                type.HasFieldByName(REGISTERED_ID_NAME) == false &&
                type.HasPropertyByName(REGISTERED_ID_NAME) == false);
        }

        protected static GameType rootGameType { get; private set; }

        [HideReferenceObjectPicker]
        [HideDuplicateReferenceBox]
        [Serializable]
        [JsonObject(MemberSerialization.OptIn)]
        public abstract class GameItemPrefab : BaseConfigClass
        {
            public const string NULL_ID = GamePrefabCoreBundle<TPrefab, TGeneralSetting>.NULL_ID;

            protected const string ACTIVE_STATE_DEBUGGING_MODE_HORIZONTAL_GROUP =
                "ActiveStateDebuggingModeHorizontalGroup";

            protected const string BASIC_SETTING_CATEGORY = "基本设置";

            protected const string TESTING_SETTING_CATEGORY = "测试工具";

            protected const string EXTENDED_CLASS_CATEGORY = "扩展类";

            public static Dictionary<string, Type> extendedTypes = new();

            public static Dictionary<string, TPrefab> allPrefabsByID = new();

            public static Dictionary<string, List<TPrefab>> allPrefabsByType = new();

            public static Type prefabType = typeof(TPrefab);

            public static bool gameTypeAbandoned = false;

            private static TGeneralSetting generalSetting;

            /// <summary>
            /// 推荐的ID后缀
            /// </summary>
            protected virtual string preferredIDSuffix => "";

            protected virtual bool showChangeTypeButton => true;

            /// <summary>
            /// 是否自动添加到预制体列表，需要类里有名为registeredID的Field或者Property
            /// </summary>
            public virtual bool autoAddToPrefabList => false;

            protected bool isIDEndsWithPreferredSuffix => id.IsNullOrEmptyAfterTrim() ||
                                                          preferredIDSuffix.IsNullOrEmptyAfterTrim() ||
                                                          id.EndsWith("_" + preferredIDSuffix);

            [LabelText("ID", SdfIconType.Globe)]
            [StringIsNotNullOrEmpty]
            [InfoBox(@"@""ID最好以 _"" + preferredIDSuffix + "" 结尾""", InfoMessageType.Warning,
                "@!" + nameof(isIDEndsWithPreferredSuffix))]
            [InfoBox("ID不能有空格", InfoMessageType.Warning,
                @"@id != null && id.Contains(' ')")]
            [InfoBox("ID不能为null", InfoMessageType.Error, "@id == " + nameof(NULL_ID))]
            [Placeholder("@" + nameof(GetIDPlaceholderText) + "()")]
            [JsonProperty(Order = -10000), PropertyOrder(-10000)]
            public string id;

            [LabelText("是否启用", SdfIconType.Activity)]
            [JsonProperty(Order = -9000), PropertyOrder(-9000)]
            [HorizontalGroup(ACTIVE_STATE_DEBUGGING_MODE_HORIZONTAL_GROUP)]
            public bool isActive = true;

            [LabelText("调试模式", SdfIconType.BugFill)]
            [JsonProperty(Order = -8000), PropertyOrder(-8000)]
            [HorizontalGroup(ACTIVE_STATE_DEBUGGING_MODE_HORIZONTAL_GROUP)]
            public bool isDebugging = false;

            [LabelText("扩展预制体类"), FoldoutGroup(EXTENDED_CLASS_CATEGORY, Expanded = false)]
            [ShowInInspector]
            [HideIfNull]
            [PropertyOrder(-7000)]
            [EnableGUI]
            public Type extendedPrefabType => GetExtendedPrefabType(id);

            [FormerlySerializedAs("gameTypeIDs")]
            [LabelText("种类"), FoldoutGroup(BASIC_SETTING_CATEGORY)]
            [JsonProperty(Order = -6000), PropertyOrder(-6000)]
            [ValueDropdown(nameof(GetTypeNameList))]
            [StringIsNotNullOrEmpty]
            [HideIf(nameof(gameTypeAbandoned))]
            [ListDrawerSettings(ShowFoldout = false)]
            public List<string> gameTypesID;

            [ShowInInspector]
            [HideInEditorMode]
            [JsonIgnore]
            //[HideIf(nameof(gameTypeAbandoned))]
            public IReadOnlyDictionary<string, GameType> gameTypesDict { get; private set; }

            [HideInInspector] [JsonIgnore] public GameType uniqueGameType;

            [LabelText("名称", SdfIconType.FileEarmarkPersonFill), FoldoutGroup(BASIC_SETTING_CATEGORY, Expanded = false)]
            [JsonProperty(Order = -5000), PropertyOrder(-5000)]
            public StringTranslation name = new()
            {
                { "Chinese", "预制体" },
                { "English", "Prefab" }
            };

            [LabelText("当前类型"), FoldoutGroup(EXTENDED_CLASS_CATEGORY)]
            [EnableGUI]
            [ShowInInspector]
            [ShowIf("@" + nameof(currentType) + " != " + nameof(prefabType))]
            public Type currentType => GetType();

            #region GUI

            protected override void OnInspectorInit()
            {
                //generalSetting = GetGeneralSetting();

                CheckGameTypeIfAbandoned();

                if (gameTypeAbandoned == false)
                {
                    gameTypesID ??= new();

                    if (gameTypesID.Count == 0)
                    {
                        gameTypesID.Add(GetStaticGeneralSetting().defaultTypeID);
                    }
                }
            }

            public virtual void OnTypeChanged()
            {

            }

            private string GetIDPlaceholderText()
            {
                const string placeholderText = "请输入ID";
                if (preferredIDSuffix.IsNullOrEmptyAfterTrim())
                {
                    return placeholderText;
                }

                return placeholderText + $"并以_{preferredIDSuffix}结尾";
            }

            [Button("补全ID结尾")]
            [HideIf(nameof(isIDEndsWithPreferredSuffix))]
            [OrderRelativeTo(nameof(id))]
            private void CompleteIDSuffix()
            {
                if (id.IsNullOrEmptyAfterTrim())
                {
                    id = "";
                }

                if (id.EndsWith("_") == false)
                {
                    id += "_";
                }

                id += preferredIDSuffix;

            }

            [Button("改变类型")]
            [ShowIf(nameof(showChangeTypeButton))]
            [PropertyOrder(1000)]
            [FoldoutGroup(EXTENDED_CLASS_CATEGORY)]
            private void ChangeType()
            {
                if (TryGetStaticGeneralSetting(out var setting))
                {
                    setting.InvokeMethod("ChangeType");
                }
                else
                {
                    Note.note.Error("没有找到对应的通用用设置");
                }
            }

#if UNITY_EDITOR
            [Button("打开预制体脚本"), HorizontalGroup(OPEN_SCRIPT_BUTTON_HORIZONTAL_GROUP)]
            private void OpenPrefabScript()
            {
                GetType().OpenScriptOfType();
            }
#endif

            #endregion

            #region Init & Check

            /// <summary>
            /// 当加载此预制体时调用
            /// </summary>
            protected override void OnInit()
            {
                if (isActive == false)
                {
                    return;
                }

                CheckSettings();

                if (allPrefabsByID.ContainsKey(id))
                {
                    Note.note.Warning($"已存在相同id");
                }

                allPrefabsByID[id] = (TPrefab)this;

                if (gameTypeAbandoned == false)
                {
                    var tempGameTypes = new List<GameType>();

                    foreach (var gameTypeID in gameTypesID)
                    {
                        var gameType = GameType.root.GetChildGameType(gameTypeID);

                        if (gameType == null)
                        {
                            Note.note.Error($"指定的种类 {gameTypeID} 不存在");
                        }

                        tempGameTypes.Add(gameType);

                        if (!allPrefabsByType.ContainsKey(gameTypeID))
                        {
                            allPrefabsByType[gameTypeID] = new();
                        }

                        allPrefabsByType[gameTypeID].Add((TPrefab)this);
                    }

                    var tempDict = new Dictionary<string, GameType>();

                    foreach (var gameType in tempGameTypes)
                    {
                        foreach (var parent in gameType.GetAllParents(true))
                        {
                            if (parent.id == GameType.ALL_ID)
                            {
                                continue;
                            }

                            tempDict[parent.id] = parent;
                        }
                    }

                    gameTypesDict = tempDict;

                    uniqueGameType = tempGameTypes[0];
                }
            }

            public override void CheckSettings()
            {
                if (id.IsNullOrEmptyAfterTrim())
                {
                    Note.note.Error($"id不能为空");
                }

                if (id == NULL_ID)
                {
                    Note.note.Error($"id不能为 {NULL_ID}");
                }

                if (isIDEndsWithPreferredSuffix == false)
                {
                    Note.note.Warning($"id：{id}最好以 _{preferredIDSuffix} 结尾");
                }

                if (gameTypeAbandoned == false)
                {
                    if (gameTypesID == null || gameTypesID.Count == 0)
                    {
                        Note.note.Error("没有任何种类");
                    }

                    if (gameTypesID.Count > 1 && GetStaticGeneralSettingStrictly().isTypeIDUnique)
                    {
                        Note.note.Error("种类ID不唯一");
                    }

                    if (gameTypesID.ContainsSame())
                    {
                        Note.note.Error("种类ID有重复");
                    }
                }
            }

            #endregion

            #region GameType



            #endregion

            #region GetGeneralSetting

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static void SetStaticGeneralSetting(TGeneralSetting newGeneralSetting)
            {
                Note.note.AssertIsNotNull(newGeneralSetting, nameof(newGeneralSetting));
                generalSetting = newGeneralSetting;
            }

            /// <summary>
            /// 获得此预制体对应的通用设置，Editor和Runtime下调用皆可
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static TGeneralSetting GetStaticGeneralSetting()
            {
                if (generalSetting == null)
                {
                    return GameCoreSettingBase.FindGeneralSetting<TGeneralSetting>();
                }

                return generalSetting;
            }

            /// <summary>
            /// 获得此预制体对应的通用设置，Editor和Runtime下调用皆可，如果没有找到则会报错
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            [NotNull]
            public static TGeneralSetting GetStaticGeneralSettingStrictly()
            {
                var setting = GetStaticGeneralSetting();

                if (setting == null)
                {
                    Note.note.Error("找不到通用设置");
                }

                return setting;
            }

            /// <summary>
            /// 尝试获得此预制体对应的通用设置，Editor和Runtime下调用皆可
            /// </summary>
            /// <param name="setting">获得的通用设置</param>
            /// <returns>尝试结果</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool TryGetStaticGeneralSetting(out TGeneralSetting setting)
            {
                if (generalSetting == null)
                {
                    setting = GameCoreSettingBase.FindGeneralSetting<TGeneralSetting>();
                }
                else
                {
                    setting = generalSetting;
                }

                return setting != null;
            }

            #endregion

            #region GetPrefab

            /// <summary>
            /// 通过ID获得预制体，如果没有找到则会警告
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static TPrefab GetPrefab(string id)
            {
                if (allPrefabsByID.TryGetValue(id, out var targetPrefab))
                {
                    return targetPrefab;
                }

                Note.note.Warning($"不存在id为{id}的{typeof(TPrefab)}");
                return null;
            }

            public static T GetPrefab<T>(string id) where T : TPrefab
            {
                return GetPrefab(id) as T;
            }

            /// <summary>
            /// 通过ID获得预制体，如果没有找到则会报错
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static TPrefab GetPrefabStrictly(string id)
            {
                if (id == null)
                {
                    Note.note.Error("id为null");
                    return null;
                }

                if (allPrefabsByID.TryGetValue(id, out var targetPrefab))
                {
                    return targetPrefab;
                }

                Note.note.Error($"不存在id为{id}的{typeof(TPrefab)}");
                return null;
            }

            public static T GetPrefabStrictly<T>(string id) where T : TPrefab
            {
                var prefab = GetPrefabStrictly(id);

                if (prefab is T t)
                {
                    return t;
                }

                Note.note.Error($"id为{id}的{typeof(TPrefab)}类型不是{typeof(T)}");
                return null;
            }

            /// <summary>
            /// 获得随机的预制体
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static TPrefab GetRandomPrefab()
            {
                return allPrefabsByID.Values.Choose();
            }

            /// <summary>
            /// 获取所有预制体
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static IReadOnlyList<TPrefab> GetAllPrefabs()
            {
                return generalSetting.GetAllPrefabs();
            }

            /// <summary>
            /// 获取所有预制体
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static IReadOnlyList<T> GetAllPrefabs<T>() where T : TPrefab
            {
                return GetAllPrefabs().Where(prefab => prefab is T).Cast<T>().ToList();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static TPrefab GetFirstPrefab()
            {
                return generalSetting.GetAllPrefabs().FirstOrDefault();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T GetFirstPrefab<T>() where T : TPrefab
            {
                return generalSetting.GetAllPrefabs().FirstOrDefault(prefab => prefab is T) as T;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static TPrefab GetLastPrefab()
            {
                return generalSetting.GetAllPrefabs().LastOrDefault();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static T GetLastPrefab<T>() where T : TPrefab
            {
                return generalSetting.GetAllPrefabs().LastOrDefault(prefab => prefab is T) as T;
            }

            /// <summary>
            /// 从通用设置里用ID获取预制体，一般用于在Editor模式下调用，效率较低
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            public static TPrefab GetPrefabFromGeneralSetting(string id)
            {
                if (TryGetStaticGeneralSetting(out var generalSetting))
                {
                    return generalSetting.GetPrefabStrictly(id);
                }

                return null;
            }

            #endregion

            #region GetPrefabByType

            /// <summary>
            /// 所有预制体里是否包含特定种类ID的预制体
            /// </summary>
            /// <param name="typeID"></param>
            /// <returns></returns>
            public static bool ContainsType(string typeID) => allPrefabsByType.ContainsKey(typeID);

            /// <summary>
            /// 所有预制体里是否包含特定种类ID的预制体，如果没有会报错
            /// </summary>
            /// <param name="typeID"></param>
            /// <returns></returns>
            public static bool ContainsTypeStrictly(string typeID)
            {
                bool result = ContainsType(typeID);
                if (result == false)
                {
                    Note.note.Error($"没有预制体为此typeID:{typeID}");
                }

                return result;
            }

            /// <summary>
            /// 随机获得一个特定种类的预制体
            /// </summary>
            /// <param name="typeID"></param>
            /// <returns></returns>
            public static TPrefab GetRandomPrefabByType(string typeID)
            {
                return allPrefabsByType.ContainsKey(typeID) ? allPrefabsByType[typeID].Choose() : null;
            }

            /// <summary>
            /// 随机获得一个特定种类的预制体ID
            /// </summary>
            /// <param name="typeID"></param>
            /// <returns></returns>
            public static string GetRandomPrefabIDByType(string typeID)
            {
                GameItemPrefab randomPrefab = GetRandomPrefabByType(typeID);

                return randomPrefab == null ? "" : randomPrefab.id;
            }

            /// <summary>
            /// 获取特定种类的所有预制体
            /// </summary>
            /// <param name="typeID"></param>
            /// <returns></returns>
            public static IReadOnlyList<TPrefab> GetPrefabsByType(string typeID)
            {
                if (allPrefabsByType.TryGetValue(typeID, out var prefabs))
                {
                    return prefabs;
                }

                var gameType = rootGameType.GetChildGameType(typeID);

                var leaves = gameType.GetAllLeaves(true).ToList();

                if (leaves.Count == 0)
                {
                    return ArraySegment<TPrefab>.Empty;
                }

                var results = new List<TPrefab>();

                foreach (var leaf in leaves)
                {
                    if (allPrefabsByType.TryGetValue(leaf.id, out var leafPrefabs))
                    {
                        results.AddRange(leafPrefabs);
                    }
                    else
                    {
                        Note.note.Error($"不存在此typeID:{leaf.id}");
                    }
                }

                allPrefabsByType[typeID] = results;

                return results;
            }

            /// <summary>
            /// 获取特定种类的所有预制体ID
            /// </summary>
            /// <param name="typeID"></param>
            /// <returns></returns>
            public static IEnumerable<string> GetPrefabsIDByType(string typeID)
            {
                return GetPrefabsByType(typeID).Select(prefab => prefab.id);
            }

            #endregion

            #region Type

            private static void CheckGameTypeIfAbandoned()
            {
                var generalSetting = GetStaticGeneralSetting();

                Note.note.AssertIsNotNull(generalSetting, nameof(generalSetting));

                gameTypeAbandoned = generalSetting.gameTypeAbandoned;
            }

            private IEnumerable GetTypeNameList()
            {
                var generalSetting = GetStaticGeneralSetting();

                if (generalSetting == null)
                {
                    Note.note.Warning("找不到通用设置");
                    return null;
                }

                return generalSetting.GetTypeNameList();
            }

            #endregion

            #region Extension

            public static Type GetExtendedPrefabType(string id)
            {
                if (id.IsNullOrEmptyAfterTrim())
                {
                    return null;
                }

                return extendedTypes.TryGetValue(id, out var type) ? type : null;
            }

            public static void RefreshExtendedPrefabs()
            {
                if (TryGetStaticGeneralSetting(out var generalSetting) == false)
                {
                    Note.note.Warning($"找不到{typeof(TPrefab)}对应的通用设置类型:{typeof(TGeneralSetting)}");
                    return;
                }

                extendedTypes.Clear();

                foreach (Type type in typeof(TPrefab).FindDerivedClasses(false, false))
                {
                    string id = string.Empty;

                    var instance = type.CreateInstance() as TPrefab;

                    if (instance == null)
                    {
                        Note.note.Warning($"{type}实例化成{typeof(TPrefab)}失败");
                        continue;
                    }

                    if (instance.isActive == false)
                    {
                        continue;
                    }

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

                    if (extendedTypes.ContainsKey(id))
                    {
                        Note.note.Warning($"重复注册扩展预制体id:{id}，原先注册的扩展预制体被覆盖");
                    }

                    if (generalSetting.ContainsID(id) == false)
                    {
                        if (instance.autoAddToPrefabList)
                        {
                            instance.id = id;
                            if (generalSetting.TryAddPrefab(instance, out var newPrefab))
                            {

                            }
                        }
                        else
                        {
                            Note.note.Warning($"通用设置里不包含此ID:{id}的预制体，无法注册扩展预制体");
                            continue;
                        }
                    }

                    extendedTypes[id] = type;
                }
            }

            #endregion

            public override string ToString()
            {
                return $"({GetType()}, id:{id})";
            }

            public static void InitStatic()
            {
                CheckGameTypeIfAbandoned();

                Note.note.Log($"正在加载{typeof(TPrefab)}的预制体扩展");

                RefreshExtendedPrefabs();

                Note.note.Log($"一共加载了{extendedTypes.Count}个自定义预制体扩展类");

                Note.note.Log($"{typeof(TPrefab)}的预制体扩展加载结束");
            }
        }

        [HideDuplicateReferenceBox]
        [HideReferenceObjectPicker]
        [Serializable]
        public class GameTypeBasicInfo : BaseConfigClass, IUniversalTree<GameTypeBasicInfo>
        {
            [LabelText("ID")] [StringIsNotNullOrEmpty]
            public string id;

            [LabelText("名称")] public StringTranslation name = new();

            [LabelText("子种类")] public List<GameTypeBasicInfo> subtypes = new();

            [HideInInspector] public string parentID;

            public IEnumerable<GameTypeBasicInfo> GetChildren() => subtypes;

            public GameTypeBasicInfo GetParent() => throw new NotImplementedException();

            public bool DirectEquals(GameTypeBasicInfo other) => id == other.id;

            protected override void OnInspectorInit()
            {
                base.OnInspectorInit();

                if (name.allTranslations.Count == 0)
                {
                    name = "待补全";
                }
            }
        }

        [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
        public abstract class GameItemGeneralSetting : GeneralSettingBase, IPrefabCoreGeneralSetting
        {
            public const string NULL_ID = GamePrefabCoreBundle<TPrefab, TGeneralSetting>.NULL_ID;

            protected const string EXTENDED_TYPES_SETTING_CATEGORY = "扩展类";

            protected const string GAME_TYPE_SETTING_CATEGORY = "种类设置";

            protected const string MISCELLANEOUS_SETTING_CATEGORY = "杂项设置";

            protected const string TEST_CATEGORY = "测试";

            private const string DEBUGGING_MODE_STATE_SETTING_CATEGORY = TEST_CATEGORY + 
                                                                         "/调试模式设置";

            private const string PREFABS_ACTIVE_STATE_SETTING_CATEGORY = TEST_CATEGORY + 
                                                                         "/预制体启用状态设置";

            [LabelText("Excel存储路径")]
            [GUIColor(0.906f, 0.635f, 0.227f)]
            [ShowInInspector, DisplayAsString, PropertyOrder(-995)]
            [EnableGUI, LabelWidth(80)]
            public string excelFilePath
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get =>
                    dataFolderPath.PathCombine(
                        $"{settingName.GetTranslation("English")}GeneralSetting.xlsx");
            }

            /// <summary>
            /// 设置里预制体的名字
            /// </summary>
            public virtual StringTranslation prefabName => new()
            {
                { "English", "未注册预制体名称" },
                { "Chinese", "Unregistered Prefab Name" }
            };

            /// <summary>
            /// 设置里预制体的后缀名称
            /// </summary>
            public virtual StringTranslation prefabSuffixName => new()
            {
                { "English", "Prefab" },
                { "Chinese", "预制体" }
            };

            /// <summary>
            /// 预制体列表里最少需要的预制体，否则GUI会弹出警告
            /// </summary>
            public virtual int minPrefabCount => 0;

            /// <summary>
            /// 添加到预制体列表里的Prefab的Type需求
            /// </summary>
            protected virtual Type requirePrefabType => typeof(TPrefab);

            /// <summary>
            /// 是否显示将第一个预制体设置成默认ID和Name的按钮，仅测试用
            /// </summary>
            protected virtual bool showSetFirstPrefabIDAndNameToDefaultButton => true;

            [LabelText("扩展预制体类"), TitleGroup(EXTENDED_TYPES_SETTING_CATEGORY)]
            [ReadOnly]
            [EnableGUI]
            [ShowInInspector]
            [PropertyOrder(-100)]
            protected Dictionary<string, Type> extendedPrefabTypes => GameItemPrefab.extendedTypes;

            [LabelText("是否弃用种类设置"), TitleGroup(GAME_TYPE_SETTING_CATEGORY)]
            [ShowInInspector]
            public virtual bool gameTypeAbandoned => false;

            /// <summary>
            /// 种类树次根的ID
            /// </summary>
            //public virtual string SUBROOT_TYPE_ID => "SUBROOT";
            [LabelText(@"@prefabName + ""种类"""), TitleGroup(GAME_TYPE_SETTING_CATEGORY)]
            [NonSerialized, OdinSerialize]
            [HideIf(nameof(gameTypeAbandoned))]
            protected GameTypeBasicInfo typeBasicInfo = new();

            /// <summary>
            /// 默认种类ID
            /// </summary>
            [LabelText("默认种类"), TitleGroup(GAME_TYPE_SETTING_CATEGORY)]
            [ValueDropdown(nameof(GetTypeNameList))]
            [StringIsNotNullOrEmpty]
            [HideIf(nameof(gameTypeAbandoned))]
            public string defaultTypeID;

            [LabelText(@"@prefabName + ""种类唯一"""), TitleGroup(GAME_TYPE_SETTING_CATEGORY)]
            [HideIf(nameof(gameTypeAbandoned))]
            public bool isTypeIDUnique = true;

            /// <summary>
            /// 是否在游戏编辑器的左侧罗列预制体
            /// </summary>
            [LabelText(@"@""是否在左侧显示"" + prefabName + prefabSuffixName"), TitleGroup(MISCELLANEOUS_SETTING_CATEGORY)]
            public bool willShowPrefabOnWindowSide = false;

            [LabelText(@"@prefabName + prefabSuffixName + ""设置""")]
            [SerializeField, Space(10)]
            [InfoBox("ID重复", InfoMessageType.Error, nameof(ContainsSameID))]
            [InfoBox("存在空ID", InfoMessageType.Error, nameof(ContainsEmptyID))]
            [InfoBox(@"@""存在未初始化的"" + prefabName + prefabSuffixName", InfoMessageType.Error,
                nameof(ContainsNullPrefab))]
            [InfoBox(@"@""至少添加"" + minPrefabCount.ToString() + ""个"" + prefabName + prefabSuffixName",
                InfoMessageType.Warning,
                @"@allGameItemPrefabs != null && allGameItemPrefabs.Count < minPrefabCount")]
            [InfoBox("请至少添加一个", InfoMessageType.Info, "@allGameItemPrefabs.Count <= 0")]
            [Searchable]
            [OnInspectorInit(nameof(OnInspectorInit))]
            [OnCollectionChanged(nameof(OnAllGameItemPrefabsChanged))]
            [JsonProperty(PropertyName = "Prefabs")]
            [PropertyOrder(99)]
            [ListItemSelector(nameof(AllGameItemPrefabsListSelectHandle), 0.9f, 0.9f, 1f, 0.12f)]
            [ListDrawerSettings(NumberOfItemsPerPage = 6)]
            protected List<TPrefab> allGameItemPrefabs = new();


            protected TPrefab selectedPrefab { get; private set; } = null;
            protected int selectedPrefabIndex { get; private set; } = -1;

            #region GUI

            protected override void OnInspectorInit()
            {
                base.OnInspectorInit();

                allGameItemPrefabs ??= new();

                //Init Type Info
                typeBasicInfo ??= new();
                typeBasicInfo.parentID = "";

                if (typeBasicInfo.name == null || typeBasicInfo.name.allTranslations.Count == 0)
                {
                    typeBasicInfo.name = prefabName.GetClone();
                }

                if (typeBasicInfo.id.IsNullOrEmptyAfterTrim())
                {
                    typeBasicInfo.id = prefabName.GetTranslation("English").ToSnakeCase();
                }

                if (typeBasicInfo.subtypes.Count == 0)
                {
                    var newSubtype = new GameTypeBasicInfo
                    {
                        id = ("default" + prefabName.GetTranslation("English") + " Type").ToSnakeCase()
                    };

                    newSubtype.name = new()
                    {
                        { "English", newSubtype.id.ToPascalCase(" ") },
                        { "Chinese", "默认" + prefabName.GetTranslation("Chinese") + "种类" }
                    };

                    typeBasicInfo.subtypes.Add(newSubtype);
                }

                foreach (var gameTypeInfo in typeBasicInfo.subtypes)
                {
                    if (gameTypeInfo.id.IsNullOrEmptyAfterTrim())
                    {
                        continue;
                    }

                    if (gameTypeInfo.name == null || gameTypeInfo.name.allTranslations.Count == 0)
                    {
                        gameTypeInfo.name = new()
                        {
                            { "Any", gameTypeInfo.id.ToPascalCase(" ") }
                        };
                    }
                }

                if (defaultTypeID.IsNullOrEmpty())
                {
                    defaultTypeID = GetTypeIDList().FirstOrDefault();
                }

                //Prefab Extension
                GameItemPrefab.RefreshExtendedPrefabs();

                for (int i = 0; i < allGameItemPrefabs.Count; i++)
                {
                    var currentPrefab = allGameItemPrefabs[i];

                    if (currentPrefab.extendedPrefabType == null)
                    {
                        continue;
                    }

                    if (currentPrefab.GetType() != currentPrefab.extendedPrefabType)
                    {
                        allGameItemPrefabs[i] = currentPrefab.ConvertToChildOrParent(currentPrefab.extendedPrefabType);
                        allGameItemPrefabs[i].OnTypeChanged();
                    }
                }
            }

            //protected virtual void OnDrawPrefabListButton()
            //{
            //    if (SirenixEditorGUI.ToolbarButton(EditorIcons.Plus))
            //    {
            //        AddNewPrefabGUI();
            //    }
            //}

            //protected virtual void AddNewPrefabGUI()
            //{
            //    var typeSelector = new TypeSelector(GetDerivedPrefabsWithoutRegisteredID(), false);

            //    typeSelector.SetSelection(typeof(Prefab));
            //    typeSelector.SelectionConfirmed += types =>
            //    {
            //        var type = types.FirstOrDefault();
            //        allGameItemPrefabs.Add(type.CreateInstance() as Prefab);
            //    };

            //    typeSelector.ShowInPopup(450);
            //}

            public override void CheckSettingsGUI()
            {
                base.CheckSettingsGUI();

                foreach (var prefab in allGameItemPrefabs)
                {
                    prefab.CheckSettings();
                }
            }

            protected virtual void OnAllGameItemPrefabsChanged()
            {
                allGameItemPrefabs.RemoveAllNull();

                foreach (var prefab in allGameItemPrefabs.ToArray())
                {
                    if (prefab.GetType().IsDerivedFrom(requirePrefabType, true, true, true) == false)
                    {
                        Note.note.Warning($"{prefab.GetType().Name}不是所需类:{requirePrefabType.Name}或其子类");
                        allGameItemPrefabs.Remove(prefab);
                    }
                }
            }

            [Button(@"@""将第一个"" + prefabName + prefabSuffixName + ""设成默认ID和默认名称""")]
            [TitleGroup(TEST_CATEGORY, Order = 100)]
            [ShowIf("@allGameItemPrefabs != null && allGameItemPrefabs.Count > 0 && allGameItemPrefabs[0] != null" +
                    "&& showSetFirstPrefabIDAndNameToDefaultButton")]
            private void SetFirstPrefabIDAndNameToDefault()
            {
                allGameItemPrefabs[0].id = $"default{prefabName.GetTranslation("English")}".ToSnakeCase();
                allGameItemPrefabs[0].name = new()
                {
                    { "English", $"Default {prefabName.GetTranslation("English")}".ToPascalCase(" ") },
                    { "Chinese", $"默认{prefabName.GetTranslation("Chinese")}" }
                };
            }

            [Button(@"@""打开所有"" + " + nameof(prefabName) + "+" + nameof(prefabSuffixName) + @"+""的调试模式""")]
            [TitleGroup(TEST_CATEGORY)]
            [HorizontalGroup(DEBUGGING_MODE_STATE_SETTING_CATEGORY)]
            private void EnableAllPrefabsDebuggingMode()
            {
                foreach (var prefab in allGameItemPrefabs)
                {
                    prefab.isDebugging = true;
                }
            }

            [Button(@"@""关闭所有"" + " + nameof(prefabName) + "+" + 
                    nameof(prefabSuffixName) + @"+""的调试模式""")]
            [HorizontalGroup(DEBUGGING_MODE_STATE_SETTING_CATEGORY)]
            private void DisableAllPrefabsDebuggingMode()
            {
                foreach (var prefab in allGameItemPrefabs)
                {
                    prefab.isDebugging = false;
                }
            }

            [Button(@"@""启用所有的"" + " + nameof(prefabName) + "+" + 
                    nameof(prefabSuffixName))]
            [HorizontalGroup(PREFABS_ACTIVE_STATE_SETTING_CATEGORY)]
            private void EnableAllPrefabsActiveState()
            {
                foreach (var prefab in allGameItemPrefabs)
                {
                    prefab.isActive = true;
                }
            }

            [Button(@"@""禁用所有的"" + " + nameof(prefabName) + "+" + 
                    nameof(prefabSuffixName))]
            [HorizontalGroup(PREFABS_ACTIVE_STATE_SETTING_CATEGORY)]
            private void DisableAllPrefabsActiveState()
            {
                foreach (var prefab in allGameItemPrefabs)
                {
                    prefab.isActive = false;
                }
            }

            private void AllGameItemPrefabsListSelectHandle(int index)
            {
                if (index < allGameItemPrefabs.Count && index >= 0)
                {
                    OnAllGameItemPrefabsListSelectGUI(index, allGameItemPrefabs[index]);
                }
            }

            protected virtual void OnAllGameItemPrefabsListSelectGUI(int index, TPrefab selectedPrefab)
            {
                this.selectedPrefabIndex = index;
                this.selectedPrefab = selectedPrefab;
            }

#if UNITY_EDITOR
            protected void ChangeType()
            {
                var targetTypes = typeof(TPrefab).FindDerivedClasses(true, false).Where(type =>
                    type.HasFieldByName(REGISTERED_ID_NAME) == false &&
                    type.HasPropertyByName(REGISTERED_ID_NAME) == false);

                TypeSelector typeSelector = new TypeSelector(targetTypes,
                    false);
                typeSelector.SelectionConfirmed += (t =>
                {
                    var targetType = t.FirstOrDefault();
                    allGameItemPrefabs[selectedPrefabIndex] = selectedPrefab.ConvertToChildOrParent(targetType);
                    allGameItemPrefabs[selectedPrefabIndex].OnTypeChanged();
                });
                typeSelector.SetSelection(GetType());
                typeSelector.ShowInPopup(400);
            }
#endif


            #endregion

            #region Check & Init

            public override void CheckSettings()
            {
                base.CheckSettings();

                allGameItemPrefabs ??= new();

                if (ContainsNullPrefab())
                {
                    Note.note.Error($"存在为Null的{prefabName}{prefabSuffixName}");
                }

                if (ContainsEmptyID())
                {
                    Note.note.Error($"存在为空的{prefabName}{prefabSuffixName}ID");
                }

                if (ContainsSameID())
                {
                    Note.note.Error($"存在重复的{prefabName}{prefabSuffixName}ID");
                }

                if (allGameItemPrefabs.Count < minPrefabCount)
                {
                    Note.note.Warning($"至少添加{minPrefabCount}个{prefabName}{prefabSuffixName}");
                }
            }

            protected override void OnPreInit()
            {
                base.OnPreInit();

                //Init Type Info
                if (gameTypeAbandoned == false)
                {
                    if (typeBasicInfo == null || typeBasicInfo.id.IsNullOrEmptyAfterTrim())
                    {
                        Note.note.Error($"{GetType()}的type为Null或者type.id为空");
                        return;
                    }

                    rootGameType = GameType.CreateSubroot(typeBasicInfo.id, typeBasicInfo.name);

                    typeBasicInfo.PreorderTraverse(
                        node => node.subtypes.Examine(subtype => subtype.parentID = node.id),
                        true);

                    typeBasicInfo.PreorderTraverse(
                        node =>
                        {
                            if (node.id.IsNullOrEmptyAfterTrim())
                            {
                                Note.note.Error($"{GetType()}存在type的子节点的ID为空");
                            }

                            GameType.Create(node.id, node.name, node.parentID);
                        },
                        false);
                }

                GameItemPrefab.InitStatic();
            }

            protected override void OnInit()
            {
                base.OnInit();

                //Init Prefabs
                int itemSettingCount = allGameItemPrefabs.Count;

                if (itemSettingCount == 0)
                {
                    Note.note.Warning($"没有任何需要加载的{prefabName}{prefabSuffixName}，请检查{prefabName}通用配置文件");
                }
                else
                {
                    Note.note.Log($"准备加载{itemSettingCount}个{prefabName}{prefabSuffixName}");
                }

                GameItemPrefab.SetStaticGeneralSetting(this as TGeneralSetting);

                int skipCardSettingCount = 0;
                foreach (var prefab in allGameItemPrefabs)
                {
                    prefab.Init();

                    //try
                    //{

                    //}
                    //catch (Exception e)
                    //{
                    //    note.Warning(e.ToString());
                    //    skipCardSettingCount++;
                    //}
                }

                if (skipCardSettingCount <= 0)
                {
                    if (itemSettingCount > 0)
                    {
                        Note.note.Log($"{prefabName}{prefabSuffixName}全部加载");
                    }
                }
                else
                {
                    Note.note.Warning($"跳过了{skipCardSettingCount}个{prefabName}{prefabSuffixName}的加载");
                }
            }

            protected override void OnPostInit()
            {
                base.OnPostInit();


            }

            #endregion

            #region Excel

            //private struct TableCellData
            //{
            //    public object value;
            //    public string valueString;
            //}
            [TitleGroup("Excel", Order = 400)]
            [Button("写入Excel文件")]
            private void WriteToExcel()
            {
                if (dataFolderRelativePath.IsNullOrEmptyAfterTrim())
                {
                    Note.note.Error($"相对路径为空，无法写入Excel文件");
                }

                dataFolderPath.CreateDirectory();

                var columnNames = new List<string>();
                var rows = new List<Dictionary<string, (object value, string valueString)>>();

                JsonSerializer serializer = new();

                serializer.Converters.AddRange(CustomJSONConverter.converters);

                foreach (var prefab in allGameItemPrefabs)
                {
                    var o = JObject.FromObject(prefab, serializer);

                    foreach (var (propertyName, jToken) in o)
                    {
                        if (columnNames.Contains(propertyName) == false)
                        {
                            columnNames.Add(propertyName);
                        }
                    }
                }

                foreach (var prefab in allGameItemPrefabs)
                {
                    var o = JObject.FromObject(prefab, serializer);

                    var row = new Dictionary<string, (object value, string valueString)>();

                    foreach (var (propertyName, jToken) in o)
                    {
                        row[propertyName] = (prefab.GetFieldValueByName<object>(propertyName), jToken.ToString());
                    }

                    rows.Add(row);
                }

                try
                {
                    using var stream = new FileStream(excelFilePath, FileMode.Create, FileAccess.Write);
                    IWorkbook workbook = new XSSFWorkbook();
                    var excelSheet = (XSSFSheet)workbook.CreateSheet("Sheet1");

                    ExcelCellStyle.HandleWorkbook(workbook, excelSheet);

                    excelSheet.HorizontallyCenter = true;
                    excelSheet.VerticallyCenter = true;
                    excelSheet.Autobreaks = true;

                    IRow excelRow = excelSheet.CreateRow(0);
                    int columnIndex = 0;

                    foreach (string columnName in columnNames)
                    {
                        var newCell = excelRow.CreateCell(columnIndex);
                        newCell.SetCellValue(columnName);
                        newCell.CellStyle = ExcelCellStyle.GetTitleCellStyle();
                        columnIndex++;
                    }

                    int rowIndex = 1;

                    foreach (var row in rows)
                    {
                        excelRow = excelSheet.CreateRow(rowIndex);

                        int rowMaxLine = 0;
                        foreach (var columnName in columnNames)
                        {
                            if (row.TryGetValue(columnName, out var cellData))
                            {
                                var columnValueString = cellData.valueString;
                                rowMaxLine = rowMaxLine.Max(columnValueString.GetLineCount());
                            }
                        }

                        excelRow.Height = (short)(rowMaxLine * 300);

                        int cellIndex = 0;
                        foreach (var columnName in columnNames)
                        {
                            if (row.TryGetValue(columnName, out var cellData))
                            {
                                var newCell = excelRow.CreateCell(cellIndex);
                                newCell.SetCellValue(cellData.valueString);
                                newCell.CellStyle = ExcelCellStyle.GetValueCellStyle(cellData.value);
                            }

                            cellIndex++;
                        }

                        rowIndex++;
                    }

                    for (int i = 0; i < columnNames.Count; i++)
                    {
                        excelSheet.AutoSizeColumn(i);
                    }

                    workbook.Write(stream);
                }
                catch (IOException ioException)
                {
                    Debug.LogError("确保Excel已经关闭");
                    Debug.LogError(ioException);
                }
            }

            [TitleGroup("Excel", Order = 400)]
            [Button("读取Excel文件")]
            private void ReadFromExcel()
            {
                if (dataFolderRelativePath.IsNullOrEmptyAfterTrim())
                {
                    Note.note.Error($"相对路径为空，无法读取Excel文件");
                }

                if (excelFilePath.ExistsFile() == false)
                {
                    Note.note.Error("不存在指定的Excel文件，请先创建Excel文件或写入Excel文件");
                }

                try
                {
                    using var stream = new FileStream(excelFilePath, FileMode.Open);

                    stream.Position = 0;

                    XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                    var sheet = (XSSFSheet)xssWorkbook.GetSheetAt(0);

                    IRow headerRow = sheet.GetRow(0);

                    int totalColumnCount = headerRow.LastCellNum;

                    var validColumnIndices = new HashSet<int>();
                    var validPropertyNames = new Dictionary<int, string>();

                    for (int columnIndex = 0; columnIndex < totalColumnCount; columnIndex++)
                    {
                        ICell cell = headerRow.GetCell(columnIndex);

                        if (cell == null || cell.ToString().IsNullOrEmptyAfterTrim())
                        {
                            Debug.LogError($"第{columnIndex + 1}列的标题无效，为空");
                            continue;
                        }

                        var propertyName = cell.ToString().Trim();

                        if (columnIndex == 0)
                        {
                            if (propertyName != "id")
                            {
                                Debug.LogError($"第1列的标题必须是id");
                                return;
                            }
                        }
                        else
                        {
                            validColumnIndices.Add(columnIndex);
                            validPropertyNames[columnIndex] = propertyName;
                        }
                    }

                    for (int rowIndex = (sheet.FirstRowNum + 1); rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        IRow row = sheet.GetRow(rowIndex);

                        if (row == null)
                        {
                            continue;
                        }

                        var firstCell = row.GetCell(0);

                        if (firstCell.CellType == CellType.Blank)
                        {
                            Debug.LogError($"第{rowIndex + 1}行的第一个单元格为空，此处应为ID");
                            continue;
                        }

                        var id = firstCell.ToString();

                        if (id.IsNullOrEmptyAfterTrim())
                        {
                            Debug.LogError($"第{rowIndex + 1}行的ID为空，无效");
                            continue;
                        }

                        var prefab = GetPrefab(id);

                        if (prefab == null)
                        {
                            AddPrefab(id);
                            prefab = GetPrefab(id);

                            Debug.LogWarning($"新添了id为{id}的预制体");
                        }

                        foreach (var validColumnIndex in validColumnIndices)
                        {
                            var propertyName = validPropertyNames[validColumnIndex];

                            var cell = row.GetCell(validColumnIndex);

                            if (cell == null || cell.ToString().IsNullOrEmpty())
                            {
                                continue;
                            }

                            var fieldInfo = prefab.GetType().GetFieldByName(propertyName);

                            if (fieldInfo == null)
                            {
                                Debug.LogError($"第{rowIndex + 1}行，第{validColumnIndex + 1}列：" +
                                               $"{prefab.GetType()}不存在名为{propertyName}的字段");
                                continue;
                            }

                            var propertyValueJSON = cell.ToString();

                            try
                            {
                                if (fieldInfo.FieldType == typeof(string))
                                {
                                    fieldInfo.SetValue(prefab, propertyValueJSON);
                                }
                                else if (fieldInfo.FieldType == typeof(bool))
                                {
                                    propertyValueJSON = propertyValueJSON.Trim('\"', '\'', ' ', '\n', '\r').ToUpper();
                                    if (propertyValueJSON == "TRUE")
                                    {
                                        fieldInfo.SetValue(prefab, true);
                                    }
                                    else if (propertyValueJSON == "FALSE")
                                    {
                                        fieldInfo.SetValue(prefab, false);
                                    }
                                    else
                                    {
                                        throw new JsonReaderException($"{propertyValueJSON} 不是True也不是False");
                                    }
                                }
                                else
                                {
                                    if (fieldInfo.FieldType.IsEnum)
                                    {
                                        propertyValueJSON = "\"" + propertyValueJSON + "\"";
                                    }

                                    var propertyValue = JsonConvert.DeserializeObject(propertyValueJSON,
                                        fieldInfo.FieldType,
                                        CustomJSONConverter.converters);

                                    fieldInfo.SetValue(prefab, propertyValue);
                                }
                            }
                            catch (JsonReaderException jsonReaderException)
                            {
                                Debug.LogError($"在读取第{rowIndex + 1}行，第{validColumnIndex + 1}列，发生JSON解序列化错误，请注意格式");
                                Debug.LogError(jsonReaderException);
                            }
                        }
                    }
                }
                catch (IOException ioException)
                {
                    Debug.LogError("确保Excel已经关闭");
                    Debug.LogError(ioException);
                }
            }

            [TitleGroup("Excel", Order = 400)]
            [Button("打开Excel文件")]
            public void OpenExcel()
            {
                excelFilePath.OpenFile();
            }

            #endregion

            #region Prefab

            #region AddPrefab

            /// <summary>
            /// 添加预制体，当预制体列表不存在参数newPrefabID对应的ID时，才会添加新的ID为newPrefabID的预制体
            /// 仅用于Editor模式
            /// </summary>
            /// <param name="newPrefabID"></param>
            public void AddPrefab(string newPrefabID)
            {
                var newPrefab = (TPrefab)typeof(TPrefab).CreateInstance();
                newPrefab.id = newPrefabID;

                AddPrefab(newPrefab);
            }

            /// <summary>
            /// 添加预制体，当预制体列表不存在参数prefab的ID时，才会添加此prefab
            /// 仅用于Editor模式
            /// </summary>
            /// <param name="prefab"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddPrefab([NotNull] TPrefab prefab)
            {
                if (prefab.id.IsNullOrEmptyAfterTrim())
                {
                    Note.note.Warning("id为空");
                }

                if (ContainsID(prefab.id) == false)
                {
                    allGameItemPrefabs.Add(prefab);
                }
            }

            /// <summary>
            /// 添加预制体到列表第一个，当预制体列表不存在参数prefab的ID时，才会添加此prefab
            /// 仅用于Editor模式
            /// </summary>
            /// <param name="prefab"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddPrefabToFirst([NotNull] TPrefab prefab)
            {
                if (prefab.id.IsNullOrEmptyAfterTrim())
                {
                    Note.note.Warning("id为空");
                }

                if (ContainsID(prefab.id) == false)
                {
                    allGameItemPrefabs.Insert(0, prefab);
                }
            }

            /// <summary>
            /// 添加预制体，当预制体列表不存在参数prefab的ID时，才会添加此prefab
            /// 与AddPrefab不同的是，当prefab的ID为空或者已经存在prefab.id的时候，会报错而不是发出警告
            /// 仅用于Editor模式
            /// </summary>
            /// <param name="prefab"></param>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddPrefabStrictly([NotNull] TPrefab prefab)
            {
                if (prefab.id.IsNullOrEmptyAfterTrim())
                {
                    Note.note.Error("id为空");
                }

                if (ContainsID(prefab.id) == false)
                {
                    allGameItemPrefabs.Add(prefab);
                }
                else
                {
                    Note.note.Error($"添加id为{prefab.id}的{prefabName}失败，已存在此id");
                }
            }

            /// <summary>
            /// 尝试添加往预制体列表里添加新的prefab，当id为空或者id已经存在时，会返回False，否则返回True
            /// </summary>
            /// <param name="prefab"></param>
            /// <param name="newPrefab"></param>
            /// <returns></returns>
            public bool TryAddPrefab(TPrefab prefab, out TPrefab newPrefab)
            {
                if (prefab.id.IsNullOrEmptyAfterTrim())
                {
                    Note.note.Warning("id为空");
                    newPrefab = null;
                    return false;
                }

                if (ContainsIDIgnoreActiveState(prefab.id) == false)
                {
                    allGameItemPrefabs.Add(prefab);
                    newPrefab = prefab;
                    return true;
                }

                newPrefab = null;
                return false;
            }

            /// <summary>
            /// 尝试添加往预制体列表里添加新的prefab，当newPrefabID为空或者newPrefabID已经存在时，会返回False，否则返回True
            /// </summary>
            /// <param name="newPrefabID"></param>
            /// <param name="newPrefab"></param>
            /// <returns></returns>
            public bool TryAddPrefab(string newPrefabID, out TPrefab newPrefab)
            {
                var newPrefabTemp = (TPrefab)typeof(TPrefab).CreateInstance();
                newPrefabTemp.id = newPrefabID;

                return TryAddPrefab(newPrefabTemp, out newPrefab);
            }

            #endregion

            #region Getprefab

            /// <summary>
            /// 获取所有预制体
            /// </summary>
            /// <returns>返回包含所有预制体的List</returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public IReadOnlyList<TPrefab> GetAllPrefabs()
            {
                return allGameItemPrefabs;
            }

            /// <summary>
            /// 通过ID获取预制体，如果没有会返回Null
            /// 最好仅在Editor模式下调用，因为此方法效率较低
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TPrefab GetPrefab(string id)
            {
                if (allGameItemPrefabs == null || allGameItemPrefabs.Count == 0)
                {
                    return null;
                }

                return allGameItemPrefabs.FirstOrDefault(prefab =>
                    prefab != null && prefab.id == id && prefab.isActive);
            }

            /// <summary>
            /// 严格通过ID获取预制体，如果没有就报错
            /// 最好仅在Editor模式下调用，因为此方法效率较低
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TPrefab GetPrefabStrictly(string id)
            {
                var prefab = GetPrefab(id);
                if (prefab == null)
                {
                    Note.note.Error($"id为{id}的{prefabName}{prefabSuffixName}不存在");
                }

                return prefab;
            }

            /// <summary>
            /// 随机获取一个预制体
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TPrefab GetRandomPrefab()
            {
                return allGameItemPrefabs.Where(prefab => prefab is { isActive: true }).Choose();
            }

            /// <summary>
            /// 随机获取预制体的ID，如果无预制体返回NUll
            /// </summary>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public string GetRandomPrefabID()
            {
                return GetRandomPrefab()?.id;
            }

            /// <summary>
            /// 获取全部预制体的ID
            /// </summary>
            /// <returns>返回所有预制体的ID构成的IEnumerable</returns>
            public IEnumerable<string> GetPrefabIDList()
            {
                if (allGameItemPrefabs != null)
                {
                    foreach (var prefab in allGameItemPrefabs)
                    {
                        if (prefab is { isActive: true })
                        {
                            yield return prefab.id;
                        }
                    }
                }

            }

            /// <summary>
            /// 获取特定ID预制体的名称
            /// 最好仅在Editor模式下调用，因为此方法效率较低
            /// </summary>
            /// <param name="id">预制体ID</param>
            /// <returns></returns>
            public StringTranslation GetPrefabName(string id)
            {
                var result = GetPrefab(id);

                if (result != null)
                {
                    return result.name;
                }

                return $"{id}不存在，获取名称失败";
            }

            /// <summary>
            /// 获取所有预制体的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
            /// 比如 [ValueDropdown("@GameSetting.inputSystemGeneralSetting.GetPrefabNameList()")]
            /// </summary>
            /// <returns>返回为ValueDropdownItem构成的IEnumerable</returns>
            public IEnumerable<ValueDropdownItem<string>> GetPrefabNameList()
            {
                if (allGameItemPrefabs != null)
                {
                    foreach (var prefab in allGameItemPrefabs)
                    {
                        if (prefab is { isActive: true })
                        {
                            yield return new ValueDropdownItem<string>(prefab.name, prefab.id);
                        }
                    }
                }
            }

            /// <summary>
            /// 获取特定Type对应的类或其派生类的预制体的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
            /// 比如 [ValueDropdown("@GameSetting.uiPanelGeneralSetting.GetPrefabNameList(typeof(ContainerUIPreset))")]
            /// </summary>
            /// <param name="specificType">约束预制体的类型</param>
            /// <returns>返回为ValueDropdownItem构成的IEnumerable</returns>
            public IEnumerable<ValueDropdownItem<string>> GetPrefabNameList(Type specificType)
            {
                if (allGameItemPrefabs != null)
                {
                    foreach (var prefab in allGameItemPrefabs)
                    {
                        if (prefab is { isActive: true } && specificType.IsInstanceOfType(prefab))
                        {
                            yield return new ValueDropdownItem<string>(prefab.name, prefab.id);
                        }
                    }
                }
            }

            #endregion

            #region Contains

            /// <summary>
            /// 预制体列表是否包含某个特定ID
            /// 最好仅在Editor模式下调用，因为此方法效率较低
            /// </summary>
            /// <param name="id"></param>
            /// <returns></returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsID(string id)
            {
                if (allGameItemPrefabs == null || allGameItemPrefabs.Count == 0)
                {
                    return false;
                }

                return allGameItemPrefabs.Any(prefab => prefab != null && prefab.id == id && prefab.isActive);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsIDIgnoreActiveState(string id)
            {
                if (allGameItemPrefabs == null || allGameItemPrefabs.Count == 0)
                {
                    return false;
                }

                return allGameItemPrefabs.Any(prefab => prefab != null && prefab.id == id);
            }

            /// <summary>
            /// 预制体列表里是否包含重复ID
            /// </summary>
            /// <returns></returns>
            public bool ContainsSameID()
            {
                return allGameItemPrefabs.Where(prefab => prefab != null).ContainsSame(prefab => prefab.id);
            }

            /// <summary>
            /// 预制体列表里是否包含空ID
            /// </summary>
            /// <returns></returns>
            public bool ContainsEmptyID()
            {
                if (allGameItemPrefabs == null || allGameItemPrefabs.Count == 0)
                {
                    return false;
                }

                return allGameItemPrefabs.Where(prefab => prefab != null).Select(prefab => prefab.id)
                    .Any(StringFunc.IsNullOrEmptyAfterTrim);
            }

            /// <summary>
            /// 预制体列表里是否包含Null
            /// </summary>
            /// <returns></returns>
            public bool ContainsNullPrefab()
            {
                return allGameItemPrefabs.ContainsNull();
            }

            #endregion

            #endregion

            #region Type

            public string GetTypeName(string typeID)
            {
                if (rootGameType == null)
                {
                    return typeBasicInfo.GetAllChildren(false).FirstOrDefault(node => node.id == typeID)?.name;
                }

                return GameType.GetGameType(typeID)?.name;
            }

            /// <summary>
            /// 获取所有叶节点类别的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
            /// </summary>
            /// <returns></returns>
            public IEnumerable<ValueDropdownItem<string>> GetTypeNameList()
            {
                return typeBasicInfo.GetAllLeaves(false)
                    .Select(subType => new ValueDropdownItem<string>(subType.name, subType.id));
            }

            /// <summary>
            /// 获取所有叶节点类别的ID列表
            /// </summary>
            /// <returns></returns>
            public IEnumerable<string> GetTypeIDList()
            {
                return typeBasicInfo.GetAllLeaves(false).Select(subType => subType.id);
            }

            /// <summary>
            /// 获取所有类别的名称和ID，一般用于Odin插件里的ValueDropdown Attribute
            /// </summary>
            /// <returns></returns>
            public IEnumerable<ValueDropdownItem<string>> GetAllTypeNameList()
            {
                return typeBasicInfo.GetAllChildren(false)
                    .Select(subType => new ValueDropdownItem<string>(subType.name, subType.id));
            }

            /// <summary>
            /// 获取所有类别的ID列表
            /// </summary>
            /// <returns></returns>
            public IEnumerable<string> GetAllTypeIDList()
            {
                return typeBasicInfo.GetAllChildren(false).Select(subType => subType.id);
            }

            /// <summary>
            /// 是否包含此种类
            /// </summary>
            /// <param name="typeID"></param>
            /// <returns></returns>
            public bool ContainsType(string typeID)
            {
                return typeBasicInfo.GetAllLeaves(false).Any(node => node.id == typeID);
            }

            private GameTypeBasicInfo GetGameTypeBasicInfo(string typeID)
            {
                return typeBasicInfo.GetAllChildren(false).FirstOrDefault(node => node.id == typeID);
            }

            public IReadOnlyList<TPrefab> GetPrefabsByType(string typeID)
            {
                if (typeBasicInfo.TryFindChild(false,
                        typeInfo => typeInfo.id == typeID, out var typeInfo) == false)
                {
                    Note.note.Warning($"不存在ID为{typeID}的{prefabName}种类");
                    return null;
                }

                if (allGameItemPrefabs == null)
                {
                    return null;
                }

                var results = new List<TPrefab>();

                foreach (var prefab in allGameItemPrefabs)
                {
                    if (prefab is { isActive: true })
                    {
                        foreach (var gameTypeID in prefab.gameTypesID)
                        {
                            if (typeInfo.HasChild(true, typeInfo => typeInfo.id == gameTypeID))
                            {
                                results.Add(prefab);
                                break;
                            }
                        }
                    }
                }

                return results;
            }

            public IEnumerable<ValueDropdownItem<string>> GetPrefabNameListByType(string typeID)
            {
                foreach (var prefab in GetPrefabsByType(typeID))
                {
                    yield return new ValueDropdownItem<string>(prefab.name, prefab.id);
                }
            }

            #endregion
        }
    }
}
