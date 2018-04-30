using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadNextScene : MonoBehaviour {

	public void StartGame()
	{
		SceneManager.LoadScene(1);
	}

}
