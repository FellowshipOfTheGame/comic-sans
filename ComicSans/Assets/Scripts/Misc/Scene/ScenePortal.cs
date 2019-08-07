using UnityEngine;

namespace ComicSans
{

	// Controls a Scene Portal.
	[AddComponentMenu("Scripts/Scene/Portal")]
	public class ScenePortal : MonoBehaviour {

		[Tooltip("Name of the scene to be loaded.")]
		[SerializeField] private string sceneName = "";

		[Tooltip("Position where the player will appear the next scene where SceneSettings.useReturnPos is enabled.")]
		[SerializeField] private Vector2 returnSpawnPosition = Vector3.zero;


		void OnTriggerEnter2D(Collider2D other)
		{

			// Assigns a return position to the Player and loads the target Scene.
			GameController.instance.returnSpawnPos = returnSpawnPosition;
			GameController.instance.LoadScene(sceneName);
			
		}

	}

}
