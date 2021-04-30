using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponManager : MonoBehaviour
{
    /* 공유 자원, 클래스 변수 = 정적 변수
    장점 : 쉬운 접근 가능, 단점 : 보호수준 떨어짐, 메모리 낭비 */

    // 무기 교체
    public static bool isChangeWeapon;

    // 현재 무기와 현재 무기 애니메이션
    public static Transform currentWeapon; // 기존의 무기를 가려주는 역할만 함
    public static Animator currentWeaponAnim;

    // 현재 무기 타입
    [SerializeField]
    private string currentWeaponType;

    // 무기 교체 딜레이, 무기 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeWeaponDelayTime;
    [SerializeField]
    private float changeWeaponEndDelayTime;

    // 무기 종류들 전부 관리
    [SerializeField]
    private Gun[] guns;
    [SerializeField]
    private Hand[] hands;

    // 관리 차원에서 쉽게 무기 접근이 가능하도록 만듦
    private Dictionary<string, Gun> gunDictionary = new Dictionary<string, Gun>();
    private Dictionary<string, Hand> handDictionary = new Dictionary<string, Hand>();

    // 필요 컴포넌트
    [SerializeField]
    private GunCtrl theGunCtrl;
    [SerializeField]
    private HandCtrl theHandCtrl;


    // Start is called before the first frame update
    void Start()
    {
        for(int i=0; i<guns.Length; i++)
        {
            gunDictionary.Add(guns[i].gunName, guns[i]);
        }

        for(int i=0; i<hands.Length; i++)
        {
            handDictionary.Add(hands[i].handName, hands[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isChangeWeapon)
        {
            // 무기 교체 실행
            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                StartCoroutine(ChangeWeaponCoroutine("HAND", "맨손"));
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                StartCoroutine(ChangeWeaponCoroutine("GUN", "SubMachineGun1"));
            }
        }
    }

    public IEnumerator ChangeWeaponCoroutine(string _type, string _name)
    {
        isChangeWeapon = true;
        currentWeaponAnim.SetTrigger("Weapon_Out");

        yield return new WaitForSeconds(changeWeaponDelayTime);

        CancelPreWeaponAction();
        WeaponChange(_type, _name);

        yield return new WaitForSeconds(changeWeaponEndDelayTime);

        currentWeaponType = _type;
        isChangeWeapon = false;
    }

    private void CancelPreWeaponAction()
    {
        switch(currentWeaponType)
        {
            case "GUN":
                theGunCtrl.CancelFineSight();
                theGunCtrl.CancelReload();
                GunCtrl.isActivate = false;
                break;

            case "HAND":
                HandCtrl.isActivate = false;
                break;
        }
    }

    private void WeaponChange(string _type, string _name)
    {
        if(_type == "GUN")
        {
            theGunCtrl.GunChange(gunDictionary[_name]);
        }
        else if(_type == "HAND")
        {
            theHandCtrl.HandChange(handDictionary[_name]);
        }
    }
}
