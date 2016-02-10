
public interface IAttacker
{
	IDamageable CurrentTarget {	get; }

	void Attack(IDamageable target);
}
