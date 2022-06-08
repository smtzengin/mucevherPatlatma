using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public AudioSource gemSound, explosionSound, gameOverSound;

    private void Awake()
    {
        instance = this;
    }

    public void MakeGemSound()
    {
        gemSound.Stop();

        gemSound.pitch = Random.Range(0.8f, 1.2f);

        gemSound.Play();
    }

    public void MakeExplosionSound()
    {
        explosionSound.Stop();

        explosionSound.pitch = Random.Range(0.8f, 1.2f);

        explosionSound.Play();
    }

    public void GameOverSound()
    {
        gameOverSound.Play();
    }
}
