using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //COMPONENTES
    private Animator mAnimator;

    // ------------------------------------------------------------------------

    void Awake()
    {
        //Obteneomps referencia a componentes
        mAnimator = GetComponent<Animator>();
    }

    // ------------------------------------------------------------------------

    void Start()
    {
        //Asignamos Delegado al Evento de Pollo Vendido
        GameManager.Instance.OnChickenSold += OnChickenSoldDelegate;
    }

    // ------------------------------------------------------------------------

    private void OnChickenSoldDelegate(int chickenValue)
    {
        //Reproducimos Animacion de CashoutShake
        mAnimator.Play("CashoutShake");
    }

    // ------------------------------------------------------------------------
}
