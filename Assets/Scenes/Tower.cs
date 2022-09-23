using UnityEngine;

public enum TowerType
{
	Laser, Mortar
}

public abstract class Tower : GameTileContent
{
	[SerializeField, Range(1.5f, 10.5f)]
	protected float targetingRange = 1.5f;

	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.yellow;
		Vector3 position = transform.localPosition;
		position.y += 0.01f;
		Gizmos.DrawWireSphere(position, targetingRange);
	}

	const int enemyLayerMask = 1 << 9;
	protected bool AcquireTarget(out TargetPoint target)
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
			target = targetsBuffer[Random.Range(0, hits)].GetComponent<TargetPoint>();
			Debug.Assert(target != null, "Targeted non-enemy!", targetsBuffer[0]);
			return true;
		}
		target = null;
		return false;
	}

	static Collider[] targetsBuffer = new Collider[100];

	protected bool TrackTarget(ref TargetPoint target)
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

	public abstract TowerType TowerType { get; }
}