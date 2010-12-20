This sample shows how to show on one plotter two (or more) different line charts and how to manage the way they are shown together.
To show each independent chart, which is going to be shown at the plotter, it is necessary to create an InjectedPlotter inside of parent ChartPlotter.
Such InjectedPlotter is created in Window1.xaml.
There are four different modes in which inner InjectedPlotter will set it's Visible rect. You should specify this mode in InnerPlotter.ConjunctionMode
property.
