using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobManager : MonoBehaviour
{
    public int RewardMoney = 50;
    public bool isDead = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TowerAttack") && !isDead)
        {
            isDead = true;
            Die();
        }
    }
    void Die()
    {
        PlayerManager player = FindObjectOfType<PlayerManager>();
        if (player != null)
        {
            {
                player.AddMoney(RewardMoney);
            }
        }
        Destroy(gameObject);
    }
}
