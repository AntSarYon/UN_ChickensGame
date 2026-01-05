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
        //Asignamos Delegados a Eventos

        // Evento de Pollo Vendido
        DayStatusManager.Instance.OnChickenSold += OnChickenSoldDelegate;

    }

    // ------------------------------------------------------------------------

    private void OnChickenSoldDelegate(float chickenValue)
    {
        //Reproducimos Animacion de CashoutShake
        mAnimator.Play("CashoutShake");
    }

}
