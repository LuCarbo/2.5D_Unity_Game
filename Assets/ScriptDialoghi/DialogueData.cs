using UnityEngine;

[CreateAssetMenu(fileName = "NuovoDialogo", menuName = "Sistema Dialoghi/Dialogo")]
public class DialogueData : ScriptableObject
{
    public string nomePersonaggio;

    [TextArea(3, 10)] // Rende il box di testo più grande nell'editor
    public string[] frasi;
}