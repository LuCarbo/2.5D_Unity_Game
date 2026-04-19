using UnityEngine;

public class BossMovement : MonoBehaviour
{
    [Header("Impostazioni Movimento")]
    public float moveSpeed = 3f;

    [Header("Riferimenti Grafici")]
    public Transform visualPivot;
    public Transform attackPivot;

    [Tooltip("Spunta questa casella se il PNG originale guarda verso SINISTRA")]
    public bool isSpriteFacingLeft = false;

    [Tooltip("Usa questo per correggere lo sfasamento. Prova 90, -90, o 180!")]
    public float attackAngleOffset = -90f; // <--- NUOVA VARIABILE

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void MoveTowardsPlayer(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector3(direction.x * moveSpeed, rb.linearVelocity.y, direction.z * moveSpeed);
        FacePlayer(target.position);
    }

    public void StopMoving()
    {
        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
    }

    private void FacePlayer(Vector3 targetPos)
    {
        // 1. FLIP VISIVO DELLO SPRITE
        if (visualPivot != null)
        {
            Vector3 currentScale = visualPivot.localScale;
            float absoluteX = Mathf.Abs(currentScale.x);

            if (targetPos.x > transform.position.x)
                currentScale.x = isSpriteFacingLeft ? -absoluteX : absoluteX;
            else if (targetPos.x < transform.position.x)
                currentScale.x = isSpriteFacingLeft ? absoluteX : -absoluteX;

            visualPivot.localScale = currentScale;
        }

        // 2. ROTAZIONE DELL'ATTACK POINT A 8 DIREZIONI
        if (attackPivot != null)
        {
            Vector3 directionToPlayer = targetPos - transform.position;

            float angle = Mathf.Atan2(directionToPlayer.x, directionToPlayer.z) * Mathf.Rad2Deg;

            // Aggiungiamo il tuo Offset qui prima di arrotondare!
            angle += attackAngleOffset;

            float snappedAngle = Mathf.Round(angle / 45f) * 45f;

            attackPivot.rotation = Quaternion.Euler(0f, snappedAngle, 0f);
        }
    }
    public void LockMovement()
    {
        // Blocca fisicamente la posizione su X e Z, ma lascia libera la Y (così la gravità funziona ancora).
        // FreezeRotation assicura che il boss non cada a faccia in giù.
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
    }

    public void UnlockMovement()
    {
        // Rimuove il blocco da X e Z per farlo tornare a camminare.
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}