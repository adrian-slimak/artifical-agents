from mlagents.environment import UnityEnvironment
import configs.learning_parameters as _lp
from .brain import Brain

class MultiEnvManager:
    def __init__(self, n_env: int = 1):
        self.environments = []
        self.n_env = n_env

        for worker_id in range(n_env):
            self.environments.append(self.create_environment(worker_id))

        self.external_brains = {}
        self.init_brains()

    def init_brains(self):
        for brain_name in _lp.brains:
            self.external_brains[brain_name] = Brain(brain_name, 0)
            self.external_brains[brain_name].init_stacked(_lp.merged_environment_parameters, self.n_env)

    @staticmethod
    def create_environment(worker_id: int) -> UnityEnvironment:
        return UnityEnvironment(file_name=_lp.unity_environment_path, worker_id=worker_id, initialization_input=_lp.initialization_input)

    def run_episode(self, models, number_of_steps):

        for env in self.environments:
            env.reset()

        for step in range(number_of_steps):
            for env in self.environments:
                env._step_receive_observations()

            agent_observations = {brain_name: self.external_brains[brain_name].get_stacked_observations() for brain_name in _lp.brains}

            agent_actions = {brain_name: models[brain_name](agent_observations[brain_name]) for brain_name in _lp.brains}

            for brain_name in _lp.brains:
                self.external_brains[brain_name].set_agents_actions(agent_actions[brain_name])

            for env in self.environments:
                env._step_send_actions()

        for env in self.environments:
            env._episode_completed()

        fitness = {brain_name: self.external_brains[brain_name].get_stacked_fitness() for brain_name in _lp.brains}

        return fitness

    def close(self) -> None:
        for env in self.environments:
            env.close()
