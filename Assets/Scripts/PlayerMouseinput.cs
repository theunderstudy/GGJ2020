using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMouseinput : Singleton<PlayerMouseinput>
{

    private Ray m_Ray;
    private RaycastHit m_Hit;
    private Camera m_Camera;

    private PlayerAction CurrentPlayerAction;
    public PlayerAction[] PlayerActions;
    protected override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;

        SetPlayerAction(PlayerActions[0]);
    }


    private void Update()
    {
        CheckPlayerInput();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DayNightManager.Instance.StartNewDay();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SetPlayerAction(PlayerActions[0]);
        } 
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetPlayerAction(PlayerActions[1]);
        }         
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            SetPlayerAction(PlayerActions[2]);
        }


    }


    public void CheckPlayerInput()
    {      
        if (Input.GetMouseButton(0))
        {
            if (CurrentPlayerAction != null)
            {
                CurrentPlayerAction.MouseDown();
            }
        }
        else
        {
            if (CurrentPlayerAction != null)
            {
                CurrentPlayerAction.MousePositionUpdated();
            }
        }
    }
    public GridTile GetTileAtMousePosition()
    {
        m_Ray = m_Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(m_Ray, out m_Hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("GridTile")))
        {
            GridTile _tile = m_Hit.collider.gameObject.GetComponentInParent<GridTile>();

            if (_tile == null)
            {
                // check if they hit an upgrade
                UpgradeBase _upgrade = m_Hit.collider.gameObject.GetComponentInParent<UpgradeBase>();
                if (_upgrade != null)
                {
                    _tile = _upgrade.ParentTile;
                }
            }
            return _tile;
        }
        return null;
    }

    public void SetPlayerAction(PlayerAction newAction)
    {
        CurrentPlayerAction = newAction;

    }
}
