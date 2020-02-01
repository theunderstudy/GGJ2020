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

    public GameObject[] GridHighlights;

    protected override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;

    }

    private void Start()
    {
        SetPlayerAction(PlayerActions[0]);

    }


    private void Update()
    {
        if (PlayerController.Instance.bWorking)
        {
            return;
        }

        CheckPlayerInput();


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetPlayerAction(PlayerActions[0]);// till
            UIManager.Instance.HighlightButton(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UIManager.Instance.HighlightButton(1);
            SetPlayerAction(PlayerActions[1]);// plant
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetPlayerAction(PlayerActions[2]);// water

            UIManager.Instance.HighlightButton(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetPlayerAction(PlayerActions[3]);// harvest

            UIManager.Instance.HighlightButton(3);
        }
        
    }


    public void CheckPlayerInput()
    {
        if (Input.GetMouseButton(0))
        {
            if (CurrentPlayerAction != null)
            {
                CurrentPlayerAction.MouseDown();
                HighlightWorkableTiles();
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

    private void HighlightWorkableTiles()
    {
        for (int i = 0; i < GridHighlights.Length; i++)
        {
            GridHighlights[i].SetActive(false);
        }

        if (CurrentPlayerAction!=null)
        {
            GridTile _playerTile = PlayerController.Instance.CurrentTile;
            if (_playerTile == null)
            {
                return;
            }
            if (CurrentPlayerAction.CanUpgrade(_playerTile.UpgradeType))
            {
                GameObject _highlight = GetInactiveHightlight();
                _highlight.gameObject.SetActive(true);
                _highlight.transform.parent = _playerTile.transform;
                _highlight.transform.localPosition = Vector3.zero;
            }

            foreach (var _tile in _playerTile.AdjacentTiles)
            {
                if (CurrentPlayerAction.CanUpgrade(_tile.Value.UpgradeType))
                {
                    GameObject _highlight = GetInactiveHightlight();
                    _highlight.gameObject.SetActive(true);
                    _highlight.transform.parent = _tile.Value.transform;
                    _highlight.transform.localPosition = Vector3.zero;
                }
            }
        }
    }

    private GameObject GetInactiveHightlight()
    {
        for (int i = 0; i < GridHighlights.Length; i++)
        {
            if (!GridHighlights[i].activeInHierarchy)
            {
                return GridHighlights[i];
            }
        }

        return null;
    }

    public void SetPlayerAction(PlayerAction newAction)
    {
        CurrentPlayerAction = newAction;
        HighlightWorkableTiles();
    }
}
