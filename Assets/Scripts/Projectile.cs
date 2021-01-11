using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public enum ProjectileType{normal, charged};
    public ProjectileType type;
    public Transform normalExplosionEffect;
    public Transform specialExplosionEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void setProjectileType(ProjectileType myType) {
        type = myType;
    }

    // Update is called once per frame
    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<EnemyScript>()){
            collision.gameObject.GetComponent<EnemyScript>().applyDamage(type == ProjectileType.normal? 2 : 5);
            Instantiate(type == ProjectileType.normal? normalExplosionEffect : specialExplosionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
