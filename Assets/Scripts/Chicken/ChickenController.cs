using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    //Flags de Estado del Pollito
    [HideInInspector] public bool eatingFlag = false;
    [HideInInspector] public bool starvingFlag = false;
    [HideInInspector] public bool drinkingFlag = false;
    [HideInInspector] public bool stressfulFlag = false;
    [HideInInspector] public bool fightingFlag = false;
    [HideInInspector] public bool sleepingFlag = false;

    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool onHover = false;
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
        onHover = false;
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

        //Funcion Delegafa del Evento "Orden de Dormir"
        DayStatusManager.instance.OnSleepOrderClicked += OnSleepOrderClickedDelegate;
    }

    //-----------------------------------------------------------------------------

    private void OnChickenSoldDelegate(float chickenPrice)
    {
        //Hacemos que las gallinas se muevan mas rapido por 2 segundos
        //como consecuencia de ver morir a su amigo
        mSelfMovementToTarget.MultiplySpeedTemporary(2);
    }

    private void OnSleepOrderClickedDelegate(bool sleepOrder)
    {
        //Actualizamos el SleepingFlag en base a si la orden esta activa o no
        sleepingFlag = sleepOrder;
    }

    //------------------------------------------------------------------------------------------
    // FUNCION: REVISAR SI TIENE HAMBRE

    public void CheckIfStarving()
    {
        //Si el Stat de Hambre esta muy elevado...
        if (mChickenStats.hambre >= 95)
        {
            //Activamos el flag de "Starving" 
            starvingFlag = true;

            //Entramos en Animacion de Starving
            mSpritesController.EnterStarvingAnim();
        }
        else
        {
            //Desactivamos el flag de "Starving" 
            starvingFlag = false;

            //Si el Pollito no tiene l mouse encima...
            if (!onHover)
            {
                //Hacemos que su color vuelva a la normalidad...
                mSpritesController.ExitStarvingAnim();
            }
            
        }
    }

    //------------------------------------------------------------------------------------------
    // FUNCION: REVISAR Y ACTUALIZAR STATS

    public void ManageStats()
    {

        //Manejamos los Stats segun loos flags
        mChickenStats.ManageStats_HambreYPeso(eatingFlag);
        mChickenStats.ManageStats_HP(fightingFlag, starvingFlag);
        mChickenStats.ManageStats_Estres(sleepingFlag);
    }

    //------------------------------------------------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        //Si el pollito sigue vivo...
        if (isAlive)
        {
            //Revisamos si tenemos hambre...
            CheckIfStarving();

            //Controlamos los Stats
            ManageStats();

            //Controlamos la Animacion de Caminata
            mSpritesController.ManageWalkingAnim();

            //Si el pollito esta peleando
            if (fightingFlag)
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
            if (eatingFlag || drinkingFlag || sleepingFlag || fightingFlag)
            {
                //Si el Stat de hambre esta por debajo de 40
                if (mChickenStats.hambre < 40)
                {
                    //Desactivamos los Flags de Comiendo y Bebiendo
                    eatingFlag = false;
                    drinkingFlag = false;
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

    //------------------------------------------------------------------------------------------

    private void FixedUpdate()
    {
        //Si el Pollito esta vivo...
        if (isAlive)
        {
            //Si el Pollito está comiendo, Bebiendo, Durmiendo, o Peleando
            if (eatingFlag || drinkingFlag || sleepingFlag || fightingFlag)
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
        //Activamos Flag de Hover
        onHover = true;

        //Controlamos la animacion de cuando se hace Hover
        mSpritesController.EnterHoverAnimation();

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

    //------------------------------------------------------------------------------------------

    private void OnMouseExit()
    {
        //Desactivamos Flag de Hover
        onHover = false;

        //Controlamos la animacion de cuando se hace Hover
        mSpritesController.ExitHoverAnimation();

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

            //Activamos Flag de "siendo arrastrado"
            isBeingDragged = true;

            //Controlamos la Animacion de Drag
            mSpritesController.EnterDragAnimation();

            
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
        if (isBeingDragged)
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

        //Hacemos que el Sprite vuelva a la Normalidad
        mSpritesController.SetSpriteBackToNormal();

        //Desactivamos flag de "esta siendo agarrado"
        isBeingDragged = false;

        //Si esta en Zona de Venta...
        if (bInBillingZone)
        {
            //Decimos al BillingZone del nivel que Trate de vendernos
            BillingZoneController.instance.TryToSellChicken();
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si el pollito esta vivo...
        if (isAlive)
        {
            //Si estams colisionando con otro Pollito...
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Si los Stats del Pollo indican que esta estresado...
                if (mChickenStats.estres > mChickenStats.estresParaPelear)
                {
                    //Activamos Flag de "Esta peleando"
                    fightingFlag = true;

                    //Controlamos la Animacion de Pelea
                    mSpritesController.EnterFightAnim();
                }
                //En caso el nivel de estres no esté en el Nivel...
                else
                {
                    //Obtenemos los Stats del pollo con el que hemos chocado
                    ChickenStats otherChickenStats = collision.gameObject.GetComponent<ChickenStats>();
                    
                    //Revisamos si el Estres del otro Pollo essta en el limite...
                    if (otherChickenStats.estres >= otherChickenStats.estresParaPelear)
                    {
                        //De ser el caso...

                        //Activamos Flag de "Esta peleando"
                        fightingFlag = true;

                        //Controlamos la Animacion de Pelea
                        mSpritesController.EnterFightAnim();
                    }
                    //En caso tampoco este estresado...
                    else
                    {
                        //Hacemos que se asigne un nuevo TargetRandom
                        GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();
                    }
                }
            }

            //Si chocamos con un contenedor de Comida o Agua
            else if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Water"))
            {
                //Hacemos que el Pollito MIRE en direccion a la colision.
                mSpritesController.LookAtTarget(collision.transform.position);

                //Si esta chocando con comida...
                if (collision.gameObject.CompareTag("Food"))
                {
                    //Si tiene hambre...
                    if (mChickenStats.hambre > 40)
                    {
                        //Activamos Flag de "Esta comiendeo"
                        eatingFlag = true;
                    }
                    //En caso no tenga hambre...
                    else
                    {
                        //Seteamos un nuevo target de movimiento random
                        GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();

                        //Desactivamos Flag de "Esta comiendeo"
                        eatingFlag = false;
                    }
                }

                //Si esta chocando con Agua...
                else if (collision.gameObject.CompareTag("Water"))
                {
                    //AJUSTAR ESTO!!!

                    //Si tiene hambre...
                    if (mChickenStats.hambre > 40)
                    {
                        //Activamos Flag de "Esta peleando"
                        drinkingFlag = true;
                        eatingFlag = true;
                    }
                    else
                    {
                        //Seteamos un nuevo target de movimiento random
                        GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();

                        //Desactivamos Flag de "Esta comiendeo"
                        drinkingFlag = false;
                        eatingFlag = false;
                    }
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionStay2D(Collision2D collision)
    {
        //Si estamos manteniendo el contacto con un recurso  de Comida o Agua...
        if (collision.gameObject.CompareTag("Food") || collision.gameObject.CompareTag("Water"))
        {
            //Hacemos que el Pollito MIRE en direccion a la colision.
            mSpritesController.LookAtTarget(collision.transform.position);

            //Si esta chocando con comida...
            if (collision.gameObject.CompareTag("Food"))
            {
                //Si su Hambre baja debajo de 95...
                if (mChickenStats.hambre < 95)
                {
                    //Salimos de la Animacion de Starving
                    mSpritesController.ExitStarvingAnim();
                }

                //Si el Comedero esta vacio...
                if (collision.gameObject.GetComponent<Food>().mFoodLevelSlider.value == 0)
                {
                    //Desactivamos Flag de "Esta comiendo"
                    eatingFlag = false;
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (isAlive)
        {
            //Si ha dejado de chocar con otro pollito
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Desactivamos flag de "esta peleando" (porsiacaso)
                fightingFlag = false;

                //Salimos de la Animacion de Pelea
                mSpritesController.ExitFightAnim();
            }
            //Si el objeto con el que colisionamos es otro Pollito
            else if (collision.gameObject.CompareTag("Food"))
            {
                //Desactivamos Flag de "Esta comiendo"
                eatingFlag = false;
            }

            //Si el objeto con el que colisionamos es otro Pollito
            else if (collision.gameObject.CompareTag("Water"))
            {
                //AJUSTAR ESTO!!

                //Desactivamos Flag de "Esta peleando"
                drinkingFlag = false;
                eatingFlag = false;
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si el Pollito ha entrado en la zona de Venta...
        if (collision.gameObject.CompareTag("BillingZone"))
        {
            //Activamos flag de "en zona de venta"
            bInBillingZone = true;
        }
    }

    //------------------------------------------------------------------------------------------

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
