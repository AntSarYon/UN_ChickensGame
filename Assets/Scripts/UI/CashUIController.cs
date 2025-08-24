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
        DayStatusManager.Instance.OnChickenSold += OnChickenSoldDelegate;
        DayStatusManager.Instance.OnFoodRefill += OnFoodRefillDelegate;
        DayStatusManager.Instance.OnGasRefill += OnGasRefillDelegate;
        DayStatusManager.Instance.OnGenerateNewChicken += OnGenerateNewChickenDelegate;

        //Traemos la cantidad de Dinero disponible
        UpdateCashAmount();
    }

    private void OnGenerateNewChickenDelegate()
    {
        mAnimator.Play("reduce");
    }

    void Update()
    {
        UpdateCashAmount();
    }

    //------------------------------------------------------------------------

    private void OnGasRefillDelegate()
    {
        mAnimator.Play("reduce");
    }

    //------------------------------------------------------------------------

    private void OnChickenSoldDelegate(float chickenPrice)
    {
        mAnimator.Play("increase");
    }

    public void OnFoodRefillDelegate()
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
