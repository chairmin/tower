using UnityEngine;

public class Tower : GameTileContent
{
	[SerializeField, Range(1.5f, 10.5f)]
	float targetingRange = 1.5f;

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Vector3 position = transform.localPosition;
		position.y += 0.01f;
		Gizmos.DrawWireSphere(position, targetingRange);

		if (target != null)
		{
			Gizmos.DrawLine(position, target.Position);
		}
	}

	TargetPoint target;

	public override void GameUpdate()
	{
		if (TrackTarget() || AcquireTarget())
		{
			Shoot();
			//Debug.Log("Acquired target!");
		}
		else
		{
			laserBeam.localScale = Vector3.zero;
		}
	}

	void Shoot()
	{
		Vector3 point = target.Position;
		turret.LookAt(point);

		laserBeam.localRotation = turret.localRotation;
		float d = Vector3.Distance(turret.position, point);
		laserBeamScale.z = d;
		laserBeam.localScale = laserBeamScale;
		laserBeam.localPosition =
			turret.localPosition + 0.5f * d * laserBeam.forward;

		target.Enemy.ApplyDamage(damagePerSecond * Time.deltaTime);
	}

	const int enemyLayerMask = 1 << 9;
	bool AcquireTarget()
	{
		//Collider[] targets = Physics.OverlapSphere(
		//	transform.localPosition, targetingRange, enemyLayerMask
		//);
		Vector3 a = transform.localPosition;
		Vector3 b = a;
		//b.y += 3f;
		//Collider[] targets = Physics.OverlapCapsule(
		//	a, b, targetingRange, enemyLayerMask
		//);

		//if (targets.Length > 0)
		//{
		//	target = targets[0].GetComponent<TargetPoint>();
		//	Debug.Assert(target != null, "Targeted non-enemy!", targets[0]);
		//	return true;
		//}
		b.y += 2f;
		int hits = Physics.OverlapCapsuleNonAlloc(
			a, b, targetingRange, targetsBuffer, enemyLayerMask
		);
		if (hits > 0)
		{
			target = targetsBuffer[0].GetComponent<TargetPoint>();
			Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[0]);
			return true;
		}
		target = null;
		return false;
	}

	static Collider[] targetsBuffer = new Collider[1];

	bool TrackTarget()
	{
		if (target == null)
		{
			return false;
		}
		//Vector3 a = transform.localPosition;
		//Vector3 b = target.Position;
		//if (Vector3.Distance(a, b) > targetingRange + 0.125f * target.Enemy.Scale)
		//{
		//	target = null;
		//	return false;
		//}
		Vector3 a = transform.localPosition;
		Vector3 b = target.Position;
		float x = a.x - b.x;
		float z = a.z - b.z;
		float r = targetingRange + 0.125f * target.Enemy.Scale;
		if (x * x + z * z > r * r)
		{
			target = null;
			return false;
		}
		return true;
	}

	[SerializeField]
	Transform turret = default, laserBeam = default;

	Vector3 laserBeamScale;

	void Awake()
	{
		laserBeamScale = laserBeam.localScale;
	}

	[SerializeField, Range(1f, 100f)]
	float damagePerSecond = 10f;
}