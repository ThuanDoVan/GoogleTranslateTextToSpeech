using UnityEngine;
using UnityEngine.UI;

public class GUIDemo : MonoBehaviour {
  // private string textToSpeech = "EDIT ME";
  // void OnGUI() {
  //
  //   GUI.Box(new Rect(Screen.width / 2 - 250, Screen.height / 2 - 400, 500, 800), "TTS");
  //   textToSpeech = GUI.TextField(new Rect(Screen.width / 2 - 200, Screen.height / 2 - 250, 400, 100),textToSpeech,25);
  //   if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 150, 200, 50),"Speech")) {
  //     TextToSpeechManager.Instance.PlayTextToSpeech(textToSpeech);
  //   }
  //   
  // }

  public InputField inputField;

  public void OnSpeech() {
    if (!string.IsNullOrEmpty(inputField.text)) {
      TextToSpeechManager.Instance.PlayTextToSpeech(inputField.text);
    }
  }
}
