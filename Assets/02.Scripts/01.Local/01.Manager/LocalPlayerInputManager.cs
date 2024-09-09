using UnityEngine;
using UnityEngine.InputSystem;

public class LocalPlayerInputManager : MonoBehaviour
{
    public static LocalPlayerInputManager instance; 
    // Input Action Asset 참조
    public InputActionAsset inputActionAsset;

    #region 1P

    // 1P Action Map
    private InputActionMap player1ActionMap;

    // 1P InputAction
    private InputAction moveAction_1P;

    // 1P 입력 값
    public Vector2 moveInput_1P { get; private set; }
    #endregion

    #region 2P

    // 2P Action Map
    private InputActionMap player2ActionMap;
    
    // 2P InputAction
    private InputAction moveAction_2P;

    // 2P 입력 값
    public Vector2 moveInput_2P { get; private set; }
    #endregion

    void OnEnable()
    {
        // 1P와 2P의 Action Map을 가져옴
        player1ActionMap = inputActionAsset.FindActionMap("Player1");
        player2ActionMap = inputActionAsset.FindActionMap("Player2");

        // 1P와 2P의 개별 액션 가져오기
        moveAction_1P = player1ActionMap.FindAction("Move");
        moveAction_2P = player2ActionMap.FindAction("Move");

        // 두 Action Map 활성화
        player1ActionMap.Enable();
        player2ActionMap.Enable();
    }

    void OnDisable()
    {
        // 두 Action Map 비활성화
        player1ActionMap.Disable();
        player2ActionMap.Disable();
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    void Update()
    {
        HandleActions_1P();
        HandleActions_2P();
        
    }

    private void HandleActions_1P()  // 1P의 입력 처리 
    {
        moveInput_1P = moveAction_1P.ReadValue<Vector2>();

    }
    private void HandleActions_2P() // 2P의 입력 처리 
    {
        moveInput_2P = moveAction_2P.ReadValue<Vector2>();
    }
}
