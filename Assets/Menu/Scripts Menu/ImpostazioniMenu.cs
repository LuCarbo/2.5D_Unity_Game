using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ImpostazioniMenu : MonoBehaviour
{
    [Header("Riferimenti Schermo e Grafica")]
    public TMP_Dropdown dropdownRisoluzione;
    public TMP_Dropdown dropdownModalitaSchermo;
    public TMP_Dropdown dropdownQualita;

    [Header("Riferimenti Audio")]
    public AudioMixer audioMixer;
    public Slider sliderMaster;
    public Slider sliderMusica;
    public Slider sliderEffetti;

    private Resolution[] risoluzioni;

    void Start()
    {
        // --- 1. GESTIONE E CARICAMENTO RISOLUZIONI ---
        risoluzioni = Screen.resolutions;
        dropdownRisoluzione.ClearOptions();

        List<string> opzioni = new List<string>();
        int risoluzioneAttualeIndex = 0;

        for (int i = 0; i < risoluzioni.Length; i++)
        {
            string opzione = risoluzioni[i].width + " x " + risoluzioni[i].height;
            opzioni.Add(opzione);

            if (risoluzioni[i].width == Screen.currentResolution.width &&
                risoluzioni[i].height == Screen.currentResolution.height)
            {
                risoluzioneAttualeIndex = i;
            }
        }
        dropdownRisoluzione.AddOptions(opzioni);

        // Se c'è una risoluzione salvata, la carica. Altrimenti usa quella del monitor.
        if (PlayerPrefs.HasKey("RisoluzioneSalvata"))
        {
            int indiceSalvato = PlayerPrefs.GetInt("RisoluzioneSalvata");
            dropdownRisoluzione.value = indiceSalvato;
            ImpostaRisoluzione(indiceSalvato);
        }
        else
        {
            dropdownRisoluzione.value = risoluzioneAttualeIndex;
        }
        dropdownRisoluzione.RefreshShownValue();

        // --- 2. CARICAMENTO MODALITA' SCHERMO ---
        if (PlayerPrefs.HasKey("SchermoSalvato"))
        {
            int modalitaSalvata = PlayerPrefs.GetInt("SchermoSalvato");
            dropdownModalitaSchermo.value = modalitaSalvata;
            ImpostaModalitaSchermo(modalitaSalvata);
        }
        else
        {
            dropdownModalitaSchermo.value = Screen.fullScreen ? 0 : 1;
        }
        dropdownModalitaSchermo.RefreshShownValue();

        // --- 3. CARICAMENTO QUALITA' GRAFICA ---
        if (dropdownQualita != null)
        {
            if (PlayerPrefs.HasKey("QualitaSalvata"))
            {
                int qualitaSalvata = PlayerPrefs.GetInt("QualitaSalvata");
                dropdownQualita.value = qualitaSalvata;
                ImpostaQualita(qualitaSalvata);
            }
            else
            {
                dropdownQualita.value = QualitySettings.GetQualityLevel();
            }
            dropdownQualita.RefreshShownValue();
        }

        // --- 4. CARICAMENTO AUDIO ---
        // Legge il taccuino: se non c'è nulla salvato, mette il volume al massimo (1f)
        sliderMaster.value = PlayerPrefs.GetFloat("VolMasterSalvato", 1f);
        sliderMusica.value = PlayerPrefs.GetFloat("VolMusicaSalvato", 1f);
        sliderEffetti.value = PlayerPrefs.GetFloat("VolEffettiSalvato", 1f);

        // Applica i volumi fisicamente al Mixer all'avvio
        ImpostaVolumeMaster(sliderMaster.value);
        ImpostaVolumeMusica(sliderMusica.value);
        ImpostaVolumeEffetti(sliderEffetti.value);
    }

    // ==========================================
    // FUNZIONI DELLO SCHERMO E DELLA GRAFICA
    // ==========================================

    public void ImpostaRisoluzione(int indice)
    {
        Resolution res = risoluzioni[indice];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);

        // Salva la scelta!
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