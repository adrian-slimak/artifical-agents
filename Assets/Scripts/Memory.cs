using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;

public class Memory
{
    MemoryMappedFile m_MMF;
    unsafe byte* m_Pointer;

    unsafe byte* mmf_observations_pointer;
    internal int mmf_size_observations;

    unsafe byte* mmf_actions_pointer;
    internal int mmf_size_actions;

    unsafe byte* mmf_fitness_pointer;
    internal int mmf_size_fitness;

    public unsafe Memory(string brainName, int workerID)
    {
        m_MMF = MemoryMappedFile.CreateOrOpen($"{brainName}_brain_{workerID}", 15000);

        using (MemoryMappedViewAccessor viewAccessor = m_MMF.CreateViewAccessor())
            viewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref m_Pointer);
    }

    public unsafe void Init()
    {
        mmf_observations_pointer = m_Pointer;
        mmf_actions_pointer = m_Pointer + mmf_size_observations;
        mmf_fitness_pointer = m_Pointer + mmf_size_observations + mmf_size_actions;
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
