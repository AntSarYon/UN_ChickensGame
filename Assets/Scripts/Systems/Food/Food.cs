using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Food : Interactable
{

    [Header("Slider de comida")]
    public Slider mFoodLevelSlider;

    [Header("Corral al que pertenece")]
    public Yard yard;

    [Header("Velocidad de variacion de comida")]
    [Range(1, 5)][SerializeField] private float foodDecreaseSpeed;

    //Lista de GameObjects (Pollos) que estan chocando con la Comida
    private List<GameObject> chickensList = new List<GameObject>();

    [Header("Slots para Pollito")]
    [SerializeField] private List<Transform> slotsList = new List<Transform>();

    public bool bFullSlots;

    //Referencia a Componentes
    private Collider2D mCollider;
    private Animator mAnimator;

    //--------------------------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mCollider = GetComponent<Collider2D>();
        mAnimator = GetComponent<Animator>();

        interactionMessage = "Llenar";

        // Flag, Slots llenos
        bFullSlots = false;
    }

    //--------------------------------------------------------------------------------------

    void Start()
    {
        //Traemos los parametros del RulesManager
        foodDecreaseSpeed = GameRulesManager.instance.foodDecreaseSpeed;
    }

    //--------------------------------------------------------------------------------------

    void Update()
    {
        //Si hay al menos 1 pollo consumiendo comida
        if (chickensList.Count > 0)
        {
            /*// Inicializamos contador de Pollitos que SI ESTAN COMIENDO
            // (Puede haber pollitos que estan chocando, pero que no estan comiendo)
            int eatingChicks = 0;

            //Por cada pollito que este chocando
            foreach (GameObject chick in chickensList)
            {
                // Si su flag de "Comiendo" esta activa
                if (chick.GetComponent<ChickenController>().bIsEating)
                {
                    // Se incrementa el contador de pollitos comiendo
                    eatingChicks++;
                }
            }*/

            //Reducimos el valor del Slider, en base a cuantos pollitos estan comiendo, y a la velocidad definida
            //mFoodLevelSlider.value -= eatingChicks * Time.deltaTime * foodDecreaseSpeed;
        }
    }

    //--------------------------------------------------------------------------------------

    private new void OnTriggerEnter(Collider other)
    {
        //Si el polito entra en la zona...
        if (other.CompareTag("Chicken"))
        {
            //Si aun hay menos de 5 pollitos en el comedero
            if (chickensList.Count < 5)
            {
                //Lo Agregamos a la Lista
                chickensList.Add(other.gameObject);

                //Activamos su flag de "Comiendo"
                other.GetComponent<ChickenController>().bIsWalking = false;
                other.GetComponent<ChickenController>().bIsEating = true;

                //Obtenemos el indice de Slot que le corresponde a este pollito 
                int slotIndex = chickensList.Count - 1;

                // Posicionamos a pollito en el Slot
                other.transform.position = slotsList[slotIndex].position;

                Debug.Log("Se ha agregado el Pollito al Slot");

                //Si se ha llegado al limite de pollos (5) activaoms flag de FULL
                if(chickensList.Count == 5) bFullSlots = true;
            }
            else
            {
                //Mantenemos desactivado su flag de "Comiendo"
                other.GetComponent<ChickenController>().bIsEating = false;
            }

        }
    }

    //--------------------------------------------------------------------------------------

    private void OnCollisionExit(Collision collision)
    {
        //Si el objeto con el que deja de colisionar es un Pollo...
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si el pollo estaba en la lista...
            if (chickensList.Contains(collision.gameObject))
            {
                //Lo quitamos de la Lista
                chickensList.Remove(collision.gameObject);
            }
        }
    }

    // ------------------------------------------------------------

    public void Refill()
    {
        //Reproducimos Animacion
        mAnimator.Play("refill");

        //Llevamnos el valor del Slider al Maximo
        mFoodLevelSlider.value = mFoodLevelSlider.maxValue;
    }

    public void Denegate()
    {
        //Reproducimos Animacion
        mAnimator.Play("denegate");
    }

    // ------------------------------------------------------------

    public override void Interact(Transform holdingZone)
    {
        if (holdingZone.childCount > 0)
        {
            // Si la zona de Agarre tiene una FoodBag como hijo...
            if (holdingZone.GetChild(0).CompareTag("FoodBag"))
            {
                // Obtenemos referencia al Objeto sujetado
                FoodBag foodBag = holdingZone.GetChild(0).GetComponent<FoodBag>();

                //Hacemos un Refill
                Refill();

                //Dropeamos la bolsa
                foodBag.Drop();

                // La desactivamos para que vuelva al Pool
                foodBag.gameObject.SetActive(false);
            }
            else Denegate();
        }

        // Si no tiene nada, o el objeto no es una bolsa de comida
        else Denegate();

    }

    //--------------------------------------------------------------------------------------

}
