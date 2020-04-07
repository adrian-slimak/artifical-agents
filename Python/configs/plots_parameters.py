from configs.learning_parameters import number_of_steps, number_of_generations

plot_size = (18, 8)  # Width / Height

plot_subplots = (2, 3)  # Rows / Columns

plot_structure = \
    {
        'prey1':
            {
                'title': 'Prey food collected',
                'position': (0, 0),
                'labels': ['episode', 'fitness'],
                'lines': ['avg', 'best', 'worst'],
                'lims': [number_of_generations, 20],
            },

        'prey2':
            {
                'title': 'Prey live stats',
                'position': (0, 1),
                'labels': ['step', 'value'],
                'lines': ['alive', 'density', 'dispersion'],
                'lims': [number_of_steps, 30],
            },

        'prey3':
            {
                'title': 'Prey mean stats',
                'position': (0, 2),
                'labels': ['step', 'value'],
                'lines': ['survivorship', 'density', 'dispersion'],
                'lims': [number_of_generations, 60],
            },

        'predator1':
            {
                'title': 'Predator food collected',
                'position': (1, 0),
                'labels': ['episode', 'fitness'],
                'lines': ['avg', 'best', 'worst'],
                'lims': [number_of_generations, 10]
             },

        'predator2':
            {
                'title': 'Predator live stats',
                'position': (1, 1),
                'labels': ['step', 'value'],
                'lines': ['alive', 'density', 'dispersion', 'attacks'],
                'lims': [number_of_steps, 30],
            },

        'predator3':
            {
                'title': 'Predator mean stats',
                'position': (1, 2),
                'labels': ['step', 'value'],
                'lines': ['survivorship', 'swarm density', 'swarm dispersion', 'attack attempts'],
                'lims': [number_of_generations, 30],
            },
    }