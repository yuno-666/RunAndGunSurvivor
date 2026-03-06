using System.Collections;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    const int maxRemaining = 10; //充填数の上限

    [Header("弾数・保有マガジン数")]
    public int bulletRemaining = maxRemaining; //残弾数
    public int magazine = 1; //マガジン数 ※充填時に消費

    [Header("充填時間")]
    public float recoveryTime = 1.0f; //マガジン補充時間
    float counter; //充填までの残時間

    Coroutine bulletRecover; //発生中のコルーチン情報の参照用

    //弾の消費
    public void ConsumeBullet()
    {
        if (bulletRemaining > 0)//残弾があれば
        {
            bulletRemaining--;//一つ減らす
        }

    }

    //残数の取得
    public int GetBulletRemaining()
    {
        return bulletRemaining;
    }

    //弾の充填
    public void AddBullet(int num)
    {
        //今の残数を決められた最大の数にする
        bulletRemaining = num;//残弾数を最大にする
    }

    //充填メソッド
    public void RecoverBullet()
    {
        if (bulletRecover == null)//コルーチンが発動していない
        {
            if (magazine > 0)//マガジンの残数があれば
            {
                magazine--;//マガジンを消費

                //コルーチンを発動とコルーチン情報を変数に格納
                bulletRecover = StartCoroutine(RecoverBulletCol());

            }
        }
    }

    //充填コルーチン
    IEnumerator RecoverBulletCol()
    {
        counter = recoveryTime;//グローバル変数counterのセットアップ
        while (counter>0)
        {
            yield return new WaitForSeconds(1.0f);//ウェイト処理
            counter--;
        }
        //Debug.Log("コルーチンスタート！");
        //何かしらの処理
        //yield return new WaitForSeconds(recoveryTime); //ウェイト処理
        AddBullet(maxRemaining);
        bulletRecover = null;//コルーチン情報をリセット
    }

    //画面上に簡易GUI表示
    void OnGUI()
    {
        //残弾数を表示（左50、上50、幅100、高さ30：黒）
        GUI.color = Color.black;
        string label = "Bullet: " + bulletRemaining;
        GUI.Label(new Rect(50, 50, 100, 30), label);

        //残マガジンを表示（左50、上75、幅100、高さ30：黒)
        label = "magazine: " + magazine;
        GUI.Label(new Rect(50, 75, 100, 30), label);

        //充填開始～充填完了まで（左50、上25、幅100、高さ30：赤)
        //赤文字点滅
        
        if (bulletRecover != null)
        {
            GUI.color = Color.red;//赤字

            float val = Mathf.Sin(Time.time * 50);
            if (val > 0)
            {
                label = "bulletRecover：" + counter;
            }
            GUI.Label(new Rect(50, 25, 100, 30), label);
            //else
            //{
            //    label = "";
            //}

        }
       
    }
}
