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

	private SelectionManager m_selectionManager;
	private UnitInfo m_unitInfo;

	void Awake()
	{
		m_selectionManager = GameObject.FindGameObjectWithTag("Player").GetComponent<SelectionManager>();
		m_unitInfo = new UnitInfo();
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

	private void HandleUnitSelected(Unit unit)
	{
		m_unitInfo.Unit = unit;
	}
}
