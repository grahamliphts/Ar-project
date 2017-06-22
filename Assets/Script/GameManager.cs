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

    [SerializeField]
    GameObject[] _spellsPlayer1;

    [SerializeField]
    GameObject[] _spellsPlayer2;

    bool[] _spellsUsedPlayer1 = { false, false, false };
    bool[] _spellsUsedPlayer2 = { false, false, false };

    [SerializeField]
    int _nukeDamage = 400;

    [SerializeField]
    int _shieldPower = 400;

    [SerializeField]
    int _healZone = 400;



    void Start ()
    {
		
	}
	

	void Update ()
    {
        if(Input.GetKeyUp(KeyCode.Space))
        {
            Resolve();
        }
    }

    public UnitsScripts FindTargetCloser(Transform positionSource, int playerToSearch, int distanceMax = -1)
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
                if (distance == -1 || newDistance < distance && newDistance < distanceMax)
                {
                    distance = newDistance;
                    target = unit;
                }
            }
        }

        return target;
    }

    public UnitsScripts FindAllyToHeal(Transform positionSource, int playerToSearch, int distanceMax = -1)
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
                if (distance == -1 || newDistance < distance && newDistance < distanceMax)
                {
                    distance = newDistance;
                    target = unit;
                }
            }
        }

        return target;
    }

    public void Resolve()
    {

        // Déclenche et applique l'atout Shield
        for (int player = 1; player <= 2; player++)
        {
            if (CheckSpellActive(player, 0) && SpellAuthorization(player, 0))
            {
                for (int j = 3; j >= 0; j--)
                {
                    UnitsScripts unit = (player == 1 ? _unitsPlayer1[j] : _unitsPlayer2[j]);

                    unit.ApplyShield(_shieldPower);
                }

                if (player == 1)
                    _spellsUsedPlayer1[0] = true;
                if (player == 2)
                    _spellsUsedPlayer2[0] = true;
            }
        }

        // Déclenche et applique l'atout Nuke
        for (int player = 1; player <= 2; player++)
        {
            if (CheckSpellActive(player, 1) && SpellAuthorization(player, 1))
            {
                for (int j = 3; j >= 0; j--)
                {
                    UnitsScripts unit = (player == 1 ? _unitsPlayer1[j] : _unitsPlayer2[j]);

                    unit.ApplyNuke(_nukeDamage);
                }

                if (player == 1)
                    _spellsUsedPlayer1[1] = true;
                if (player == 2)
                    _spellsUsedPlayer2[1] = true;
            }
        }
        
        // Déclenche et applique l'atout Heal
        for (int player = 1; player <= 2; player++)
        {
            if (CheckSpellActive(player, 2) && SpellAuthorization(player, 2))
            {
                for (int j = 3; j >= 0; j--)
                {
                    UnitsScripts unit = (player == 1 ? _unitsPlayer1[j] : _unitsPlayer2[j]);

                    unit.ApplyHealZone(_healZone);
                }

                if (player == 1)
                    _spellsUsedPlayer1[2] = true;
                if (player == 2)
                    _spellsUsedPlayer2[2] = true;
            }
        }


        // Tirs des vaisseaux
        for (int i = 3; i >= 0; i--)
        {
            for (int player = 1; player <= 2; player++)
            {
                UnitsScripts unit = (player == 1 ? _unitsPlayer1[i] : _unitsPlayer2[i]);

                unit.Alignement();
            }
        }
    }

    public bool CheckSpellActive(int player, int id)
    {
        if (player == 1)
        {
            if (_spellsUsedPlayer1[id] == true)
                return false;

            if (id == 0)
            {
                MeshRenderer mesh = _spellsPlayer1[0].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
                return mesh.enabled;
            }
            if (id == 1)
            {
                ParticleSystemRenderer ps = _spellsPlayer1[1].transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                return ps.enabled;
            }
            if (id == 2)
            {
                ParticleSystemRenderer ps = _spellsPlayer1[2].transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                return ps.enabled;
            }
        }
        if (player == 2)
        {
            if (_spellsUsedPlayer2[id] == true)
                return false;

            if (id == 0)
            {
                MeshRenderer mesh = _spellsPlayer2[0].transform.GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
                return mesh.enabled;
            }
            if (id == 1)
            {
                ParticleSystemRenderer ps = _spellsPlayer2[1].transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                return ps.enabled;
            }
            if (id == 2)
            {
                ParticleSystemRenderer ps = _spellsPlayer2[2].transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
                return ps.enabled;
            }
        }
        return false;
    }

    public bool SpellAuthorization(int player, int id)
    {
        if (player == 1)
        {
            if (_spellsUsedPlayer1[id] == true)
                return false;

            if (id == 0)
            {
                return _unitsPlayer1[0].IsAlive();
            }
            if (id == 1)
            {
                return _unitsPlayer1[1].IsAlive();
            }
            if (id == 2)
            {
                return (_unitsPlayer1[2].IsAlive() || _unitsPlayer1[3].IsAlive());
            }
        }
        if (player == 2)
        {
            if (_spellsUsedPlayer2[id] == true)
                return false;

            if (id == 0)
            {
                return _unitsPlayer2[0].IsAlive();
            }
            if (id == 1)
            {
                return _unitsPlayer2[1].IsAlive();
            }
            if (id == 2)
            {
                return (_unitsPlayer2[2].IsAlive() || _unitsPlayer2[3].IsAlive());
            }
        }

        return false;
    }
    
}
