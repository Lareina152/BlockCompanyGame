using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManger : Singleton<TransitionManger>
{
    public CanvasGroup fadeCanvasGroup;
    public float fadeDuration;
    private bool isFade;

    [SerializeField]
    private string initialSceneName;

    public void Transition(string from, string to)
    {
        if (!isFade) //遮挡了
            StartCoroutine(TransitionToScene(from, to));
    }

    protected override void Awake()
    {
        base.Awake();

        InitScene(initialSceneName);
    }

    [Button]
    private void InitScene(string initialSceneName)
    {
        SceneManager.LoadSceneAsync(initialSceneName, LoadSceneMode.Additive);
    }

    private IEnumerator TransitionToScene(string from, string to)
    {
        yield return Fade(1); //变黑

        yield return SceneManager.UnloadSceneAsync(from);
        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newScene);
        yield return Fade(0); //变白

    }

    //淡入淡出场景 1是黑，0是透明
    private IEnumerator Fade(float targetAlpha)
    {
        isFade = true; //没有遮挡
        fadeCanvasGroup.blocksRaycasts = true; //鼠标点击有效
        float speed = Mathf.Abs(fadeCanvasGroup.alpha - targetAlpha) / fadeDuration;
        while (!Mathf.Approximately(fadeCanvasGroup.alpha, targetAlpha))
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        fadeCanvasGroup.blocksRaycasts = false; //鼠标点击无效
        isFade = false; //遮挡了
    }
}

