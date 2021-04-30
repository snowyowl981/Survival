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
    public static Transform currentWeapon;
    public static Animator currentWeaponAnim;

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

    [SerializeField]
    private GunCtrl theGunCtrl;
    [SerializeField]
    private HandCtrl theHandCtrl;

    // 현재 무기의 타입
    [SerializeField]
    private string currentWeaponType;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
