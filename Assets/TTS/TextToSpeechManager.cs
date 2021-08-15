using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(AudioSource))]
public class TextToSpeechManager : MonoBehaviour {

  public static TextToSpeechManager Instance;
  private AudioSource audioSource;
  private const string API_CALL =
    "https://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q={0}&tl=En-gb";
  // private Dictionary<string,AudioClip> cachedSound = new Dictionary<string,AudioClip>();
  private HashSet<string> playingSound = new HashSet<string>();

  public const string SAVE_SOUND_DIR = "TTSSOUNDCACHED";

  private string LocalSoundFilePath(string text) {
    return Path.Combine(Application.persistentDataPath, SAVE_SOUND_DIR, $"{text}.mp3");
  }

  private string LocalSoundDirPath => Path.Combine(Application.persistentDataPath, SAVE_SOUND_DIR);
  private void Awake() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad(this.gameObject);
    } else {
      Destroy(this.gameObject);
    }

    audioSource = GetComponent<AudioSource>();
  }

  public void PlayTextToSpeech(string text) {
    text = text.ToLower();
    text.TrimEnd(' ');
    text.TrimStart(' ');
    if (playingSound.Contains(text)) {
      return;
    } /*else if(cachedSound.ContainsKey(text)) {
      audioSource.PlayOneShot(cachedSound[text]);
    }*/ else {
      StartCoroutine(GetAudioClip(text));
    }
  }
  
  
  IEnumerator GetAudioClip(string text) {
    var isLocal = IsAudioClipExistLocal(text);
    var uri = string.Format(API_CALL,text);
    if (isLocal) {
      uri = new Uri(LocalSoundFilePath(text)).AbsoluteUri;
    }
    Debug.Log("LoadFileFromPath: " + uri);
    using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(uri, AudioType.MPEG)) {
      playingSound.Add(text);
      yield return www.SendWebRequest();

      if (www.isHttpError || www.isNetworkError)
      {
        Debug.LogError(www.error);
        playingSound.Remove(text);

      }
      else {
        if(!isLocal)
          SaveAudioClipToLocal(text, www);
        AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
        audioSource.PlayOneShot(myClip);
        playingSound.Remove(text);
      }
    }
  }

  private bool IsAudioClipExistLocal(string text) {
    return File.Exists(LocalSoundFilePath(text));
  }

  private void SaveAudioClipToLocal(string text, UnityWebRequest requestedData) {
    if (!Directory.Exists(LocalSoundDirPath)) {
      Directory.CreateDirectory(LocalSoundDirPath);
    }
    Debug.Log("SaveFileToPath: " + LocalSoundFilePath(text));
    System.IO.File.WriteAllBytes (LocalSoundFilePath(text), requestedData.downloadHandler.data);
  }
}
