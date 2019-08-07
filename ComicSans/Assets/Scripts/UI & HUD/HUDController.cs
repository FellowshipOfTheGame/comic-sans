using UnityEngine;
using UnityEngine.UI;

namespace ComicSans.UIandHUD
{

	// Controls the game HUD.
	[AddComponentMenu("Scripts/Controller/HUD")]
	public class HUDController : MonoBehaviour {

		public static HUDController instance;

		[Tooltip("Parent GameObject for the other HUD objects.")]
		[SerializeField] private GameObject HUDContainer = null;

		[Tooltip("UI elements for the player lifes, element 0 will be the last to disappear.")]
		[SerializeField] private Image[] playerHealthIcons = null;

		[Tooltip("UI for the Boss health bar.")]
		[SerializeField] private Image bossHealthBar = null;

		[Tooltip("UI to display the Boss name.")]
		[SerializeField] private Text bossNameText = null;
		

		// Used to store the inital Boss life to calculate how much of the Boss life should be filled.
		private int initialBossLife = 5000;

		private void Awake () {
			
			if(instance != null)
			{
				Destroy(gameObject);
				return;
			}

			instance = this;

			if(playerHealthIcons.Length < 1)
				Debug.Log("HUDController.Awake: No Player health icons assigned!");

			if(bossNameText == null)
				Debug.Log("HUDController.Awake: No Image component assigned as Boss health bar!");

			if(bossNameText == null)
				Debug.Log("HUDController.Awake: No Text component to display the Boss name assigned!");

		}

		public void UpdatePlayerLifeIcons(int life)
		{

			if(playerHealthIcons.Length < 3)
			{
				Debug.LogWarning("HUDController.UpdatePlayerLifeIcons: You need 3 playerHealthIcons, but " + playerHealthIcons.Length + " were assigned!");
				return;
			}

			for(int i = 0; i < 3; i++) 
			{

				if(playerHealthIcons[i] == null)
				{
					Debug.LogWarning("HUDController.UpdatePlayerLifeIcons: playerHealthIcon on position " + i + " is null!");
					return;
				}

				if(i < life)
					playerHealthIcons[i].enabled = true;
				else
					playerHealthIcons[i].enabled = false;
			}
		}

		public void InitializeBossHUD(string bossName, int bossLife) 
		{
			initialBossLife = bossLife; // Stores the initial Boss life.

			if(bossHealthBar != null) // Sets the Boss health bar to be fully filled.
			{
				bossHealthBar.fillAmount = 1; 
				bossHealthBar.enabled = true;
			}

			if(bossNameText != null) // Set the Boss name.
			{
				bossNameText.text = bossName;
				bossNameText.enabled = true;
			}		
		
		}

		public void UpdateBossHealthBar (int life)
		{

			if(bossHealthBar != null)	
				bossHealthBar.fillAmount = (float)life / (float)initialBossLife; // Calculates and assigns how much of the life
																				 // bar should be filled.

		}

		public void UpdateBossName (string name)
		{

			if(bossNameText != null)
				bossNameText.text = name;

		}

		public void EnableHUD() { HUDContainer.SetActive(true); }

		public void DisableHUD() { HUDContainer.SetActive(false); }
		
	}

}
