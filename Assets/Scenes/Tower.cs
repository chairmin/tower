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
			Debug.Log("Acquired target!");
		}
	}

	const int enemyLayerMask = 1 << 9;
	bool AcquireTarget()
	{
		//Collider[] targets = Physics.OverlapSphere(
		//	transform.localPosition, targetingRange, enemyLayerMask
		//);
		Vector3 a = transform.localPosition;
		Vector3 b = a;
		b.y += 3f;
		Collider[] targets = Physics.OverlapCapsule(
			a, b, targetingRange, enemyLayerMask
		);

		if (targets.Length > 0)
		{
			target = targets[0].GetComponent<TargetPoint>();
			Debug.Assert(target != null, "Targeted non-enemy!", targets[0]);
			return true;
		}
		target = null;
		return false;
	}

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
}