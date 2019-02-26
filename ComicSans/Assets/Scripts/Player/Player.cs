using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
[AddComponentMenu("Scripts/Player")]
public class Player : MonoBehaviour {

    public static Player instance;

	private Animator _animator;
    private Rigidbody2D _rigidbody;
    private Collider2D _collider;
    private SpriteRenderer _renderer;

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
        public float delay = 0.1f;
        public ObjectPool bulletPool;
        [HideInInspector] public Coroutine ShootingCoroutine;
    }
    [SerializeField] private Shooting shooting;

    // Use this for initialization
	void Awake () {

        if(instance != null)
		{
			Destroy(gameObject);
			return;
		}

		instance = this;

		_animator = GetComponentInChildren<Animator>();
		if(_animator == null)
			Debug.Log("Player.Start: No Animator found on player!");


        _rigidbody = GetComponentInChildren<Rigidbody2D>();
        if(_animator == null)
			Debug.Log("Player.Start: No Rigidbody2D found on player!");

        _collider = GetComponentInChildren<Collider2D>();
        if(_animator == null)
			Debug.Log("Player.Start: No Collider2D found on player!");

        _renderer = GetComponentInChildren<SpriteRenderer>();
        if(_animator == null)
			Debug.Log("Player.Start: No SpriteRenderer found on player!");

	}

    public void OnEnable()
    {
        health.Hp = 3;

        if(shooting.ShootingCoroutine != null)
        {
            StopCoroutine(shooting.ShootingCoroutine);
            shooting.ShootingCoroutine = null; 
        }
    }
	
	// Update is called once per frame
	void Update () {

        // Makes the player move.
        Vector2 vel = new Vector2();
        vel.x = (Input.GetAxisRaw("Horizontal"));
        vel.y = Input.GetAxisRaw("Vertical");
        vel.Normalize();
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

        if (Input.GetButton("Fire1"))
            if(shooting.ShootingCoroutine == null)
                shooting.ShootingCoroutine = StartCoroutine(Shot(shooting.delay));
        if (Input.GetButtonUp("Fire1"))
        {
            StopCoroutine(shooting.ShootingCoroutine);
            shooting.ShootingCoroutine = null;
        }

	}

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Damage")
            TakeDamage();         

    }

    void TakeDamage()
    {

        // Makes the player unable to take damage after defeating a Boss.
        if(BossScript.instance == null)
            return;

        health.Hp--;
        if (health.Hp > 0)
            StartCoroutine(ResetPlayer());
        else
        {
            BossScript.instance.Win();
            
            if(HUDController.instance != null)
                HUDController.instance.DisableHUD();

            gameObject.SetActive(false);
        }

    }

    IEnumerator ResetPlayer()
    {
        float time = 0;

        transform.position = GameManager.instance.playerSettings.spawnPoint;
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
