using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WeaponSway : MonoBehaviour
{
    [Header("回転の揺れパラメータ")]

    [Tooltip("片手持ちの際の回転の揺れの大きさ")]
    public float swayRotationAmount = 3.0f;

    [Tooltip("片手持ちの際の揺れの速さ")]
    public float swaySpeed = 10.0f;

    [Tooltip("揺れが適用される/元に戻る際の滑らかさ")]
    public float swaySmoothness = 5.0f;

    [Header("参照")]
    [Tooltip("揺れを適用する対象のTransform（ライフルのモデル部分など）")]
    public Transform swayTarget;

    private bool isOneHanded = false;
    private Vector3 currentSwayPivot = Vector3.zero;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private float randomSeedX;
    private float randomSeedY;

    void Start()
    {
        if (swayTarget == null)
        {
            Debug.LogError("Sway Targetが設定されていません！", this);
            this.enabled = false;
            return;
        }

        initialLocalPosition = swayTarget.localPosition;
        initialLocalRotation = swayTarget.localRotation;

        // 揺れに使用するランダムシードを初期化
        randomSeedX = Random.Range(0f, 100f);
        randomSeedY = Random.Range(0f, 100f);
    }

    void Update()
    {
        if (swayTarget == null) return;

        if (isOneHanded)
        {
            ApplyPureRotationalSway();
        }
        else
        {
            ReturnToInitial();
        }
    }

    private void ApplyPureRotationalSway()
    {
        // Perlinノイズを使って滑らかな揺れ（回転）を生成
        float time = Time.time * swaySpeed;
        float swayX = (Mathf.PerlinNoise(time, randomSeedX) - 0.5f) * 2f;
        float swayY = (Mathf.PerlinNoise(randomSeedY, time) - 0.5f) * 2f;

        Quaternion swayRotation = Quaternion.Euler(swayY * swayRotationAmount, swayX * swayRotationAmount, 0);

        // --- ピボットを基点とした純粋な回転計算 ---
        // 1. 目標の回転を計算
        Quaternion targetRotation = swayRotation * initialLocalRotation;

        // 2. 目標の回転によって、初期位置がピボット周りにどう移動するかを計算
        Vector3 pivotToModel = initialLocalPosition - currentSwayPivot;
        Vector3 targetPosition = currentSwayPivot + (swayRotation * pivotToModel);

        // 3. 計算した目標値に滑らかに追従させる
        swayTarget.localPosition = Vector3.Lerp(swayTarget.localPosition, targetPosition, Time.deltaTime * swaySmoothness);
        swayTarget.localRotation = Quaternion.Slerp(swayTarget.localRotation, targetRotation, Time.deltaTime * swaySmoothness);
    }

    private void ReturnToInitial()
    {
        swayTarget.localPosition = Vector3.Lerp(swayTarget.localPosition, initialLocalPosition, Time.deltaTime * swaySmoothness);
        swayTarget.localRotation = Quaternion.Slerp(swayTarget.localRotation, initialLocalRotation, Time.deltaTime * swaySmoothness);
    }

    // XR Grab Interactableのイベントから呼び出すための公開メソッド
    public void OnSelectChanged(BaseInteractionEventArgs args)
    {
        if (args.interactableObject is not IXRSelectInteractable interactable)
            return;

        int handCount = interactable.interactorsSelecting.Count;
        Debug.Log($"Hand Count: {handCount}");
        isOneHanded = (handCount == 1);

        if (isOneHanded)
        {
            var interactor = interactable.interactorsSelecting[0];
            Transform attachTransform = interactable.GetAttachTransform(interactor);
            if (attachTransform != null)
            {
                currentSwayPivot = swayTarget.parent.InverseTransformPoint(attachTransform.position);
            }
        }
    }
}
