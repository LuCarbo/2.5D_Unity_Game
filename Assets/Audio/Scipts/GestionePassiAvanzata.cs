using UnityEngine;

[System.Serializable]
public class SuonoTerreno
{
    public TerrainLayer layer;
    public AudioClip[] suoniPassi;
}

public class GestionePassiAvanzata : MonoBehaviour
{
    [Header("Impostazioni Passi")]
    public Transform puntoPiedi;
    public float lunghezzaRaggio = 0.5f;
    public LayerMask layerDaIgnorare;

    [Header("Il Metronomo (Ritmo dei passi)")]
    public float intervalloCamminata = 0.35f;
    public float intervalloCorsa = 0.22f;

    private float timerPasso;

    [Header("Suoni")]
    public SuonoTerreno[] suoniTerreni;
    public AudioClip[] passiDefault;

    private AudioSource audioSource;
    private CharacterController characterController;
    private PlayerInputHandler _input;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputHandler>();
        timerPasso = intervalloCamminata;
    }

    void Update()
    {
        float velocitaAttuale = _input.MoveInput.magnitude;
        bool staCorreendo = _input.IsRunning;

        Vector3 origineBase = puntoPiedi != null ? puntoPiedi.position : transform.position;
        Vector3 origineRaggio = origineBase + Vector3.up * 0.5f;
        float raggioTotale = lunghezzaRaggio + 1.0f;

        bool aTerra = Physics.Raycast(origineRaggio, Vector3.down, raggioTotale, ~layerDaIgnorare)
              && characterController.isGrounded;
        Debug.DrawRay(origineRaggio, Vector3.down * raggioTotale, aTerra ? Color.green : Color.red);

        if (velocitaAttuale > 0.1f && aTerra)
        {
            timerPasso -= Time.deltaTime;

            if (timerPasso <= 0f)
            {
                EseguiControlloTerreno(origineRaggio, raggioTotale);
                timerPasso = staCorreendo ? intervalloCorsa : intervalloCamminata;
            }
        }
        else if (!aTerra)
        {
            // In aria: timer in pausa
        }
        else
        {
            // Fermo a terra: azzera cosi il primo passo parte subito
            timerPasso = 0f;
        }
    }

    private void EseguiControlloTerreno(Vector3 origineRaggio, float raggioTotale)
    {
        if (Physics.Raycast(origineRaggio, Vector3.down, out RaycastHit hit, raggioTotale, ~layerDaIgnorare))
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();

            if (terrain != null)
            {
                TerrainLayer layerCalpestato = OttieniLayerCalpestato(hit.point, terrain);

                if (layerCalpestato != null)
                {
                    foreach (SuonoTerreno st in suoniTerreni)
                    {
                        if (st.layer == layerCalpestato)
                        {
                            RiproduciSuono(st.suoniPassi);
                            return;
                        }
                    }
                }
            }
        }

        RiproduciSuono(passiDefault);
    }

    private void RiproduciSuono(AudioClip[] arraySuoni)
    {
        if (arraySuoni != null && arraySuoni.Length > 0)
        {
            int indice = Random.Range(0, arraySuoni.Length);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(arraySuoni[indice]);
        }
    }

    private TerrainLayer OttieniLayerCalpestato(Vector3 puntoImpatto, Terrain terrain)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 posTerrain = terrain.transform.position;

        float mapX = ((puntoImpatto.x - posTerrain.x) / terrainData.size.x) * terrainData.alphamapWidth;
        float mapZ = ((puntoImpatto.z - posTerrain.z) / terrainData.size.z) * terrainData.alphamapHeight;

        if (mapX < 0 || mapX >= terrainData.alphamapWidth || mapZ < 0 || mapZ >= terrainData.alphamapHeight)
            return null;

        float[,,] splatmapData = terrainData.GetAlphamaps((int)mapX, (int)mapZ, 1, 1);

        float mixPiuForte = 0f;
        int indicePiuForte = 0;

        for (int i = 0; i < terrainData.alphamapLayers; i++)
        {
            if (splatmapData[0, 0, i] > mixPiuForte)
            {
                mixPiuForte = splatmapData[0, 0, i];
                indicePiuForte = i;
            }
        }

        return terrainData.terrainLayers[indicePiuForte];
    }
}