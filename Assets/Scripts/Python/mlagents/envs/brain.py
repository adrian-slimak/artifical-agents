import logging
import numpy as np
import io

from mlagents.envs.communicator_objects.agent_info_pb2 import AgentInfoProto
from mlagents.envs.communicator_objects.brain_parameters_pb2 import BrainParametersProto
from mlagents.envs.timers import hierarchical_timer, timed
from typing import Dict, List, NamedTuple, Optional
from PIL import Image

logger = logging.getLogger("mlagents.envs")


class CameraResolution(NamedTuple):
    height: int
    width: int
    num_channels: int

    @property
    def gray_scale(self) -> bool:
        return self.num_channels == 1

    def __str__(self):
        return f"CameraResolution({self.height}, {self.width}, {self.num_channels})"


class BrainParameters:
    def __init__(
        self,
        brain_name: str,
        vector_observation_space_size: int,
        camera_resolutions: List[CameraResolution],
        vector_action_space_size: List[int],
        vector_action_descriptions: List[str],
        vector_action_space_type: int,
    ):
        """
        Contains all brain-specific parameters.
        """
        self.brain_name = brain_name
        self.vector_observation_space_size = vector_observation_space_size
        self.number_visual_observations = len(camera_resolutions)
        self.camera_resolutions = camera_resolutions
        self.vector_action_space_size = vector_action_space_size
        self.vector_action_descriptions = vector_action_descriptions
        self.vector_action_space_type = ["discrete", "continuous"][
            vector_action_space_type
        ]

    def __str__(self):
        return """Unity brain name: {}
        Number of Visual Observations (per agent): {}
        Camera Resolutions: {}
        Vector Observation space size (per agent): {}
        Vector Action space type: {}
        Vector Action space size (per agent): {}
        Vector Action descriptions: {}""".format(
            self.brain_name,
            str(self.number_visual_observations),
            str([str(cr) for cr in self.camera_resolutions]),
            str(self.vector_observation_space_size),
            self.vector_action_space_type,
            str(self.vector_action_space_size),
            ", ".join(self.vector_action_descriptions),
        )

    @staticmethod
    def from_proto(brain_param_proto: BrainParametersProto, agent_info: AgentInfoProto) -> "BrainParameters":
        """
        Converts brain parameter proto to BrainParameter object.
        :param brain_param_proto: protobuf object.
        :return: BrainParameter object.
        """
        resolutions = [
            # CameraResolution(obs.shape[0], obs.shape[1], obs.shape[2])
            # for obs in agent_info.observations
            # if len(obs.shape) >= 3
        ]

        total_vector_obs = len(agent_info.stackedObservations)

        brain_params = BrainParameters(
            brain_name=brain_param_proto.brain_name,
            vector_observation_space_size=total_vector_obs,
            camera_resolutions=resolutions,
            vector_action_space_size=list(brain_param_proto.vector_action_size),
            vector_action_descriptions=list(
                brain_param_proto.vector_action_descriptions
            ),
            vector_action_space_type=brain_param_proto.vector_action_space_type,
        )
        return brain_params


class BrainInfo:
    def __init__(
        self,
        visual_observation,
        vector_observation,
        reward=None,
        agents=None,
        local_done=None,
        max_reached=None,
        action_mask=None,
    ):
        """
        Describes experience at current step of all agents linked to a brain.
        """
        self.visual_observations = visual_observation
        self.vector_observations = vector_observation
        self.rewards = reward
        self.local_done = local_done
        self.max_reached = max_reached
        self.agents = agents
        self.action_masks = action_mask

    @staticmethod
    def merge_memories(m1, m2, agents1, agents2):
        if len(m1) == 0 and len(m2) != 0:
            m1 = np.zeros((len(agents1), m2.shape[1]))
        elif len(m2) == 0 and len(m1) != 0:
            m2 = np.zeros((len(agents2), m1.shape[1]))
        elif m2.shape[1] > m1.shape[1]:
            new_m1 = np.zeros((m1.shape[0], m2.shape[1]))
            new_m1[0 : m1.shape[0], 0 : m1.shape[1]] = m1
            return np.append(new_m1, m2, axis=0)
        elif m1.shape[1] > m2.shape[1]:
            new_m2 = np.zeros((m2.shape[0], m1.shape[1]))
            new_m2[0 : m2.shape[0], 0 : m2.shape[1]] = m2
            return np.append(m1, new_m2, axis=0)
        return np.append(m1, m2, axis=0)

    @staticmethod
    @timed
    def process_pixels(image_bytes: bytes, gray_scale: bool) -> np.ndarray:
        """
        Converts byte array observation image into numpy array, re-sizes it,
        and optionally converts it to grey scale
        :param gray_scale: Whether to convert the image to grayscale.
        :param image_bytes: input byte array corresponding to image
        :return: processed numpy array of observation from environment
        """
        with hierarchical_timer("image_decompress"):
            image_bytearray = bytearray(image_bytes)
            image = Image.open(io.BytesIO(image_bytearray))
            # Normally Image loads lazily, this forces it to do loading in the timer scope.
            image.load()
        s = np.array(image) / 255.0
        if gray_scale:
            s = np.mean(s, axis=2)
            s = np.reshape(s, [s.shape[0], s.shape[1], 1])
        return s

    @staticmethod
    def from_agent_proto(worker_id: int, agent_info_list: List[AgentInfoProto], brain_params: BrainParameters) -> "BrainInfo":
        """
        Converts list of agent infos to BrainInfo.
        """

        total_num_actions = sum(brain_params.vector_action_space_size)
        mask_actions = np.ones((len(agent_info_list), total_num_actions))

        vector_obs = BrainInfo._process_vector_observations(
            brain_params, agent_info_list
        )

        agents = ["agent0"]
        brain_info = BrainInfo(
            visual_observation=[],
            vector_observation=vector_obs,
            reward=0,
            agents=agents,
            local_done=False,
            max_reached=False,
            action_mask=mask_actions,
        )
        return brain_info

    @staticmethod
    def _process_vector_observations(brain_params: BrainParameters, agent_info_list: List[AgentInfoProto]) -> np.ndarray:
        # visual_observations = np.reshape(agent_info_list[0].stackedObservations, (100, 400))
        vector_observations = np.array(agent_info_list[0].stackedObservations)
        return vector_observations


def safe_concat_lists(l1: Optional[List], l2: Optional[List]) -> Optional[List]:
    if l1 is None:
        if l2 is None:
            return None
        else:
            return l2.copy()
    else:
        if l2 is None:
            return l1.copy()
        else:
            copy = l1.copy()
            copy.extend(l2)
            return copy


def safe_concat_np_ndarray(
    a1: Optional[np.ndarray], a2: Optional[np.ndarray]
) -> Optional[np.ndarray]:
    if a1 is not None and a1.size != 0:
        if a2 is not None and a2.size != 0:
            return np.append(a1, a2, axis=0)
        else:
            return a1.copy()
    elif a2 is not None and a2.size != 0:
        return a2.copy()
    return None


# Renaming of dictionary of brain name to BrainInfo for clarity
AllBrainInfo = Dict[str, BrainInfo]
