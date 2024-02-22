using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using UnityEngine;

public class AnimateHitPoints : MonoBehaviour
{
    [SerializeField]
    private float originalScale = 1f;
    
    public void AnimHitPoints()
    {
        
        LeanTween.scaleX(gameObject, 1.5f, .3f).setEaseInBounce().setOnComplete(() => { ReturnToSize(); });
        LeanTween.scaleY(gameObject, 1.5f, .3f).setEaseInBounce();

        
    }

    private void ReturnToSize()
    {
        Debug.Log("ReturnToSize");
        LeanTween.scaleX(gameObject, 1, 0).setEaseInBounce();
        LeanTween.scaleY(gameObject, 1, 0).setEaseInBounce();
    }

}
