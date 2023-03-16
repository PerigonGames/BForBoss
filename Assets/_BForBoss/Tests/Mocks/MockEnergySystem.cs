using BForBoss;

namespace Tests
{
    public class MockEnergySystem : IEnergySystem
    {
        public void Accrue(EnergyAccruementType accruementType, float multiplier = 1)
        {
            
        }

        public void Expend(EnergyExpenseType expenseType, float multiplier = 1)
        {
            
        }

        public bool CanExpend(EnergyExpenseType expenseType, float multiplier = 1)
        {
            return true;
        }
    }
}
