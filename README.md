This project is a fluid simulator built in Unity through the Smooth Particle Hydrodynamics (SPH) method. The particles are represented by white cube objects enclosed within an invisible boundary. There are several variables that can be adjusted within the FluidRenderer class to modify the behaviour of the fluid, including:
* Particle number
* Gravity
* Boxwidth (size of the invisible boundary)
* Pressure Force
* Viscosity


This is a clip of the simulator running with 200 particles with a low-viscosity setting. Despite the large number of particles, the simulation runs relatively smoothly thanks to the power of compute shaders and parallel processing.

https://github.com/user-attachments/assets/af9962d4-39e7-4692-bb2f-68290f132567

