using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    // --------------------------------------------------
    // Liste unités - 0 = Tank - 1 = Heal - 2 et 3 = DPS
    // --------------------------------------------------
    [SerializeField]
    UnitsScripts[] _unitsPlayer1;

    [SerializeField]
    UnitsScripts[] _unitsPlayer2;
    // --------------------------------------------------

    void Start ()
    {
		
	}
	

	void Update ()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Synchronisation();
        }
    }

    public UnitsScripts FindTargetCloser(Transform positionSource, int playerToSearch)
    {
        float distance = -1;
        UnitsScripts target = null;

        UnitsScripts[] unitsToSearch = (playerToSearch == 1) ? _unitsPlayer1 : _unitsPlayer2;
        foreach (UnitsScripts unit in unitsToSearch)
        {
            if (unit.IsAlive())
            {
                Transform positionTarget = unit.GetPosition();

                float newDistance = Mathf.Sqrt(Mathf.Pow(positionTarget.position.x - positionSource.position.x, 2) + Mathf.Pow(positionTarget.position.z - positionSource.position.z, 2));
                if (distance == -1 || newDistance < distance)
                {
                    distance = newDistance;
                    target = unit;
                }
            }
        }

        return target;
    }

    public UnitsScripts FindAllyToHeal(Transform positionSource, int playerToSearch)
    {
        float distance = -1;
        UnitsScripts target = null;

        UnitsScripts[] unitsToSearch = (playerToSearch == 1) ? _unitsPlayer1 : _unitsPlayer2;
        foreach (UnitsScripts unit in unitsToSearch)
        {
            if (unit.IsAlive() && unit.IsDamaged())
            {
                Transform positionTarget = unit.GetPosition();

                float newDistance = Mathf.Sqrt(Mathf.Pow(positionTarget.position.x - positionSource.position.x, 2) + Mathf.Pow(positionTarget.position.y - positionSource.position.y, 2));
                if (distance == -1 || newDistance < distance)
                {
                    distance = newDistance;
                    target = unit;
                }
            }
        }

        return target;
    }

    public void Synchronisation()
    {
        for(int i = 3; i >= 0; i--)
        {
            for (int player = 1; player <= 2; player++)
            {
                UnitsScripts unit = (player == 1 ? _unitsPlayer1[i] : _unitsPlayer2[i]);

                unit.Alignement();
            }
        }
    }

}
