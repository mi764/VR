using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WeaponSway : MonoBehaviour
{
    [Header("��]�̗h��p�����[�^")]

    [Tooltip("�Ў莝���̍ۂ̉�]�̗h��̑傫��")]
    public float swayRotationAmount = 3.0f;

    [Tooltip("�Ў莝���̍ۂ̗h��̑���")]
    public float swaySpeed = 10.0f;

    [Tooltip("�h�ꂪ�K�p�����/���ɖ߂�ۂ̊��炩��")]
    public float swaySmoothness = 5.0f;

    [Header("�Q��")]
    [Tooltip("�h���K�p����Ώۂ�Transform�i���C�t���̃��f�������Ȃǁj")]
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
            Debug.LogError("Sway Target���ݒ肳��Ă��܂���I", this);
            this.enabled = false;
            return;
        }

        initialLocalPosition = swayTarget.localPosition;
        initialLocalRotation = swayTarget.localRotation;

        // �h��Ɏg�p���郉���_���V�[�h��������
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
        // Perlin�m�C�Y���g���Ċ��炩�ȗh��i��]�j�𐶐�
        float time = Time.time * swaySpeed;
        float swayX = (Mathf.PerlinNoise(time, randomSeedX) - 0.5f) * 2f;
        float swayY = (Mathf.PerlinNoise(randomSeedY, time) - 0.5f) * 2f;

        Quaternion swayRotation = Quaternion.Euler(swayY * swayRotationAmount, swayX * swayRotationAmount, 0);

        // --- �s�{�b�g����_�Ƃ��������ȉ�]�v�Z ---
        // 1. �ڕW�̉�]���v�Z
        Quaternion targetRotation = swayRotation * initialLocalRotation;

        // 2. �ڕW�̉�]�ɂ���āA�����ʒu���s�{�b�g����ɂǂ��ړ����邩���v�Z
        Vector3 pivotToModel = initialLocalPosition - currentSwayPivot;
        Vector3 targetPosition = currentSwayPivot + (swayRotation * pivotToModel);

        // 3. �v�Z�����ڕW�l�Ɋ��炩�ɒǏ]������
        swayTarget.localPosition = Vector3.Lerp(swayTarget.localPosition, targetPosition, Time.deltaTime * swaySmoothness);
        swayTarget.localRotation = Quaternion.Slerp(swayTarget.localRotation, targetRotation, Time.deltaTime * swaySmoothness);
    }

    private void ReturnToInitial()
    {
        swayTarget.localPosition = Vector3.Lerp(swayTarget.localPosition, initialLocalPosition, Time.deltaTime * swaySmoothness);
        swayTarget.localRotation = Quaternion.Slerp(swayTarget.localRotation, initialLocalRotation, Time.deltaTime * swaySmoothness);
    }

    // XR Grab Interactable�̃C�x���g����Ăяo�����߂̌��J���\�b�h
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
