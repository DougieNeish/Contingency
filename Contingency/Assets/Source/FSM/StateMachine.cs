
public class StateMachine<EntityType>
{
	private EntityType m_owner;
	private State<EntityType> m_currentState;
	private State<EntityType> m_previousState;
	private State<EntityType> m_globalState;

	public StateMachine(EntityType owner)
	{
		m_owner = owner;
		m_currentState = null;
		m_previousState = null;
		m_globalState = null;
	}

	public void Update()
	{
		if (m_currentState != null)
		{
			m_currentState.Execute(m_owner);
		}

		if (m_globalState != null)
		{
			m_globalState.Execute(m_owner);
		}
	}

	public void ChangeState(State<EntityType> newState)
	{
		if (m_currentState != null)
		{
			m_previousState = m_currentState;
			m_currentState.Exit(m_owner); 
		}

		m_currentState = newState;
		m_currentState.Enter(m_owner);
	}

	public void RevertToPreviousState()
	{
		ChangeState(m_previousState);
	}

	public bool IsInState(State<EntityType> state)
	{
		return state.GetType() == m_currentState.GetType();
	}
}
