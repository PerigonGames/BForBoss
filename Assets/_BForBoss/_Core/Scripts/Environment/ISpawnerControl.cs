namespace BForBoss
{
    public interface ISpawnerControl
    {
        void PauseAllSpawning();
        void ResumeAllSpawning();
        void ResumeSpawning(int spawnerIndex);
    }
}
