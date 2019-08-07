using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using ComicSans.Boss;
using ComicSans.Boss.ActionSystem;
using ComicSans.UIandHUD;
using ComicSans.PoolingSystem;
using ComicSans.DataContainers;

namespace ComicSans.Player
{

    // The base script for the Player.
    [RequireComponent(typeof(Rigidbody2D))]
    [AddComponentMenu("Scripts/Entity/Player")]
    public class PlayerScript : EntityScript {

        public static PlayerScript instance;

        private Rigidbody2D _rigidbody;
        private Collider2D _collider;

        private Animator _animator;
        private SpriteRenderer _renderer;

        [System.Serializable]
        private class Shooting
        {
            [HideInInspector] public bool isShooting = false;

            [Tooltip("Delay between shots.")]
            public float delay = 0.1f;

            [Tooltip("Prefab pool to be used for the shots.")]
            public PoolInfo bulletPool = null;

            [Tooltip("Positions where the projectiles should be spawned.")]
            public ProjectileSpawn spawn = new ProjectileSpawn();

            // Shooting coroutine, used for starting and stoping the shooting.
            [HideInInspector] public Coroutine ShootingCoroutine = null;
        }
        
        [Tooltip("Configurations realted to the player shooting.")]
        [SerializeField] private Shooting shooting = null;

        // Stores if input events are setup on InputManager.
        private bool inputEventsOk = false;

        protected override void Awake () {

            if(instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            base.Awake(); 

            // Gets the RigidBody of the Player.
            _rigidbody = GetComponentInChildren<Rigidbody2D>();

            // Gets the Collider of the Player.
            _collider = GetComponentInChildren<Collider2D>();   

            // Gets the Animator of the Player.
            _animator = GetComponentInChildren<Animator>();

            // Gets the SpriteRenderer of the Player.
            _renderer = GetComponentInChildren<SpriteRenderer>();

        }

        private void OnEnable()
        {

            Initialize();
            
        }

        public void Initialize()
        {

            // Logs errors if some essential component is missing.
            if(InputController.instance == null)
                Debug.LogError("Player.OnEnable: No InputManager found!");
            if(_rigidbody == null)
                Debug.LogError("Player.OnEnable: No Rigidbody2D found on player!");
            if(_collider == null)
                Debug.LogError("Player.OnEnable: No Collider2D found on player!");
            if(_renderer == null)
                Debug.LogError("Player.OnEnable: No SpriteRenderer found on player!");
            if(_animator == null)
                Debug.LogError("Player.OnEnable: No Animator found on player!");
            

            _collider.enabled = true; // Guarantees the player collider is enabled.
            health.Hp = 3; // Reset the player health.

            // Guarantees the player is not shooting.
            shooting.isShooting = false;
            if(shooting.ShootingCoroutine != null)
            {
                StopCoroutine(shooting.ShootingCoroutine);
                shooting.ShootingCoroutine = null; 
            }

            // Adds the input events to the input manager.
            if(!inputEventsOk)
            {
                InputController.instance.OnShotDown += ToggleShooting;
                InputController.instance.OnPauseDown += TogglePause;

                inputEventsOk = true;
            }

        }
        

        private void OnDisable()
        {

            // Removes the input events to the input manager.
            InputController.instance.OnShotDown -= ToggleShooting;
            InputController.instance.OnPauseDown -= TogglePause;

            inputEventsOk = false;
            
        }


        // Update is called once per frame
        private void Update () {

            if(GameController.instance.AllowPlayerControl)
            {
                if(GameController.instance.currentGameState == GameController.GameState.Play || GameController.instance.currentGameState == GameController.GameState.Win)
                {

                    // Makes the Player move.
                    Vector2 vel = new Vector2();
                    
                    vel.x =  InputController.instance.xAxis;
                    vel.y =  InputController.instance.yAxis;

                    _rigidbody.velocity = vel * velocity;

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
                    if(Mathf.Abs(transform.position.x) > SceneSettings.instance.positionConstraints.x || Mathf.Abs(transform.position.y) > SceneSettings.instance.positionConstraints.y)
                    {
                        transform.position = new Vector3(Mathf.Clamp(transform.position.x, 
                                                        -SceneSettings.instance.positionConstraints.x, SceneSettings.instance.positionConstraints.x),
                                                        Mathf.Clamp(transform.position.y, 
                                                        -SceneSettings.instance.positionConstraints.y, SceneSettings.instance.positionConstraints.y),
                                                        0);
                    }
                }
            }
        }

        public void StopMovimentation()
        {

            // Guarantees the Player is not moving.
            _rigidbody.velocity = Vector3.zero;

            // Guarantees the Player is not shooting.
            shooting.isShooting = false;
            if(shooting.ShootingCoroutine != null)
            {
                StopCoroutine(shooting.ShootingCoroutine);
                shooting.ShootingCoroutine = null; 
            }
        }

        // Makes the Player start and stop shooting.s
        public void ToggleShooting() 
        {

            if(GameController.instance != null && 
            !(GameController.instance.currentGameState == GameController.GameState.Play || GameController.instance.currentGameState == GameController.GameState.Win))
            { 
                return;
            }

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
            else if(GameController.instance.currentGameState == GameController.GameState.Play)
                GameController.instance.SetPause(true);

        }

        protected override void Damage()
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
                StartCoroutine(Reset(health.invincibilityTime));
            }
            else
            {
                Die();
            }

        }

        protected override void Die()
        {
            BossScript.instance.PlayerDefeated();
                
            if(HUDController.instance != null)
                HUDController.instance.DisableHUD();

            gameObject.SetActive(false);

        }

        protected override IEnumerator Reset(float invincibilityMultiplier)
        {

            // Guarantees the Player is not moving.
            _rigidbody.velocity = Vector2.zero;        

            float time = 0;

            // Repositions the Player on the spawnPoint.
            transform.position = SceneSettings.instance.playerSpawnPoint;
            
            // Set invincibility animation.
            if(_animator != null)
                _animator.SetBool("Invincible", true);
            _collider.enabled = false;

            // Makes the Player invincible for some time.
            while(time < health.invincibilityTime * invincibilityMultiplier)
            {
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            // Stops invincibility animation.
            if(_animator != null)
                _animator.SetBool("Invincible", false);
            _collider.enabled = true;

            invincible = false;

        }

        private IEnumerator Shot(float delay)
        {
            while(true)
            {

                PoolingController.instance.Spawn(shooting.bulletPool, new Vector3(transform.position.x + shooting.spawn.position.x,
                                                                                transform.position.y + shooting.spawn.position.y,
                                                                                transform.position.z),
                                                                                Quaternion.Euler(0, 0, shooting.spawn.rotation));

                float time = 0;
                while(time < delay)
                {
                    time += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        protected override void UpdateLifeHUD()
        {

            if(HUDController.instance != null)
                HUDController.instance.UpdatePlayerLifeIcons(health.Hp);
            else
                Debug.LogWarning("PlayerScript.UpdateLifeHUD: No HUDController found!");

        }
    }

}