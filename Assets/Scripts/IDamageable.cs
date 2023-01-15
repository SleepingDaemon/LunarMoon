public interface IDamageable
{
    public float Health { get; set; }

    public void OnDamage(float amount);
}
