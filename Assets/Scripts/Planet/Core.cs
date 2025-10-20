using System;
using TMPro;
using UnityEngine;

public class Core : MonoBehaviour
{
    [Header("Core 설정")]
    public int maxHP = 100;
    private int currentHP;
    public TMP_Text CoreHpText;
    [SerializeField] private InventoryManger inventoryManger;

    private void Awake()
    {
        currentHP = maxHP;
        UpdateHPText();
    }

    private void UpdateHPText()
    {
        if (CoreHpText != null)
        {
            CoreHpText.text = $"Core HP: {currentHP}";
        }
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        UpdateHPText();
        Debug.Log($"Core HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            currentHP = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        Destroy(gameObject);
        Debug.Log("GameOver");
        // 이후에 GameOver 연출이나 Scene 전환 로직을 여기에 추가 가능
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( inventoryManger != null)
        {
            if(collision.gameObject.TryGetComponent<Ore>(out var ore))
            {
                inventoryManger.AddOre(ore.oreType, ore.amount);
                Destroy(collision.gameObject);
            }
        }
    }
    
    // 1회성 HP 회복 메서드
    public void HealHP(int amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        UpdateHPText();
        Debug.Log($"Core HP Healed: +{amount}, Current: {currentHP}/{maxHP}");
    }
    
    // 외부에서 UI 갱신 호출용
    public void RefreshHPText()
    {
        UpdateHPText();
    }
}
