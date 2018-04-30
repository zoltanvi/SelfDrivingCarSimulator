using UnityEngine;

public class Manager : MonoBehaviour
{
	public static Manager instance = null;

	//private bool playing = false;

	void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
		InitGame();

	}

	void InitGame()
	{
	

	}



}
