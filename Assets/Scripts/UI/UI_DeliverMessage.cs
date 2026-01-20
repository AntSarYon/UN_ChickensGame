using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_DeliverMessage : MonoBehaviour
{
    private Animator mAnimator;

    [SerializeField] private AudioClip clipAppear;

    // ------------------------------------------------------

    void Awake()
    {
        mAnimator = GetComponent<Animator>();
    }

    // -------------------------------------------------------

    public void ShowMessage()
    {
        //Disparamos Animacion de aparecer
        mAnimator.Play("appear");

        // Reproducimos sonido de Aparicion
        AudioManager.Instance.PlaySFX(clipAppear);
    }
}
