using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{

    [Header("Slider de comida")]
    public Slider mFoodLevelSlider;

    [Header("Corral al que pertenece")]
    public Yard yard;

    [Header("Velocidad de variacion de comida")]
    [Range(1,5)] [SerializeField] private float foodDecreaseSpeed;

    //Lista de GameObjects (Pollos) que estan chocando con la Comida
    private List<GameObject> chickensList = new List<GameObject>();

    //Referencia a Componentes
    private Collider2D mCollider;
    private Animator mAnimator;

    //--------------------------------------------------------------------------------------

    void Awake()
    {
        //Obtenemos referencia a componentes
        mCollider = GetComponent<Collider2D>();
        mAnimator = GetComponent<Animator>();
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
            // Inicializamos contador de Pollitos que SI ESTAN COMIENDO
            // (Puede haber pollitos que estan chocando, pero que no estan comiendo)
            int eatingChicks = 0;

            //Por cada pollito que este chocando
            foreach(GameObject chick in chickensList)
            {
                // Si su flag de "Comiendo" esta activa
                if (chick.GetComponent<ChickenController>().eatingFlag)
                {
                    // Se incrementa el contador de pollitos comiendo
                    eatingChicks++;
                }
            }

            //Reducimos el valor del Slider, en base a cuantos pollitos estan comiendo, y a la velocidad definida
            mFoodLevelSlider.value -= eatingChicks * Time.deltaTime * foodDecreaseSpeed;
        }
    }

    //--------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        //Si el objeto con el que ha empezado la colision es un Pollo...
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Agregamos el Pollo a la Lista
            chickensList.Add(collision.gameObject);
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

    //--------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        //Si el triggerpertenece a la zona de interaccion del jugador...
        if (other.CompareTag("PlayerInteractionZone"))
        {
            //Obtenemos el PickupController del PlayerBody (Padre del Triger)
            //para asignarle que este será el Objeto a coger.
            other.GetComponentInParent<PickUpController>().targetObject = this.gameObject;

            // Mostramos el Toggle con ingredientes
            //ShowIngredientsInfo();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Si el Triger del que salimos es la zona de interacción
        if (other.tag == "PlayerInteractionZone")
        {
            //Si la ultima referencia que tenia la zona era la de este objeto...
            if (other.GetComponentInParent<PickUpController>().targetObject == this.gameObject)
            {
                //Obtenemos el PickupController del Player (Padre del Triger)
                //para indicar que ya no habrá ningun Objeto Asignado.
                other.GetComponentInParent<PickUpController>().targetObject = null;
            }
        }
    }
}
