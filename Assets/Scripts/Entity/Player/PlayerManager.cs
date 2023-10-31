using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerManager : UniqueMonoBehaviour<PlayerManager>, IManagerBehaviour
{
    [LabelText("左玩家出生点")]
    [SerializeField]
    private Vector2 leftPlayerSpawnPosition;

    [LabelText("右玩家出生点")]
    [SerializeField]
    private Vector2 rightPlayerSpawnPosition;

    public static bool playerCreated { get; private set; } = false;

    public static PlayerController leftPlayerCtrl { get; private set; }

    public static Player leftPlayer => leftPlayerCtrl.player;

    public static PlayerController rightPlayerCtrl { get; private set; }

    public static Player rightPlayer => rightPlayerCtrl.player;

    public static event 
        Action<(PlayerController leftPlayerCtrl, PlayerController rightPlayerController)> 
        OnPlayerCreatedEvent;


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Gizmos.DrawSphere(leftPlayerSpawnPosition, 0.2f);
        Gizmos.DrawSphere(rightPlayerSpawnPosition, 0.2f);
    }

    void IManagerBehaviour.OnEnterProcedure(string procedureID)
    {
        if (procedureID == GameInitializationProcedure.registeredID)
        {
            GameStateManager.OnGameEnded += ResetAllPlayers;

            GameStateManager.OnGameStarted += () =>
            {
                if (playerCreated == false)
                {
                    CreatePlayer();
                }
            };
        }
    }

    [Button("创建玩家")]
    public static (PlayerController leftPlayerCtrl, PlayerController rightPlayerCtrl) CreatePlayer()
    {
        var leftPlayer = Player.Create<Player>(GameSetting.stageGeneralSetting.leftPlayerID);

        var leftPlayerCtrl = EntityManager.Create(leftPlayer, 
            instance.leftPlayerSpawnPosition);

        var rightPlayer = Player.Create<Player>(GameSetting.stageGeneralSetting.rightPlayerID);

        var rightPlayerCtrl = EntityManager.Create(rightPlayer, 
            instance.rightPlayerSpawnPosition);

        playerCreated = true;

        PlayerManager.leftPlayerCtrl = leftPlayerCtrl as PlayerController;

        PlayerManager.rightPlayerCtrl = rightPlayerCtrl as PlayerController;

        OnPlayerCreatedEvent?.Invoke((PlayerManager.leftPlayerCtrl,
            PlayerManager.rightPlayerCtrl));

        return (PlayerManager.leftPlayerCtrl, PlayerManager.rightPlayerCtrl);
    }

    [Button("重置所有玩家")]
    public static void ResetAllPlayers()
    {
        if (playerCreated)
        {
            leftPlayerCtrl.transform.position = instance.leftPlayerSpawnPosition;
            rightPlayerCtrl.transform.position = instance.rightPlayerSpawnPosition;

            EnablePlayerMovement();

            leftPlayerCtrl.gameObject.SetActive(true);
            rightPlayerCtrl.gameObject.SetActive(true);
        }
    }

    [Button("禁用玩家移动")]
    public static void DisablePlayerMovement()
    {
        leftPlayerCtrl.movementController.enableMovement = false;
        rightPlayerCtrl.movementController.enableMovement = false;
    }

    [Button("启用玩家移动")]
    public static void EnablePlayerMovement()
    {
        leftPlayerCtrl.movementController.enableMovement = true;
        rightPlayerCtrl.movementController.enableMovement = true;
    }
}
