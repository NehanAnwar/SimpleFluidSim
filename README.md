# TO VIT
### Unpack win64-exe and run FluidSimulation.exe to run the packaged simulation

#### I'm not sure if the repo can be run directly with dotnet run since some of the dotnet generated files have my local paths inside them, there is probably a proper way to distribute monogame projects onto github and I will fix this by the final submission.


 I currently have the simulation working with 1500 particles with slightly optimized particle calculations, particles only interact with each other if they are inside the same "grid" within the 3d space.

 It doesn't show perfect liquid behavior currently and fails to even out both sides of the container, I think most of this is due to badly tweaked numbers but may require a fundamental change in how I calculate collisions.
 
  After running the sim for a while with viscosity >= 0, all entropy dies out and the particles almost lock into a grid, I'm not sure if this is expected behavior from particle based liquids but I'll try to stop entropy loss in my next fix.

I do not have any automatic tests written into the program yet.
