using Basis;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basis.GameItem;
using ConfigurationBasis;
using UnityEngine;

public sealed class TurningSystemConfiguration : 
    GamePrefabCoreBundle<TurningSystemConfiguration, TurningSystemGeneralSetting>.GameItemPrefab
{
    [LabelText("每一回合的现实时间"), SuffixLabel("毫秒")]
    public int realTimeEveryTurn;

    [LabelText("启用初始回合数")]
    public bool enableStartTurn = false;

    [LabelText("开始的回合数")]
    [ShowIf(nameof(enableStartTurn))]
    public int startTurn = 0;

    [LabelText("是否在Awake阶段进行回合")]
    public bool playOnAwake = true;

    [LabelText("初始进行多少回合")]
    [ShowIf(nameof(playOnAwake))]
    public ulong turnLeftToStopOnAwake = ulong.MaxValue / 10;
}

public sealed class TurningSystem : UniqueMonoBehaviour<TurningSystem>
{
    [LabelText("当前回合"), DisplayAsString]
    public long currentTurn = long.MinValue;

    [LabelText("已经过的回合"), DisplayAsString]
    public ulong turnPassed = 0;

    [LabelText("还剩多少回合暂停"), DisplayAsString]
    public ulong turnLeftToStop = 0;

    private readonly List<Action> everyTurnEvent = new();
    private readonly Dictionary<int, List<Action>> eventWaitList = new();
    private int nextEventTurnLeft = 0;
    private int nextEventTurn = 0;
    

    [ShowInInspector]
    [DisplayAsString]
    public bool isOn { get; private set; }

    [ValueDropdown("@GameSetting.turningSystemGeneralSetting.GetPrefabNameList()")]
    public string configID;

    [HideInEditorMode]
    [LabelText("配置")]
    public TurningSystemConfiguration config;

    protected override void Awake()
    {
        base.Awake();

        config = GameCoreSettingBase.turningSystemGeneralSetting.GetPrefabStrictly(configID);

        if (config.enableStartTurn)
        {
            currentTurn = config.startTurn;
        }

        if (config.playOnAwake)
        {
            Play(config.turnLeftToStopOnAwake);
        }
        else
        {
            Stop();
        }

        nextEventTurn = 0;
        nextEventTurnLeft = 0;

        _ = UpdateEvent();
    }

    private void UpdateEventWaitList(int turns)
    {
        if (turns <= 0)
        {
            return;
        }

        var allTurnLefts = eventWaitList.Keys.ToList();
        allTurnLefts.Sort();

        foreach (var turnLeft in allTurnLefts)
        {
            if (turnLeft <= turns)
            {
                if (isOn == false)
                {
                    Note.note.Warning($"在isOn=false时执行了事件，" +
                                      $"turns={turns}, " +
                                      $"nextEventTurn={nextEventTurn}, " + 
                                      $"nextEventTurnLeft={nextEventTurnLeft}, " +
                                      $"turnLeftToStop={turnLeftToStop}, " + 
                                      $"turnLeft={turnLeft}");
                }

                foreach (var action in eventWaitList[turnLeft])
                {
                    action();
                }

                eventWaitList.Remove(turnLeft);
            }
            else
            {
                eventWaitList.ChangeKey(turnLeft, turnLeft - turns);
            }
        }
    }

    private async UniTask UpdateEvent()
    {
        while (true)
        {
            if (isOn == false)
            {
                if (nextEventTurn > 0 && nextEventTurn != nextEventTurnLeft)
                {
                    UpdateEventWaitList(nextEventTurn - nextEventTurnLeft);
                    nextEventTurn = nextEventTurnLeft;
                }
                
                await UniTask.NextFrame();
                continue;
            }

            if (turnLeftToStop <= 0)
            {
                isOn = false;
                continue;
            }

            foreach (var action in everyTurnEvent)
            {
                action();
            }

            if (nextEventTurnLeft <= 0 && eventWaitList.Count > 0)
            {

                if (nextEventTurn > 0)
                {
                    UpdateEventWaitList(nextEventTurn);
                }

                nextEventTurn = eventWaitList.Keys.Min();
                nextEventTurnLeft = nextEventTurn;
            }

            await UniTask.Delay(config.realTimeEveryTurn);

            currentTurn++;
            turnPassed++;
            turnLeftToStop--;
            nextEventTurnLeft--;

            if (turnLeftToStop <= 0)
            {
                isOn = false;
            }
        }
        
    }

    public static void Play(ulong turnLeftToStop = ulong.MaxValue)
    {
        instance.isOn = true;
        instance.turnLeftToStop = turnLeftToStop;
    }

    public static void Stop()
    {
        instance.isOn = false;
    }

    public static void AddEvent(int turnLeft, Action action)
    {
        if (turnLeft <= 0)
        {
            action();
        }

        if (instance.eventWaitList.ContainsKey(turnLeft))
        {
            instance.eventWaitList[turnLeft] = new List<Action>();
        }

        instance.eventWaitList[turnLeft].Add(action);
    }

    public static void AddEventNextTurn(Action action)
    {
        AddEvent(1, action);
    }

    public static void AddEventToEveryTurn(Action action)
    {
        instance.everyTurnEvent.Add(action);
    }
}
