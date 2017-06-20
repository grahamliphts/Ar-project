using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsScripts : MonoBehaviour
{
    public enum Role
    {
        Tank,
        Heal,
        Dps
    }
    public Role _role;

    [SerializeField]
    GameManager _gameManager;

    [SerializeField]
    int _team;

    [SerializeField]
    int _pv;

    [SerializeField]
    int _damage;

    private int pvInitial;

    void Start()
    {
        pvInitial = _pv;
    }

    public void Alignement()
    {
        bool healDone = false;
        if(_role == Role.Heal)
        {
            UnitsScripts targetHeal = _gameManager.FindAllyToHeal(transform, _team);

            if (targetHeal != null)
            {
                targetHeal.ApplyHeal(_damage);
                healDone = true;
                Debug.Log("Heal Team" + _team + " ship : " + targetHeal.name);
            }
        }

        if((_role != Role.Heal) || (_role == Role.Heal && !healDone))
        {
            UnitsScripts target = _gameManager.FindTargetCloser(transform, _team == 1 ? 2 : 1);

            if(target != null)
            {
                // Alignement Model
                // TODO

                // Fire + Particles
                Fire(target);
            }
        }
    }
   
    private void Fire(UnitsScripts target)
    {
        Debug.Log(name + " attack " + target.name);
        target.ApplyDamage(_damage);
    }



    public bool IsAlive()
    {
        return _pv > 0 ? true : false;
    }

    public bool IsDamaged()
    {
        return _pv != pvInitial ? true : false;
    }

    public Transform GetPosition()
    {
        return transform;
    }

    public void ApplyDamage(int damage)
    {
        _pv -= damage;
    }

    public void ApplyHeal(int heal)
    {
        if (_pv + heal > pvInitial)
            _pv = pvInitial;
        else
            _pv += heal;
    }

}
