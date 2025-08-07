namespace _1.Scripts.Interfaces.NPC
{
    public interface IPatrolable
    {
        public float MinWaitingDuration { get; }
        public float MaxWaitingDuration { get; }
        public float MinWanderingDistance { get; }
        public float MaxWanderingDistance { get; }
    }
}