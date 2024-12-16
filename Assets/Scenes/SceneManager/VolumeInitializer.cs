using UnityEngine;

public class VolumeInitializer : MonoBehaviour
{
    private const string VOLUME_KEY = "GameVolume"; // Sama dengan key di MainMenu
    private const float DEFAULT_VOLUME = 0.5f; // Nilai default

    public GameObject[] audioGameObjects; // Array GameObject yang memiliki AudioSource

    void Start()
    {
        // Ambil nilai volume tersimpan dari PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat(VOLUME_KEY, DEFAULT_VOLUME);

        // Terapkan volume ke semua AudioSource di setiap GameObject
        foreach (GameObject obj in audioGameObjects)
        {
            if (obj != null)
            {
                // Ambil semua AudioSource dalam GameObject
                AudioSource[] audioSources = obj.GetComponents<AudioSource>();
                foreach (AudioSource audioSource in audioSources)
                {
                    if (audioSource != null)
                    {
                        audioSource.volume = savedVolume; // Terapkan nilai volume
                    }
                }
            }
        }
    }
}