## Challenges/Fixes
- It was difficult to create high-quality terrain and meshes on our own  ->  so we largely imported free assets from the Unity Store and other sources
- Our initial pitch was overly ambitious  ->  so we focused on implementing the core features (e.g. hypothermia, combat)
- Creating the snowy ambience required the addition of particle effects, dark lighting, and UI features such as the icy vignette
- Enemy health bar was confusing to implement and make functional, especially the GUI of it; hard to make visuals line up with
actual damage / remaining health  ->  lots of playing around with transform of objects, fixed visualization of health bar, but still minor bug with damage indicator display
- Animations were difficult to create and implement, especially the enemy wolf animations  ->  simplified axe and knife swing and got them to display and occur correctly, imported wolf animations but still issue w/ walking 
- Damage output had frequent bugs, damage dealt would be different than listed damage on item prefab  ->  fixed error in scripts and root prefabs, damage registers and displays correctly
