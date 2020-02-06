import mmap
from numpy import fromstring

class Memory:
    def __init__(self, port):
        self.agents_observations_memory = mmap.mmap(fileno=-1, length=200000, tagname='agents_observations_'+str(port))
        self.agents_actions_memory = mmap.mmap(fileno=-1, length=200000, tagname='agents_actions_' + str(port))
        self.agents_fitness_memory = mmap.mmap(fileno=-1, length=200000, tagname='agents_fitness_' + str(port))

    def ReadAgentsObservations(self, brains):
        state = {}
        for brain in brains.values():
            self.agents_observations_memory.seek(brain.mmf_offset_observations)
            observations = fromstring(self.agents_observations_memory.read(brain.mmf_size_observations), dtype='f')
            state[brain.brain_name] = observations.reshape((brain.agents_count, 1, brain.observation_vector_size))

        return state

    def ReadAgentsFitness(self, brains):
        fitness = {}
        for brain in brains.values():
            self.agents_fitness_memory.seek(brain.mmf_offset_fitness)
            fitness[brain.brain_name] = fromstring(self.agents_fitness_memory.read(brain.mmf_size_fitness), dtype='f')

        return fitness

    def WriteAgentsActions(self, brains, agents_actions):
        for brain in brains.values():
            if agents_actions:
                byteArray = agents_actions[brain.brain_name].tobytes()
                self.agents_actions_memory.seek(brain.mmf_offset_actions)
                self.agents_actions_memory.write(byteArray)