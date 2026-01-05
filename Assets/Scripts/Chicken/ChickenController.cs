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
    [HideInInspector] public bool bIsStarving = false;
    [HideInInspector] public bool bIsSleeping = false;
    [HideInInspector] public bool bIsAngry = false;
    [HideInInspector] public bool bIsFighting = false;
    [HideInInspector] public bool bOnFloor = true;

    [HideInInspector] public bool bTargeted = false;
    [HideInInspector] public bool bThrown = false;

    [HideInInspector] public bool bInTempSleeping = false; // Flag de Dormido temporalmente
    [HideInInspector] public bool bInColdSleepState = false;  // Dormido por frio

    [HideInInspector] public bool bIsEating = false;
    // Referencia a Cmedero del que esta comienndo
    [HideInInspector] public Food assignedFood;

    [Header("Corral")]
    public Yard assignedYard;

    [Header("Chicken UI")]
    public ChickenUI chickenUI;

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

    [SerializeField] private float temperatureSleepThreshold = 15f;
    [SerializeField] private float temperatureWakeThreshold = 15f;  // Puede ser igual a sleepThreshold
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

        //Empieza con la referencia a comedero vacia
        assignedFood = null;

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

        ////Debug.log("Me dormi porque hace frio");
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

        //Debug.log("Me dormi temporalmente");
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
        mChickenStats.ManageStats_Saciedad(bIsEating);

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
                        //Debug.log("Durmiendo por frio");
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
                        //Debug.log("Estoy comiendo");

                        //Si el comedero queda vacio, o el Stat de hambre baja de 15
                        if (assignedFood.mFoodLevelSlider.value == 0 || mChickenStats.hambre < 15)
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
                            //Debug.log("Estooy caminando arbitrariamente");
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
                            //Debug.log("Estoy buscando comida");

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
                //Debug.log("Me estann cargando");

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
                mSpritesController.SetSleeping(false);
            }

            //Independientemente de lo que este haciendo...


            //Si el HP del pollito llega  0
            if (mChickenStats.hp == 0)
            {
                // Desactivamos Flag de "esta vivo"
                bIsAlive = false;

                //Reproducimos las Acciones de Muerte.
                Die();

                //Debug.log("Me mori");
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

                //Si no esta dormido a causa del frio, y no esta comiendo
                if (!bIsSleeping && !bIsEating)
                {
                    //Activa su flag de caminar
                    bIsWalking = true;
                }
            }

            //Si estams colisionando con otro Pollito...
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Obtenemos los Stats del pollo con el que hemos chocado
                //ChickenStats otherChickenStats = collision.gameObject.GetComponent<ChickenStats>();

                // Condicion de PELEA
                if (false)
                {
                    //Controlamos la Animacion de Pelea
                    //mSpritesController.EnterFightAnim();
                }
                else
                {
                    //Hacemos que el pollito se mueva en direccion contraria del otro pollito 
                    GetComponent<SelfMovementToTarget>().SetNewRandomWaypointInOpositeDirection(collision.transform.position);
                }
            }

            //En caso tampoco este felicidadado...
            else
            {
                //Hacemos que el pollito se mueva en direccion contraria del otro pollito 
                GetComponent<SelfMovementToTarget>().SetNewRandomWaypointInOpositeDirection(collision.transform.position);

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
    }

    //------------------------------------------------------------------------------------------

    private void OnCollisionExit(Collision collision)
    {
        if (bIsAlive)
        {
            //Si ha dejado de chocar con otro pollito
            if (collision.gameObject.CompareTag("Chicken"))
            {
                //Salimos de la Animacion de Pelea

            }
        }
    }

    //------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider collision)
    {
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

        //Debug.log("Me Desperté");
    }

    // --------------------------------------------

    public void Try_AssignFood(Food targetFood, Transform slot)
    {
        // Si no se esta asignado a ningun comedero...
        if (assignedFood == null)
        {
            // Reiniciamos el stat de Saciedad a 0
            mChickenStats.RestartSaciedad();

            // Modificamos los flags de estado de pollito
            bIsWalking = false;
            bIsEating = true;

            //Nos asignamos al comedero recibido como parametro
            assignedFood = targetFood;

            //Modificamos la posicion del Pollito para que este en el Slot
            transform.position = new Vector3(
                slot.position.x,
                transform.position.y,
                slot.position.z
                );
        }
    }

    public void Try_AbandonFood()
    {
        // Si se esta consuimendo comida de un comedero...
        if (assignedFood != null)
        {
            //Hacemos que el comedero del que estamos consumiendo nos libere
            assignedFood.ReleaseChicken(this);
        }

    }

    // ----------------------------------------------------------------

    public void Sell()
    {
        //Primero nos aseguramos que no este vinculado a ningun comedero
        Try_AbandonFood();

        //Luego Destruimos le objeto
        Destroy(this.gameObject);
    }
}
