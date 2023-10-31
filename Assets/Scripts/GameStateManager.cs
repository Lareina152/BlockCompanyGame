using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameStateManager : UniqueMonoBehaviour<GameStateManager>, IManagerBehaviour
{
    public static bool gameStarted { get; private set; }

    public static event Action OnGameStarted;

    public static event Action OnGameEnded;

    [SerializeField]
    private bool autoStart = false;

    void IManagerBehaviour.OnEnterProcedure(string procedureID)
    {
        if (procedureID == MainMenuProcedure.registeredID)
        {
            if (autoStart)
            {
                StartGame();
            }
        }
    }

    [Button("开始游戏")]
    public static void StartGame()
    {
        gameStarted = true;
        OnGameStarted?.Invoke();
    }

    [Button("结束游戏")]
    public static void EndGame()
    {
        OnGameEnded?.Invoke();
        gameStarted = false;
    }

    [Button("重新开始游戏")]
    public static void RestartGame()
    {
        EndGame();
        StartGame();
    }
}
