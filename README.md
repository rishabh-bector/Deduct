# D I G I C O D E

![](https://github.com/rishabh-bector/Digicode/blob/master/Assets/Screenshots/thin.png)

This is a 64x64 programming game made for [LowRezJam 2020](https://itch.io/jam/lowrezjam-2020). The instructions of the game are included in this file, since I didn't have time to figure out how to fit them into a 64px square view. 

## Getting Started

Once you pass the main menu (click the arrow) and select the first level (click the exclamation mark), you'll see the following advanced mission command system:

![](https://github.com/rishabh-bector/Digicode/blob/master/Assets/Screenshots/full.png)

This is DIGICODE. The mostly-empty larger view under the rows of slots shows the map. In the middle-left of the map shown below is our drone (the square with 2 colors). There is also a dark path leading to the light green target. 

![](https://github.com/rishabh-bector/Digicode/blob/master/Assets/Screenshots/map.png)

Your task is to guide the drone to the target. However, you are a highly intelligent being, with highly abstract tools. The top display shows your controls:

![](https://github.com/rishabh-bector/Digicode/blob/master/Assets/Screenshots/control.png)
  
Each 3x5 color-coded group of slots is a _function_. Each function contains 15 slots, and each slot can hold 1 instruction. The start button is the green one. Next to it is the stop button, which will stop your program and reset the drone. Your task is to guide your drone to the target by placing instructions into these slots. Clicking on a slot will open the instruction menu, which displays all available instructions. As you can see, you have 5 functions available to you:

![](https://github.com/rishabh-bector/Digicode/blob/master/Assets/Screenshots/instructionmenu.png)
  
The first three (blue, grey, and green) are for basic movement. Blue will move the drone forward, grey will turn left, and green will turn right. Let's try it out! 

## Writing Programs

Click the very first slot, which is in the topmost left of the first function (black). This is the _main_ function, and all the instructions inside of it are run from left-to-right, row-by-row when you hit the start button. 

When you see the instruction menu pop up, click the blue square which will insert the "move forward" instruction into the first slot of function 1. Once you hit start, the drone will move forward in the upward direction, since it is currently facing up. Now just continue to add instructions until you complete the level by reaching the target!

## About Functions

Your score for a level in DIGICODE is equal to the number of instructions you placed to complete it. Lower scores are better, so you must use as _few_ instructions as possible. Functions are an easy way to reuse a set of instructions. In the instruction menu, we can see that 4 of the available instructions match the colors of 4 of the functions. When building your programs, placing down an instruction which corresponds to a function will simply run _all_ of the instructions in that function. To try it, write and run this program:

![](https://github.com/rishabh-bector/Digicode/blob/master/Assets/Screenshots/functionprog.png)
  
Notice how our drone moved forward 4 spaces. This is because we told our drone to simply "run the purple function twice", and we've also told our drone that the purple function consists of 2 "move forward" instructions. Combining functions and instructions like this can allow us to program complex drone paths with just a handful of instructions.

## Begin

Here are a few keyboard shortcuts which will help you along the way:

`Escape` - Exit to level selection screen  
`Shift` + `Backspace` - Clear all slots of whichever function is selected

You now have the tools needed to become a true DIGICODE master. Have fun solving all the levels! 
