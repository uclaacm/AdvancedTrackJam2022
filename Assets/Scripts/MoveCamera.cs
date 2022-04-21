using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform focus;
    [SerializeField, Min(0f)]
    float focusRadius = 1f;

	Vector3 focusPoint;

	void Awake()
	{
		focusPoint = focus.position;
	}

	void LateUpdate()
	{
		UpdateFocusPoint();
		transform.position = new Vector3(focusPoint.x, focusPoint.y, transform.position.z);
	}

	void UpdateFocusPoint()
	{
		Vector3 targetPoint = focus.position;
		if (focusRadius > 0f)
		{
			float distance = Vector3.Distance(targetPoint, focusPoint);
			if (distance > focusRadius)
			{
				focusPoint = Vector3.Lerp(
					targetPoint, focusPoint, focusRadius / distance
				);
			}
		}
		else
		{
			focusPoint = targetPoint;
		}
	}
}
