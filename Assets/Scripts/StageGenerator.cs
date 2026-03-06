using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    const int stageChipSize = 30;//Zのスケールが30のステージ

    int currentChipIndex;//現在作成済みのステージ番号

    public Transform character;//Plalyerの位置
    public GameObject[] stageChips;//生成されるステージのカタログ
    public int startChipIndex;//最初のステージ番号
    public int preInstantiate;//どこまで先のステージを用意しておくか
    //今現在ヒエラルキーに存在しているステージ情報を生成順に取得
    public List<GameObject> generatedStageList = new List<GameObject>();

    void Start()
    {
        //初期の現在番号を定めている
        currentChipIndex = startChipIndex - 1;
        //初期状態からいくつかのステージを生成
        UpdateStage(preInstantiate);
    }

    void Update()
    {
        //キャラがどのステージのIndex番号にいるかを常に把握
        int charaPositionIndex = (int)(character.position.z / stageChipSize);

        //キャラのいる位置+5個先　が作成済みのステージ番号を上回ってしまったら
        if (charaPositionIndex + preInstantiate > currentChipIndex)
        {
            //不足分のステージを生成＆古いステージの廃棄
            UpdateStage(charaPositionIndex + preInstantiate);
        }
    }

    //ステージ生成＆古いステージの廃棄
    void UpdateStage(int toChipIndex)
    {
        //作りたい番号（引数）より現在の番号のほうが大きければ何もしない
        if (toChipIndex <= currentChipIndex) return;

        for (int i = currentChipIndex + 1; i <= toChipIndex; i++)
        {
            //戻り値GameObjectがGenerateStageメソッドで返ってくるので変数stageObjectに格納
            GameObject stageObject = GenerateStage(i);

            //確保したstageObject情報をリストに加える
            generatedStageList.Add(stageObject);
        }

        //リストの数が8になったら、一番古いStageを廃棄開始
        while (generatedStageList.Count > preInstantiate + 2)
            DestroyOldestStage();//廃棄

        //現在番号を更新
        currentChipIndex = toChipIndex;
    }

    //指定のインデックス位置にStageをランダム生成
    GameObject GenerateStage(int chipIndex)
    {
        int nextStageChip = Random.Range(0, stageChips.Length);

        GameObject stageObject = (GameObject)Instantiate(
            stageChips[nextStageChip],
            //ステージサイズと番号を利用しステージの場所を割り出してる
            new Vector3(0, 0, chipIndex * stageChipSize),
            Quaternion.identity);

        return stageObject;
    }
    void DestroyOldestStage()
    {
        //リストから先頭のオブジェクト情報を取得
        GameObject oldestStage = generatedStageList[0];
        //リストの先頭の情報（0番）をリスト上から抹消
        generatedStageList.RemoveAt(0);
        //ヒエラルキーからも対象ステージを抹消
        Destroy(oldestStage);
    }
}
