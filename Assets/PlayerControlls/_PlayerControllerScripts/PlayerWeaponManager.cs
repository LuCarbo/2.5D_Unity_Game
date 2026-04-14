using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
    private Animator animator;
    private RuntimeAnimatorController baseController;

    [Header("Audio Combattimento")]
    // 1. Riferimento allo script che gestisce l'audio
    public GestioneSuoniCombattimento suoniCombattimento;

    [Header("Nuovi Suoni Spada")]
    // 2. I suoni specifici che la spada si porta dietro
    public AudioClip[] fendentiSpada;
    public AudioClip[] impattiSpadaCarne;
    public AudioClip[] impattiSpadaMuro; // --- AGGIUNTO: Il suono metallico contro il muro ---

    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null)
        {
            baseController = animator.runtimeAnimatorController;
        }
    }

    public void EquipWeapon(AnimatorOverrideController newWeaponController)
    {
        if (animator != null && newWeaponController != null)
        {
            animator.runtimeAnimatorController = newWeaponController;
            Debug.Log("Animazioni spada equipaggiate!");

            // --- INVIA I 3 SUONI DELLA SPADA AL GESTORE AUDIO ---
            if (suoniCombattimento != null)
            {
                // Ora gli passiamo tutti e 3 gli array richiesti!
                suoniCombattimento.CambiaSuoniArma(fendentiSpada, impattiSpadaCarne, impattiSpadaMuro);
                Debug.Log("Suoni spada equipaggiati!");
            }
        }
    }

    public void UnequipWeapon()
    {
        if (animator != null && baseController != null)
        {
            animator.runtimeAnimatorController = baseController;
            // Se in futuro vorrai, potrai rimettere i suoni dei pugni qui
        }
    }
}