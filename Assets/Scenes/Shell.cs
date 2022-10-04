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

	float age;
	public override bool GameUpdate()
	{
		age += Time.deltaTime;
        Vector3 p = launchPoint + launchVelocity * age;
        p.y -= 0.5f * 9.81f * age * age;
        transform.localPosition = p;

        Vector3 d = launchVelocity;
		d.y -= 9.81f * age;
		if (p.y <= 0f)
		{
			OriginFactory.Reclaim(this);
			return false;
		}
		transform.localRotation = Quaternion.LookRotation(d);
		return true;
	}
}