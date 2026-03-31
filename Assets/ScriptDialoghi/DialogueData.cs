using UnityEngine;

[CreateAssetMenu(fileName = "NuovoDialogo", menuName = "Sistema Dialoghi/Dialogo")]
public class DialogueData : ScriptableObject
{
   

    [TextArea(3, 10)] // Rende il box di testo più grande nell'editor
    public string[] frasi;
}