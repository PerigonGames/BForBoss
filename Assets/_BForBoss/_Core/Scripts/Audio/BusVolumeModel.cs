using FMOD;
using FMOD.Studio;
using FMODUnity;
using Debug = UnityEngine.Debug;

namespace BForBoss.Audio
{
    public class BusVolumeModel : IVolumeModel
    {
        private const string BUS_PREFIX = "bus:/";
        private Bus _bus;

        private readonly string _name;

        protected virtual string DebugString => _name + " is not a valid audio bus";
        
        public BusVolumeModel(string name)
        {
            _name = name;
            _bus = RuntimeManager.GetBus(BUS_PREFIX + name);
        }

        public void SetVolume(float volume)
        {
            if (!_bus.isValid())
            {
                Debug.LogWarning(DebugString);
                return;
            }
            _bus.setVolume(volume);
        }

        public float GetVolume()
        {
            if (_bus.isValid() && _bus.getVolume(out float volume) == RESULT.OK)
            {
                return volume;
            }
            Debug.LogWarning(DebugString);
            return 0f;
        }
    }

    public class MasterVolumeModel : BusVolumeModel
    {
        public MasterVolumeModel() : base("")
        {
            
        }

        protected override string DebugString => "The master bus is not valid";
    }
}
