using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private const string MasterParam = "Vol_Master";
    private const string BGMParam = "Vol_BGM";
    private const string SFXParam = "Vol_SFX";

    private const string MasterPref = "MasterVolumePref";
    private const string BGMPref = "BGMVolumePref";
    private const string SFXPref = "SFXVolumePref";

    void Awake()
    {
        Time.timeScale = 1;
    }

    void Start()
    {
        // Load saved volumes or default to 0.75
        float masterVol = PlayerPrefs.GetFloat(MasterPref, 0.75f);
        float bgmVol = PlayerPrefs.GetFloat(BGMPref, 0.45f);
        float sfxVol = PlayerPrefs.GetFloat(SFXPref, 0.65f);

        masterSlider.value = masterVol;
        bgmSlider.value = bgmVol;
        sfxSlider.value = sfxVol;

        SetVolume(MasterParam, masterVol);
        SetVolume(BGMParam, bgmVol);
        SetVolume(SFXParam, sfxVol);

        // Add listeners
        masterSlider.onValueChanged.AddListener(value => OnSliderChanged(MasterParam, MasterPref, value));
        bgmSlider.onValueChanged.AddListener(value => OnSliderChanged(BGMParam, BGMPref, value));
        sfxSlider.onValueChanged.AddListener(value => OnSliderChanged(SFXParam, SFXPref, value));
    }

    void OnSliderChanged(string parameter, string prefKey, float value)
    {
        SetVolume(parameter, value);
        PlayerPrefs.SetFloat(prefKey, value);
        PlayerPrefs.Save();
    }

    void SetVolume(string parameter, float linearValue)
    {
        float dB = Mathf.Log10(Mathf.Clamp(linearValue, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameter, dB);
    }
}
