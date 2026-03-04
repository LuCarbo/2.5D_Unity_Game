using UnityEngine;

public class GestionePausa : MonoBehaviour
{
    [Header("Trascina qui il Pannello Impostazioni")]
    public GameObject pannelloImpostazioni;

    private bool giocoInPausa = false;

    void Update()
    {
        // Se premiamo il tasto ESC (Escape) sulla tastiera
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (giocoInPausa)
            {
                RiprendiGioco();
            }
            else
            {
                MettiInPausa();
            }
        }
    }

    public void MettiInPausa()
    {
        pannelloImpostazioni.SetActive(true); // Accende il menu
        Time.timeScale = 0f; // FERMA IL TEMPO NEL GIOCO
        giocoInPausa = true;

        // Mostra e sblocca il cursore del mouse per poter cliccare
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RiprendiGioco()
    {
        pannelloImpostazioni.SetActive(false); // Spegne il menu
        Time.timeScale = 1f; // FA RIPARTIRE IL TEMPO
        giocoInPausa = false;

        // Se il tuo gioco nasconde il mouse mentre cammini, togli i commenti (//) dalle due righe sotto:
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }
}