# SPH Fluid Simulation
This project is a fluid simulator built in Unity through the Smooth Particle Hydrodynamics (SPH) method. The particles are represented by white cube objects enclosed within an invisible boundary. There are several variables that can be adjusted within the FluidRenderer class to modify the behaviour of the fluid, including:
* Particle number
* Gravity
* Boxwidth (size of the invisible boundary)
* Pressure Force
* Viscosity



https://github.com/user-attachments/assets/4a5e8df2-f13d-484c-b192-736db30c386a


This is a clip of the simulator running with 200 particles with a low-viscosity setting. Despite the large number of particles, the simulation runs relatively smoothly thanks to the power of compute shaders and parallel processing.

https://github.com/user-attachments/assets/af9962d4-39e7-4692-bb2f-68290f132567

The particles settle down to form a flat surface when first generated, and are constantly adjusting their positions to maintain constant density throughout. I have also made it so that the particles will be attracted to the mouse position when the user holds down the left mouse button.

https://github.com/user-attachments/assets/63de5811-6f40-4dc6-ad88-0b07e22f4384

This is a clip of a simulation with the same number of particles, but triple the viscosity. As shown in the video above, the fluid behaves much stiffer compared to the first setting, and does not seem flexible and freely flowing. This feature is especially more apparent when compared with a clip of a simulation with the first viscosity setting, but with similar user interaction.

https://github.com/user-attachments/assets/9ac87623-6291-4c10-af37-18d4b60c6e07

The difference in behaviour between the two fluids when a droplet is picked up and pushed down highlights the impact of changing viscosity. When the droplet is first collected, there is an obvious contrast in particle velocity between the two simulation settings. In the high-viscosity setting, particles move much more slowly, to the point where those farther from the mouse pointer appear almost stationary. In contrast, in the low-viscosity setting, particles flow much faster toward the mouse pointer. Considering that viscosity is a measure of a fluid's resistance to flow, this slower movement in the high-viscosity setting makes sense as the particles seem to be influenced less by the motion of those around them.
