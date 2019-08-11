using UnityEngine;

using System.Collections;

using ComicSans.Player;

namespace ComicSans
{

	// Controls a Scene Portal.
	[AddComponentMenu("Scripts/Scene/Portal")]
	public class ScenePortal : MonoBehaviour {

		[Tooltip("Name of the scene to be loaded.")]
		[SerializeField] private string sceneName = "";

		[Tooltip("Position where the player will appear the next scene where SceneSettings.useReturnPos is enabled.")]
		[SerializeField] private Vector2 returnSpawnPosition = Vector2.zero;

		[Tooltip("Where the Player should be pulled to during loading.")]
		[SerializeField] private Vector2 destinationPosition = Vector2.zero;


		void OnTriggerEnter2D(Collider2D other)
		{

			// Assigns a return position to the Player and loads the target Scene.
			GameController.instance.returnSpawnPos = returnSpawnPosition;
			GameController.instance.LoadScene(sceneName);

			PlayerScript.instance.StopMovimentation();

			// Moves the Player towards a destination point.
			StartCoroutine(MovePlayer());
			
		}

		IEnumerator MovePlayer()
		{
			
			Transform player = PlayerScript.instance.transform; // Transform of the Player.
			float velocity = PlayerScript.instance.velocity; // Velocity to be used.

			// Destination point in 3D space.
			Vector3 destination3D =  new Vector3(destinationPosition.x, destinationPosition.y, 0);

			// Direction of moviment.
			Vector3 dir = (destination3D - player.position).normalized;

			// Moves the Player to the destination.
			while(Vector3.Distance(player.position, destination3D) > 0.05f)
			{

				player.Translate(dir * velocity * Time.fixedDeltaTime);
				yield return new WaitForFixedUpdate();

			}

		}

	}

}
