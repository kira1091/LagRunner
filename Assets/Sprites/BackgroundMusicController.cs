using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    [SerializeField]
    private AudioClip backgroundMusic; // Drag your music file here in Inspector

    private AudioSource audioSource;

    void Start()
    {
        // Add AudioSource and set it up
        audioSource = gameObject.AddComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("Failed to add AudioSource!");
            return;
        }

        // Check if music file is assigned
        if (backgroundMusic == null)
        {
            Debug.LogError("No background music assigned! Drag a .wav or .mp3 file into the Inspector.");
            return;
        }

        // Set up and play
        audioSource.clip = backgroundMusic;
        audioSource.playOnAwake = false; // Manual control
        audioSource.loop = true; // Keeps it playing
        audioSource.Play();

        Debug.Log("Background music started playing!");
    }

    void OnDestroy()
    {
        // Clean up if scene changes
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }
}