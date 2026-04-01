using UnityEngine;
using UnityEngine.SceneManagement;

public class GestionePausa : MonoBehaviour
{
    [Header("Pannelli del Menu di Pausa")]
    public GameObject pannelloPausa;
    public GameObject pannelloImpostazioni;

    private bool giocoInPausa = false;
    private bool impostazioniAperte = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (impostazioniAperte)
            {
                ChiudiImpostazioni();
            }
            else if (giocoInPausa)
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
        pannelloPausa.SetActive(true);
        pannelloImpostazioni.SetActive(false);
        Time.timeScale = 0f;
        giocoInPausa = true;
        impostazioniAperte = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RiprendiGioco()
    {
        pannelloPausa.SetActive(false);
        pannelloImpostazioni.SetActive(false);
        Time.timeScale = 1f;
        giocoInPausa = false;
        impostazioniAperte = false;
    }

    public void ApriImpostazioni()
    {
        pannelloPausa.SetActive(false);
        pannelloImpostazioni.SetActive(true);
        impostazioniAperte = true;
    }

    public void ChiudiImpostazioni()
    {
        pannelloImpostazioni.SetActive(false);
        pannelloPausa.SetActive(true);
        impostazioniAperte = false;
        Debug.Log("ChiudiImpostazioni chiamato - PannelloPausa attivo: " + pannelloPausa.activeSelf);
    }

    public void TornaAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}