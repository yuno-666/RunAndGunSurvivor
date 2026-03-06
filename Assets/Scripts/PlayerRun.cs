using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRun : MonoBehaviour
{
    //横移動のX軸の限界
    const int MinLane = -2;
    const int MaxLane = 2;
    const float LaneWidth = 1.0f;//横移動幅

    //体力の最大値
    const int DefaultLife = 3;

    //ダメージをくらったときの硬直時間
    const float StunDuration = 0.5f;

    CharacterController controller;
    Animator animator;

    Vector3 moveDirection = Vector3.zero;//移動するべき量
    int targetLane;//向かうべきX座標
    int life = DefaultLife;//現・体力
    float recoverTime = 0.0f;//復帰までのカウントダウン

    float currentMoveInputX;//InputSystemの入力値を格納
    //Inputを連続で認知しないためのインターバルのコルーチン
    Coroutine resetIntervalCol;

    public float gravity = 20.0f;//重力加速度
    public float speedZ = 5.0f;//前進スピード
    public float speedX = 3.0f;//横移動スピード
    public float speedJump = 8.0f;//ジャンプ力
    public float accelerationZ = 10.0f;//前進加速力

    void OnMove(InputValue value)
    {
        //すでに前に入力検知してインターバル中であれば何もしない
        if (resetIntervalCol == null)
        {
            //検知した値をVector2で表現して変数InputVectorに格納
            Vector2 inputVector = value.Get<Vector2>();
            //変数InputVectorのうち、x座標にまつわる値を変数currentMoveInputXに格納
            currentMoveInputX = inputVector.x;
        }
    }
    void OnJump(InputValue value)
    {

        //ジャンプに関する入力を検知したらジャンプメソッド
        Jamp();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
    //現在の体力を返す
    public int Life()
    {
        return life;
    }

    //体力を1回復
    public void LifeUp()
    {
        life++;
        if (life > DefaultLife) life = DefaultLife;
    }
    //Playerを硬直させるべきかチェックするメソッド
    bool IsStun()
    {
        return recoverTime > 0 || life <= 0;
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown("left")) MoveToLeft();
        //if (Input.GetKeyDown("right")) MoveToRight();
        //if (Input.GetKeyDown("space")) Jamp();

        //左に押されていたら
        if (currentMoveInputX < 0) MoveToLeft();
        //右に押されていたら
        if (currentMoveInputX > 0) MoveToRight();

        if (IsStun())
        {
            //moveDirectionのxを0
            moveDirection.x = 0;
            //moveDirectionのzを0
            moveDirection.z = 0;

            //recoverTimeをカウントダウン
            recoverTime -= Time.deltaTime;
        }
        else
        {
            //その時のmoveDirection.zにaccelerationZの加速度を足していく
            float acceleratedZ = moveDirection.z + (accelerationZ * Time.deltaTime);
            //導き出した値に上限を設けて、それをmoveDirection.zとする
            moveDirection.z = Mathf.Clamp(acceleratedZ, 0, speedZ);

            //横移動のアルゴリズム
            //目的地自分の位置の差を取り、1レーンあたりの幅に対して割合を見る
            float ratioX = (targetLane * LaneWidth - transform.position.x) / LaneWidth;
            //割合に変数speedXを係数としてかけた値がmoveDirection.x
            moveDirection.x = ratioX * speedX;
        }
        //重力の加速度をmoveDirection.y
        moveDirection.y -= gravity * Time.deltaTime;

        //回転時、自分にとってのZ軸をグローバル座標の値に変換
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        //CharacterControllerコンポーネントのMoveメソッドに授けてPlayerを動かす
        controller.Move(moveDirection * Time.deltaTime);

        //地面についていたら重力をリセット
        if (controller.isGrounded) moveDirection.y = 0;
    }
    public void MoveToLeft()
    {
        //硬直フラグがtrueなら何もしない
        if (IsStun()) return;
        //地面にいるかつtargetがまだ最小でない
        if (controller.isGrounded && targetLane > MinLane)
        {
            targetLane--;
            currentMoveInputX = 0;//何も入力されていない状態にリセット
                                  //次の入力を検知を有効にするまでのインターバル開始
            resetIntervalCol = StartCoroutine(ResetIntervalcol());
        }
    }
    public void MoveToRight()
    {
        //硬直フラグがtrueなら何もしない
        if (IsStun()) return;
        //地面にいるかつtargetがまだ最大でない
        if (controller.isGrounded && targetLane < MaxLane)
        {
            targetLane++;
            currentMoveInputX = 0;//何も入力されていない状態にリセット
            //次の入力を検知を有効にするまでのインターバル開始
            resetIntervalCol = StartCoroutine(ResetIntervalcol());

        }
    }
    IEnumerator ResetIntervalcol()
    {
        //とりあえず0.1秒待つ
        yield return new WaitForSeconds(0.1f);
        resetIntervalCol = null;//コルーチン情報を解除
    }

    public void Jamp()
    {
        if (IsStun()) return;
        if (controller.isGrounded)
        {
            moveDirection.y = speedJump;

        }
    }

    //CharacterControllerコンポーネンがなにかとぶつかった時
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(IsStun())return;
        //相手がエネミーなら
        if(hit.gameObject.tag == "Enemy")
        {
            life--; //体力が減る
            recoverTime = StunDuration;//recovertimeに定数をセッティング
            Destroy(hit.gameObject);//ぶつかった相手を消す
        }
    }
}
