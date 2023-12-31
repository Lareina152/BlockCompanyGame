﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ConfigurationBasis
{
    [Serializable]
        [HideDuplicateReferenceBox]
        [HideReferenceObjectPicker]

    public class SpriteItemList : BaseConfigClass
    {
        [LabelText("Sprite列表")] [InfoBox("请至少添加一个Sprite", InfoMessageType.Warning, "@sprites.Count <= 0")]
        public List<SpriteItem> sprites = new();


        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            sprites ??= new();
        }

        public List<Sprite> GetSprites()
        {
            return sprites.Select(spriteItem => spriteItem.sprite).ToList();
        }

        public static implicit operator List<Sprite>(SpriteItemList spriteItemList)
        {
            return spriteItemList.GetSprites();
        }
    }

    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    public class SpriteListSetter : ObjectChooser<SpriteItemList>
    {
        public static implicit operator List<Sprite>(SpriteListSetter setter)
        {
            return setter.GetValue().GetSprites();
        }
    }
}
