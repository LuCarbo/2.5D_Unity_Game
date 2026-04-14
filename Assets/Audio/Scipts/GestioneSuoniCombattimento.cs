using UnityEngine;

public class GestioneSuoniCombattimento : MonoBehaviour
{
    [Header("1. Fendente (A vuoto)")]
    public AudioClip[] suoniFendente;

    [Header("2. Impatto (Nemici)")]
    public AudioClip[] suoniImpattoNemico;

    [Header("3. Impatto (Muri/Oggetti)")]
    public AudioClip[] suoniImpattoMuro;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void RiproduciFendente()
    {
        if (suoniFendente != null && suoniFendente.Length > 0)
        {
            int indice = Random.Range(0, suoniFendente.Length);
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.PlayOneShot(suoniFendente[indice]);
        }
    }

    public void RiproduciImpatto(string tipoBersaglio)
    {
        AudioClip[] suoniDaUsare = null;

        if (tipoBersaglio == "Nemico")
            suoniDaUsare = suoniImpattoNemico;
        else if (tipoBersaglio == "Muro")
            suoniDaUsare = suoniImpattoMuro;

        if (suoniDaUsare != null && suoniDaUsare.Length > 0)
        {
            int indice = Random.Range(0, suoniDaUsare.Length);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(suoniDaUsare[indice]);
        }
    }

    public void CambiaSuoniArma(AudioClip[] nuoviFendenti, AudioClip[] nuoviImpattiNemico, AudioClip[] nuoviImpattiMuro)
    {
        suoniFendente = nuoviFendenti;
        suoniImpattoNemico = nuoviImpattiNemico;
        suoniImpattoMuro = nuoviImpattiMuro;
    }
}