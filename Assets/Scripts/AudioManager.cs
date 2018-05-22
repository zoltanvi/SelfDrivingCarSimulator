using UnityEngine.Audio;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

	public static AudioManager Instance = null;

	public AudioClip HoverClip;
	public AudioClip ClickClip;

	public AudioSource AudioSource;

	/// <summary>
	/// Példányosít egyet önmagából (Singleton)
	/// </summary>
	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	void Start()
	{
		AudioSource.playOnAwake = false;
	}

	public void HoverButton()
	{
		AudioSource.PlayOneShot(HoverClip);
	}

	public void ClickButton()
	{
		AudioSource.PlayOneShot(ClickClip);
	}
}
