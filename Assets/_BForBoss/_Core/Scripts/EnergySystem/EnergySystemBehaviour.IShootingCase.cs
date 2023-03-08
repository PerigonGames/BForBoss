using Perigon.Weapons;

namespace BForBoss
{
    public partial class EnergySystemBehaviour: IShootingCases
    {
        public bool CanShoot => CanExpend(EnergyExpenseType.Shot);
        public void OnShoot()
        {
            Expend(EnergyExpenseType.Shot);
        }
    }
}
