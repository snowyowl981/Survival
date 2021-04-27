using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{   
    //스피드 조정 변수
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float runSpeed;
    private float applySpeed;

    [SerializeField]
    private float jumpForce;

    [SerializeField]
    private float crouchSpeed;

    // 상태 변수
    private bool isRun = false;
    private bool isGround = true;
    private bool isCrouch = false;

    //땅 착지 여부
    private CapsuleCollider capsuleCollider;

    // 앉았을 때 얼마나 앉을지 결정하는 변수
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;

    // 카메라 민감도
    [SerializeField]
    private float lookSensitivity;

    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currnetCameraRotationX = 0.0f;

    // 필요 컴포넌트
    [SerializeField]
    private Camera theCamera;
    private Rigidbody myRig;
    private Transform tr;

    // [SerializeField]
    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        myRig = GetComponent<Rigidbody>();
        tr = GetComponent<Transform>();
        applySpeed = walkSpeed;
        // localPosition을 쓰는 이유는 월드기준이 아닌 플레이어 기준
        // 월드기준에서 y값을 0으로 맞추면 땅에 박힘, 플레이어 기준에서 0은 캐릭터의 중간정도.
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;

    }

    // Update is called once per frame
    void Update()
    {
        IsGround();
        Move();
        CameraRotation();
        CharacterRoation();
        TryRun();
        TryJump();
        TryCrouch();
    }

    // 앉기 시도
    private void TryCrouch()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }
    }

    // 앉기 동작
    private void Crouch()
    {
        // 스위치 역할
        isCrouch = !isCrouch;

        if(isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        // y값 혼자 수정 불가능, 고로 벡터째로 수정해야함
        /* theCamera.transform.localPosition = new Vector3(theCamera.transform.localPosition.x,
                                                        applyCrouchPosY,
                                                        theCamera.transform.localPosition.z); */
        StartCoroutine(CrouchCoroutine());
    }

    // 부드러운 앉기 동작
    IEnumerator CrouchCoroutine()
    {
        float _posY = theCamera.transform.localPosition.y;
        int count = 0;
        while(_posY != applyCrouchPosY)
        {
            count++;
            // 보간의 단점은 정확하게 결과값을 산출해낼 수 없음(무한히 반복되기 때문)
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.1f);

            // x, y, z를 각각 수정할 수 없기 때문에 벡터를 생성해 대입
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);

            // 임시변수를 지정함으로써 무한반복을 벗어나 정해진값으로 하도록 유도
            // 보간의 단점 보완
            if(count > 15)
            {
                break;
            }
            // 1프레임 대기
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }

    // 지면 체크
    private void IsGround()
    {
        isGround = Physics.Raycast(tr.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }


    // 점프 시도
    private void TryJump()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }
    
    // 점프
    private void Jump()
    {
        // 앉은 상태에서 점프시 앉은 상태 해제
        if(isCrouch)
        {
            Crouch();
        }
        myRig.velocity = tr.up * jumpForce;
    }

    // 달리기 시도
    private void TryRun()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            if(isCrouch)
            {
                Crouch();
            }
            Running();
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            RunningCancel();
        }
    }

    // 달리기
    private void Running()
    {
        isRun = true;
        applySpeed = runSpeed;
    }

    // 달리기 캔슬
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }

    // 움직임 실행
    private void Move()
    {   
        // GetAxis : 부드러운 이동, GetAxisRaw : 즉각적인 이동(서바이벌이므로 Raw 사용)
        // Horizontal 유니티 기본 좌우 제공, Vertical 전후 제공
        float moveDirX = Input.GetAxisRaw("Horizontal");
        float moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = tr.right * moveDirX;
        Vector3 _moveVertical = tr.forward * moveDirZ;

        // normalized 벡터 합을 1로 정규화
        // applySpeed를 넣음으로써 같은 코드를 두 번 작성할 필요가 없음
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;
        // 약 0.0016
        myRig.MovePosition(tr.position + _velocity * Time.deltaTime);
    }

    // 상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currnetCameraRotationX -= _cameraRotationX;
        currnetCameraRotationX = Mathf.Clamp(currnetCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currnetCameraRotationX, 0.0f, 0.0f);
    }

    // 좌우 캐릭터 회전
    private void CharacterRoation()
    {
        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRig.MoveRotation(myRig.rotation * Quaternion.Euler(_characterRotationY));
    }
}
