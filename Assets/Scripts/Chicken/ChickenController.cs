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
    [HideInInspector] public bool isBeingDragged = false;

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

    [Header("Chicken UI")]
    [SerializeField] private ChickenUI chickenUI;

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

        //Iniciamos Flags
        isAlive = true;
        isBeingDragged = false;
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
            //Controlamos la Animacion de Caminata
            mSpritesController.ManageWalkingAnim();

            //Controlamos la Animacion de Hambre
            mSpritesController.ManageStarvingAnim();

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

            //Si el Pollito esta haciendo alguna de estas acciones...
            if (mChickenStats.eatingFlag || mChickenStats.drinkingFlag || mChickenStats.sleepingFlag || mChickenStats.fightingFlag)
            {
                //Si el Stat de hambre esta por debajo de 40
                if (mChickenStats.hambre < 40)
                {
                    //Desactivamos los Flags de Comiendo y Bebiendo
                    mChickenStats.eatingFlag = false;
                    mChickenStats.drinkingFlag = false;
                }
            }
            else
            {
                //Seteamos una direccion de Movimiento
                mSelfMovementToTarget.SetMovementDirection();
            }

            //Si el HP del pollito llega  0
            if (mChickenStats.hp == 0)
            {
                // Desactivamos Flag de "esta vivo"
                isAlive = false;

                //Su probabilidad de escape del Drag es minima (0)
                dragEscapeProb = dragEscapeMinProb;

                //Reproducimos las Acciones de Muerte.
                Die();
            }
        }

    }

    private void FixedUpdate()
    {
        //Si el Pollito esta vivo...
        if (isAlive)
        {
            //Si el Pollito está comiendo, Bebiendo, Durmiendo, o Peleando
            if (mChickenStats.eatingFlag || mChickenStats.drinkingFlag || mChickenStats.sleepingFlag || mChickenStats.fightingFlag)
            {
                //Hacemos que deje de moverse (Velocidad a 0)
                mSelfMovementToTarget.StopMoving();
            }

            //En caso no este haciendo alguna accion en Particular...
            else
            {
                //Hacemos que el Pollito de Mueva hacia su Target de Movimiento
                //(aplica tanto randomWaypoint como Target)
                mSelfMovementToTarget.MoveToTarget();

                //Revisamos si es que necesita un nuevo RandomWaypoint (ya llegó al anterior)
                mSelfMovementToTarget.CheckIfNeedNewRandomWaypoint();
            }
        }

        //Si el Pollito esta muerto...
        else
        {
            //Hacemos que deje de moverse (Velocidad a 0)
            mSelfMovementToTarget.StopMoving();
        }
    }

    //------------------------------------------------------------------------------

    private void OnMouseOver()
    {
        //Controlamos la animacion de cuando se hace Hover
        mSpritesController.ManageHoverAnimation();

        //Si el Pollito esta vivo...
        if (isAlive)
        {
            //Si No esta siendo arrastrado
            if (!isBeingDragged)
            {
                //Mostramos la info del UI del pollito
                chickenUI.ShowChickenInfo();
            }
        }
        
    }

    private void OnMouseExit()
    {
        //Controlamos la animacion de cuando se hace Hover
        mSpritesController.ManageUnhoverAnimation();

        //Si el Pollito esta vivo...
        if (isAlive)
        {
            //Si la UI esta activa...
            if (chickenUI.gameObject.activeSelf)
            {
                //Ocultamos la info del UI
                chickenUI.HideChickenInfo();
            }
        }
        
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

            //Activamos Flag de "esta siendo arrastrado"
            isBeingDragged = true;

            //Controlamos la Animacion de Drag
            mSpritesController.ManageDragAnimation();

            
        }
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Mientras se mantenga el Mouse oprimido y se detecta el arrastre...

    private void OnMouseDrag()
    {
        //Si la UI del pollito esta activa...
        if (chickenUI.gameObject.activeSelf)
        {
            //Ocultamos su informacion...
            chickenUI.HideChickenInfo();
        }


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

        //Controlamos la animacion de soltar
        mSpritesController.ManageUndragAnimation();

        //Desactivamos flag de "esta siendo agarrado"
        isBeingDragged = false;

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
        //Si el pollito esta vivo...
        if (isAlive)
        {
            //Si estams colisionando con otro Pollito...
            if (collision.gameObject.CompareTag("Chicken"))
            {

            }

            //Si chocamos con un contenedor de Comida o Agua
            else if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Water"))
            {

            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Water"))
        {

        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isAlive)
        {
            //Si ha dejado de chocar con otro pollito
            if (collision.gameObject.CompareTag("Chicken"))
            {

            }
        }
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

    //-----------------------------------------------------------------------------------

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

        //Desactivamos la UI del Pollito
        chickenUI.gameObject.SetActive(false);
    }
}
