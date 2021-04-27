using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCtrl : MonoBehaviour
{   
    // 현재 장착된 핸드형 타입 무기
    [SerializeField]
    private Hand currentHand;

    // 공격중?
    private bool isAttack = false;
    private bool isSwing = false;

    // 맞은 객체의 정보
    private RaycastHit hitInfo;

    // Update is called once per frame
    void Update()
    {
        TryAttack();
    }

    private void TryAttack()
    {   
        // 좌클릭시 공격(유니티 기본)
        if(Input.GetButton("Fire1"))
        {
            if(!isAttack)
            {
                // 코루틴
                StartCoroutine(AttackCoroutine());
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        isAttack = true;
        currentHand.anim.SetTrigger("Attack");
        yield return new WaitForSeconds(currentHand.attackDelayA);
        isSwing = true;

        // 공격 활성화 시점
        StartCoroutine(HitCoroutine());

        yield return new WaitForSeconds(currentHand.attackDelayB);
        isSwing = false;

        yield return new WaitForSeconds(currentHand.attackDelay - currentHand.attackDelayA - currentHand.attackDelayB);
        isAttack = false;
    }

    IEnumerator HitCoroutine()
    {
        while(isSwing)
        {
            if(CheckObject())
            {
                isSwing = false;
                Debug.Log(hitInfo.transform.name);
            }
            // 1프레임 대기
            yield return null;
        }
    }

    private bool CheckObject()
    {
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, currentHand.Range))
        {
            return true;
        }
        return false;
    }
}
