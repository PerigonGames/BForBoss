namespace BForBoss
{
    public interface ICharacterSpeed
    {
        float Speed { get; }
    }
    
    public partial class FirstPersonPlayer : ICharacterSpeed
    {
        public float Speed => GetVelocity().magnitude;
    }
}