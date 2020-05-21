from simulation.predator_agents import PredatorAgents
from simulation.prey_agents import PreyAgents
import numpy as np

class Simulation:
    def __init__(self):
        self.prey_agents: PreyAgents = None
        self.predator_agents: PredatorAgents = None

        self.prey_distances: np.ndarray = None
        self.predator_distances: np.ndarray = None
        self.prey_predator_distances: np.ndarray = None

    def run_single_episode(self, models, number_of_steps):
        self.reset()

        for current_step in range(number_of_steps):
            agent_observations = self.get_observations()

            agent_actions = {}
            for brain_name, brain_model in models.items():
                actions = brain_model(agent_observations[brain_name])
                agent_actions[brain_name] = actions

            self.set_actions(agent_actions)

        fitness = self.episode_completed()
        return fitness

    def reset(self):
        self.prey_agents.reset()
        self.predator_agents.reset()
        prey_pop = self.prey_agents.population_size
        predator_pop = self.predator_agents.population_size
        self.prey_distances = np.zeros(shape=(prey_pop, prey_pop))
        self.predator_distances = np.zeros(shape=(predator_pop, predator_pop))
        self.prey_predator_distances = np.zeros(shape=(prey_pop, predator_pop))
