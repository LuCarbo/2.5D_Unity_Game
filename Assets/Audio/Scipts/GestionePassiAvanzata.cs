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

    [Header("Sicurezza Anti-Accavallamento")]
    [Tooltip("Tempo minimo in secondi tra un passo e l'altro")]
    public float ritardoMinimo = 0.25f; // Il trucco per bloccare i doppi suoni
    private float tempoUltimoPasso = 0f;

    [Header("Suoni")]
    public SuonoTerreno[] suoniTerreni;
    public AudioClip[] passiDefault;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void RiproduciPasso()
    {
        // SISTEMA ANTI-ACCAVALLAMENTO: 
        // Se è passato troppo poco tempo dall'ultimo passo, esci subito e non fare nulla.
        if (Time.time - tempoUltimoPasso < ritardoMinimo)
        {
            return;
        }

        Vector3 origineBase = puntoPiedi != null ? puntoPiedi.position : transform.position;
        Vector3 origineRaggio = origineBase + Vector3.up * 0.5f;

        // Ho aumentato il margine da 0.5 a 1.0f per evitare "vuoti" sulle discese ripide
        float raggioTotale = lunghezzaRaggio + 1.0f;

        if (Physics.Raycast(origineRaggio, Vector3.down, out RaycastHit hit, raggioTotale))
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

        // Default se il raggio non legge il terreno
        RiproduciSuono(passiDefault);
    }

    private void RiproduciSuono(AudioClip[] arraySuoni)
    {
        if (arraySuoni != null && arraySuoni.Length > 0)
        {
            // Aggiorna il timer nel momento esatto in cui decide di suonare
            tempoUltimoPasso = Time.time;

            int indice = Random.Range(0, arraySuoni.Length);

            // Variamo leggermente il "Pitch" (l'intonazione) per rendere i passi meno robotici
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