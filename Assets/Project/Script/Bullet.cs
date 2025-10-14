using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Tooltip("弾が存在する最大時間（秒）")]
    public float lifeTime = 5f;

    [Tooltip("着弾時に生成するエフェクトのプレハブ")]
    public GameObject hitEffectPrefab;

    [Tooltip("着弾エフェクトの大きさの倍率")]
    public float hitEffectScale = 0.1f;

    void Start()
    {
        Debug.Log("弾が生成されました: " + gameObject.name);
        // 指定した時間が経過したら、自動的に自身を破棄する
        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 着弾エフェクトが設定されていれば、衝突地点に生成する
        if (hitEffectPrefab != null)
        {
            // エフェクトを生成し、そのインスタンスへの参照を'effect'変数に格納
            GameObject effect = Instantiate(hitEffectPrefab, collision.contacts[0].point, Quaternion.identity);

            // 生成したエフェクトのスケール（大きさ）を調整
            effect.transform.localScale = Vector3.one * hitEffectScale;
        }

        Debug.Log("弾が衝突しました: " + gameObject.name + " - 衝突対象: " + collision.gameObject.name);

        Destroy(gameObject, lifeTime);
    }
}