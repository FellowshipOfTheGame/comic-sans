using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[System.Serializable]
	public class PlayerSettings
	{
		
		public GameObject prefab;
		public Vector2 spawnPoint;

	}
	public PlayerSettings playerSettings;

	void Awake()
	{

		if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject);

		SceneManager.sceneLoaded += OnSceneLoaded;

		if(Player.instance == null)
		{
			GameObject _player = Instantiate(playerSettings.prefab, playerSettings.spawnPoint, new Quaternion());
			DontDestroyOnLoad(_player);
			_player.SetActive(true);
		}

	}

	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

		AudioControlCenter.instance.StopAllSounds();

		if(Player.instance.gameObject.activeSelf)
			Player.instance.OnEnable();
		else
			Player.instance.gameObject.SetActive(true);

		Player.instance.transform.position = playerSettings.spawnPoint;
    }

	public IEnumerator EndGame (float delay) {
		
		float time = 0;

		while(time < delay)
		{
			time += Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();

		}

		SceneManager.LoadSceneAsync("Transition");

	}

	public void LoadScene(string sceneName)
	{
		Debug.Log("GameManager.LoadScene: Loading " + sceneName + "...");
		SceneManager.LoadSceneAsync(sceneName);
	}
}
