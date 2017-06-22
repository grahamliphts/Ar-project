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
    ParticleSystem[] _particles;

    [SerializeField]
    int _team;

    [SerializeField]
    int _pv;

    [SerializeField]
    int _damage;

    [SerializeField]
    int _range;

    private int _pvInitial;
    public int _shield;

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
        Debug.Log(name + " fire " + gameObject.activeSelf + " " + IsAlive());
        if (!IsAlive() || gameObject.activeSelf == false)
            return;

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
                StartCoroutine("Fire", target);
            }
        }

        /*
        _inShield = false;
        _inHeal = false;
        _inPlasma = false;*/
    }
   
    private IEnumerator Fire(UnitsScripts target)
    {
        //Debug.Log(name + " attack " + target.name);

        if (_role == Role.Tank)
        {
            _particles[5].Play();
            _particles[6].Play();
            _particles[7].Play();
            _particles[8].Play();
        }
        else if(_role == Role.Heal)
        {
            _particles[4].Play();
            _particles[5].Play();
            _particles[6].Play();
            _particles[7].Play();
        }
        else if (_role == Role.Dps)
        {
            _particles[4].Play();
            _particles[5].Play();
            _particles[6].Play();
            _particles[7].Play();
            _particles[8].Play();
            _particles[9].Play();
            _particles[10].Play();
            _particles[11].Play();
        }

        yield return new WaitForSeconds(1);

        if (_role == Role.Heal)
            target.ApplyDamage(_damage / 2);
        else
            target.ApplyDamage(_damage);


        if (_role == Role.Tank)
        {
            _particles[5].Stop();
            _particles[6].Stop();
            _particles[7].Stop();
            _particles[8].Stop();
        }
        else if (_role == Role.Heal)
        {
            _particles[4].Stop();
            _particles[5].Stop();
            _particles[6].Stop();
            _particles[7].Stop();
        }
        else if (_role == Role.Dps)
        {
            _particles[4].Stop();
            _particles[5].Stop();
            _particles[6].Stop();
            _particles[7].Stop();
            _particles[8].Stop();
            _particles[9].Stop();
            _particles[10].Stop();
            _particles[11].Stop();
        }
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

        float life = _pv * 1.0f / _pvInitial;

        if (_role == Role.Tank)
        {
            if (life <= 0.8f)
                _particles[0].Play();
            if (life <= 0.6f)
                _particles[1].Play();
            if (life <= 0.4f)
                _particles[3].Play();
            if (life <= 0.2f)
                _particles[4].Play();
        }
        if (_role == Role.Heal || _role == Role.Dps)
        {
            if (life <= 0.8f)
                _particles[0].Play();
            if (life <= 0.6f)
                _particles[1].Play();
            if (life <= 0.4f)
                _particles[2].Play();
            if (life <= 0.2f)
                _particles[3].Play();
        }


        if (_pv <= 0)
            Death();
    }

    public void ApplyHeal(int heal)
    {
        if (_pv + heal > _pvInitial)
            _pv = _pvInitial;
        else
            _pv += heal;

        float life = _pv * 1.0f / _pvInitial;
        if (_role == Role.Tank)
        {
            if (life > 0.8f)
                _particles[0].Stop();
            if (life > 0.6f)
                _particles[1].Stop();
            if (life > 0.4f)
                _particles[3].Stop();
            if (life > 0.2f)
                _particles[4].Stop();
        }
        if (_role == Role.Heal || _role == Role.Dps)
        {
            if (life > 0.8f)
                _particles[0].Stop();
            if (life > 0.6f)
                _particles[1].Stop();
            if (life > 0.4f)
                _particles[2].Stop();
            if (life > 0.2f)
                _particles[3].Stop();
        }
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
        Debug.Log("Heal de " + heal + " " + name);
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

    public void DesactiveAtout()
    {
        _inHeal = false;
        _inPlasma = false;
        _inShield = false;

        _shield = 0;
    }
}
