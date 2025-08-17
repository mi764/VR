using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// NearFarInteractorを使用するために追加
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class RifleController : MonoBehaviour
{
    [Header("射撃設定")]
    [Tooltip("発射される弾のプレハブ")]
    public GameObject bulletPrefab;

    [Tooltip("弾が発射される位置")]
    public Transform muzzlePoint;

    [Tooltip("弾の初速")]
    public float bulletSpeed = 15f;

    [Tooltip("連射速度（1秒間に発射できる弾数）")]
    public float fireRate = 1f;

    [Header("エフェクト")]
    [Tooltip("発砲音")]
    public AudioClip fireSound;

    [Tooltip("弾が装填されているときに表示するオブジェクト")]
    public GameObject loadedIndicator;

    private AudioSource audioSource;
    private float nextFireTime = 0f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (loadedIndicator != null)
        {
            loadedIndicator.SetActive(Time.time >= nextFireTime);
        }
    }

    public void StartShooting(ActivateEventArgs args)
    {
        if (args.interactableObject is not IXRSelectInteractable selectInteractable)
        {
            return;
        }

        IXRSelectInteractor firstInteractor = selectInteractable.interactorsSelecting.FirstOrDefault();

        // トリガーを引いた手が、最初に掴んだ手でなければ処理を中断
        if (firstInteractor != args.interactorObject)
        {
            return;
        }

        // --- これが最終的な直接持ち判定です ---
        bool isDirectGrab = false;

        // パターン1：トリガーを引いたのが、そもそもDirectInteractorの場合
        if (args.interactorObject is XRDirectInteractor)
        {
            isDirectGrab = true;
        }
        // パターン2：トリガーを引いたのがNearFarInteractorで、現在"近距離(Near)"モードが有効な場合
        else if (args.interactorObject is NearFarInteractor nearFarInteractor &&
                 nearFarInteractor.selectionRegion.Value == NearFarInteractor.Region.Near)
        {
            isDirectGrab = true;
        }

        // 直接持ちでない場合は、発砲をキャンセル
        if (!isDirectGrab)
        {
            return;
        }
        // ------------------------------------

        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            FireBullet();
        }
    }

    private void FireBullet()
    {
        if (bulletPrefab == null || muzzlePoint == null)
        {
            Debug.LogError("Bullet Prefab または Muzzle Point が設定されていません。");
            return;
        }

        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }

        GameObject bullet = Instantiate(bulletPrefab, muzzlePoint.position, muzzlePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(muzzlePoint.forward * bulletSpeed, ForceMode.Impulse);
        }
    }
}
