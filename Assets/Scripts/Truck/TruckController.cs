using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TruckController : MonoBehaviour
{
    public static TruckController Instance;
    
    // Flag "Llegando"
    [HideInInspector] public bool bIsArriving;

    // Cantidad de Pollitos en el Camion
    private int currentChickensInTruck = 0;
    private int targetChickens = 0;

    [Header("Contador")]
    [SerializeField] private TextMeshProUGUI txtCounter;

    [Header("Clips de Audio")]
    [SerializeField] private AudioClip clipArriving;
    [SerializeField] private AudioClip clipLeaving;

    private Animator mAnimator;

    // ------------------------------------------
    void Awake()
    {
        Instance = this;

        // Iniciaizamos Contadores de Pollitos
        currentChickensInTruck = 0;
        targetChickens = 3;

        //Flag de "Llegando" inicia en falso
        bIsArriving = false;

        mAnimator = GetComponent<Animator>();
    }

    // ------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        mAnimator.Play("hide");

        UpdateCounter();
    }

    // -----------------------------------------

    public void PlayArrive()
    {
        bIsArriving = true;
        mAnimator.Play("arrive");

        AudioManager.Instance.PlaySFX(clipArriving);
    }

    public void PlayRun()
    {
        bIsArriving = false;
        mAnimator.Play("Run");

        AudioManager.Instance.PlaySFX(clipLeaving);

        //Llamamos a la evaluacion de Pollitos tras 5 segundos
        Invoke(nameof(EvaluateChickens), 4.5f);
    }

    // ----------------------------------------

    public void SellChicken(ChickenController chicken)
    {
        // Obtenemos los Stats del Pollo
        ChickenStats chkStats = chicken.GetComponent<ChickenStats>();

        // Vendemos el pollo mandando su stat de Peso
        DayStatusManager.Instance.TriggerEvent_ChickenSold(chkStats.peso);

        //Aumentamos la cantidad de Pollitos en el camion
        currentChickensInTruck++;

        // Actualizamos el Counter visual
        UpdateCounter();

        // Desactivamos el Pollito
        chicken.gameObject.SetActive(false);
    }

    public void RejectChicken(ChickenController chicken)
    {
        GameSoundsController.Instance.PlayHitSound();

        //chicken.gameObject.SetActive(false);
    }

    // -----------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        //Si el objeto con el que se colisiono es un pollo
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Validamos que tiene su componente de chicken controller
            ChickenController chicken = collision.gameObject.GetComponent<ChickenController>();
            if (chicken)
            {
                //Si el polito esta vivo
                if (chicken.bIsAlive)
                {
                    //Vendemos el Pollito
                    SellChicken(chicken);
                }
                else
                {
                    // Rechazamos el Pollo
                    RejectChicken(chicken);
                }
                
            }
        }
    }

    // -----------------------------------------------------------------

    private void UpdateCounter()
    {
        // Actualizamos el Texto de Counter del camion
        txtCounter.text = $"{currentChickensInTruck} / {targetChickens}";
    }

    // ------------------------------------------------------------------
    // FUNNCION: EVALLUACION DE POLLOS AL FINAL DE LA CAMPANA / DIA
    public void EvaluateChickens()
    {
        // Si hay menos pollos VIVOS que el mínimo requerido -> Trigger Game Over
        if (currentChickensInTruck < targetChickens)
        {
            DayStatusManager.Instance.bGameOver = true;
            DayStatusManager.Instance.TriggerEvent_GameOver();
        }
    }
}
