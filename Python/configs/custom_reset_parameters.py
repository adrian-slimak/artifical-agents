custom_reset_parameters_1 = \
    {
        "prey":
            {
                "count": 60,
                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 15,
                                "angle": 220,
                                "range": 10
                            },
                    },
                "actions_vector_size": 2,
                "max_move_speed": 5,
                "max_turn_speed": 100,
                "energy_drain_time": 1,
                "energy_drain_speed": 1,
            },

        "predator":
            {
                "count": 0,
                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 12,
                                "angle": 180,
                                "range": 15
                            },
                    },
                "actions_vector_size": 2,
                "max_move_speed": 10,
                "max_turn_speed": 150,
                "energy_drain_time": 1,
                "energy_drain_speed": 1,
            },

        "environment":
            {
                "food":
                    {
                        "spawn_method": 1,   # 0 - Grid  1 - Random
                        "spawn_per_step": 0.1,
                        "spawn_amount_reset": 500,  # for Random Spawn
                        "spawn_grid_step": 5,  # for Grid Spawn
                    }
            }
    }