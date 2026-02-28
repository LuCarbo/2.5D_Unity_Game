using UnityEngine;
using UnityEngine.AI; // FONDAMENTALE: ci serve per usare il NavMesh

[RequireComponent(typeof(NavMeshAgent))] // Aggiunge in automatico il componente necessario
public class AnimalsMovement : MonoBehaviour
{
    [Header("Impostazioni Area")]
    [Tooltip("Il raggio massimo entro cui l'animale può allontanarsi dal punto di partenza")]
    public float wanderRadius = 10f;

    [Tooltip("Ogni quanti secondi l'animale sceglie una nuova destinazione")]
    public float wanderTimer = 4f;

    private NavMeshAgent _agent;
    private float _timer;
    private Vector3 _startPosition; // Memorizziamo il centro della sua "casa"

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Salviamo la posizione iniziale come centro dell'area di vagabondaggio
        _startPosition = transform.position;

        // Impostiamo il timer al massimo così si muove appena avvii il gioco
        _timer = wanderTimer;
    }

    void Update()
    {
        _timer += Time.deltaTime;

        // Se è scaduto il tempo, scegliamo un nuovo punto
        if (_timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(_startPosition, wanderRadius, -1);
            _agent.SetDestination(newPos);
            _timer = 0; // Resetta il timer
        }
    }

    // ===================================================================
    // METODO PER TROVARE UN PUNTO CASUALE SUL NAVMESH
    // ===================================================================
    Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
    {
        // Genera un punto casuale in un cerchio 2D e lo trasforma in 3D
        Vector2 randomPoint2D = Random.insideUnitCircle * distance;
        Vector3 randomDirection = new Vector3(randomPoint2D.x, 0, randomPoint2D.y);

        randomDirection += origin;

        NavMeshHit navHit;
        // Chiede a Unity: "Qual è il punto camminabile più vicino a questa direzione a caso?"
        NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

        return navHit.position;
    }

    // ===================================================================
    // DEBUG VISIVO NELL'EDITOR
    // ===================================================================
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        // Se il gioco è in play usiamo startPosition, altrimenti usiamo la posizione attuale
        Vector3 center = Application.isPlaying ? _startPosition : transform.position;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}
