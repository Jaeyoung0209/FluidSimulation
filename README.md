This project is a fluid simulator built in Unity through the Smooth Particle Hydrodynamics (SPH) method. The particles are represented by white cube objects enclosed within an invisible boundary. There are several variables that can be adjusted within the FluidRenderer class to modify the behaviour of the fluid, including:
* Particle number
* Gravity
* Boxwidth (size of the invisible boundary)
* Pressure Force
* Viscosity


This is a clip of the simulator running with 200 particles with a low-viscosity setting. Despite the large number of particles, the simulation runs relatively smoothly thanks to the power of compute shaders and parallel processing.

https://github.com/user-attachments/assets/af9962d4-39e7-4692-bb2f-68290f132567

The particles settle down to form a flat surface when first generated, and are constantly adjusting their positions to maintain constant density throughout. I have also made it so that the particles will be attracted to the mouse position when the user holds down the left mouse button.

https://github.com/user-attachments/assets/63de5811-6f40-4dc6-ad88-0b07e22f4384

This is a clip of a simulation with the same number of particles, but triple the viscosity. As shown in the video, the fluid behaves much stiffer compared to the first setting, and does not seem flexible and freely flowing. This feature is especially more apparent when compared with a clip of a simulation with the first viscosity setting, but with similar user interaction.

https://github.com/user-attachments/assets/9ac87623-6291-4c10-af37-18d4b60c6e07

The difference between how the body of fluid behaves when the droplet that has been picked up is pushed down highlights the difference made by the change in viscosity. The velocities of particles in high-viscosity setting are less affected by the movement of surrounding particles, which makes sense considering the definition of viscosity is the measure of fluid's resistance to flow.
