using Tayx.Graphy;

namespace BForBoss
{
    public class GraphyAdapter : ISpecification
    {
        private GraphyManager Manager => GraphyManager.Instance;
        public void SetShowFPSActive(bool isOn)
        {
            if (Manager != null)
            {
                Manager.FpsModuleState = GetState(isOn);
            }
        }

        public void SetShowRAMUsageActive(bool isOn)
        {
            if (Manager != null)
            {
                Manager.RamModuleState = GetState(isOn);
            }
        }

        public void SetShowPCSpecificationsActive(bool isOn)
        {
            if (Manager != null)
            {
                Manager.AdvancedModuleState = GetState(isOn);
            }
        }

        // TODO - Implement when needed
        public void SetAudioUsage(bool isOn)
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
            else
            {
                return GraphyManager.ModuleState.OFF;
            }
        }
    }
}