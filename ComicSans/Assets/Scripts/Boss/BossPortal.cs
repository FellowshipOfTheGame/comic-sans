using UnityEngine;

[AddComponentMenu("Scripts/Boss/Portal")]
public class BossPortal : MonoBehaviour {

	[SerializeField] private string sceneName;

	void OnTriggerEnter2D(Collider2D other)
	{

		GameController.instance.LoadScene(sceneName);

	}

}
