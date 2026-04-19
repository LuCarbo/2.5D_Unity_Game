using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ImpostazioniMenu : MonoBehaviour
{
    [Header("Riferimenti Audio")]
    public AudioMixer audioMixer;

    [Header("Selector")]
    public SelectorOpzioni selectorRisoluzione;
    public SelectorOpzioni selectorSchermo;
    public SelectorOpzioni selectorQualita;

    [Header("Slider Audio")]
    public Slider sliderMaster;
    public Slider sliderMusica;
    public Slider sliderEffetti;

    private Resolution[] risoluzioni;

    void Awake()
    {
        
    }
    void Start()
    {

        // --- RISOLUZIONI ---
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

        if (selectorSchermo != null)
        {
            int modalitaSalvata = PlayerPrefs.GetInt("SchermoSalvato", Screen.fullScreen ? 0 : 1);
            selectorSchermo.Inizializza(new string[] { "Schermo Intero", "Finestra" }, modalitaSalvata);
            selectorSchermo.onCambioOpzione.AddListener(ImpostaModalitaSchermo);
        }

        if (selectorQualita != null)
        {
            string[] livelliQualita = QualitySettings.names;
            int qualitaSalvata = PlayerPrefs.GetInt("QualitaSalvata", QualitySettings.GetQualityLevel());
            selectorQualita.Inizializza(livelliQualita, qualitaSalvata);
            selectorQualita.onCambioOpzione.AddListener(ImpostaQualita);
        }

        // --- AUDIO: imposta i valori di default a 1 (massimo) ---
        if (sliderMaster != null)
        {
            sliderMaster.minValue = 0.0001f;
            sliderMaster.maxValue = 1f;
            sliderMaster.value = PlayerPrefs.GetFloat("VolMasterSalvato", 1f);
            sliderMaster.onValueChanged.AddListener(ImpostaVolumeMaster);
        }
        if (sliderMusica != null)
        {
            sliderMusica.minValue = 0.0001f;
            sliderMusica.maxValue = 1f;
            sliderMusica.value = PlayerPrefs.GetFloat("VolMusicaSalvato", 1f);
            sliderMusica.onValueChanged.AddListener(ImpostaVolumeMusica);
        }
        if (sliderEffetti != null)
        {
            sliderEffetti.minValue = 0.0001f;
            sliderEffetti.maxValue = 1f;
            sliderEffetti.value = PlayerPrefs.GetFloat("VolEffettiSalvato", 1f);
            sliderEffetti.onValueChanged.AddListener(ImpostaVolumeEffetti);
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
        float valoreDb = Mathf.Log10(volume) * 20;
        if (!audioMixer.SetFloat("MasterVol", valoreDb))
        {
            Debug.LogError("Parametro 'MasterVol' non trovato nell'AudioMixer!");
        }
        else
        {
            Debug.Log("MasterVol impostato a: " + valoreDb);
        }
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