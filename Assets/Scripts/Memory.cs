using System.IO.MemoryMappedFiles;
using UnityEngine;

public class Memory
{
    MemoryMappedFile m_MMF;
    unsafe byte* m_Pointer;

    int m_workerID;

    unsafe byte* mmf_observations_pointer;
    internal int mmf_size_observations;

    unsafe byte* mmf_actions_pointer;
    internal int mmf_size_actions;

    unsafe byte* mmf_fitness_pointer;
    internal int mmf_size_fitness;

    public unsafe Memory(string brainName, int workerID)
    {
        m_MMF = MemoryMappedFile.OpenExisting($"{brainName}_brain");
        m_workerID = workerID;

        using (MemoryMappedViewAccessor viewAccessor = m_MMF.CreateViewAccessor())
            viewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref m_Pointer);
    }

    public unsafe void Init(int agentsCount, int observationsVectorSize, int actionsVectorSize)
    {
        mmf_size_observations = agentsCount * observationsVectorSize * 4;
        mmf_size_actions = agentsCount * actionsVectorSize * 4;
        mmf_size_fitness = agentsCount * 4;

        mmf_observations_pointer = m_Pointer + mmf_size_observations * m_workerID;
        mmf_actions_pointer = 100000 + m_Pointer + mmf_size_actions * m_workerID;
        mmf_fitness_pointer = 150000 + m_Pointer + mmf_size_fitness * m_workerID;
    }

    public unsafe UPC.MMArray GetObservationsMemoryArray(int offset, int length)
    {
        return new UPC.MMArray((float*) (mmf_observations_pointer + offset*4), length);
    }

    public unsafe UPC.MMArray GetActionsMemoryArray(int offset, int length)
    {
        return new UPC.MMArray((float*) (mmf_actions_pointer + offset*4), length);
    }

    public unsafe UPC.MMArray GetFitnessMemoryArray(int offset, int length)
    {
        return new UPC.MMArray((float*)(mmf_fitness_pointer + offset*4), length);
    }

    public unsafe void Reset()
    {
        mmf_observations_pointer = null;
        mmf_actions_pointer = null;
        mmf_fitness_pointer = null;
        mmf_size_actions = -1;
        mmf_size_observations = -1;
        mmf_size_fitness = -1;
    }
}
