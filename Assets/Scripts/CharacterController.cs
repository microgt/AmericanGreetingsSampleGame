using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 1;
    public bool isGrounded = false;
    public LayerMask layerMask;
    public Transform jumpLoc;
    public float jumpHeight = 2;
    int jumps = 2;
    public Transform shootLoc;
    public Rigidbody2D bullet;
    public Rigidbody2D chargedBullet;
    public float chargeTime = 1;
    float fireRate = 0.25f;
    Animator anim;
    AudioSource aSource;
    public AudioClip shotSFX;
    public AudioClip chargingSFX;
    public AudioClip chargedSFX;
    public AudioClip chargedShotSFX;
    public AudioClip jumpSFX;
    public AudioClip damageSFX;
    float health = 100;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        aSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.getGameOver()) return;
        
        if (Input.GetAxis("Horizontal") != 0)
        {
            move(Input.GetAxis("Horizontal"));
        }
        if (Input.GetButtonDown("Jump"))
        {
            jump();
        }
        if (Input.GetButton("Fire1"))
        {
            chargeTime -= Time.deltaTime;
            if (chargeTime > 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Flashing"))
            {
                anim.Play("Flashing", 0);
                aSource.clip = chargingSFX;
            }
            if (chargeTime <= 0 && !anim.GetCurrentAnimatorStateInfo(0).IsName("Flashing1"))
            {
                anim.Play("Flashing1", 0);
                aSource.Stop();
            }

            if((aSource.clip != null && !(aSource.clip.name.Equals("chargingSFX"))) && chargeTime > 0 && !aSource.isPlaying){
                aSource.clip = chargingSFX;
                aSource.Play();
            }else if(aSource.clip != null && (!aSource.clip.name.Equals("chargedSFX")) && chargeTime <= 0 && !aSource.isPlaying){
                aSource.clip = chargedSFX;
                aSource.loop = true;
                aSource.Play();
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            shoot(chargeTime <= 0? chargedBullet : bullet);
            aSource.clip = null;
            aSource.loop = false;
        }
        isGrounded = Physics2D.Raycast(jumpLoc.position, -jumpLoc.up * 0.05f, layerMask);
        fireRate -= Time.deltaTime;
    }

    void move(float input)
    {
        if(input > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        float direction = speed * input;
        Vector2 movement = new Vector2(rb.position.x + direction, rb.position.y);
        rb.position = movement;
    }
    void jump()
    {
        if(jumps <= 0)
        {
            if(isGrounded) jumps = 2;
        }
        else
        {
            rb.AddForce(transform.up * jumpHeight, ForceMode2D.Force);
            aSource.PlayOneShot(jumpSFX);
            jumps--;
        }  
    }
    void shoot(Rigidbody2D rb2d)
    {
        if (fireRate > 0) return;
        aSource.PlayOneShot(chargeTime <=0? chargedShotSFX : shotSFX);
        Rigidbody2D shot = Instantiate(rb2d, shootLoc.position, Quaternion.identity);
        shot.GetComponent<Projectile>().setProjectileType(chargeTime <= 0? Projectile.ProjectileType.charged : Projectile.ProjectileType.normal);
        shot.AddForce(transform.right * 1000, ForceMode2D.Force);
        Destroy(shot.gameObject, 3);
        fireRate = 0.25f;
        chargeTime = 1;
        anim.Play("Default", 0);
    }

    public void giveDamage(float dmg){
        health -= dmg;
        aSource.PlayOneShot(damageSFX);
        GameManager.instance.updateHealthbar(health/100);
        if(health <= 0){
            GameManager.instance.gameOver();
        }
    }
}
