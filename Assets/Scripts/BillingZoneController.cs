using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillingZoneController : MonoBehaviour
{
    public static BillingZoneController instance;

    //Flag - Pollito sujetado emn zona de venta
    private bool bChickenDraggedInSaleZone;

    //Referencia a la Gallina que se va a vender
    private ChickenStats chickenForSale;

    // COMPONENTES
    private Animator mAnimator;

    //----------------------------------------------------------
    void Awake()
    {
        //Asignamos la instancia de la BillingZone
        instance = this;

        //Iniciamos flag de Pollitoen Venta en Falso
        bChickenDraggedInSaleZone = false;

        // Iniciamos sin ninguna referencia de gallina en venta
        chickenForSale = null;

        //Obtenemos referencia a componentes
        mAnimator = GetComponent<Animator>();
    }

    //---------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el objeto que entra en la Zona es una Gallina
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si la gallina esta siendo sujetada...
            if (collision.gameObject.GetComponent<Draggable>().bIsBeingDragged)
            {
                //Activamos flag de "Gallina en zona de venta"
                bChickenDraggedInSaleZone = true;

                //Almacenamos referencia a los stats de la Gallina
                chickenForSale = collision.gameObject.GetComponent<ChickenStats>();

                //Reproducimos Animacion de Hover
                mAnimator.SetBool("Hover", true);

                Debug.Log("Gallina en zona de Cash");
            }
        }
    }

    //-----------------------------------------------------------------------------------

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Si el objeto que sale en la Zona es una Gallina
        if (collision.gameObject.CompareTag("Chicken"))
        {
            //Si la gallina estaba siendo sujetada...
            if (collision.gameObject.GetComponent<Draggable>().bIsBeingDragged)
            {
                //Desactivamos flag de "Gallina en zona de venta"
                bChickenDraggedInSaleZone = false;

                //Eliminamos referencia a Gallina
                chickenForSale = null;

                //Desactivamos el parametro de animacion de Hover
                mAnimator.SetBool("Hover", false);

                Debug.Log("Gallina salió de zona de Cash");
            }
        }
    }

    //-----------------------------------------------------------------------------------

    public void TryToSellChicken()
    {
        Debug.Log("Jugador solto el click");

        //Si el flag de "Gallina en zona de venta" est activo, y se tiene referencia a ella
        if (bChickenDraggedInSaleZone || chickenForSale != null)
        {
            //Decimos al GameManager que dispare el evento de Pollo vendido
            GameManager.Instance.TriggerEvent_ChickenSold(20);

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
