using UnityEngine;

public class GivePotion : MonoBehaviour
{
    [Header("Impostazioni Pozione")]
    public int quantitaDaConsegnare = 1;
    public bool haGiaDatoPozione = false; // Per evitare che te le dia all'infinito parlandogli di nuovo

    [Header("Riferimento Giocatore")]
    public PlayerPotions inventarioGiocatore; // Trascina qui il Giocatore dall'Inspector

    // Questa è la funzione che verrà chiamata quando il dialogo finisce
    public void Consegna()
    {
        if (!haGiaDatoPozione && inventarioGiocatore != null)
        {
            inventarioGiocatore.AddPotion(quantitaDaConsegnare);
            haGiaDatoPozione = true;
            Debug.Log("Pozione consegnata dopo il dialogo!");
        }
        else if (haGiaDatoPozione)
        {
            Debug.Log("Questo NPC ti ha già dato la sua pozione.");
        }
    }
}