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
    [HideInInspector] public bool bIsAlive = true;
    [HideInInspector] public bool bIsWalking = false;
    [HideInInspector] public bool bIsEating = false;
    [HideInInspector] public bool bIsStarving = false;
    [HideInInspector] public bool bIsSleeping = false;
    [HideInInspector] public bool bIsAngry = false;
    [HideInInspector] public bool bIsFighting = false;
    [HideInInspector] public bool bOnFloor = true;

    [HideInInspector] public bool bTargeted = false;
    [HideInInspector] public bool bThrown = false;

    [HideInInspector] public bool bInTempSleeping = false; // Flag de Dormido temporalmente
    [HideInInspector] public bool bInColdSleepState = false;  // Dormido por frio

    [Header("Corral")]
    public Yard assignedYard;

    [Header("Chicken UI")]
    [SerializeField] private ChickenUI chickenUI;

    // COMPONENTES
    private ChickenStats mChickenStats;
    private PickeableObject mPickeable;
    private SpritesController mSpritesController;
    private SelfMovementToTarget mSelfMovementToTarget;

    // Timers para estado de sueño aleatorio
    private float sleepCheckTimer = 0f;
    private float sleepDurationTimer = 0f;

    // Probabilidad de caer en sueno aleatoriamente
    private float tempSleepProb = 0.30f;

    [SerializeField] private float temperatureSleepThreshold = 25f;
    [SerializeField] private float temperatureWakeThreshold = 25f;  // Puede ser igual a sleepThreshold
    [SerializeField] private float temperatureDeathThreshold = 40f;  // Temperatura a la que muere por calor

    private AudioSource mAudioSource;

    // Clips de Audio del Pollito:
    [Header("Clips de Audio")]
    [SerializeField] private AudioClip clipDragged;
    [SerializeField] private AudioClip clipEscaped;
    [SerializeField] private AudioClip clipWings;
    [SerializeField] private AudioClip clipHit;

    // Corutinas
    private Coroutine cor_CheckIfStarving;


    //-----------------------------------------------------------------------------

    void Awake()
    {
        // Referencia a Componentes
        mSpritesController = GetComponent<SpritesController>();
        mChickenStats = GetComponent<ChickenStats>();
        mSelfMovementToTarget = GetComponent<SelfMovementToTarget>();
        mPickeable = GetComponent<PickeableObject>();

        mAudioSource = GetComponent<AudioSource>();

        //Iniciamos Flags
        bIsAlive = true;
        bIsWalking = true;

        bTargeted = false;
        bIsAngry = false;
        bIsFighting = false;

        
    }

    //-----------------------------------------------------------------------------

    void Start()
    {
        if (TemperatureManager.Instance != null)
        {
            // Suscribir al evento de temperatura
            TemperatureManager.Instance.OnTemperatureChanged += OnTemperatureChangedDelegate;

            // Si el pollo nace con temperatura baja, debe dormir inmediatamente
            if (TemperatureManager.Instance.temperature < temperatureSleepThreshold)
            {
                // Mandamos a dormir a pollit por Frio
                SleepForCold();
            }
        }

        // Inicializamos timers para sueño
        sleepCheckTimer = Random.Range(5f, 8f); // Tiempo entre sueños aleatorios
        sleepDurationTimer = 0f; // Timer duracion de sueño

        //ARRANCAMOS CORUTINA DE REVISION DE HAMBRE
        cor_CheckIfStarving = StartCoroutine(CheckIfStarving(0.5f));

    }

    // ------------------------------------------------------------------

    // FUNCION: Dormir por frio
    private void SleepForCold()
    {
        // Activaoms Flags para indicar esta durmiendo por frio
        bIsSleeping = true;
        bInColdSleepState = true;
        bInTempSleeping = false;

        //Desactivamos otros Flags
        bIsEating = false;
        bIsWalking = false;

        //Hacemos que el Pollito deje de moverse
        mSelfMovementToTarget.StopMoving();

        //Activamos la annimacion de dormido
        mSpritesController.SetSleeping(true);

        Debug.Log("Me dormi porque hace frio");
    }

    // -------------------------------------------------------------------
    // FUNCION: Dormir temporalmente
    private void SleepTemporally()
    {
        // Activaoms Flags para indicar esta durmiendo por frio
        bIsSleeping = true;
        bInColdSleepState = false;
        bInTempSleeping = true;

        //Desactivamos otros Flags
        bIsEating = false;
        bIsWalking = false;

        //Hacemos que el Pollito deje de moverse
        mSelfMovementToTarget.StopMoving();

        //Activamos la annimacion de dormido
        mSpritesController.SetSleeping(true);

        //Iniciamos el contador para el tiempo de suenio temporal
        sleepDurationTimer = Random.Range(6f, 18f);

        Debug.Log("Me dormi temporalmente");
    }

    //------------------------------------------------------------------------

    private void OnDestroy()
    {
        if (TemperatureManager.Instance != null)
        {
            TemperatureManager.Instance.OnTemperatureChanged -= OnTemperatureChangedDelegate;
        }
    }

    //------------------------------------------------------------------------------------------
    // CORUTINA: REVISAR SI TIENE HAMBRE
    private IEnumerator CheckIfStarving(float checkInterval)
    {
        //Repetiremos indefinidamente...
        while (true)
        {
            //Si el Stat de Hambre esta muy elevado...
            if (mChickenStats.hambre >= 90)
            {
                //Activamos el flag de "Starving" 
                bIsStarving = true;
            }
            // En caso NO TENGA TANTA HAMBRE
            else
            {
                //Desactivamos el flag de "Starving" 
                bIsStarving = false;
            }

            // Cada intervalo
            yield return new WaitForSeconds(checkInterval);
        }
    }

    //------------------------------------------------------------------------------------------
    // FUNCION: REVISAR Y ACTUALIZAR STATS

    public void ManageStats()
    {
        //Manejamos los Stats segun loos flags
        mChickenStats.ManageStats_HambreYPeso(bIsEating, !mPickeable.isPickeable);
        mChickenStats.ManageStats_HP(bIsStarving);
    }

    //------------------------------------------------------------------------------------------

    // Update is called once per frame
    void Update()
    {
        //Si el pollito sigue vivo...
        if (bIsAlive)
        {
            //Si el pollito NO ESTA SIEDO CARGADO
            if (mPickeable.isPickeable)
            {
                //Controlamos sus estadisticas
                ManageStats();

                // Si esta dormido
                if (bIsSleeping)
                {
                    // Si esta en una siesta temporal
                    if (bInTempSleeping)
                    {
                        Debug.Log("Durmiendo por frio");
                        // Decrementamos timer de sueño
                        sleepDurationTimer -= Time.deltaTime;

                        // Permanecer dormido hasta que el timer expire
                        if (sleepDurationTimer <= 0f)
                        {
                            WakeUp();
                        }
                    }

                    // No despertar por timer mientras esté en sueño por temperatura fría
                }
                //Si no esta dormido
                else
                {
                    //Si esta comiedo...
                    if (bIsEating)
                    {
                        Debug.Log("Estoy comiendo");

                        //Si el Stat de hambre baja de 15
                        if (mChickenStats.hambre < 15)
                        {
                            //Desactivamos el Flag de Comiendo
                            bIsEating = false;
                        }
                    }
                    //Si esta caminando
                    else if (bIsWalking)
                    {
                        // Si el pollito no tiene hambre
                        if (!bIsStarving)
                        {
                            Debug.Log("Estooy caminando arbitrariamente");
                            //Reducimos el Timer para su siesta espontanea
                            sleepCheckTimer -= Time.deltaTime;

                            // Si el timer llega a 0
                            if (sleepCheckTimer <= 0f)
                            {
                                // Obtenemos Probabilidad de dormirse (ej. 30%)
                                if (Random.Range(0f, 1f) <= tempSleepProb)
                                {
                                    // De cumplirse, se duerme
                                    SleepTemporally();
                                }

                                // Reiniciamos el timer de chequeo
                                sleepCheckTimer = Random.Range(5f, 8f);
                            }
                        }

                        //Si el pollito SI tiene hambre...
                        else if (bIsStarving)
                        {
                            Debug.Log("Estoy buscando comida");

                            // No inducimos al pollito a dormirse

                            // Asignamos el comedero mas cercano como Target de movimiento
                            mSelfMovementToTarget.target = FoodsManager.Instance.GetClosestFood(transform);
                        }
                    }

                }
            }
            //En caso si este siendo cargado
            else 
            {
                Debug.Log("Me estann cargando");

                //Apagamos todos los otros Flags
                bIsWalking = false;
                bOnFloor = false;

                bIsAngry = false;
                bIsFighting = false;

                bIsEating = false;
                bIsStarving = false;

                bIsSleeping = false;
                bInColdSleepState = false;
                bInTempSleeping = false;

                // Desactivamos animaciones de Dormir
                //mSpritesController.SetSleeping(false);
            }

            //Independientemente de lo que este haciendo...


            //Si el HP del pollito llega  0
            if (mChickenStats.hp == 0)
            {
                // Desactivamos Flag de "esta vivo"
                bIsAlive = false;

                //Reproducimos las Acciones de Muerte.
                Die();

                Debug.Log("Me mori");
            }
        }

    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionEnter(Collision collision)
    {
        //Si el pollito esta vivo...
        if (bIsAlive)
        {
            //Si hemos impactado el suelo...
            if (collision.gameObject.CompareTag("Floor"))
            {
                // Activamos el flag de "Sobre el suelo"
                bOnFloor = true;

                //Si no esta dormido a causa del frio...
                if (!bIsSleeping)
                {
                    //Activa su flag de caminar
                    bIsWalking = true;
                }
            }

            //Si estams colisionando con otro Pollito...
            if (collision.gameObject.CompareTag("Chicken"))
            {

                //Controlamos la Animacion de Pelea
                //mSpritesController.EnterFightAnim();

                //Obtenemos los Stats del pollo con el que hemos chocado
                //ChickenStats otherChickenStats = collision.gameObject.GetComponent<ChickenStats>();
            }

            //En caso tampoco este felicidadado...
            else
            {
                //Si el objeto colsiionado esta a la izquierda
                if (collision.transform.position.x < transform.position.x)
                {
                    //Hacemos que se asigne un nuevo TargetRandom hacia la derecha
                    GetComponent<SelfMovementToTarget>().SetNewRandomWaypointToRight(collision.transform.position.x);
                }
                //Si el objeto colsiionado esta a la derecha
                else if (collision.transform.position.x > transform.position.x)
                {
                    //Hacemos que se asigne un nuevo TargetRandom hacia la izquierda
                    GetComponent<SelfMovementToTarget>().SetNewRandomWaypointToLeft(collision.transform.position.x);
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
                    bIsEating = true;

                    // Caso contrario, mostramos el Dislike
                    chickenUI.ShowDislike();
                }

                //En caso no tenga hambre...
                else
                {
                    //Seteamos un nuevo target de movimiento random
                    GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();

                    //Desactivamos Flag de "Esta comiendeo"
                    bIsEating = false;
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
                    bIsEating = true;
                }
                else
                {
                    //Seteamos un nuevo target de movimiento random
                    GetComponent<SelfMovementToTarget>().SetNewRandomWaypoint();

                    //Desactivamos Flag de "Esta comiendeo"
                    bIsEating = false;
                }
            }
        }
    }


    //------------------------------------------------------------------------------------------

    private void OnCollisionStay(Collision collision)
    {
        //Si estamos en contacto con el suelo...
        if (collision.gameObject.CompareTag("Floor"))
        {
            //Mantenemos activo el flag de "Sobre el suelo"
            bOnFloor = true;
        }

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

                }

                //Si el Comedero esta vacio, o ya sacio su hambre...
                if (collision.gameObject.GetComponent<Food>().mFoodLevelSlider.value == 0 || mChickenStats.hambre <= 15)
                {
                    //Quitamos el comedero como target
                    mSelfMovementToTarget.target = null;

                    //Desactivamos Flag de "Esta comiendo"
                    bIsEating = false;
                }
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionExit(Collision collision)
    {
        if (bIsAlive)
        {
            //Si dejamos de tener contacto con el suelo...
            if (collision.gameObject.CompareTag("Floor"))
            {
                // Desactivamos el flag de "Sobre el suelo"
                bOnFloor = false;
            }

            //Si ha dejado de chocar con otro pollito
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Salimos de la Animacion de Pelea

            }
            //Si el objeto con el que colisionamos es otro Pollito
            else if (collision.gameObject.CompareTag("Food"))
            {
                //Desactivamos Flag de "Esta comiendo"
                bIsEating = false;

                //Desactivamos el Globo de reaccion
                chickenUI.HideReaction();
            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider collision)
    {
        //Si el Triger al que entramos es la zona de COMIDA
        if (collision.CompareTag("Food"))
        {
            // Activamos Flag de "Esta comiendo"
            bIsEating = true;

            //Desactivamos flag de "Caminanndo"
            bIsWalking = false;
        }

        //Si el Triger al que entramos es la zona de APLAUSO
        if (collision.tag == "ApplauseArea")
        {
            //Despertarnos
            WakeUp();

            //Multiplicamos la velocidad por 0.75 segundos...
            mSelfMovementToTarget.MultiplySpeedTemporary(0.75f);

            //Hacemos que se aleje del centro del aplauso
            RunAwayFromApplause(collision.transform.parent.position);
        }

        //Si se llega
        if (collision.CompareTag("ChickenLimitZone"))
        {
            Destroy(this.gameObject);
        }
    }


    //-----------------------------------------------------------------------------------

    public void Die()
    {
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
    // FUNCION: Hacer que el pollo se aleje del círculo de aplauso

    public void RunAwayFromApplause(Vector3 applauseCircleCenter)
    {
        // Si está durmiendo despertarlo primero
        if (bIsSleeping)
        {
            WakeUp();
        }

        //Calculamos la dirección de escape (opuesta al centro del círculo de aplauso)
        Vector3 runAwayDirection = (transform.position - applauseCircleCenter).normalized;

        //Usamos el método de escape para que se mueva en esa dirección
        mSelfMovementToTarget.EscapeInDirection(runAwayDirection, 8f);
    }

    //-----------------------------------------------------------------------------------
    // FUNCION: Validar temperatura del corral y forzar comportamiento
    private void OnTemperatureChangedDelegate(float currentTemperature)
    {
        //Siempre y cuando el Pollo este vivo
        if (bIsAlive)
        {
            // MUERTE POR CALOR EXTREMO
            if (currentTemperature > temperatureDeathThreshold && bIsAlive)
            {
                Die();
                return;
            }

            // Si está por debajo del umbral de dormir Y no está en sueño por frío
            if (currentTemperature < temperatureSleepThreshold && !bInColdSleepState)
            {
                // En caso aun no este durmiendo
                if (!bIsSleeping)
                {
                    //Lo mandamos a dormir por frio
                    SleepForCold();
                }
                
            }
            // Si está por encima del umbral de despertar Y está en sueño por frío
            else if (currentTemperature >= temperatureWakeThreshold && bInColdSleepState)
            {
                // Salir de sueño
                WakeUp();

                //Configuramos nuevo punto de movimiento
                mSelfMovementToTarget.SetNewRandomWaypoint();
            }
        }

    }

    // ----------------------------------------------------------

    public void WakeUp()
    {
        // Despertar definitivamente y resetear timer de sueño
        bInColdSleepState = false;
        bInTempSleeping = false;
        bIsSleeping = false;

        mSpritesController.SetSleeping(false);

        sleepCheckTimer = Random.Range(12f, 26f);

        //Activamos flag de "caminando"
        bIsWalking = true;

        Debug.Log("Me Desperté");
    }
}
