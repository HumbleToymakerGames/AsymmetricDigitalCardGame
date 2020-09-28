using UnityEngine;

public class DamageWrapper
{
	public int Damage
	{ get; private set; }

	public DamageType damageType
	{ get; private set; }

	public DamageWrapper(int _damage, DamageType _damageType) { Damage = _damage; damageType = _damageType; }


	public void SubtractDamage(int _damage)
	{
		Damage = Mathf.Max(Damage - _damage, 0);
	}

	public void AddDamage(int _damage)
	{
		Damage += _damage;
	}



}
