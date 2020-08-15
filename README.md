# Intro

This is a 64x64 programming game made for [LowRezJam 2020](https://itch.io/jam/lowrezjam-2020). The instructions of the game are included in this file, since I didn't have time to figure out how to fit them into a 64px square view. 

## Welcome to DIGICODE

Once you pass the main menu and select the first level, you'll see the following advanced mission command system:

<full view>

This is DIGICODE. The mostly-empty larger view under the rows of slots shows the map:

<map view>

You can see our drone in the bottom left (the square with 2 colors), as well as the darker path to the light green target, which is to the upper-right of the drone. Your task is to guide this drone to the target. However, you are a highly intelligent being, with highly abstract tools. The top view display shows your controls:

<control view>
  
And each 3x5 color-coded group of slots is a _function_. Each function contains 15 slots, and each slot can hold 1 instruction. Your task is to guide your drone to the target by placing instructions into these slots. Clicking on a slot will open the instruction menu, which displays all available instructions:

<instruction menu>
  
The first three (blue, grey, and green) are for basic movement. Blue will move the drone forward, grey will turn left, and green will turn right. Let's try it out! Click the very first slot, which is in the topmost left of the first function (black). This is the _main_ function, and all the instructions inside of it are run from left-to-right, row-by-row when you hit the start button. 

When you see the instruction menu pop up, click the blue square which will insert the "move forward" instruction into the first slot of function 1. Once you hit start, the drone will move forward in the upward direction, since it is currently facing up. Try it now! The start button is the green one between the function view and the map view. Next to it is the stop button, which will stop your program and reset the drone. 
