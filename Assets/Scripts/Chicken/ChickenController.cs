using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    //Flags de Estado del Pollito
    [HideInInspector] public bool eatingFlag = false;
    [HideInInspector] public bool drinkingFlag = false;
    [HideInInspector] public bool stressfulFlag = false;
    [HideInInspector] public bool fightingFlag = false;
    [HideInInspector] public bool sleepingFlag = false;

    //Flag - esta en zona de venta
    private bool bInBillingZone;

    // COMPONENTES
    private ChickenStats mChickenStats;
    private SpritesController mSpritesController;
    private Draggable mDraggable;
    private SelfMovementToTarget mSelfMovementToTarget;


    //-----------------------------------------------------------------------------

    void Awake()
    {
        // Referencia a Componentes
        mSpritesController = GetComponent<SpritesController>();
        mDraggable = GetComponent<Draggable>();
        mChickenStats = GetComponent<ChickenStats>();
        mSelfMovementToTarget = GetComponent<SelfMovementToTarget>();
    }

    //-----------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        //Definimos un nuevo destino aleatorio para la gallina
        mSelfMovementToTarget.SetNewRandomWaypoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    //------------------------------------------------------------------------------

    private void OnMouseOver()
    {
        
    }

    private void OnMouseExit()
    {
        
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Al hacer Click sobre el Objeto (con collider)

    private void OnMouseDown()
    {
        //Agarramos a la gallina
        mDraggable.Catch();
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mientras se mantenga el Mouse oprimido y se detecta el arrastre...

    private void OnMouseDrag()
    {
        //Movemos la Posicion del pollo
        mDraggable.MovePosition();
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Cuando soltamos ewl Click

    private void OnMouseUp()
    {
        //Soltamos la pollito
        mDraggable.Drop();

        //Si esta en Zona de Venta...
        if (bInBillingZone)
        {
            //Decimos al BillingZone del nivel que Trate de vendernos
            BillingZoneController.instance.TryToSellChicken();
        }
    }

    //------------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        
    }

    //------------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el Pollito ha entrado en la zona de Venta...
        if (collision.gameObject.CompareTag("BillingZone"))
        {
            //Activamos flag de "en zona de venta"
            bInBillingZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Si el Pollito ha salido de la zona de Venta...
        if (collision.gameObject.CompareTag("BillingZone"))
        {
            //Desactivamos flag de "en zona de venta"
            bInBillingZone = false;
        }
    }
}
