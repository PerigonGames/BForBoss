using Tayx.Graphy;

namespace BForBoss
{
    public class GraphyAdapter : ISpecification
    {
        private GraphyManager Manager => GraphyManager.Instance;

        void ISpecification.SetShowFPSActive(bool isOn)
        {
            if (Manager != null)
            {
                Manager.FpsModuleState = GetState(isOn);
            }
        }

        void ISpecification.SetShowRAMUsageActive(bool isOn)
        {
            if (Manager != null)
            {
                Manager.RamModuleState = GetState(isOn);
            }
        }

        void ISpecification.SetShowPCSpecificationsActive(bool isOn)
        {
            if (Manager != null)
            {
                Manager.AdvancedModuleState = GetState(isOn);
            }
        }

        // TODO - Implement when needed
        void ISpecification.SetAudioUsage(bool isOn)
        {
            if (Manager != null)
            {
                Manager.AudioModuleState = GraphyManager.ModuleState.OFF;
            }
        }

        private GraphyManager.ModuleState GetState(bool isOn)
        {
            if (isOn)
            {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                return GraphyManager.ModuleState.FULL;
#endif
                return GraphyManager.ModuleState.BASIC;
            }

            return GraphyManager.ModuleState.OFF;
        }
    }
}