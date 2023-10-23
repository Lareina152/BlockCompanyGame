using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConfigurationBasis;
using UnityEngine;

namespace Basis
{
    public static class TaskFunc
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UniTask DelaySeconds(this float delay)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(delay));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DelayAction(this float delay, Action action)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            action();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DelayAction(this float delay, Func<UniTaskVoid> func)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            func();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DelayAction(this int delay, Action action)
        {
            DelayAction(delay.F(), action);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DelayAction(this int delay, Func<UniTaskVoid> func)
        {
            DelayAction(delay.F(), func);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DelayFrameAction(this int delayFrameCount, Action action)
        {
            await UniTask.DelayFrame(delayFrameCount);

            action();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DelayAction<T>(this float gap, IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);

                await UniTask.Delay(TimeSpan.FromSeconds(gap));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DoFuncNTimes(this int nTimes, float gapTime, Action action)
        {
            for (int i = 0; i < nTimes; i++)
            {
                action();

                await UniTask.Delay(TimeSpan.FromSeconds(gapTime));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DoFuncNTimes(this int nTimes, float gapTime, Func<UniTaskVoid> func)
        {
            for (int i = 0; i < nTimes; i++)
            {
                func();

                await UniTask.Delay(TimeSpan.FromSeconds(gapTime));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DoFuncNTimes(this int nTimes, FloatSetter gapTime, Action action)
        {
            for (int i = 0; i < nTimes; i++)
            {
                action();

                await UniTask.Delay(TimeSpan.FromSeconds(gapTime));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static async void DoFuncNTimes(this int nTimes, FloatSetter gapTime, Func<UniTaskVoid> func)
        {
            for (int i = 0; i < nTimes; i++)
            {
                func();

                await UniTask.Delay(TimeSpan.FromSeconds(gapTime));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void TriggerAction(this Animator target, string triggerName, float triggerTime = 0.05f)
        {
            target.SetBool(triggerName, true);
            DelayAction(triggerTime, () =>
            {
                target.SetBool(triggerName, false);
            });
        }
    }
}
