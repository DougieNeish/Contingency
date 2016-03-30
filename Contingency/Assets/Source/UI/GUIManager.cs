using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour
{
	public class UnitInfo
	{
		private Unit m_unit;
		private Text m_health;
		private Text m_state;
		private Text m_stance;

		#region Properties
		public Unit Unit
		{
			get { return m_unit; }
			set { m_unit = value; }
		}

		public Text Health
		{
			get { return m_health; }
			set { m_health = value; }
		}

		public Text State
		{
			get { return m_state; }
			set { m_state = value; }
		}

		public Text Stance
		{
			get { return m_stance; }
			set { m_stance = value; }
		}
		#endregion
		
		public UnitInfo()
		{
			m_health = GameObject.FindGameObjectWithTag("GUI/UnitInfo/Health").GetComponent<Text>();
			m_state = GameObject.FindGameObjectWithTag("GUI/UnitInfo/State").GetComponent<Text>();
			m_stance = GameObject.FindGameObjectWithTag("GUI/UnitInfo/Stance").GetComponent<Text>();

			Reset();
		}

		public void Update()
		{
			if (m_unit != null)
			{
				m_health.text = "Health: " + m_unit.Health;
				m_state.text = "State: " + m_unit.StateMachine.CurrentState.ToString();
				m_stance.text = "Stance: " + m_unit.Stance.ToString();
			}
		}

		public void Reset()
		{
			m_unit = null;
			m_health.text = "Health: -";
			m_state.text = "State: -";
			m_stance.text = "Stance: -";
		}
	}

	private Game m_game;
	private SelectionManager m_selectionManager;
	private UnitInfo m_unitInfo;
	private Text m_selectedPlayer;

	void Awake()
	{
		m_game = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Game>();
		m_selectionManager = GameObject.FindGameObjectWithTag("Player").GetComponent<SelectionManager>();
		m_unitInfo = new UnitInfo();
		m_selectedPlayer = GameObject.FindGameObjectWithTag("GUI/UnitSpawn/SelectedPlayer").GetComponent<Text>();
		m_selectedPlayer.text = Player.PlayerType.Human.ToString();
	}

	void OnEnable()
	{
		m_selectionManager.OnAnyUnitSelected += HandleUnitSelected;
	}

	void OnDisable()
	{
		m_selectionManager.OnAnyUnitSelected -= HandleUnitSelected;
	}

	void Update()
	{
		m_unitInfo.Update();
	}

	public void SetStance(int stance)
	{
		m_unitInfo.Unit.Stance = (Unit.CombatStance)stance;
	}

	public void SpawnUnit(int unitType)
	{
		int player = m_selectedPlayer.text == Player.PlayerType.Human.ToString() ? 0 : 1;
		m_game.BeginSpawn(player, (UnitController.UnitType)unitType);
	}

	public void TogglePlayer()
	{
		m_selectedPlayer.text = m_selectedPlayer.text == Player.PlayerType.Human.ToString() ? Player.PlayerType.AI.ToString() : Player.PlayerType.Human.ToString();
	}

	private void HandleUnitSelected(Unit unit)
	{
		m_unitInfo.Unit = unit;
	}
}
