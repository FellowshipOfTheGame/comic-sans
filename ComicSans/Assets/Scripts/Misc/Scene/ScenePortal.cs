using UnityEngine;

namespace ComicSans
{

	// Controls a Scene Portal.
	[AddComponentMenu("Scripts/Scene/Portal")]
	public class ScenePortal : MonoBehaviour {

		[SerializeField] private string sceneName = "";
		[SerializeField] private Vector2 returnSpawnPosition = Vector3.zero;


		void OnTriggerEnter2D(Collider2D other)
		{
			GameController.instance.returnSpawnPos = returnSpawnPosition;
			GameController.instance.LoadScene(sceneName);
		}

	}

}
