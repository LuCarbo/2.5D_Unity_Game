using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cam;

    void Start() => cam = Camera.main.transform;

    void LateUpdate()
    {
        // Fa sì che la nuvoletta sia sempre rivolta verso la camera
        transform.LookAt(transform.position + cam.forward);
    }
}