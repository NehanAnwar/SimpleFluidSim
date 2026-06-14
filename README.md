# C# Monogame Simple Physics Simulation

This is a basic U-Tube Fluid Test using large particles to simulate fluid behaviors.

### Controls:
- Space - Pause Physics 
- R - Reset
- ESC - Exit

### Parameters:
Particle.cs/Radius - Size of Particles

Particle.cs/Mass - Physics Mass of Particles

FluidWorld.cs/RepulsionStrength - How strongly particles repel each other, does not apply to obstacles

FluidWorld.cs/Viscosity - Friction between particles


FluidWorld.cs/InteractionRadius - 0-1.0 Smaller sphere inside particle that calculates Particle-on-Particle collisions and also Collision grid size **recommended not to change!**

### Requirements:
 - .Net SDK
### How To Run:

    dotnet restore
    dotnet run