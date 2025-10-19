using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ForgeNodeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button nodeButton;
    [SerializeField] private Image nodeIcon;

    private BaseForgeSO forgeSO;
    private Action<BaseForgeSO> onClickCallback;

    public void Initialize(BaseForgeSO forgeData, Action<BaseForgeSO> onClick)
    {
        forgeSO = forgeData;
        onClickCallback = onClick;

        // 버튼 리스너 등록
        if (nodeButton != null)
        {
            nodeButton.onClick.RemoveAllListeners();
            nodeButton.onClick.AddListener(OnButtonClicked);
        }
        else
        {
            // nodeButton이 할당되지 않았으면 자동으로 찾기
            nodeButton = GetComponent<Button>();
            if (nodeButton != null)
            {
                nodeButton.onClick.RemoveAllListeners();
                nodeButton.onClick.AddListener(OnButtonClicked);
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (forgeSO == null) return;

        // 아이콘 표시 (있다면)
        // if (nodeIcon != null && forgeSO.icon != null)
        // {
        //     nodeIcon.sprite = forgeSO.icon;
        // }
    }

    private void OnButtonClicked()
    {
        onClickCallback?.Invoke(forgeSO);
    }

    // 외부에서 버튼 활성화/비활성화
    public void SetInteractable(bool interactable)
    {
        if (nodeButton != null)
        {
            nodeButton.interactable = interactable;
        }
    }

    // 노드 정보 업데이트 (예: 비용이나 상태가 변경되었을 때)
    public void RefreshUI()
    {
        UpdateUI();
    }
}
