using UnityEngine;

public class CicloGiornoNotte : MonoBehaviour
{
    [Header("Impostazioni Tempo")]
    [Tooltip("Quanto dura un giorno intero in secondi (es. 120 = 2 minuti)")]
    public float durataGiornoInSecondi = 120f;

    [Range(0f, 1f)]
    [Tooltip("L'ora attuale (0 = mezzanotte, 0.5 = mezzogiorno)")]
    public float tempoAttuale = 0.5f;

    [Header("Impostazioni Luce")]
    public Light luceSole;
    [Tooltip("Usa questo gradiente per cambiare colore al sole (es. arancione al tramonto)")]
    public Gradient coloreSole;
    public AnimationCurve intensitaSole;

    void Update()
    {
        // 1. Facciamo scorrere il tempo
        tempoAttuale += Time.deltaTime / durataGiornoInSecondi;
        if (tempoAttuale >= 1f) tempoAttuale = 0f; // Azzera a mezzanotte

        // 2. Calcoliamo la rotazione (360 gradi in un giorno intero)
        // Partiamo da -90 (mezzanotte) e ruotiamo sull'asse X
        float rotazioneSole = (tempoAttuale * 360f) - 90f;
        luceSole.transform.rotation = Quaternion.Euler(rotazioneSole, 170f, 0f);

        // 3. (Opzionale) Cambiamo colore e intensità
        if (coloreSole != null && intensitaSole != null)
        {
            luceSole.color = coloreSole.Evaluate(tempoAttuale);
            luceSole.intensity = intensitaSole.Evaluate(tempoAttuale);
        }
    }
}