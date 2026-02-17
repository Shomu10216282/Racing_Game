using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMasterVolume(float level)
    {
        //audioMixer.SetFloat("Master_Vol", level);
        audioMixer.SetFloat("Master_Vol", Mathf.Log10(level) * 20f);
    }

    public void SetSFXVolume(float level)
    {
        //audioMixer.SetFloat("SFX_Vol", level);
        audioMixer.SetFloat("SFX_Vol", Mathf.Log10(level) * 20f);
    }

    public void SetMusicVolume(float level)
    {
        //audioMixer.SetFloat("Music_Vol", level);
        audioMixer.SetFloat("Music_Vol", Mathf.Log10(level) * 20f);
    }
}
