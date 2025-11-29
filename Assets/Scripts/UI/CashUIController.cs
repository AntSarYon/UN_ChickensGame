using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashUIController : MonoBehaviour
{
    public static CashUIController instance;

    //Texto de Dinero en cuenta...
    [SerializeField] private TextMeshProUGUI txtCashAmount;

    //COMPONENTES
    private Animator mAnimator;

    //------------------------------------------------------------------------

    void Awake()
    {
        // Asignamos instancia de UI de Cash
        instance = this;

        //Obtenemos referencias a componentes
        mAnimator = GetComponent<Animator>();
    }

    //------------------------------------------------------------------------

    void Start()
    {
        //Traemos la cantidad de Dinero disponible
        UpdateCashAmount();
    }

    void Update()
    {
        UpdateCashAmount();
    }

    //------------------------------------------------------------------------

    public void PlayIncreaseCash()
    {
        mAnimator.Play("increase");
    }

    public void PlayReduceCash()
    {
        mAnimator.Play("reduce");
    }

    //------------------------------------------------------------------------

    public void UpdateCashAmount()
    {
        //Traemos el valor del Cash desde el GameManager
        txtCashAmount.text = Mathf.Round(DayStatusManager.Instance.currentCash).ToString();
    }

}
