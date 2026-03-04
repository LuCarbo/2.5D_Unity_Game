using UnityEngine;
using UnityEngine.SceneManagement; // Serve per caricare i livelli

public class MainMenuManager : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloImpostazioni;

    // --- FUNZIONI DEL MENU PRINCIPALE ---

    public void Gioca()
    {
        // Carica la scena di gioco. Mettiamo l'Indice 1 (ti spiego sotto cos'è)
        SceneManager.LoadScene(1);
    }

    public void ApriImpostazioni()
    {
        // Spegne i bottoni principali e accende il menu impostazioni
        pannelloPrincipale.SetActive(false);
        pannelloImpostazioni.SetActive(true);
    }

    public void Esci()
    {
        Debug.Log("Il gioco si sta chiudendo...");
        Application.Quit(); // Funziona a gioco esportato
    }

    // --- FUNZIONI DEL MENU IMPOSTAZIONI ---

    public void ChiudiImpostazioni()
    {
        // Fa l'esatto opposto: spegne le impostazioni e riaccende il menu principale
        pannelloImpostazioni.SetActive(false);
        pannelloPrincipale.SetActive(true);
    }
}