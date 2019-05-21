using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Scripts/Player")]
public class Player : MonoBehaviour {

    public static Player instance;

	private InputManager _input;

    private Rigidbody2D _rigidbody;
    private Collider2D _collider;

	private Animator _animator;
    private SpriteRenderer _renderer;

    private bool invincible = false;

    [SerializeField]private Vector2 positionConstraints = new Vector2( 8, 8);
    
    [SerializeField] private float speed;    

    [System.Serializable]
    private class Health
    {

        public int hp = 3;
        public int Hp
        {
            get
            {
                return hp;
            }
            set
            {

                if (value >= 3)
                    hp = 3;
                if (value < 0)
                    hp = 0;
                else
                    hp = value;

                if(HUDController.instance != null)
                    HUDController.instance.UpdatePlayerLifeIcons(hp);
                else
                    Debug.LogWarning("Player.Health.Hp.set: No HUDController found!");

            }
        }

        public float invencibilityTime = 2f;
    }
    [SerializeField]private Health health;

    [System.Serializable]
    private class Shooting
    {
        public bool isShooting = false;

        public float delay = 0.1f;
        public ObjectPool bulletPool;
        [HideInInspector] public Coroutine ShootingCoroutine;
    }
    [SerializeField] private Shooting shooting;

    // Stores if input events are setup on InputManager.
    private bool inputEventsOk = false;

    // Use this for initialization
	void Awake () {

        if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;

		// Gets the InputManager of the Player.
        _input = GetComponentInChildren<InputManager>();    

        // Gets the RigidBody of the Player.
        _rigidbody = GetComponentInChildren<Rigidbody2D>();

        // Gets the Collider of the Player.
        _collider = GetComponentInChildren<Collider2D>();   

        // Gets the Animator of the Player.
		_animator = GetComponentInChildren<Animator>();

        // Gets the SpriteRenderer of the Player.
        _renderer = GetComponentInChildren<SpriteRenderer>();

	}

    public void OnEnable()
    {

        // Logs errors if some essential component is missing.
        if(_input == null)
			Debug.LogError("Player.OnEnable: No InputManager found on player!");
        if(_rigidbody == null)
			Debug.LogError("Player.OnEnable: No Rigidbody2D found on player!");
        if(_collider == null)
			Debug.LogError("Player.OnEnable: No Collider2D found on player!");
        if(_renderer == null)
			Debug.LogError("Player.OnEnable: No SpriteRenderer found on player!");
        if(_animator == null)
			Debug.LogError("Player.OnEnable: No Animator found on player!");
        

        _collider.enabled = true; // Guarantees the player collider is enabled.
        health.Hp = 3; // Reset the player life.

        // Guarantees the player is not shooting.
        shooting.isShooting = false;
        if(shooting.ShootingCoroutine != null)
        {
            StopCoroutine(shooting.ShootingCoroutine);
            shooting.ShootingCoroutine = null; 
        }

        // Adds the input events to the input manager.
        if(_input != null && !inputEventsOk)
        {
            _input.OnShotDown += ToggleShooting;
            _input.OnPauseDown += TogglePause;

            inputEventsOk = true;
        }
        
    }
	

    public void OnDisable()
    {

        // Removes the input events to the input manager.
        if(_input != null)
        {
            _input.OnShotDown -= ToggleShooting;
            _input.OnPauseDown -= TogglePause;

            inputEventsOk = false;
        }
        
    }


	// Update is called once per frame
	void Update () {

        if(GameController.instance.AllowPlayerControl)
        {
            if(GameController.instance.currentGameState == GameController.GameState.Play || GameController.instance.currentGameState == GameController.GameState.Win)
            {

                // Makes the Player move.
                Vector2 vel = new Vector2();
                
                vel.x = _input.xAxis;
                vel.y = _input.yAxis;

                _rigidbody.velocity = vel * speed;

                // Handles player animation.
                if (vel.x != 0)
                {

                    if(_animator != null)
                        _animator.SetBool("Mov_Horizontal", true);

                    if(vel.x > 0)
                        _renderer.flipX = false;
                    else
                        _renderer.flipX = true;

                }
                else
                {

                    if(_animator != null)         
                        _animator.SetBool("Mov_Horizontal", false);

                    _renderer.flipX = false;

                }

                // Constraints the player to the player zone.
                if(Mathf.Abs(transform.position.x) > positionConstraints.x || Mathf.Abs(transform.position.y) > positionConstraints.y)
                {
                    transform.position = new Vector3(Mathf.Clamp(transform.position.x, -positionConstraints.x, positionConstraints.x),
                                                    Mathf.Clamp(transform.position.y, -positionConstraints.y, positionConstraints.y),
                                                    0);
                }
            }
        }
	}

    // Makes the Player start and stop shooting.s
    public void ToggleShooting() 
    {

        shooting.isShooting =! shooting.isShooting;

        // Makes the Player shoot.
        if (shooting.isShooting && shooting.ShootingCoroutine == null)
        {
                shooting.ShootingCoroutine = StartCoroutine(Shot(shooting.delay));

        }
        else if(shooting.ShootingCoroutine != null)
        {
                StopCoroutine(shooting.ShootingCoroutine);
                shooting.ShootingCoroutine = null;
        }

    }

    // Makes the Player pause and unpause the game.
    public void TogglePause()
    {

        if(GameController.instance.currentGameState == GameController.GameState.Paused)
            GameController.instance.SetPause(false);
        else
            GameController.instance.SetPause(true);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Damage")
            Damage();         

    }

    void Damage()
    {

        if(invincible || GameController.instance.currentGameState != GameController.GameState.Play)
            return;

        // Makes the player unable to take damage after defeating a Boss.
        if(BossScript.instance == null)
            return;

        health.Hp--;
        if (health.Hp > 0)
        {
            invincible = true;
            StartCoroutine(ResetPlayer());
        }
        else
        {
            BossScript.instance.PlayerDie();
            
            if(HUDController.instance != null)
                HUDController.instance.DisableHUD();

            gameObject.SetActive(false);
        }

    }

    IEnumerator ResetPlayer()
    {
        float time = 0;

        transform.position = GameController.instance.playerSettings.spawnPoint;
        
        if(_animator != null)
            _animator.SetBool("Invencible", true);
        _collider.enabled = false;

        while(time < health.invencibilityTime)
        {
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        if(_animator != null)
            _animator.SetBool("Invencible", false);
        _collider.enabled = true;

        invincible = false;

    }

    IEnumerator Shot(float delay)
    {
        while(true)
        {

            shooting.bulletPool.Spawn(transform.position + new Vector3( 0, 1, 0), transform.rotation);

            float time = 0;
            while(time < delay)
            {
                time += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
        }
    }
}
