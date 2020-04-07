import mmap
from tensorflow import io, float32

OBSERVATIONS_SIZE = 100 * 1024
ACTIONS_SIZE = 50 * 1024
FITNES_SIZE = 50 * 1024
STATS_SIZE = 10 * 1024
MMF_SIZE = OBSERVATIONS_SIZE + ACTIONS_SIZE + FITNES_SIZE + STATS_SIZE

class Memory:
    def __init__(self, brain_name, worker_id=0, n_envs=1):
        self._mmf = mmap.mmap(fileno=-1, length=MMF_SIZE, tagname=f'{brain_name}_brain')
        self.worker_id = worker_id
        self.n_envs = n_envs

        self.mmf_offset_observations = -1
        self.mmf_size_observations = -1

        self.mmf_offset_actions = -1
        self.mmf_size_actions = -1

        self.mmf_offset_fitness = -1
        self.mmf_size_fitness = -1

        self.mmf_offset_stats = -1
        self.mmf_size_stats = -1

    def init(self, agents_count, observations_vector_size, fitness_vector_size, actions_vector_size, stats_vector_size):
        if self.n_envs > 1 and self.worker_id != 0:
            raise Exception('For multi_unity_environment, worker_id in brain must be equal 0!')

        self.mmf_size_observations = agents_count * observations_vector_size * self.n_envs * 4
        self.mmf_size_actions = agents_count * actions_vector_size * self.n_envs * 4
        self.mmf_size_fitness = agents_count * fitness_vector_size * self.n_envs * 4
        self.mmf_size_stats = stats_vector_size * self.n_envs * 4

        self.mmf_offset_observations = self.mmf_size_observations * self.worker_id
        self.mmf_offset_actions = OBSERVATIONS_SIZE + self.mmf_size_actions * self.worker_id
        self.mmf_offset_fitness = OBSERVATIONS_SIZE + ACTIONS_SIZE + self.mmf_size_fitness * self.worker_id
        self.mmf_offset_stats = OBSERVATIONS_SIZE + ACTIONS_SIZE + FITNES_SIZE + self.mmf_size_stats * self.worker_id

        # size_needed = self.mmf_size_observations+self.mmf_offset_actions+self.mmf_size_fitness
        # if MMF_SIZE < size_needed:
        #     raise Exception(f"Memory Mapped File not large enough! {MMF_SIZE}<{size_needed}")

    def read_observations(self):
        self._mmf.seek(self.mmf_offset_observations)

        return io.decode_raw(self._mmf.read(self.mmf_size_observations), out_type=float32)

    def read_fitness(self):
        self._mmf.seek(self.mmf_offset_fitness)

        return io.decode_raw(self._mmf.read(self.mmf_size_fitness), out_type=float32)

    def read_stats(self):
        self._mmf.seek(self.mmf_offset_stats)

        return io.decode_raw(self._mmf.read(self.mmf_size_stats), out_type=float32)

    def write_actions(self, agents_actions):
        self._mmf.seek(self.mmf_offset_actions)
        byteArray = agents_actions.tobytes()
        self._mmf.write(byteArray)
