using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
   public string sceneForm;
    public string sceneToGo;

    public void TeleportToScene()
    {
        TransitionManger.Instance.Transition(sceneForm, sceneToGo);
    }
}
