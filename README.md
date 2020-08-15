# D I G I C O D E

This is a 64x64 programming game made for [LowRezJam 2020](https://itch.io/jam/lowrezjam-2020). The instructions of the game are included in this file, since I didn't have time to figure out how to fit them into a 64px square view. 

## Welcome

Once you pass the main menu and select the first level, you'll see the following advanced mission command system:

<full view>

This is DIGICODE. The mostly-empty larger view under the rows of slots shows the map:

<map view>

You can see our drone in the bottom left (the square with 2 colors), as well as the darker path to the light green target, which is to the upper-right of the drone. Your task is to guide this drone to the target. However, you are a highly intelligent being, with highly abstract tools. The top view display shows your controls:

<control view>
  
And each 3x5 color-coded group of slots is a _function_. Each function contains 15 slots, and each slot can hold 1 instruction. Your task is to guide your drone to the target by placing instructions into these slots. Clicking on a slot will open the instruction menu, which displays all available instructions:

<instruction menu>
  
The first three (blue, grey, and green) are for basic movement. Blue will move the drone forward, grey will turn left, and green will turn right. Let's try it out! Click the very first slot, which is in the topmost left of the first function (black). This is the _main_ function, and all the instructions inside of it are run from left-to-right, row-by-row when you hit the start button. 

When you see the instruction menu pop up, click the blue square which will insert the "move forward" instruction into the first slot of function 1. Once you hit start, the drone will move forward in the upward direction, since it is currently facing up. Try it now! The start button is the green one between the function view and the map view. Next to it is the stop button, which will stop your program and reset the drone. Now just continue to add instructions until you complete the level by reaching the target!

## About Functions

Scores in DIGICODE are calculated by subtracting the number of instructions placed by the player from the max number of slots available. This means that in order to get the best score, you must use as _few_ instructions as possible. Functions are an easy way to reuse instructions. Opening up the instruction menu, we see that 4 of the available instructions match the colors of 4 of the functions (excluding the main one). When building your programs, placing down an instruction which corresponds to a function will simply run _all_ of the instructions in that function. To try it, write and run this program:

<function program>
  
Notice how our drone moved forward 4 spaces! This is because we told our drone to simply "run the purple function twice", and we've also told our drone that the purple function consists of 2 "move forward" instructions. Combining functions and instructions like this can allow us to program complex drone paths with just a handlful of instructions.

## Goodbye

You now have the tools needed to become a true DIGICODE master. Have fun solving all the levels! 
