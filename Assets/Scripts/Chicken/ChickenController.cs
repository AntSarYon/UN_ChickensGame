using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChickenController : MonoBehaviour
{
    [Header("Nombre de Pollito")]
    public string chickName;

    [Header("Tipo de Pollo")]
    public ChickenType type;

    [Header("Flags de Estados")]
    [HideInInspector] public bool eatingFlag = false;
    [HideInInspector] public bool starvingFlag = false;

    [HideInInspector] public bool isAlive = true;

    [HideInInspector] public bool isEstimulated = false;
    [HideInInspector] public bool isDisgusted = false;
    [HideInInspector] public bool sleepingFlag = false;

    [HideInInspector] public bool onHover = false;
    [HideInInspector] public bool isBeingDragged = false;
    [HideInInspector] public bool bBeingThrow = false;

    [Header("Corral")]
    public Yard assignedYard;

    //Flag - esta en zona de venta
    private bool bInBillingZone;

    [Header("Probabilidad que el pollito escape")]
    [Range(0.00f,1.00f)] public float dragEscapeProb;
    private float dragEscapeDefaultProb;
    private float dragEscapeMinProb = 0;

    [Header("Chicken UI")]
    [SerializeField] private ChickenUI chickenUI;

    // COMPONENTES
    private ChickenStats mChickenStats;
    private SpritesController mSpritesController;
    private Draggable mDraggable;
    private SelfMovementToTarget mSelfMovementToTarget;

    // Timers para estado de sueño aleatorio
    private float sleepCheckTimer = 0f;
    private float sleepDurationTimer = 0f;

    // Temperatura - agrupar y dormir
    private bool tempSleeping = false;
    private bool groupingToSleep = false;
    private bool inColdSleepState = false;  // Flag para rastrear si está en sueño por frío
    private Vector3 tempGatherPoint;
    [SerializeField] private float temperatureSleepThreshold = 25f;
    [SerializeField] private float temperatureWakeThreshold = 25f;  // Puede ser igual a sleepThreshold
    [SerializeField] private float temperatureDeathThreshold = 40f;  // Temperatura a la que muere por calor
    [SerializeField] private float gatherPointOffset = 1.5f;
    [SerializeField] private float gatherArriveDistance = 0.8f;

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
        dragEscapeDefaultProb = dragEscapeProb;
        dragEscapeMinProb = 0;

        //Iniciamos Flags
        isAlive = true;
        onHover = false;
        isBeingDragged = false;
    }

    //-----------------------------------------------------------------------------

    void Start()
    {
        //Definimos un nuevo destino aleatorio para el pollito
        mSelfMovementToTarget.SetNewRandomWaypoint();

        //Agregamos Funcion Delegado al Evento de Pollo vendido
        DayStatusManager.Instance.OnChickenSold += OnChickenSoldDelegate;

        // Inicializamos timers para sueño aleatorio
        sleepCheckTimer = Random.Range(3f, 3f);
        sleepDurationTimer = 0f;

        // Suscribir al evento de temperatura del corral asignado
        if (assignedYard != null)
        {
            assignedYard.OnTemperatureChanged += ValidateTemperature;

            // Si el pollo nace con temperatura baja, debe dormir inmediatamente
            if (assignedYard.temperature < temperatureSleepThreshold)
            {
                inColdSleepState = true;
                tempSleeping = true;
                sleepingFlag = true;
                mSelfMovementToTarget.StopMoving();
                mSpritesController.SetSleeping(true);
                sleepDurationTimer = Random.Range(10f, 30f);
            }
        }

    }

    private void OnDestroy()
    {
        if (assignedYard != null)
        {
            assignedYard.OnTemperatureChanged -= ValidateTemperature;
        }
    }

    //-----------------------------------------------------------------------------

    private void OnChickenSoldDelegate(float chickenPrice)
    {
        //Hacemos que las gallinas se muevan mas rapido por 2 segundos
        //como consecuencia de ver morir a su amigo
        mSelfMovementToTarget.MultiplySpeedTemporary(2);
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
            mSpritesController.EnableStarvingAnim();
        }
        else
        {
            //Desactivamos el flag de "Starving" 
            starvingFlag = false;

            //Si el Pollito no tiene l mouse encima...
            if (!onHover)
            {
                //Hacemos que su color vuelva a la normalidad...
                mSpritesController.DisableStarvingAnim();
            }
            
        }
    }

    //------------------------------------------------------------------------------------------
    // FUNCION: REVISAR Y ACTUALIZAR STATS

    public void ManageStats()
    {
        //Manejamos los Stats segun loos flags
        mChickenStats.ManageStats_HambreYPeso(eatingFlag, isBeingDragged);
        mChickenStats.ManageStats_HP(starvingFlag);
        mChickenStats.ManageStats_felicidad(eatingFlag, isBeingDragged);
    }

    //------------------------------------------------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        //Si el pollito sigue vivo...
        if (isAlive)
        {
            //Controlamos los Stats
            ManageStats();

            // PRIORIDAD MÁXIMA: Si está en sueño por frío, forzar estado de dormir
            if (inColdSleepState)
            {
                sleepingFlag = true;
                tempSleeping = true;
                isBeingDragged = false;
                eatingFlag = false;
                mSelfMovementToTarget.StopMoving();
                mSpritesController.SetSleeping(true);

                // Decrementar timer de sueño
                sleepDurationTimer -= Time.deltaTime;
                // (No despertar por timer mientras esté en sueño por frío - solo por temperatura)
            }

            //Si no esta siendo Draggeado...
            else if (!isBeingDragged)
            {
                //Revisamos si tenemos hambre...
                CheckIfStarving();

                //Controlamos la Animacion de Caminata
                mSpritesController.ManageWalkingAnim();
            }

            //Si no esta peleand
            else if (isBeingDragged)
            {
                //Regresamos la Probabilidad a la normalidad
                dragEscapeProb = dragEscapeDefaultProb;
            }

            //Si el Pollito esta haciendo alguna de estas acciones...
            if (eatingFlag)
            {
                //Si el Stat de hambre esta por debajo de 40
                if (mChickenStats.hambre < 40)
                {
                    //Desactivamos los Flags de Comiendo y Bebiendo
                    eatingFlag = false;
                }
            }
            else if (!inColdSleepState)
            {
                //Si esta dormido, no moverse
                if (sleepingFlag)
                {
                    // Permanecer dormido hasta que el timer expire
                    sleepDurationTimer -= Time.deltaTime;
                    if (sleepDurationTimer <= 0f)
                    {
                        WakeFromSleep();
                    }
                }
                else
                {
                    //Seteamos una direccion de Movimiento
                    mSelfMovementToTarget.SetMovementDirection();

                    //Intento aleatorio de dormirse cuando está idle
                    if (!isBeingDragged && !eatingFlag && !starvingFlag)
                    {
                        sleepCheckTimer -= Time.deltaTime;
                        if (sleepCheckTimer <= 0f)
                        {
                            // Probabilidad de dormirse (ej. 30%)
                            if (Random.Range(0f, 1f) <= 0.30f)
                            {
                                // Entrar a modo dormido
                                sleepingFlag = true;
                                // Detener movimiento y animación de dormir
                                mSelfMovementToTarget.StopMoving();
                                mSpritesController.SetSleeping(true);
                                // Duración del sueño aleatoria
                                sleepDurationTimer = Random.Range(6f, 18f);
                            }

                            // Reiniciamos el chequeo
                            sleepCheckTimer = Random.Range(3f, 3f);
                        }
                    }
                }
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
            //Si no esta siendo
            if (!bBeingThrow)
            {
                //Si el Pollito está durmiendo, NO moverse
                if (sleepingFlag || tempSleeping)
                {
                    //Hacemos que deje de moverse (Velocidad a 0)
                    mSelfMovementToTarget.StopMoving();

                }
                //Si el Pollito está comiendo, Bebiendo, o Peleando
                else if (eatingFlag)
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
                //chickenUI.ShowChickenInfo();
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
                //chickenUI.HideChickenInfo();
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
            //mDraggable.Catch();

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
            //chickenUI.HideChickenInfo();
        }


        //Si el flag de "Esta siendo Arrastrado" esta activo
        if (isBeingDragged)
        {
            //Movemos la Posicion del pollo
            //mDraggable.MovePosition();
        }
        
    }

    //-----------------------------------------------------------------------------------
    // Funcion - Cuando soltamos ewl Click

    private void OnMouseUp()
    {
        //Soltamos al pollito
        //mDraggable.Drop();

        //Hacemos que el Sprite vuelva a la Normalidad
        mSpritesController.SetSpriteBackToNormal();

        //Desactivamos flag de "esta siendo agarrado"
        isBeingDragged = false;

        //Si esta en Zona de Venta...
        if (bInBillingZone)
        {
            //Decimos al BillingZone del nivel que Trate de vendernos
            //BillingZoneController.instance.TryToSellChicken();
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        //Si el pollito esta vivo...
        if (isAlive)
        {
            //Si estams colisionando con otro Pollito...
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Si los Stats del Pollo indican que esta felicidadado...
                if (mChickenStats.felicidad > mChickenStats.felicidadParaPelear)
                {
                    //Controlamos la Animacion de Pelea
                    //SpritesController.EnterFightAnim();
                }
                //En caso el nivel de felicidad no est� en el Nivel...
                else
                {
                    //Obtenemos los Stats del pollo con el que hemos chocado
                    ChickenStats otherChickenStats = collision.gameObject.GetComponent<ChickenStats>();
                    
                    //Revisamos si el felicidad del otro Pollo essta en el limite...
                    if (otherChickenStats.felicidad >= otherChickenStats.felicidadParaPelear)
                    {
                        //De ser el caso...

                        //Controlamos la Animacion de Pelea
                        //mSpritesController.EnterFightAnim();
                    }
                    //En caso tampoco este felicidadado...
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

                        //Obtenemos la lista de ingredientes de esta comida...
                        List<Ingredient> foodIngredientes = collision.gameObject.GetComponent<Food>().ingredientsList;

                        //Iniciamos flag de si le gusta la comida
                        bool likefood = false;

                        //Por cada ingrediente de este comedero
                        foreach (Ingredient ing in foodIngredientes)
                        {
                            //Revisamos si elcomedero tiene un ingrediente
                            if (type.likedIngredients.Contains(ing))
                            {
                                //Basta con que tenga 1, para marcar como like
                                likefood = true;
                            }
                        }

                        // Si le gusta la comida, mostramos el Like
                        if (likefood)
                        {
                            chickenUI.ShowLike();

                            // Activamos Fag de Estimulacion
                            isEstimulated = true;
                        }
                        // Caso contrario, mostramos el Dislike
                        else chickenUI.ShowDislike();
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
                        eatingFlag = true;
                    }
                    else
                    {
                        //Seteamos un nuevo target de movimiento random
                        GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();

                        //Desactivamos Flag de "Esta comiendeo"
                        eatingFlag = false;
                    }
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionStay(Collision collision)
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
                    mSpritesController.DisableStarvingAnim();
                }

                //Si el Comedero esta vacio...
                if (collision.gameObject.GetComponent<Food>().mFoodLevelSlider.value == 0)
                {
                    //Desactivamos Flag de "Esta comiendo"
                    eatingFlag = false;

                    // Desactivamos Fag de Estimulacion
                    isEstimulated = false;
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionExit(Collision collision)
    {
        if (isAlive)
        {
            //Si ha dejado de chocar con otro pollito
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Salimos de la Animacion de Pelea
                mSpritesController.ExitFightAnim();
            }
            //Si el objeto con el que colisionamos es otro Pollito
            else if (collision.gameObject.CompareTag("Food"))
            {
                //Desactivamos Flag de "Esta comiendo"
                eatingFlag = false;

                // Desactivamos Fag de Estimulacion
                isEstimulated = false;

                //Desactivamos el Globo de reaccion
                chickenUI.HideReaction();
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider collision)
    {
        //Si el Triger al que entramos es la zona de interacci�n
        if (collision.tag == "PlayerInteractionZone")
        {
            //Obtenemos el PickupController del PlayerBody (Padre del Triger)
            //para asignarle que este ser� el Objeto a coger.
            collision.GetComponentInParent<PickUpController>().targetObject = this.gameObject;

            //Controlamos la animacion de cuando se hace Hover
            mSpritesController.EnterHoverAnimation();

        }

        //Si el Triger al que entramos es la zona de interacción
        if (collision.tag == "ApplauseArea")
        {
            //Multiplicamos la velocidad por 1.5 segundos...
            mSelfMovementToTarget.MultiplySpeedTemporary(0.75f);

            // Si el área de aplauso está activa, despertarnos (esto cubre casos donde
            // el overlap desde PlayerController no alcanzó al chicken por diferencias de collider)
            WakeUpForApplause();

            //Hacemos que se aleje del círculo de aplauso
            RunAwayFromApplause(collision.transform.parent.position);
        }

        //Si el Pollito ha entrado en la zona de Venta...
        if (collision.gameObject.CompareTag("BillingZone"))
        {
            //Activamos flag de "en zona de venta"
            bInBillingZone = true;
        }

        //Si el Pollito ha entrado en la zona de Venta...
        if (collision.gameObject.CompareTag("Toy"))
        {

            //Si al (tipo) Pollito le gustan os juguetes...
            if (type.likeToys)
            {
                // Activamos Fag de Estimulacion
                isEstimulated = true;

                //Mostramos el icono de Estimulacion
                chickenUI.ShowEstimulation();

            }
        }

        if (collision.CompareTag("ChickenLimitZone"))
        {
            Destroy(this.gameObject);
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnTriggerExit(Collider collision)
    {
        //Si el Triger del que salimos es la zona de interacci�n
        if (collision.tag == "PlayerInteractionZone")
        {
            //Si la ultima referencia que tenia la zona era la de este objeto...
            if (collision.GetComponentInParent<PickUpController>().targetObject == this.gameObject)
            {
                //Obtenemos el PickupController del Player (Padre del Triger)
                //para indicar que ya no habr� ningun Objeto Asignado.
                collision.GetComponentInParent<PickUpController>().targetObject = null;

                //Controlamos la animacion de cuando se hace Hover
                mSpritesController.ExitHoverAnimation();
            }
        }

        //Si el Pollito ha salido de la zona de Venta...
        if (collision.gameObject.CompareTag("BillingZone"))
        {
            //Desactivamos flag de "en zona de venta"
            bInBillingZone = false;
        }

        //Si el Pollito ha entrado en la zona de Venta...
        if (collision.gameObject.CompareTag("Toy"))
        {

            //Si al (tipo) Pollito le gustaban los juguetes...
            if (type.likeToys)
            {
                //Ocultamos el icono de Estimulacion
                chickenUI.HideEstimulation();
            }
        }
    }

    //-----------------------------------------------------------------------------------

    public void Die()
    {
        //Seteamos que la probabilidad de ecape este al 0%
        dragEscapeProb = dragEscapeMinProb;

        //Llamamos al Evento de Pollito muerto
        DayStatusManager.Instance.TriggerEvent_OnChickenDeath();

        //Hacemos que se reproduzca el Sonido de Pollito muerto
        GameSoundsController.Instance.PlayChickenDeathSound();

        //Reproducimos la Animacion de Muerte
        mSpritesController.PlayDeath();

        //Desactivamos la UI del Pollito
        chickenUI.gameObject.SetActive(false);
    }

    //-----------------------------------------------------------------------------------
    // FUNCION: Despertar del sueño
    public void WakeFromSleep()
    {
        if (!sleepingFlag) return;

        sleepingFlag = false;
        // NOTA: no reseteamos tempSleeping ni groupingToSleep aquí
        // Solo ValidateTemperature() controla eso cuando sale del estado de frío
        mSpritesController.SetSleeping(false);

        // Reiniciamos el timer para volver a intentar dormir en el futuro
        sleepCheckTimer = Random.Range(12f, 26f);
    }

    //-----------------------------------------------------------------------------------
    // FUNCION: Hacer que el pollo se aleje del círculo de aplauso

    public void RunAwayFromApplause(Vector3 applauseCircleCenter)
    {
        // Si está durmiendo por frío, despertarlo primero
        if (inColdSleepState)
        {
            inColdSleepState = false;
            tempSleeping = false;
            groupingToSleep = false;
            sleepingFlag = false;
            mSpritesController.SetSleeping(false);
        }

        //Aumentamos la velocidad temporalmente para escapar
        mSelfMovementToTarget.MultiplySpeedTemporary(0.75f);

        //Calculamos la dirección de escape (opuesta al centro del círculo de aplauso)
        Vector3 runAwayDirection = (transform.position - applauseCircleCenter).normalized;

        //Usamos el método de escape para que se mueva en esa dirección
        mSelfMovementToTarget.EscapeInDirection(runAwayDirection, 8f);
    }

    //-----------------------------------------------------------------------------------
    // FUNCION: Validar temperatura del corral y forzar comportamiento
    private void ValidateTemperature(float currentTemperature)
    {
        // MUERTE POR CALOR EXTREMO
        if (currentTemperature > temperatureDeathThreshold && isAlive)
        {
            Die();
            return;
        }

        // Si está por debajo del umbral de dormir Y no está en sueño por frío
        if (currentTemperature < temperatureSleepThreshold && !inColdSleepState)
        {
            // Activar sueño por frío
            inColdSleepState = true;
            tempSleeping = true;
            sleepingFlag = true;
            
            // Cancelar acciones que impidan dormir
            isBeingDragged = false;
            eatingFlag = false;
            
            // Detener movimiento y activar animación
            mSelfMovementToTarget.StopMoving();
            mSpritesController.SetSleeping(true);
            sleepDurationTimer = Random.Range(10f, 30f);
        }
        // Si está por encima del umbral de despertar Y está en sueño por frío
        else if (currentTemperature >= temperatureWakeThreshold && inColdSleepState)
        {
            // Salir de sueño por frío
            inColdSleepState = false;
            tempSleeping = false;
            groupingToSleep = false;
            // Despertar definitivamente y resetear timer de sueño
            sleepingFlag = false;
            mSpritesController.SetSleeping(false);
            sleepCheckTimer = Random.Range(12f, 26f);
            mSelfMovementToTarget.SetNewRandomWaypoint();
        }
    }

    //-----------------------------------------------------------------------------------
    // FUNCION: Forzar despertar (usado por aplausos)
    public void WakeUpForApplause()
    {
        // Limpiar estados de sueño por temperatura
        inColdSleepState = false;
        tempSleeping = false;
        groupingToSleep = false;

        // Asegurar que el pollo está despierto visualmente y en lógica
        sleepingFlag = false;
        mSpritesController.SetSleeping(false);

        // Evitar que se vuelvan a dormir inmediatamente
        sleepCheckTimer = Random.Range(12f, 26f);
    }
}
