using UnityEngine;

public class MoveCamera : MonoBehaviour
{
	// define focus object and radius
    public Transform focus;
    [SerializeField, Min(0f)]
    float focusRadius = 1f;

	// actual Vector3 for the point
	Vector3 focusPoint;

	void Awake()
	{
		//initialize focus point to current position of focus
		focusPoint = focus.position;
	}

	void LateUpdate()
	{
		// after everything else has ran, we will move the camera (hence the late update)
		UpdateFocusPoint();
		//note we keep the old position.z value since we don't want the camera to move in the z direction
		transform.position = new Vector3(focusPoint.x, focusPoint.y, transform.position.z);
	}

	void UpdateFocusPoint()
	{
		Vector3 targetPoint = focus.position;
		//if we have defined a focus radius
		if (focusRadius > 0f)
		{
			// we will check if the target is outside the radius
			float distance = Vector3.Distance(targetPoint, focusPoint);
			if (distance > focusRadius)
			{
				//if it is outside the radius, we will move the focus point towards the target
				focusPoint = Vector3.Lerp(
					targetPoint, focusPoint, focusRadius / distance
				);
			}
		}
		else
		{
			// if we don't have a radius, we will just set the focus point to the target
			focusPoint = targetPoint;
		}
	}
}
