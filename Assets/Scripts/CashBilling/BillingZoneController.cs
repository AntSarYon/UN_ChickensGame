using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillingZoneController : MonoBehaviour
{
    //Flag - Pollito sujetado emn zona de venta
    private bool bChickenDraggedInSaleZone;

    // Referencia a Corral en que se encuentra la Zona de Venta
    [SerializeField] private Yard yard;

    //Referencia a la Gallina que se va a vender
    private ChickenStats chickenForSale;

    // COMPONENTES
    private Animator mAnimator;

    //----------------------------------------------------------
    void Awake()
    {
        //Iniciamos flag de Pollitoen Venta en Falso
        bChickenDraggedInSaleZone = false;

        // Iniciamos sin ninguna referencia de gallina en venta
        chickenForSale = null;

        //Obtenemos referencia a componentes
        mAnimator = GetComponent<Animator>();
    }

    // --------------------------------------------------------------------------

    void Update()
    {
        // Si se tiene una referencia a un Pollito
        if (chickenForSale != null)
        {
            // Si es pollito es soltado, y aun esta dentro...
            if (!chickenForSale.GetComponent<ChickenController>().isBeingDragged)
            {
                TryToSellChicken();
            }
        }
    }

    //---------------------------------------------------------------------------

    private void OnTriggerEnter(Collider collision)
    {
        //Si el objeto que entra en la Zona es una Gallina
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si la gallina esta siendo sujetada...
            if (collision.gameObject.GetComponent<ChickenController>().isBeingDragged)
            {
                //Activamos flag de "Gallina en zona de venta"
                bChickenDraggedInSaleZone = true;

                //Almacenamos referencia a los stats de la Gallina
                chickenForSale = collision.gameObject.GetComponent<ChickenStats>();

                //Reproducimos Animacion de Hover
                mAnimator.SetBool("Hover", true);
            }
        }
    }

    //-----------------------------------------------------------------------------------

    private void OnTriggerExit(Collider collision)
    {
        //Si el objeto que sale en la Zona es una Gallina
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si la gallina estaba siendo sujetada...
            if (collision.gameObject.GetComponent<ChickenController>().isBeingDragged)
            {
                //Desactivamos flag de "Gallina en zona de venta"
                bChickenDraggedInSaleZone = false;

                //Eliminamos referencia a Gallina
                chickenForSale = null;

                //Desactivamos el parametro de animacion de Hover
                mAnimator.SetBool("Hover", false);
            }
        }
    }

    //-----------------------------------------------------------------------------------

    public void TryToSellChicken()
    {
        //Si el flag de "Gallina en zona de venta" esta activo, y se tiene referencia a ella
        if (bChickenDraggedInSaleZone || chickenForSale != null)
        {
            //Decimos al GameManager que dispare el evento de Pollo vendido

            //Si el Pollo en cuestion esta vivo...
            if (chickenForSale.GetComponent<ChickenController>().isAlive)
            {
                //Lo vendemos en base a su Peso
                DayStatusManager.Instance.TriggerEvent_ChickenSold(chickenForSale.peso);
            }
            //Si el Pollo esta muerto...
            else
            {
                //Lo vendemos con un valor de 0
                DayStatusManager.Instance.TriggerEvent_ChickenSold(0);
            }

            //Hacemos que el Manager de Sonidos reprodzca el sonido de Venta
            GameSoundsController.Instance.PlayChickenSoldSound();

            //Desactivamos el parametro de animacion de Hover
            mAnimator.SetBool("Hover", false);

            //Desactivamos a la Gallina
            chickenForSale.gameObject.SetActive(false);

            //Dejamos en vacio la referencia de Gallina en Venta
            chickenForSale = null;
        }
    }
}
