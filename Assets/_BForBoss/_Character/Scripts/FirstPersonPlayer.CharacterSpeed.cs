namespace Perigon.Character
{
    public interface ICharacterSpeed
    {
        float Speed { get; }
    }
    
    public partial class FirstPersonPlayer : ICharacterSpeed
    {
        float ICharacterSpeed.Speed => GetVelocity().magnitude;
    }
}