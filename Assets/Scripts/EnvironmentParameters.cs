using UnityEngine;

[CreateAssetMenu(fileName = "New Environment Parameters", menuName = "Environment Parameters", order = 51)]
public class EnvironmentParameters : ScriptableObject
{
    [SerializeField]
    int preys_count;
    [SerializeField]
    int preys_observations_vector_size;
    [SerializeField]
    int preys_vision_observations_vector_size;
    [SerializeField]
    int preys_actions_vector_size;


    [SerializeField]
    int predators_count;
    [SerializeField]
    int predators_observations_vector_size;
    [SerializeField]
    int predators_vision_observations_vector_size;
    [SerializeField]
    int predators_actions_vector_size;
}
