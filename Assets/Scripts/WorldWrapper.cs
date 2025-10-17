// WorldWrapper.cs
using UnityEngine;

public class WorldWrapper : MonoBehaviour
{
    [Header("맵 경계 설정")]
    [Tooltip("우주선이 이 Y좌표보다 위로 가면 아래에서 나타남")]
    public float topBoundary = 50f;

    [Tooltip("우주선이 이 Y좌표보다 아래로 가면 위에서 나타남")]
    public float bottomBoundary = -50f;

    [Tooltip("우주선이 이 X좌표보다 왼쪽으로 가면 오른쪽에서 나타남")]
    public float leftBoundary = -90f;

    [Tooltip("우주선이 이 X좌표보다 오른쪽으로 가면 왼쪽에서 나타남")]
    public float rightBoundary = 90f;

    // LateUpdate를 쓰는 건 기본이야. 모든 움직임이 끝난 후에 위치를 보정해야 끊김이 없거든.
    // 이런 디테일이 실력의 차이를 만드는 거라고.
    void LateUpdate()
    {
        Vector3 currentPosition = transform.position;

        // X축 경계 체크
        if (currentPosition.x > rightBoundary)
        {
            currentPosition.x = leftBoundary;
        }
        else if (currentPosition.x < leftBoundary)
        {
            currentPosition.x = rightBoundary;
        }

        // Y축 경계 체크
        if (currentPosition.y > topBoundary)
        {
            currentPosition.y = bottomBoundary;
        }
        else if (currentPosition.y < bottomBoundary)
        {
            currentPosition.y = topBoundary;
        }

        // 계산이 끝난 위치를 최종적으로 적용.
        transform.position = currentPosition;
    }
}