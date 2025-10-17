// DockingStation.cs
using UnityEngine;

public class DockingStation : MonoBehaviour
{
    [Tooltip("씬에 있는 인벤토리 매니저를 여기다 끌어다 놔.")]
    [SerializeField] private InventoryManger inventoryManger;

    private GameObject dockedShip; // 현재 도킹 범위 안에 있는 우주선
    private SpaceshipCargoSystem cargoSystem; // 그 우주선의 카고 시스템

    private void Update()
    {
        // 우주선이 범위 안에 있고, E키를 눌렀을 때만 작동.
        if (dockedShip != null && Input.GetKeyDown(KeyCode.E))
        {
            // 1. 카고 시스템에 짐을 내리라고 명령한다.
            if (cargoSystem != null)
            {
                cargoSystem.UnloadAllOres(inventoryManger);
            }

            // 2. 짐을 다 내렸으니 우주선을 비활성화시킨다.
            dockedShip.SetActive(false);

            // 3. 참조를 비워서 다시 E키를 눌러도 아무 일도 없게 만든다.
            dockedShip = null;
            cargoSystem = null;
            
            Debug.Log("모든 광물을 기지에 저장하고 우주선을 격납했습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // "PlayerShip" 태그를 가진 녀석만 상대한다. 다른 잡것들은 무시.
        if (other.CompareTag("Spaceship"))
        {
            dockedShip = other.gameObject;
            cargoSystem = other.GetComponent<SpaceshipCargoSystem>();

            if (cargoSystem == null)
            {
                Debug.LogWarning("우주선은 들어왔는데... 카고 시스템이 없잖아?!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 범위 밖으로 나간 녀석이 내가 기억하던 그 우주선이라면, 잊어버린다.
        if (other.gameObject == dockedShip)
        {
            dockedShip = null;
            cargoSystem = null;
        }
    }
}