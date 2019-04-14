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
