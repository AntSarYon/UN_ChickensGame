using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CashUIController : MonoBehaviour
{
    //Texto de Dinero en cuenta...
    [SerializeField] private TextMeshProUGUI txtCashAmount;

    //COMPONENTES
    private Animator mAnimator;

    //------------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencias a componentes
        mAnimator = GetComponent<Animator>();
    }

    //------------------------------------------------------------------------

    void Start()
    {
        GameManager.Instance.OnChickenSold += OnChickenSoldDelegate;
        GameManager.Instance.OnFoodRefill += OnFoodRefillDelegate;

        //Traemos la cantidad de Dinero disponible
        UpdateCashAmount();
    }

    //------------------------------------------------------------------------

    private void OnChickenSoldDelegate(int chickenPrice)
    {
        mAnimator.Play("increase");
    }

    private void OnFoodRefillDelegate()
    {
        mAnimator.Play("reduce");
    }

    //------------------------------------------------------------------------

    public void UpdateCashAmount()
    {
        //Traemos el valor del Cash desde el GameManager
        txtCashAmount.text = GameManager.Instance.currentCash.ToString();
    }

}
