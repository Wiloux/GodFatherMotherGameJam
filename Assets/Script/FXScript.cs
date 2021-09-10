using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXScript : MonoBehaviour {
    public AudioClip audio;
    public AudioClip voiceLinePunk;
    public AudioClip voiceLineManInBlack;

    public void PlayFXSound() {
        SoundManager.Instance.PlaySoundEffect(audio);
    }

    public void PlayPunkVoice() {
        SoundManager.Instance.PlaySoundEffect(voiceLinePunk);
    }

    public void PlayMIBVoice() {
        SoundManager.Instance.PlaySoundEffect(voiceLineManInBlack);
    }

    public void CanRestart() {
        GameManager.instance.canRestart = true;
    }
}
