using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSkipController : MonoBehaviour
{
    public string sceneToLoad;

    public float delayInSeconds = 5f;

    private void Start()
    {
        Invoke(nameof(LoadScene), delayInSeconds);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
