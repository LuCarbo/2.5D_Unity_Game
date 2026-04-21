using UnityEngine;
using TMPro; // Necessario per comandare il TextMeshPro

public class PlayerPotions : MonoBehaviour
{
    [Header("Inventario Pozioni")]
    public int potionCount = 0;
    public int healAmount = 20;
    public KeyCode healKey = KeyCode.H;

    [Header("UI Pozioni")]
    [Tooltip("Trascina qui il Testo_Numero dalla tua Hierarchy")]
    public TextMeshProUGUI testoPozioni; // Lo slot per il testo a schermo

    private Health healthScript;
    private PlayerVFXHandler vfxHandler;

    void Start()
    {
        healthScript = GetComponent<Health>();
        vfxHandler = GetComponent<PlayerVFXHandler>();

        // Aggiorna la grafica appena parte il livello (mostrerà "x 0")
        AggiornaTestoUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(healKey) && potionCount > 0)
        {
            UsePotion();
        }
    }

    public void AddPotion(int amount)
    {
        potionCount += amount;
        Debug.Log("Hai ricevuto una pozione! Pozioni totali: " + potionCount);

        // NUOVO: Aggiorna lo schermo quando l'NPC ti dà la pozione
        AggiornaTestoUI();
    }

    private void UsePotion()
    {
        if (healthScript.currentHealth < healthScript.maxHealth)
        {
            potionCount--;
            healthScript.ChangeHealth(healAmount);

            if (vfxHandler != null)
            {
                vfxHandler.SpawnHealAura();
            }

            Debug.Log("Pozione usata! Vita attuale: " + healthScript.currentHealth);

            // NUOVO: Aggiorna lo schermo (il numero scende)
            AggiornaTestoUI();
        }
        else
        {
            Debug.Log("Salute già al massimo, non sprecare la pozione!");
        }
    }

    // --- NUOVA FUNZIONE PER LA GRAFICA ---
    private void AggiornaTestoUI()
    {
        // Controlla che tu abbia inserito il testo nell'Inspector per evitare crash
        if (testoPozioni != null)
        {
            // Cambia la scritta a schermo unendo la lettera "x " al numero reale di pozioni
            testoPozioni.text = potionCount.ToString();
        }
    }
}