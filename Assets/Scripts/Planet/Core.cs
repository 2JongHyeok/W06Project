using TMPro;
using UnityEngine;

public class Core : MonoBehaviour
{
    [Header("Core 설정")]
    public int maxHP = 100;
    private int currentHP;
    private TMP_Text CoreHpText;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        CoreHpText.text = $"Core HP: {maxHP}";
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
}
