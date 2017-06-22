using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsScripts : MonoBehaviour
{
    public enum Role
    {
        Tank,
        Heal,
        Dps,
        Debris
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

    [SerializeField]
    int _range;

    private int _pvInitial;
    private int _shield;

    public bool _inShield = false;
    public bool _inHeal = false;
    public bool _inPlasma = false;

    private Vector3 _vectorRotation;

    void Start()
    {
        _pvInitial = _pv;
        _shield = 0;

        _vectorRotation.x = Random.Range(0, 5);
        _vectorRotation.y = Random.Range(0, 5);
        _vectorRotation.z = Random.Range(0, 5);
    }

    void Update()
    {
        if(_role == Role.Debris)
        {
            transform.GetChild(0).transform.Rotate(_vectorRotation * 0.1f);
        }
    }

    public void Alignement()
    {
        bool healDone = false;
        if(_role == Role.Heal)
        {
            UnitsScripts targetHeal = _gameManager.FindAllyToHeal(transform, _team, _range);

            if (targetHeal != null)
            {
                targetHeal.ApplyHeal(_damage);
                healDone = true;
                //Debug.Log("Heal Team" + _team + " ship : " + targetHeal.name);
            }
        }

        if((_role != Role.Heal) || (_role == Role.Heal && !healDone))
        {
            UnitsScripts target = _gameManager.FindTargetCloser(transform, _team == 1 ? 2 : 1, _range);

            if(target != null)
            {
                // Alignement Model
                transform.LookAt(target.transform);

                RaycastHit hit;
                Ray ray = new Ray(transform.position, transform.forward);
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.gameObject.tag == "Asteroid")
                        target = hit.collider.transform.GetComponentInParent<UnitsScripts>();
                }

                // Fire + Particles
                Fire(target);
            }
        }

        _inShield = false;
        _inHeal = false;
        _inPlasma = false;
    }
   
    private void Fire(UnitsScripts target)
    {
        //Debug.Log(name + " attack " + target.name);

        if (_role == Role.Heal)
            target.ApplyDamage(_damage / 2);
        else
            target.ApplyDamage(_damage);
    }



    public bool IsAlive()
    {
        return _pv > 0 ? true : false;
    }

    public bool IsDamaged()
    {
        return _pv != _pvInitial ? true : false;
    }

    public Transform GetPosition()
    {
        return transform;
    }

    public void ApplyDamage(int damage)
    {
        if (_shield <= damage)
        {
            _pv += (_shield - damage);
            _shield = 0;
        }
        else
            _shield -= damage;

        if (_pv <= 0)
            Death();
    }

    public void ApplyHeal(int heal)
    {
        if (_pv + heal > _pvInitial)
            _pv = _pvInitial;
        else
            _pv += heal;
    }


    public void ApplyShield(int shield)
    {
        // IF IN SHIELD TO DO
        if(_inShield)
            _shield = shield;
    }

    public void ApplyNuke(int damage)
    {
        // IF IN SHIELD TO DO
        if(_inPlasma)
            ApplyDamage(damage);
    }

    public void ApplyHealZone(int heal)
    {
        // IF IN SHIELD TO DO
        if(_inHeal)
            ApplyHeal(heal);
    }

    private void Death()
    {
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider Other)
    {
        if (Other.gameObject.tag == "Shield_1" && _team == 1)
        {
            _inShield = true;//   playerInBounds = true;
        }
        else if ( Other.gameObject.tag == "Shield_2" && _team == 2)
        {
            _inShield = true;//
        }

        if(Other.gameObject.tag == "Heal_1" && _team == 1)
        {
            _inHeal = true;//
        }
        else if (Other.gameObject.tag == "Heal_2" && _team == 2)
        {
            _inHeal = true;//
        }

        if(Other.gameObject.tag == "Plasma_1" || Other.gameObject.tag == "Plasma_2")
        {
            _inPlasma = true;//
        }
    }
    void OnTriggerExit(Collider Other)
    {
        Debug.Log(Other.tag);
        if (Other.gameObject.tag == "Shield_1" && _team == 1)
        {
            _inShield = false;//   playerInBounds = true;
        }
        else if (Other.gameObject.tag == "Shield_2" && _team == 2)
        {
            _inShield = false;//
        }

        if (Other.gameObject.tag == "Heal_1" && _team == 1)
        {
            _inHeal = false;//
        }
        else if (Other.gameObject.tag == "Heal_2" && _team == 2)
        {
            _inHeal = false;//
        }

        if (Other.gameObject.tag == "Plasma_1" || Other.gameObject.tag == "Plasma_2")
        {
            _inPlasma = false;//
        }
    }
}
