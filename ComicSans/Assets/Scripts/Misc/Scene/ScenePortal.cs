using UnityEngine;

[AddComponentMenu("Scripts/Scene/Portal")]
public class ScenePortal : MonoBehaviour {

	[SerializeField] private string sceneName = "";

	void OnTriggerEnter2D(Collider2D other)
	{

		GameController.instance.LoadScene(sceneName);

	}

}
