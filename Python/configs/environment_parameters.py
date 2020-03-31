environment_parameters = \
    {
        "prey":
            {
                "count": 60,

                "communication_enabled": 0,  # 0 - False, 1 - True

                "max_move_speed": 5,
                "max_turn_speed": 100,

                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 15 * 3,  # 3 x "vision_cell_number", one per plant, prey, predator
                                "cell_number": 15,
                                "angle": 220,
                                "range": 20
                            },

                        # "hearing":
                        #     {
                        #         "vector_size": 12,
                        #         "cell_number": 12,
                        #         "angle": 360,
                        #         "range": 30
                        #     },
                    },

                "actions_vector_size": 2,
                "fitness_vector_size": 1,
                "stats_vector_size": 2,

                "energy":
                {
                    "": 100,
                    "gain_per_eat": 50,
                    "drain_per_step": 0.15,
                    "drain_per_speed": 0.1,
                },
            },

        "predator":
            {
                "count": 12,

                "communication_enabled": 0,  # 0 - False, 1 - True

                "max_move_speed": 15,
                "max_turn_speed": 150,

                "confusion_effect":
                    {
                        "value": 1,
                        "distance": 3,
                    },

                "observations":
                    {
                        "vision":
                            {
                                "vector_size": 12 * 3,
                                "cell_number": 12,
                                "angle": 180,
                                "range": 25
                            },

                        # "hearing":
                        #     {
                        #         "vector_size": 12,
                        #         "cell_number": 12,
                        #         "angle": 360,
                        #         "range": 30
                        #     },
                    },

                "actions_vector_size": 2,
                "fitness_vector_size": 1,
                "stats_vector_size": 2,

                "energy":
                {
                    "": 100,
                    "gain_per_eat": 50,
                    "drain_per_step": 0.1,
                    "drain_per_speed": 0.1,
                },
            },

        "environment":
            {
                "food":
                    {
                        "spawn_method": 1,   # 0 - Grid  1 - Random
                        "spawn_per_step": 0.3,
                        "spawn_amount_reset": 400,  # for Random Spawn
                        "spawn_grid_step": 4,  # for Grid Spawn
                    }
            }
    }