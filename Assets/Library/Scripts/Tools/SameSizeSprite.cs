using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Basis;

public class SameSizeSprite : MonoBehaviour
{
    public SpriteRenderer target;

    void OnValidate() {
        if (target != null) {
            Vector3 targetSize = Vector3.Scale(target.transform.localScale, target.sprite.bounds.size);
            transform.localScale = targetSize.Divide(GetComponent<SpriteRenderer>().sprite.bounds.size);
        }
    }
}
