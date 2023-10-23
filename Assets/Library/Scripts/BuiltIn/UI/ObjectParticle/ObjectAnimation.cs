using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Basis;
using ConfigurationBasis;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ObjectAnimationBasis
{
    [HideDuplicateReferenceBox]
    [HideReferenceObjectPicker]
    [Serializable]
    public class ObjectAnimation : BaseConfigClass
    {
        [LabelText("片段列表")]
        [SerializeField]
        private List<ObjectAnimationClip> clipList = new();

        private float _totalDuration = 0;

        [LabelText("总持续时间")]
        [ShowInInspector]
        public float totalDuration
        {
            get
            {
                if (initDone)
                {
                    return _totalDuration;
                }

                return GetTotalDuration();
            }
        }

        protected override void OnInit()
        {
            base.OnInit();

            _totalDuration = GetTotalDuration();
        }

        public void Run(Transform target)
        {
            foreach (var clip in clipList)
            {
                clip.OnStart(target);

                clip.startTime.DelayAction(() =>
                {
                    if (target != null && target.gameObject.activeSelf)
                    {
                        clip.Run(target);
                    }
                });

            }
        }

        private float GetTotalDuration()
        {
            if (clipList == null || clipList.Count == 0)
            {
                return 0;
            }

            return clipList.Select(clip => clip.startTime + clip.GetTotalDuration()).Max();
        }

        public void Kill(Transform target)
        {
            if (target == null)
            {
                return;
            }

            foreach (var clip in clipList)
            {
                clip.Kill(target);
            }
        }

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            clipList ??= new();
        }

        #region Preset

        [FoldoutGroup("预设")]
        [Button("跳跃淡出预设")]
        private void AddLeapFadeOutPreset()
        {
            clipList ??= new();

            clipList.Add(new FadeOut()
            {
                startTime = 0.3f,
                fadeDuration = 0.25f,
                setAlphaToOneOnStart = true
            });

            clipList.Add(new Leap()
            {
                startTime = 0,
                leapDuration = 0.7f,
                leapPower = 50,
                leapTimes = 2,
                leapEndOffset = new()
                {
                    asVector2D = true,
                    planeType = PlaneType.XY,
                    setter2D = new()
                    {
                        isRandomValue = true,
                        randomType = "Weighted Select",
                        valueProbabilities = new()
                        {
                            new()
                            {
                                value = new Vector2(140, 50),
                                ratio = 1
                            },
                            new()
                            {
                                value = new Vector2(-140, 50),
                                ratio = 1
                            }
                        },
                        decimalPlaces = 0
                    }
                }
            });
        }

        [FoldoutGroup("预设")]
        [Button("从底下浮现并淡出预设")]
        private void AddRiseAndFadeOutPreset()
        {
            clipList ??= new();

            clipList.Add(new FadeIn()
            {
                startTime = 0,
                fadeDuration = 0.25f,
                setAlphaToZeroOnStart = true,
            });

            clipList.Add(new Move()
            {
                startTime = 0,
                moveDuration = 0.3f,
                end = new Vector2(0, 65),
                ease = Ease.OutCubic
            });

            clipList.Add(new FadeOut()
            {
                startTime = 0.3f,
                fadeDuration = 0.2f,
                setAlphaToOneOnStart = false
            });
        }

        #endregion
    }

    public abstract class ObjectAnimationClip : BaseConfigClass
    {
        [LabelText("开始时间")]
        [MinValue(0)]
        public float startTime = 0;

        public abstract float GetTotalDuration();

        public abstract void Run(Transform target);

        public abstract void Kill(Transform target);

        public virtual void OnStart(Transform target)
        {

        }
    }

    public abstract class Fade : ObjectAnimationClip
    {
        [LabelText("淡化时间")]
        [MinValue(0)]
        public float fadeDuration = 0.25f;

        public override float GetTotalDuration()
        {
            return fadeDuration;
        }

        public override void OnStart(Transform target)
        {
            base.OnStart(target);

            var canvasGroup = target.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                OnStart(canvasGroup);
            }
        }

        public override void Run(Transform target)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                Run(canvasGroup);
            }
        }

        public override void Kill(Transform target)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();

            if (canvasGroup != null)
            {
                canvasGroup.DOKill();
            }
        }

        protected abstract void Run(CanvasGroup canvasGroup);

        protected abstract void OnStart(CanvasGroup canvasGroup);
    }

    [LabelText("淡入动画")]
    public class FadeIn : Fade
    {
        [LabelText("开始时设置透明度为0")]
        [LabelWidth(180)]
        [ToggleButtons("是", "否")]
        public bool setAlphaToZeroOnStart = false;

        protected override void Run(CanvasGroup canvasGroup)
        {
            canvasGroup.DOFade(1, fadeDuration);
        }

        protected override void OnStart(CanvasGroup canvasGroup)
        {
            if (setAlphaToZeroOnStart)
            {
                canvasGroup.alpha = 0;
            }
        }
    }

    [LabelText("淡出动画")]
    public class FadeOut : Fade
    {
        [LabelText("开始时设置透明度为1")]
        [LabelWidth(180)]
        [ToggleButtons("是", "否")]
        public bool setAlphaToOneOnStart = false;

        protected override void Run(CanvasGroup canvasGroup)
        {
            canvasGroup.DOFade(0, fadeDuration);
        }

        protected override void OnStart(CanvasGroup canvasGroup)
        {
            if (setAlphaToOneOnStart)
            {
                canvasGroup.alpha = 1;
            }
        }
    }

    [LabelText("跳跃动画")]
    public class Leap : ObjectAnimationClip
    {
        [LabelText("跳跃持续时间")]
        [MinValue(0)]
        public float leapDuration = 0.7f;

        [LabelText("跳跃落点")]
        [DisableObjectChooserType("Lerp")]
        public Vector3Setter leapEndOffset = new Vector2(140, 50);

        [LabelText("跳跃力量")]
        [DisableObjectChooserType("Lerp")]
        [MinValue(0)]
        public FloatSetter leapPower = 50;

        [LabelText("跳跃次数")]
        [DisableObjectChooserType("Lerp")]
        [MinValue(0)]
        public IntegerSetter leapTimes = 2;


        public override float GetTotalDuration()
        {
            return leapDuration;
        }

        public override void Run(Transform target)
        {
            target.DOLocalJump(target.localPosition + leapEndOffset,
                leapPower,
                leapTimes,
                leapDuration,
                false);
        }

        public override void Kill(Transform target)
        {
            target.DOKill();
        }

        #region GUI

        protected override void OnInspectorInit()
        {
            base.OnInspectorInit();

            leapEndOffset ??= new();
            leapPower ??= new();
            leapTimes ??= new();
        }

        #endregion
    }

    [LabelText("移动动画")]
    public class Move : ObjectAnimationClip
    {
        [LabelText("移动持续时间")]
        [MinValue(0)]
        public float moveDuration = 0.3f;

        [LabelText("终点")]
        public Vector3Setter end = new();

        [LabelText("动画曲线")]
        [Helper("https://easings.net/")]
        public Ease ease = Ease.Linear;

        public override float GetTotalDuration()
        {
            return moveDuration;
        }

        public override void Run(Transform target)
        {
            target.DOLocalMove(target.localPosition + end, moveDuration).SetEase(ease);
        }

        public override void Kill(Transform target)
        {
            target.DOKill();
        }
    }
}
