using UnityEngine;

public class NejikoController : MonoBehaviour
{
    CharacterController controller;
    //Animator animator;

    Vector3 moveDirection = Vector3.zero; //移動するべき量

    public float gravity;//重力加速度
    public float speedZ;//前進する力
    public float speedJamp;//ジャンプ力
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //必要なコンポーネントを自動取得
        controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)//CharacterControllerコンポーネントが持っている接地のチェック（bool）
        {
            //垂直方向のボタン入力をチェック（Vertical ↑↓　WS）
            if (Input.GetAxis("Vertical") != 0.0f)
            {
                //このフレームにおける前進/後退の移動量が決まる
                moveDirection.z = Input.GetAxis("Vertical") * speedZ;
            }
            else
            {
                                moveDirection.z = 0;

            }
            //左右キーを押したときの回転
            transform.Rotate(0, Input.GetAxis("Horizontal") * 3, 0);

            if (Input.GetButtonDown("Jump"))//スペースキー
            {
                moveDirection.y = speedJamp;
                //animator.SetTrigger("Jump");
            }
        }

        //ここまででそのフレームの移動すべき量が決まる（moveDirectionのxとｙ）

        //重力分の力を毎フレーム追加
        //重力を考慮
        moveDirection.y -= gravity * Time.deltaTime;

        //移動実行
        //引数に与えたVector3値を、そのオブジェクトの向きにあわせてグローバルな値としてなにが正しいかに変換
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        //Moveメソッドに与えたVector3値分だけ実際にPlalyerが動く
        controller.Move(globalDirection * Time.deltaTime);

        //移動後に接地してたらｙの力はリセットする
        if (controller.isGrounded) moveDirection.y = 0;
        //速度が０以上なら走っているフラグをtrueにする
         //animator.SetBool("Speed", moveDirection.z > 0.0f);
    }
}
