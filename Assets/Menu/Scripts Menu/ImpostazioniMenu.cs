using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ImpostazioniMenu : MonoBehaviour
{
    [Header("Riferimenti Audio (assegna nel prefab)")]
    public AudioMixer audioMixer;

    private SelectorOpzioni selectorRisoluzione;
    private SelectorOpzioni selectorSchermo;
    private SelectorOpzioni selectorQualita;
    private Slider sliderMaster;
    private Slider sliderMusica;
    private Slider sliderEffetti;

    private Resolution[] risoluzioni;

    void Awake()
    {
        Debug.Log("Scena corrente di LogicaImpostazioniInGame: " + gameObject.scene.name);

        SelectorOpzioni[] tuttiSelector = Resources.FindObjectsOfTypeAll<SelectorOpzioni>();
        foreach (SelectorOpzioni s in tuttiSelector)
        {
            Debug.Log("Selector: " + s.gameObject.name + " | Scena: " + s.gameObject.scene.name);
            if (s.gameObject.scene.name == gameObject.scene.name)
            {
                if (s.gameObject.name == "SelectorRisoluzione") selectorRisoluzione = s;
                if (s.gameObject.name == "SelectorSchermo") selectorSchermo = s;
                if (s.gameObject.name == "SelectorQualita") selectorQualita = s;
            }
        }

        Slider[] tuttiSlider = Resources.FindObjectsOfTypeAll<Slider>();
        foreach (Slider s in tuttiSlider)
        {
            if (s.gameObject.scene.name == gameObject.scene.name)
            {
                if (s.gameObject.name == "Slider_Master") sliderMaster = s;
                if (s.gameObject.name == "Slider_Musica") sliderMusica = s;
                if (s.gameObject.name == "Slider_Effetti") sliderEffetti = s;
            }
        }

        if (selectorRisoluzione == null) Debug.LogError("SelectorRisoluzione non trovato!");
        else Debug.Log("SelectorRisoluzione trovato!");
        if (selectorSchermo == null) Debug.LogError("SelectorSchermo non trovato!");
        else Debug.Log("SelectorSchermo trovato!");
        if (selectorQualita == null) Debug.LogError("SelectorQualita non trovato!");
        else Debug.Log("SelectorQualita trovato!");
    }
    void Start()
    {
        // --- 1. RISOLUZIONI ---
        risoluzioni = Screen.resolutions;
        List<string> opzioniRisoluzione = new List<string>();
        int risoluzioneAttualeIndex = 0;

        for (int i = 0; i < risoluzioni.Length; i++)
        {
            string opzione = risoluzioni[i].width + " x " + risoluzioni[i].height;
            opzioniRisoluzione.Add(opzione);
            if (risoluzioni[i].width == Screen.currentResolution.width &&
                risoluzioni[i].height == Screen.currentResolution.height)
                risoluzioneAttualeIndex = i;
        }

        if (selectorRisoluzione != null)
        {
            int risoluzioneSalvata = PlayerPrefs.GetInt("RisoluzioneSalvata", risoluzioneAttualeIndex);
            selectorRisoluzione.Inizializza(opzioniRisoluzione.ToArray(), risoluzioneSalvata);
            selectorRisoluzione.onCambioOpzione.AddListener(ImpostaRisoluzione);
        }

        // --- 2. SCHERMO ---
        if (selectorSchermo != null)
        {
            int modalitaSalvata = PlayerPrefs.GetInt("SchermoSalvato", Screen.fullScreen ? 0 : 1);
            selectorSchermo.Inizializza(new string[] { "Schermo Intero", "Finestra" }, modalitaSalvata);
            selectorSchermo.onCambioOpzione.AddListener(ImpostaModalitaSchermo);
        }

        // --- 3. QUALITA' ---
        if (selectorQualita != null)
        {
            string[] livelliQualita = QualitySettings.names;
            int qualitaSalvata = PlayerPrefs.GetInt("QualitaSalvata", QualitySettings.GetQualityLevel());
            selectorQualita.Inizializza(livelliQualita, qualitaSalvata);
            selectorQualita.onCambioOpzione.AddListener(ImpostaQualita);
        }

        // --- 4. AUDIO ---
        if (sliderMaster != null)
        {
            sliderMaster.value = PlayerPrefs.GetFloat("VolMasterSalvato", 1f);
            sliderMaster.onValueChanged.AddListener(ImpostaVolumeMaster);
            ImpostaVolumeMaster(sliderMaster.value);
        }
        if (sliderMusica != null)
        {
            sliderMusica.value = PlayerPrefs.GetFloat("VolMusicaSalvato", 1f);
            sliderMusica.onValueChanged.AddListener(ImpostaVolumeMusica);
            ImpostaVolumeMusica(sliderMusica.value);
        }
        if (sliderEffetti != null)
        {
            sliderEffetti.value = PlayerPrefs.GetFloat("VolEffettiSalvato", 1f);
            sliderEffetti.onValueChanged.AddListener(ImpostaVolumeEffetti);
            ImpostaVolumeEffetti(sliderEffetti.value);
        }
    }

    // ==========================================
    // FUNZIONI DELLO SCHERMO E DELLA GRAFICA
    // ==========================================

    public void ImpostaRisoluzione(int indice)
    {
        Resolution res = risoluzioni[indice];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("RisoluzioneSalvata", indice);
        PlayerPrefs.Save();
    }

    public void ImpostaModalitaSchermo(int indice)
    {
        if (indice == 0) Screen.fullScreen = true;
        else if (indice == 1) Screen.fullScreen = false;
        PlayerPrefs.SetInt("SchermoSalvato", indice);
        PlayerPrefs.Save();
    }

    public void ImpostaQualita(int indice)
    {
        QualitySettings.SetQualityLevel(indice);
        PlayerPrefs.SetInt("QualitaSalvata", indice);
        PlayerPrefs.Save();
    }

    // ==========================================
    // FUNZIONI DELL'AUDIO (Decibel)
    // ==========================================

    public void ImpostaVolumeMaster(float volume)
    {
        audioMixer.SetFloat("MasterVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("VolMasterSalvato", volume);
        PlayerPrefs.Save();
    }

    public void ImpostaVolumeMusica(float volume)
    {
        audioMixer.SetFloat("MusicaVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("VolMusicaSalvato", volume);
        PlayerPrefs.Save();
    }

    public void ImpostaVolumeEffetti(float volume)
    {
        audioMixer.SetFloat("EffettiVol", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("VolEffettiSalvato", volume);
        PlayerPrefs.Save();
    }
}