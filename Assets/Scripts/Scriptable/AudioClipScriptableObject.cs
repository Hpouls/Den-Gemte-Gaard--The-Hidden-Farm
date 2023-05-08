using UnityEngine;

// Makes it create-able in the unity project folder as a Scriptable Object
[CreateAssetMenu(fileName = "AudioScriptableObject", menuName = "Extended Audio/Audio Clip")]
public class AudioClipScriptableObject : ScriptableObject {
    [Tooltip("The type of audio, ie. cow.")]
    [SerializeField] string type;

    public string Type {
        get{
            return type;
        }
        set{
            type = value;
        }
    }

    [Tooltip("Specific name of the audio file for categorizing.")]
    [SerializeField] string audioName;
    public string AudioName {
        get {
            return audioName;
        }
        set {
            audioName = value;
        }
    }

    [Tooltip("The audio clip that is to be played.")]
    [SerializeField] AudioClip audioClip;
    public AudioClip AudioClip{
        get {
            return audioClip;
        }
        set {
            audioClip = value;
        }
    }
}
