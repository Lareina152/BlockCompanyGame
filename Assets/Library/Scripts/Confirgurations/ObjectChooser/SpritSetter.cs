using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ConfigurationBasis
{
    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    public class SpriteItem : BaseConfigClass
    {
        [PreviewField(60, ObjectFieldAlignment.Center)]
        [AssetSelector(Paths = "Assets/Resources")]
        [Required]
        [HideLabel]
        public Sprite sprite;

        public override bool Equals(object obj) =>
            obj switch
            {
                null => false,
                SpriteItem spriteItem => sprite.Equals(spriteItem.sprite),
                _ => false
            };

        public override int GetHashCode()
        {
            if (sprite == null) return base.GetHashCode();

            return sprite.GetHashCode();
        }

        public static implicit operator Sprite(SpriteItem item)
        {
            return item.sprite;
        }

        public static implicit operator SpriteItem(Sprite sprite)
        {
            return new()
            {
                sprite = sprite
            };
        }
    }

    [Serializable]
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]

    public class SpriteSetter : ObjectChooser<SpriteItem>
    {
        //[ShowInInspector]
        //[HideLabel]
        //[AssetList(Path = "Resources"), PreviewField(70, ObjectFieldAlignment.Center)]
        //[AssetSelector(Paths = "Assets/Resources")]
        //[ShowIf(@"@isRandomValue == false && fixedType == ""Single Value""")]
        //public Sprite fixedSpritePreview
        //{
        //    get => value;
        //    set => this.value = value;
        //}

        #region GUI

#if UNITY_EDITOR
        private IEnumerable<Sprite> selectedSprites => Selection.objects.Select<Object, Sprite>();

        private int selectedSpritesCount => selectedSprites.Count();

        [Button("添加选中的Sprites")]
        [ShowIf("@" + nameof(selectedSpritesCount) + " > 0 && " + nameof(isRandomValue))]
        private void AddSelectedSprites()
        {
            if (isRandomValue == false)
            {
                return;
            }

            foreach (var selectedSprite in selectedSprites)
            {
                if (randomType == WEIGHTED_SELECT)
                {
                    var values = valueProbabilities.Select(item => item.value.sprite).ToList();

                    if (values.Contains(selectedSprite) == false)
                    {
                        valueProbabilities.Add(new()
                        {
                            value = selectedSprite,
                            ratio = 1
                        });
                    }

                    OnValueProbabilitiesChanged();
                }

                if (randomType == CIRCULAR_SELECT)
                {
                    var values = circularItems.Select(item => item.value.sprite).ToList();

                    if (values.Contains(selectedSprite) == false)
                    {
                        circularItems.Add(new()
                        {
                            value = selectedSprite,
                            times = 1
                        });
                    }
                }
            }
        }

#endif

        #endregion

        public static implicit operator Sprite(SpriteSetter setter)
        {
            return setter.GetValue();
        }
    }
}
