using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    Color32 baseEnemy = new Color32(0, 0, 0, 255);
    Color32 specialEnemy = new Color32(255, 255, 0, 255);
    bool isSpecialEnemy = false;
    SpriteRenderer sRenderer;
    Transform playerTransform;
    Rigidbody2D rb;
    public float movementSpeed = 1;
    float heading = 1;
    int health = 2;
    float defaultDamageTimer = 1;
    float damageTimer;
    // Start is called before the first frame update
    void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        sRenderer.color = baseEnemy;
        playerTransform = GameManager.instance.getPlayer();
        rb = GetComponent<Rigidbody2D>();
        damageTimer = defaultDamageTimer;
        setEnemyType();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) > 2){
            rb.MovePosition(rb.position + new Vector2(movementSpeed*heading, rb.velocity.y) * Time.fixedDeltaTime);
        }else if(playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= 2){
            damageTimer -= Time.deltaTime;
            if(damageTimer <= 0){
                playerTransform.GetComponent<CharacterController>().giveDamage(isSpecialEnemy? 10 : 5);
                damageTimer = defaultDamageTimer;
            }
        }
        if(playerTransform != null){
            heading = playerTransform.position.x - transform.position.x > 0? 1 : -1;
            if(heading > 0){
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.z);
            }else{
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, 180, transform.eulerAngles.z);
            }
        }
    }

    void setEnemyType(){
        isSpecialEnemy = Random.Range(0 , 1.0f) < 0.2f? true : false;
        if(isSpecialEnemy){
            sRenderer.color = specialEnemy;
            health = 5;
        }
    }
    public void applyDamage(int dmg){
        health -= dmg;
        if(health <= 0){
            GameManager.instance.increaseScore(isSpecialEnemy? 5 : 2);
            Destroy(gameObject);
        }
    }

}
