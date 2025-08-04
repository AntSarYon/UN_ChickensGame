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

    [HideInInspector] public bool isAlive = true;

    //Flag - esta en zona de venta
    private bool bInBillingZone;

    //Probabilidad de que escape del Drag
    [Header("Probabilidad de que la Gallina escape")]
    [Range(0.00f,1.00f)] public float dragEscapeProb;
    private float dragEscapedefaultProb;
    private float dragEscapeMinProb = 0;

    // COMPONENTES
    private ChickenStats mChickenStats;
    private SpritesController mSpritesController;
    private Draggable mDraggable;
    private SelfMovementToTarget mSelfMovementToTarget;

    private AudioSource mAudioSource;

    // Clips de Audio del Pollito:
    [Header("Clips de Audio")]
    [SerializeField] private AudioClip clipDragged;
    [SerializeField] private AudioClip clipEscaped;
    [SerializeField] private AudioClip clipWings;


    //-----------------------------------------------------------------------------

    void Awake()
    {
        // Referencia a Componentes
        mSpritesController = GetComponent<SpritesController>();
        mDraggable = GetComponent<Draggable>();
        mChickenStats = GetComponent<ChickenStats>();
        mSelfMovementToTarget = GetComponent<SelfMovementToTarget>();

        mAudioSource = GetComponent<AudioSource>();

        //Almacenamos la probabilidad seteada de escape
        dragEscapedefaultProb = dragEscapeProb;
        dragEscapeMinProb = 0;
    }

    //-----------------------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        //Definimos un nuevo destino aleatorio para la gallina
        mSelfMovementToTarget.SetNewRandomWaypoint();

        //Agregamos Funcion Delegado al Evento de Pollo vendido
        GameManager.Instance.OnChickenSold += OnChickenSoldDelegate;
    }

    //-----------------------------------------------------------------------------

    private void OnChickenSoldDelegate(float chickenPrice)
    {
        //Hacemos que las gallinas se muevan mas rapido por 2 segundos
        //como consecuencia de ver morir a su amigo
        mSelfMovementToTarget.MultiplySpeedTemporary(2);
    }

    // Update is called once per frame
    void Update()
    {
        //Si el pollito sigue vivo...
        if (isAlive)
        {
            //Si el HP del pollito llega  0
            if (mChickenStats.hp == 0)
            {
                // Desactivamos Flag de "esta vivo"
                isAlive = false;

                //Reproducimos las Acciones de Muerte.
                Die();
            }

            //Si el pollito esta peleando
            if (mChickenStats.fightingFlag)
            {
                //Su probabilidad de escape del Drag e sminima (0)
                dragEscapeProb = dragEscapeMinProb;
            }
            //Si no esta peleand
            else
            {
                //Regresamos la Probabilidad a la normalidad
                dragEscapeProb = dragEscapedefaultProb;
            }

        }

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
        //Obtenemos un valor random
        float randomProb = Random.Range(0.00f, 1.00f);

        //Si la probabilidad random es menor al margen definido
        if (randomProb <= dragEscapeProb)
        {
            //Escapa

            //Reproducimos sonido de Alas
            mAudioSource.PlayOneShot(clipWings, 0.50f);

            //Reproducimos sonido de Escape
            mAudioSource.PlayOneShot(clipEscaped, 0.60f);

            //Multiplicamos la velocidad por 1.5 segundos...
            mSelfMovementToTarget.MultiplySpeedTemporary(0.75f);

        }
        //Caso contrario
        else
        {
            //Reproducimos sonido de Agarre
            mAudioSource.PlayOneShot(clipDragged, 0.25f);

            //Agarramos a la gallina
            mDraggable.Catch();
        }
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mientras se mantenga el Mouse oprimido y se detecta el arrastre...

    private void OnMouseDrag()
    {
        //Si el flag de "Esta siendo Arrastrado" esta activo
        if (mDraggable.bIsBeingDragged)
        {
            //Movemos la Posicion del pollo
            mDraggable.MovePosition();
        }
        
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

    public void Die()
    {
        //Seteamos que la probabilidad de ecape este al 0%
        dragEscapeProb = dragEscapeMinProb;

        //Llamamos al Evento de Pollito muerto
        GameManager.Instance.TriggerEvent_OnChickenDeath();

        //Hacemos que se reproduzca el Sonido de Pollito muerto
        GameSoundsController.Instance.PlayChickenDeathSound();

        //Reproducimos la Animacion de Muerte
        mSpritesController.PlayDeath();
    }
}
