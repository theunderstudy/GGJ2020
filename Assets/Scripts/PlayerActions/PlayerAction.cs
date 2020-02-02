using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public UpgradeTypes UpgradeToPlace;

    public UpgradeTypes[] AffectedTypes;

    protected PlayerMouseinput MouseInput;

    protected GridTile m_SelectedTile;



    private void Start()
    {
        if (MouseInput == null)
        {
            MouseInput = PlayerMouseinput.Instance;
        }
    }

    public virtual bool CanSelectAction()
    {
        return true;
    }


    public virtual void MouseDown()
    {
        GridTile _newTile = MouseInput.GetTileAtMousePosition();
        if (_newTile != null)
        {
            if (!CanWorkTile(_newTile))
            {
                return;
            }

            if (CanUpgrade(_newTile.UpgradeType))
            {
                _newTile.UpgradeTile(UpgradeToPlace);

                m_SelectedTile.MoveTileVerticallyOverTime(0, 0.2f);
                PlayerController.Instance.StartWork(_newTile.transform.position, 1f);


            }
        }
    }

    public virtual void MousePositionUpdated()
    {
        GridTile _newTile = MouseInput.GetTileAtMousePosition();

      
        if (_newTile == null)
        {
            if (m_SelectedTile != null)
            {
                m_SelectedTile.MoveTileVerticallyOverTime(0, 0.2f);
                m_SelectedTile = null;
            }
            return;
        }
        if (!CanWorkTile(_newTile))
        {
            return;
        }
        if (_newTile == m_SelectedTile)
        {
            return;
        }

        if (m_SelectedTile != null)
        {
            m_SelectedTile.MoveTileVerticallyOverTime(0, 0.2f);
        }

        m_SelectedTile = _newTile;

        if (CanUpgrade(m_SelectedTile.UpgradeType))
        {
            m_SelectedTile.MoveTileVerticallyOverTime(0.15f, 0.02f);
        }
        else
        {
            m_SelectedTile = null;
        }
    }

    public virtual bool CanUpgrade(UpgradeTypes upgradeType)
    {
       
        for (int i = 0; i < AffectedTypes.Length; i++)
        {
            if (AffectedTypes[i] == upgradeType)
            {
                return true;
            }
        }
        return false;
    }


    public bool CanWorkTile(GridTile _tile)
    {
        if (_tile == PlayerController.Instance.CurrentTile )
        {
            return true;    
        }

        if (_tile == null)
        {
            return false;
        }
        if (PlayerController.Instance.CurrentTile == null)
        {
            return false;
        }
        if (PlayerController.Instance.CurrentTile.AdjacentTiles.ContainsKey(_tile.Key))
        {
            return true;
        }
        return false;
    }

}
