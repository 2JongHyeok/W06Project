// WorldWrapper.cs
using UnityEngine;
using Unity.Cinemachine; // Cinemachine v3

public class WorldWrapper : MonoBehaviour
{
    [Header("맵 경계 설정")]
    public float topBoundary = 50f;
    public float bottomBoundary = -50f;
    public float leftBoundary = -90f;
    public float rightBoundary = 90f;

    [Header("Cinemachine (선택)")]
    [Tooltip("시네머신 Follow가 가리키는 Transform을 지정. 비우면 이 객체(transform) 사용")]
    [SerializeField] private Transform cinemachineFollowTarget;

    void LateUpdate()
    {
        Vector3 oldPos = transform.position;
        Vector3 newPos = oldPos;
        bool wrapped = false;

        // X축
        if (newPos.x > rightBoundary) { newPos.x = leftBoundary; wrapped = true; }
        else if (newPos.x < leftBoundary) { newPos.x = rightBoundary; wrapped = true; }

        // Y축
        if (newPos.y > topBoundary) { newPos.y = bottomBoundary; wrapped = true; }
        else if (newPos.y < bottomBoundary) { newPos.y = topBoundary; wrapped = true; }

        if (!wrapped)
        {
            // 평상시: 아무 일 없음
            transform.position = newPos;
            return;
        }

        // 1) 실제 텔레포트
        transform.position = newPos;

        // 2) 델타를 시네머신에 통지 → 카메라도 같은 만큼 즉시 워프
        Vector3 delta = newPos - oldPos;
        Transform target = cinemachineFollowTarget != null ? cinemachineFollowTarget : transform;
        CinemachineCore.OnTargetObjectWarped(target, delta);
        // v3에선 위 한 줄이 모든 관련 카메라에 브로드캐스트된다.
    }
}