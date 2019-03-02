using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Scripts/Boss/Health Bar")]
public class BossHealthBar : MonoBehaviour {

	public Image bar;

	private int initialLife = 100;

	private void Awake () {
		
		// Sets the health bar tranform to the correct position and size.
		Vector3 barPosition = bar.rectTransform.position;
        barPosition.x = (Screen.width / 2) - (Screen.height / 1.66f) + 10;
		bar.rectTransform.position = barPosition; 

		Vector2 barSize = bar.rectTransform.sizeDelta;
		barSize.x = Screen.width - 2 * ((Screen.width / 2) - (Screen.height / 1.66f)) - 20;
		bar.rectTransform.sizeDelta = barSize;

	}
	
	public void SetIntitialLife(int life) {
		
		initialLife = life;
	
	}

	public void UpdateHealthBar (int life) {
		
		bar.fillAmount = (float)life / (float)initialLife;

	}
}
