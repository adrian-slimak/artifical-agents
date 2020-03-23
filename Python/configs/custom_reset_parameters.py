custom_reset_parameters_1 = \
    {
        "prey":
            {
                "count": 60,

                "communication_enabled": 0,  # 0 - False, 1 - True

                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 3 * 15,  # 3 x "vision_cell_number", one per plant, prey, predator
                                "cell_number": 15,
                                "angle": 220,
                                "range": 20
                            },

                        "hearing":
                            {
                                "vector_size": 12,
                                "cell_number": 12,
                                "angle": 360,
                                "range": 30
                            },
                    },

                "actions_vector_size": 2,

                "max_move_speed": 5,
                "max_turn_speed": 100,

                "energy":
                {
                    "": 100,
                    "drain_per_step": 0.15,
                    "drain_per_speed": 0.1,
                },
            },

        "predator":
            {
                "count": 0,

                "communication_enabled": 0,  # 0 - False, 1 - True

                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 12,
                                "cell_number": 12,
                                "angle": 180,
                                "range": 25
                            },

                        "hearing":
                            {
                                "vector_size": 12,
                                "cell_number": 12,
                                "angle": 360,
                                "range": 30
                            },
                    },

                "actions_vector_size": 2,

                "max_move_speed": 10,
                "max_turn_speed": 70,

                "energy":
                {
                    "": 100,
                    "drain_per_step": 0.15,
                    "drain_per_speed": 0.1,
                },
            },

        "environment":
            {
                "food":
                    {
                        "spawn_method": 1,   # 0 - Grid  1 - Random
                        "spawn_per_step": 0.1,
                        "spawn_amount_reset": 500,  # for Random Spawn
                        "spawn_grid_step": 4,  # for Grid Spawn
                    }
            }
    }