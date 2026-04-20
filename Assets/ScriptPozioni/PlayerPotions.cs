using UnityEngine;

public class PlayerPotions : MonoBehaviour
{
    [Header("Inventario Pozioni")]
    public int potionCount = 0; // All'inizio del gioco non hai pozioni
    public int healAmount = 20; // Quanta vita restituisce una pozione
    public KeyCode healKey = KeyCode.H; // Tasto da premere per curarsi

   
    private PlayerVFXHandler vfxHandler; // Riferimento allo script per gli effetti visivi (se vuoi aggiungere effetti quando usi la pozione)

    private Health healthScript;

    void Start()
    {
        // Recupera automaticamente lo script Health attaccato allo stesso GameObject (il Giocatore)
        healthScript = GetComponent<Health>();

        vfxHandler = GetComponent<PlayerVFXHandler>();
    }

    void Update()
    {
        // Controlla se il giocatore preme il tasto H e ha almeno una pozione nello zaino
        if (Input.GetKeyDown(healKey) && potionCount > 0)
        {
            UsePotion();
        }
    }

    // Questa è la funzione che lo script GivePotion dell'NPC chiamerà per consegnarti la roba
    public void AddPotion(int amount)
    {
        potionCount += amount;
        Debug.Log("Hai ricevuto una pozione! Pozioni totali: " + potionCount);
    }

    private void UsePotion()
    {
        // Prima di sprecare la pozione, controlliamo se la vita non è già al massimo
        if (healthScript.currentHealth < healthScript.maxHealth)
        {
            potionCount--; // Togli una pozione dall'inventario
            healthScript.ChangeHealth(healAmount); // Chiama lo script universale per alzare la vita

            if (vfxHandler != null)
            {
                vfxHandler.SpawnHealAura();
            }

            Debug.Log("Pozione usata! Vita attuale: " + healthScript.currentHealth);
        }
        else
        {
            Debug.Log("Salute già al massimo, non sprecare la pozione!");
        }
    }
}