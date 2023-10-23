using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

using Basis;
using Sirenix.OdinInspector;
using Newtonsoft.Json;

namespace ItemBasis
{
    [Serializable]
    [HideReferenceObjectPicker]
    [HideDuplicateReferenceBox]
    [JsonConverter(typeof(GameTypeConverter))]
    public class GameType : IUniversalTree<GameType>, ICloneable
    {
        public const string ALL_ID = "all";

        public static GameType root
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                _root ??= new(ALL_ID);
                return _root;
            }
        }
        private static GameType _root = new(ALL_ID);

        private static Dictionary<string, GameType> allTypesDict = new();

        [LabelText("ID")]
        public string id;

        [LabelText("名称")]
        public StringTranslation name = new();

        [LabelText("子种类")]
        public Dictionary<string, GameType> subtypes = new();

        [LabelText("父种类")]
        public GameType parent;

        public GameType(string id)
        {
            if (CheckID(id) == false)
            {
                Note.note.Error($"ID无效:{id}");
            }

            this.id = id;
        }

        #region IUniversalTree

        public IEnumerable<GameType> GetChildren() => subtypes.Values;

        public GameType GetParent() => parent;

        public bool DirectEquals(GameType rhs) => id == rhs.id;

        #endregion

        public GameType AddSubtype(string subtypeID, StringTranslation name)
        {
            if (allTypesDict.ContainsKey(subtypeID))
            {
                Note.note.Warning($"种类ID:{subtypeID}重复添加");
            }

            var subtype = new GameType(subtypeID)
            {
                parent = this,
                name = name
            };

            subtypes[subtypeID] = subtype;

            allTypesDict[subtypeID] = subtype;

            return subtype;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GameType GetGameType(string typeID)
        {
            return allTypesDict.TryGetValue(typeID, out var gameType) ? gameType : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasGameType(string typeID)
        {
            return allTypesDict.ContainsKey(typeID);
        }

        public GameType GetChildGameType(string typeID)
        {
            return this.PreorderTraverseToGet(gameType => DirectEquals(gameType, typeID), true);
        }

        public bool BelongTo(GameType rhs)
        {
            return this.HasParent(rhs, true);
        }

        public bool BelongTo(string rhsID)
        {
            return this.HasParent(rhsID, DirectEquals, true);
        }

        public override string ToString()
        {
            string subtypesLog = "";

            foreach (GameType subtype in subtypes.Values)
            {
                subtypesLog += subtype.id + ", ";
            }

            return $"GameType: subtypeID : {id}, subtypes : [{subtypesLog}]";
        }

        public object Clone()
        {
            return this;
        }

        public static bool CheckID(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return false;
            }

            if (_root == null)
            {
                return true;
            }
            
            return id != ALL_ID;
        }

        public static bool DirectEquals(GameType type, string rhsID)
        {
            return type.id == rhsID;
        }

        public static GameType CreateSubroot(string subrootID, StringTranslation name)
        {
            return root.AddSubtype(subrootID, name);
        }

        public static void Create(string newID, StringTranslation name, string parentID)
        {
            if (parentID == ALL_ID)
            {
                Note.note.Warning($"请使用CreateSubroot来创建次根种类");
            }

            GameType parentType = root.GetChildGameType(parentID);

            if (parentType == null)
            {
                Note.note.Error($"创建id为{newID}时，发现规定parentID为{parentID}的GameType不存在");
                return;
            }

            parentType.AddSubtype(newID, name);
        }

        public static implicit operator GameType(string id)
        {
            return root.GetChildGameType(id);
        }

        public static implicit operator string(GameType gameType)
        {
            return gameType.id;
        }
    }
}
