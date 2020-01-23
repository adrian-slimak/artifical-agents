using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using UnityEngine;

public class Memory
{
    MemoryMappedFile m_AgentsObservationsMemory;
    MemoryMappedFile m_AgentsActionsMemory;
    MemoryMappedFile m_AgentsFitnessMemory;

    public Memory(int m_Port)
    {
        m_AgentsObservationsMemory = MemoryMappedFile.CreateOrOpen("agents_observations_" + m_Port, 200000);
        m_AgentsActionsMemory = MemoryMappedFile.CreateOrOpen("agents_actions_" + m_Port, 200000);
        m_AgentsFitnessMemory = MemoryMappedFile.CreateOrOpen("agents_fitness_" + m_Port, 200000);
    }

    internal void WriteAgentsObservations(List<Brain> brains)
    {
        int byteObservationsArraySize = 0;
        foreach (Brain brain in brains)
        {
            byteObservationsArraySize += brain.mmf_size_observations;
        }

        using (MLAgents.TimerStack.Instance.Scoped("MemoryWrite"))
        {
            using (MemoryMappedViewAccessor viewAccessor = m_AgentsObservationsMemory.CreateViewAccessor())
            {

                var byteArray = new byte[byteObservationsArraySize];
                foreach (Brain brain in brains)
                    Buffer.BlockCopy(brain.stackedObservations, 0, byteArray, brain.mmf_offset_observations, brain.mmf_size_observations);

                viewAccessor.WriteArray(0, byteArray, 0, byteArray.Length);
            }
        }

        //float sum = 0f;
        //foreach (float f in brains[0].stackedObservations)
        //    sum += f;
        //Debug.Log($"{m_StepCount}   Current observations: {sum}");
    }

    internal void WriteAgentsFitness(List<Brain> brains)
    {
        int byteFitnessArraySize = 0;
        foreach (Brain brain in brains)
        {
            byteFitnessArraySize += brain.mmf_size_fitness;
        }

        using (MLAgents.TimerStack.Instance.Scoped("MemoryWrite"))
        {
            using (MemoryMappedViewAccessor viewAccessor = m_AgentsFitnessMemory.CreateViewAccessor())
            {

                var byteArray = new byte[byteFitnessArraySize];
                foreach (Brain brain in brains)
                    Buffer.BlockCopy(brain.agentsFitness, 0, byteArray, brain.mmf_offset_fitness, brain.mmf_size_fitness);

                viewAccessor.WriteArray(0, byteArray, 0, byteArray.Length);
            }
        }
    }

    internal void ReadAgentsActions(List<Brain> brains)
    {
        int byteActionsArraySize = 0;
        foreach (Brain brain in brains)
        {
            byteActionsArraySize += brain.mmf_size_actions;
        }

        using (MLAgents.TimerStack.Instance.Scoped("MemoryRead"))
        {
            using (MemoryMappedViewAccessor viewAccessor = m_AgentsActionsMemory.CreateViewAccessor())
            {
                byte[] byteArray = new byte[byteActionsArraySize];
                viewAccessor.ReadArray(0, byteArray, 0, byteArray.Length);
                foreach (Brain brain in brains)
                    Buffer.BlockCopy(byteArray, brain.mmf_offset_actions, brain.stackedActions, 0, brain.mmf_size_actions);
            }
        }

        //float sum = 0f;
        //foreach (float f in brains[0].stackedActions)
        //    sum += f;
        //Debug.Log($"{m_StepCount}   Acting for: {sum}");
    }
}
