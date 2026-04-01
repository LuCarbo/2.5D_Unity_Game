using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class SelectorOpzioni : MonoBehaviour
{
    public TextMeshProUGUI testoOpzione;
    public string[] opzioni;
    private int indiceAttuale = 0;

    public UnityEvent<int> onCambioOpzione;

    public void Inizializza(string[] nuoveOpzioni, int indiceIniziale = 0)
    {
        opzioni = nuoveOpzioni;
        indiceAttuale = indiceIniziale;
        AggiornaTestoUI();
    }

    public void AvantI()
    {
        indiceAttuale = (indiceAttuale + 1) % opzioni.Length;
        AggiornaTestoUI();
        onCambioOpzione.Invoke(indiceAttuale);
    }

    public void Indietro()
    {
        indiceAttuale = (indiceAttuale - 1 + opzioni.Length) % opzioni.Length;
        AggiornaTestoUI();
        onCambioOpzione.Invoke(indiceAttuale);
    }

    private void AggiornaTestoUI()
    {
        testoOpzione.text = opzioni[indiceAttuale];
    }

    public int GetIndice() => indiceAttuale;
}