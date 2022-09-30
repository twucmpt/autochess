using UnityEngine;

class Projectile : MonoBehaviour {
    public Vector3 direction;
    public float speed = 1;
    public int damage;
    public Unit user;
    public bool friendlyFire = true;

    Rigidbody2D rb;
    void Start() {rb = GetComponent<Rigidbody2D>();}

    void Update() {rb.velocity = direction * speed;}

    void OnTriggerEnter2D (Collider2D collider) {
        Entity unit = collider.transform.root.GetComponent<Entity>();
        if (unit == user) return;
        if (!friendlyFire && unit.CompareTag(user.tag)) return;

        unit.TakeDamage(damage);
        Destroy(gameObject);
    }
}