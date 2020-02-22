import mmap
from numpy import fromstring

MMF_SIZE = 15000  # number of bytes

class Memory:
    def __init__(self, brain_name, worker_id, brain):
        self._mmf = mmap.mmap(fileno=-1, length=MMF_SIZE, tagname=f'{brain_name}_brain_{worker_id}')
        self._brain = brain

        self.mmf_offset_observations = -1
        self.mmf_offset_actions = -1
        self.mmf_offset_fitness = -1

        self.mmf_size_observations = -1
        self.mmf_size_actions = -1
        self.mmf_size_fitness = -1

    def Init(self, agents_count, observations_vector_size, actions_vector_size):
        self.mmf_size_observations = agents_count * observations_vector_size * 4
        self.mmf_size_actions = agents_count * actions_vector_size * 4
        self.mmf_size_fitness = agents_count * 4

        self.mmf_offset_observations = 0
        self.mmf_offset_actions = self.mmf_size_observations
        self.mmf_offset_fitness = self.mmf_size_observations + self.mmf_size_actions

        size_needed = self.mmf_size_observations+self.mmf_offset_actions+self.mmf_size_fitness
        if MMF_SIZE < size_needed:
            raise Exception(f"Memory Mapped File not large enough! {MMF_SIZE}<{size_needed}")

    def ReadAgentsObservations(self):
        self._mmf.seek(self.mmf_offset_observations)

        observations = fromstring(self._mmf.read(self.mmf_size_observations), dtype='f')

        observations = observations.reshape((self._brain.agents_count, 1, self._brain.observation_vector_size))
        return observations

    def ReadAgentsFitness(self):
        self._mmf.seek(self.mmf_offset_fitness)

        fitness = fromstring(self._mmf.read(self.mmf_size_fitness), dtype='f')

        return fitness

    def WriteAgentsActions(self, agents_actions):
        self._mmf.seek(self.mmf_offset_actions)
        byteArray = agents_actions.tobytes()
        self._mmf.write(byteArray)