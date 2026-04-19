using UnityEngine;

public class NPCVaiVia : MonoBehaviour
{
    public Transform destinazione;
    public float velocita = 3f;
    private bool deveCamminare = false;

    public void Cammina()
    {
        deveCamminare = true;
    }

    void Update()
    {
        if (deveCamminare && destinazione != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinazione.position, velocita * Time.deltaTime);
        }
    }
}