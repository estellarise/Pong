# Pong
## Download instructions
Download key for private distribution: https://estellarise.itch.io/pong/download/HzD5bwjXpTiQy0NrAfou6MbT3wfcEtjKwQ_kzRTL
Unzip, run Pong.exe

Goal: Play until 7!

Controls:

Left Player: W,S

Right Player: Up, Down

Start new round / reset: Space

## Tools
- Framework: Monogame with C#
- Art: https://www.kenney.nl/assets 
- IDE: Visual Studio 2022
- Based on this tutorial: https://docs.monogame.net/articles/getting_started/2_creating_a_new_project_vs.html

## Notes to the Beginner Game Dev Explorer
Program.cs is the entry point to the program/game. It instantiates the Autobounce class and runs it. That's it.
You can break games into roughly two parts, much like a website: objects is to text as art is to CSS.
Or to put it another way, basic shapes swim around the screen and you splash art onto them.
The code is split similarly. Under the hood, there is an update/draw loop. Update is to objects as draw is to art.
Use update for repositioning, draw to pop the art in. (N.B.: Don't mix update code with draw code!) 
For fellow intermediate coders out there, you can treat the update function as "main." In that regard, 
I can probably use more functions than I actually have. On the other hand, the current structure is reminiscent of the 
tutorial code provided by Monogame, which will hopefully provide ease of understanding to the viewer.
In that line of thought, it'd be good to create a class of position, speed, and texture to keep objects organized. 
I totally kept it like this for pedagogical purposes, and not because I'm lazy. Yup.
### Misc Notes
- "Run" / F5 only work properly when a solution is chosen (double-click)
- Build skipped when local timestamp of target is older than latest timestamp (looking at you, time travelers!)

## To do
- Licensing
- Make repo public?

## Further research
- Figure out how to send C# code (is itch the only way?)
- What's calling Update Draw?
- Affix art to an underlying hitbox object
