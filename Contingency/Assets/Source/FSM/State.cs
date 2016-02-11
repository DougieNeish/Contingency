
public abstract class State<EntityType>
{
	public abstract void Enter(EntityType entity);
	public abstract void Execute(EntityType entity);
	public abstract void Exit(EntityType entity);
}
