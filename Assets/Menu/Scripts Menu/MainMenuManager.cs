using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Pannelli UI")]
    public GameObject pannelloPrincipale;
    public GameObject pannelloImpostazioni;

    void Start()
    {
        // Appena parte il gioco, assicurati che il menu sia acceso
        pannelloPrincipale.SetActive(true);
        pannelloImpostazioni.SetActive(false);

        // IL TRUCCO: Congela il tempo! Così il gioco dietro è fermo.
        Time.timeScale = 0f;
    }

    public void Gioca()
    {
        // Spegne il menu
        pannelloPrincipale.SetActive(false);

        // Scongela il tempo: il gioco parte!
        Time.timeScale = 1f;
    }

    public void ApriImpostazioni()
    {
        pannelloPrincipale.SetActive(false);
        pannelloImpostazioni.SetActive(true);
    }

    public void ChiudiImpostazioni()
    {
        pannelloImpostazioni.SetActive(false);
        pannelloPrincipale.SetActive(true);
    }

    public void Esci()
    {
        Debug.Log("Uscita dal gioco...");
        Application.Quit();
    }
}