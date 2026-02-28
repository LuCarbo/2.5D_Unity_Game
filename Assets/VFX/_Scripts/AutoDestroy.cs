using UnityEngine; // <--- Questa è la riga che mancava!

public class AutoDestroy : MonoBehaviour
{
    void Start()
    {
        // Cerchiamo l'Animator nell'oggetto polvere
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            // Ottieni informazioni sullo stato corrente dell'animazione (sul layer 0)
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            
            // Distruggi l'oggetto esattamente alla fine della durata dell'animazione
            Destroy(gameObject, stateInfo.length); 
        }
        else
        {
            // Se per caso ti sei dimenticato l'animator, distruggi dopo 1 secondo di sicurezza
            Destroy(gameObject, 0.5f); 
        }
    }
}