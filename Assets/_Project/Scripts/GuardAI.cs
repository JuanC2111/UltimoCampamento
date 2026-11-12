using UnityEngine;
using UnityEngine.AI;

// IA del guardia: máquina de estados finita (FSM) con 3 estados.
[RequireComponent(typeof(NavMeshAgent))]
public class GuardAI : MonoBehaviour
{
    // Estados posibles del guardia
    enum State { Patrol, Chase, Search }
    State state = State.Patrol;

    [Header("Referencias")]
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform player;

    [Header("Parametros")]
    [SerializeField] float patrolSpeed = 3f;
    [SerializeField] float chaseSpeed = 6f;
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float searchWait = 3f;

    NavMeshAgent agent;
    Renderer rend;
    int currentWaypoint;
    Vector3 lastKnownPosition;
    float searchTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        SetState(State.Patrol);
    }

    void Update()
    {
        if (player == null) return;

        float d = Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Patrol:
                Patrol();
                // Si el jugador entra en el rango, empieza la persecución
                if (d < detectionRange) SetState(State.Chase);
                break;

            case State.Chase:
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);
                // Histéresis: pierde al jugador solo si se aleja bastante
                if (d > detectionRange * 1.5f)
                {
                    lastKnownPosition = player.position;
                    SetState(State.Search);
                }
                break;

            case State.Search:
                agent.speed = patrolSpeed;
                agent.SetDestination(lastKnownPosition);
                // Si lo vuelve a ver, persigue otra vez
                if (d < detectionRange)
                {
                    SetState(State.Chase);
                    break;
                }
                // Al llegar al último punto conocido, espera y vuelve a patrullar
                if (!agent.pathPending && agent.remainingDistance < 0.5f)
                {
                    searchTimer += Time.deltaTime;
                    if (searchTimer >= searchWait) SetState(State.Patrol);
                }
                break;
        }
    }

    void Patrol()
    {
        if (waypoints.Length == 0) return;
        agent.speed = patrolSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);
        // Al llegar al waypoint, pasa al siguiente (circular)
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    // Cambia de estado y da feedback visual con el color
    void SetState(State next)
    {
        state = next;
        searchTimer = 0f;
        if (rend == null) return;
        rend.material.color = next == State.Chase ? Color.red
                            : next == State.Search ? Color.yellow
                            : Color.green;
    }

    // Muestra los rangos al seleccionar el guardia en la Scene
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange * 1.5f);
    }
}