environment_parameters = \
    {
        "prey":
            {
                "count": 60,

                "communication":
                    {
                        "enabled": 0,  # 0 - False, 1 - True
                        "food_sound_trigger": 8,  # Minimal number of food in 'vision' field to activate this sound
                        "food_sound_value": 2.5,
                        "predator_sound_trigger": 1,  # Minimal number of predators in 'vision' field to activate this sound
                        "predator_sound_value": 5,
                    },

                "max_move_speed": 0.5 * 50,
                "max_turn_speed": 4 * 50,
                # "max_move_speed": 1 * 50,
                # "max_turn_speed": 8 * 50,

                "rest_after_eat": 0,
                'distance_to_eat': 1,

                "actions_vector_size": 2,
                "fitness_vector_size": 1,
                "stats_vector_size": 5,

                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 13 * 3,  # 3 x "vision_cell_number", one per plant, prey, predator
                                "cell_number": 13,
                                "angle": 182,
                                "range": 100
                            },

                        # "hearing":
                        #     {
                        #         "vector_size": 12,
                        #         "cell_number": 12,
                        #         "angle": 360,
                        #         "range": 40
                        #     },
                    },

                "energy":
                {
                    "": 100,
                    "gain_per_eat": 30,
                    "drain_per_step": 0,  # 0.15
                    "drain_per_speed": 0,
                },
            },

        "predator":
            {
                "count": 6,

                "communication":  # If enabled, 'actions_vector_size' will automatically increase by one
                    {
                        "enabled": 0,  # 0 - False, 1 - True
                        "sound_value": 3.5,
                    },

                "max_move_speed": 1.5 * 50,
                "max_turn_speed": 3 * 50,
                # "max_move_speed": 3 * 50,
                # "max_turn_speed": 6 * 50,

                "rest_after_eat": 10,
                'distance_to_eat': 1,

                "actions_vector_size": 2,
                "fitness_vector_size": 1,
                "stats_vector_size": 5,

                "confusion_effect":
                    {
                        "value": 1,  # enabled if > 0
                        "distance": 30,
                    },

                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 13 * 3,
                                "cell_number": 13,
                                "angle": 182,
                                "range": 100
                            },

                        # "hearing":
                        #     {
                        #         "vector_size": 12,
                        #         "cell_number": 12,
                        #         "angle": 360,
                        #         "range": 40
                        #     },
                    },

                "energy":
                {
                    "": 125,
                    "gain_per_eat": 50,
                    "drain_per_step": 0,  # 0.1
                    "drain_per_speed": 0,
                },
            },

        "environment":
            {
                "world_size": 512,
                "predator_spawn_step": 500,
                "food":
                    {
                        "spawn_method": 1,   # 0 - Grid  1 - Random
                        "spawn_per_step": 0,  # 0.3
                        "spawn_amount_reset": 0,  # for Random Spawn # 200
                        "spawn_grid_step": 4,  # for Grid Spawn
                    }
            }
    }
