using UnityEngine;

public class Shell : WarEntity {
	Vector3 launchPoint, targetPoint, launchVelocity;

	public void Initialize(
		Vector3 launchPoint, Vector3 targetPoint, Vector3 launchVelocity
	)
	{
		this.launchPoint = launchPoint;
		this.targetPoint = targetPoint;
		this.launchVelocity = launchVelocity;
	}
}