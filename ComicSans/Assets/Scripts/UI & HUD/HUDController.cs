using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Scripts/HUD Controller")]
public class HUDController : MonoBehaviour {

	public static HUDController instance;

	[SerializeField] private GameObject HUDContainer;

	[SerializeField] private Image[] playerHealthIcons;
	[SerializeField] private Image bossHealthBar;
	[SerializeField] private Text bossNameText;
	

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

		PositionHUDItems();

	}
	
	private void PositionHUDItems()
	{

		 // Positions the Player health icons.
        for(int i = 0; i < playerHealthIcons.Length; i++) {

            if(playerHealthIcons[i] == null)
            {
                Debug.LogWarning("HUDController.PositionHUDItems: playerHealthIcon on position " + i + " is null!");
                return;
            }

            Vector3 iconPosition = playerHealthIcons[i].rectTransform.position;
            iconPosition.x = (Screen.width / 2) - (Screen.height / 1.66f) + (80 * i);
            playerHealthIcons[i].rectTransform.position = iconPosition; 

        }

		// Sets the health bar tranform to the correct position and size.
		if(bossHealthBar != null)
		{
			Vector3 barPosition = bossHealthBar.rectTransform.position;
			barPosition.x = (Screen.width / 2) + (Screen.height * 0.625f) - 15;
			bossHealthBar.rectTransform.position = barPosition; 

			Vector2 barSize = bossHealthBar.rectTransform.sizeDelta;
			barSize.x = (Screen.height * 1.25f) - 20;
			bossHealthBar.rectTransform.sizeDelta = barSize;
		}

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
		initialBossLife = bossLife;
		if(bossHealthBar != null)
		{
			bossHealthBar.fillAmount = 1;
			bossHealthBar.enabled = true;
		}
		

		if(bossNameText != null)
		{
			bossNameText.text = bossName;
			bossNameText.enabled = true;
		}		
	
	}

	public void UpdateBossHealthBar (int life) 
	{

		if(bossHealthBar != null)	
			bossHealthBar.fillAmount = (float)life / (float)initialBossLife;

	}

	public void EnableHUD() {
		
		HUDContainer.SetActive(true);
	
	}

	public void DisableHUD() {
		
		HUDContainer.SetActive(false);
	
	}
}
