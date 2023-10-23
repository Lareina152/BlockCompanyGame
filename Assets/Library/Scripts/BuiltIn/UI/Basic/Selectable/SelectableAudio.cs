using System.Collections;
using System.Collections.Generic;
using Basis;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class SelectableAudio : MonoBehaviour, 
    IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [LabelText("按下播放")]
    public AudioClip audioOnPointerDown;
    [LabelText("松开播放")]
    public AudioClip audioOnPointerUp;

    [LabelText("鼠标进入播放")]
    public AudioClip audioOnPointerEnter;
    [LabelText("鼠标离开播放")]
    public AudioClip audioOnPointerExit;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (audioOnPointerDown != null)
        {
            audioSource.Stop();
            audioSource.clip = audioOnPointerDown;
            audioSource.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (audioOnPointerUp != null)
        {
            audioSource.Stop();
            audioSource.clip = audioOnPointerUp;
            audioSource.Play();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioOnPointerEnter != null)
        {
            audioSource.Stop();
            audioSource.clip = audioOnPointerEnter;
            audioSource.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (audioOnPointerExit != null)
        {
            audioSource.Stop();
            audioSource.clip = audioOnPointerExit;
            audioSource.Play();
        }
    }
}
