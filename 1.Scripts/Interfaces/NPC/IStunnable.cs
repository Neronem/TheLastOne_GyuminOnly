namespace _1.Scripts.Interfaces.NPC
{
    public interface IStunnable
    {
        public bool IsStunned { get; }
        public void OnStunned(float duration);
    }
}