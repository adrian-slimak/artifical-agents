plot_size = (14, 5)  # Width / Height

plot_subplots = (2, 2)  # Rows / Columns

plot_structure = \
    {
        'prey1':
            {
                'title': 'Prey food collected',
                'position': (0, 0),
                'labels': ['episode', 'fitness'],
                'lines': ['avg', 'best', 'worst'],
                'lims': [200, 20],
            },

        # 'Prey stats':
        #     {
        #         'position': (0, 0),
        #         'labels': ['episode', 'fitness'],
        #         'lines': ['avg', 'best', 'worst'],
        #         'lims': [200, 30],
        #     },

        'predator1':
            {
                'title': 'Predator food collected',
                'position': (0, 1),
                'labels': ['episode', 'fitness'],
                'lines': ['avg', 'best', 'worst'],
                'lims': [200, 10]
             }
    }