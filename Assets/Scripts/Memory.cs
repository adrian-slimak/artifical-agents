using System.IO.MemoryMappedFiles;
using UnityEngine;

public class Memory
{
    static int OBSERVATIONS_SIZE = 100 * 1024;
    static int ACTIONS_SIZE = 50 * 1024;
    static int FITNESS_SIZE = 50 * 1024;
    static int STATS_SIZE = 10 * 1024;
    static int MMF_SIZE = OBSERVATIONS_SIZE + ACTIONS_SIZE + FITNESS_SIZE + STATS_SIZE;


    MemoryMappedFile m_MMF;
    unsafe byte* m_Pointer;

    int m_workerID;

    unsafe byte* mmf_observations_pointer;
    internal int mmf_observations_size;

    unsafe byte* mmf_actions_pointer;
    internal int mmf_actions_size;

    unsafe byte* mmf_fitness_pointer;
    internal int mmf_fitness_size;

    unsafe byte* mmf_stats_pointer;
    internal int mmf_stats_size;

    public unsafe Memory(string brainName, int workerID)
    {
        m_MMF = MemoryMappedFile.OpenExisting($"{brainName}_brain");
        m_workerID = workerID;

        using (MemoryMappedViewAccessor viewAccessor = m_MMF.CreateViewAccessor())
            viewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref m_Pointer);
    }

    public unsafe void Init(int agentsCount, int observationsVectorSize, int actionsVectorSize, int fitnessVectorSize, int statsVectorSize)
    {
        mmf_observations_size = agentsCount * observationsVectorSize * 4;
        mmf_actions_size = agentsCount * actionsVectorSize * 4;
        mmf_fitness_size = agentsCount * fitnessVectorSize * 4;
        mmf_stats_size = statsVectorSize * 4;

        mmf_observations_pointer = m_Pointer + mmf_observations_size * m_workerID;
        mmf_actions_pointer = OBSERVATIONS_SIZE + m_Pointer + mmf_actions_size * m_workerID;
        mmf_fitness_pointer = OBSERVATIONS_SIZE + ACTIONS_SIZE + m_Pointer + mmf_fitness_size * m_workerID;
        mmf_stats_pointer = OBSERVATIONS_SIZE + ACTIONS_SIZE + FITNESS_SIZE + m_Pointer + mmf_stats_size * m_workerID;
    }

    public unsafe MMArray GetObservationsMemoryArray(int offset, int length)
    {
        return new MMArray((float*) (mmf_observations_pointer + offset*4), length);
    }

    public unsafe MMArray GetActionsMemoryArray(int offset, int length)
    {
        return new MMArray((float*) (mmf_actions_pointer + offset*4), length);
    }

    public unsafe MMArray GetFitnessMemoryArray(int offset, int length)
    {
        return new MMArray((float*)(mmf_fitness_pointer + offset*4), length);
    }

    public unsafe MMArray GetStatsMemoryArray()
    {
        return new MMArray((float*)(mmf_stats_pointer), mmf_stats_size/4);
    }

    public unsafe void Reset()
    {
        mmf_observations_pointer = null;
        mmf_actions_pointer = null;
        mmf_fitness_pointer = null;
        mmf_actions_size = -1;
        mmf_observations_size = -1;
        mmf_fitness_size = -1;
        mmf_stats_size = -1;
    }
}
