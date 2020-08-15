using UnityEngine;
using System.Collections.Generic;

public enum Instructions {
    Forward,
    TurnRight,
    TurnLeft,
    Function2,
    Function3,
    Function4,
    Function5,
    None,
}

public enum Dir {
    Up,
    Down,
    Left,
    Right
}

public class Player : MonoBehaviour {
    // Config
    public Color gamePlayerColor1;
    public Color gamePlayerColor2;
    public Color gamePathColor;
    public int playFrameSpeed = 60;

    // References
    public Screen screen;
    public Board board;
    public LevelManager levelManager;

    // State
    public (int, int) currentPos = (-1, -1);
    private (int, int) oldPos = (-1, -1);
    public Dir currentDir;
    private List<(int, int)> currentPixels = new List<(int, int)>();
    private Level currentLevel;
    private (int, int) goal;

    private bool firstRun = true;
    private bool playing = false;
    private int playFrame = 0;
    private List<List<Instructions>> instructions;
    private List<Instructions> currentStack;
    private int currentInstruction;

    public void BuildToScreen(int x, int y) {
        if (x == -1 && y == -1) return;
        if (!firstRun) Revert();

        currentPos = (x, y);

        Color color00 = (currentDir == Dir.Right || currentDir == Dir.Up) ? gamePlayerColor2 : gamePlayerColor1;
        Color color10 = (currentDir == Dir.Left || currentDir == Dir.Up) ? gamePlayerColor2 : gamePlayerColor1;
        Color color01 = (currentDir == Dir.Right || currentDir == Dir.Down) ? gamePlayerColor2 : gamePlayerColor1;
        Color color11 = (currentDir == Dir.Left || currentDir == Dir.Down) ? gamePlayerColor2 : gamePlayerColor1;

        screen.SetPixelColor(x * 2, y * 2, color00);
        screen.SetPixelColor(x * 2, y * 2 + 1, color01);
        screen.SetPixelColor(x * 2 + 1, y * 2, color10);
        screen.SetPixelColor(x * 2 + 1, y * 2 + 1, color11);

        currentPixels.Add((x * 2, y * 2));
        currentPixels.Add((x * 2 + 1, y * 2));
        currentPixels.Add((x * 2, y * 2 + 1));
        currentPixels.Add((x * 2 + 1, y * 2 + 1));

        if (oldPos != currentPos) {
            screen.SetPixelColor(oldPos.Item1 * 2, oldPos.Item2 * 2, gamePathColor);
            screen.SetPixelColor(oldPos.Item1 * 2, oldPos.Item2 * 2 + 1, gamePathColor);
            screen.SetPixelColor(oldPos.Item1 * 2 + 1, oldPos.Item2 * 2, gamePathColor);
            screen.SetPixelColor(oldPos.Item1 * 2 + 1, oldPos.Item2 * 2 + 1, gamePathColor);
        }

        oldPos = (x, y);
    }

    public void Revert() {
        currentPixels.ForEach(delegate ((int, int) pixel) {
            screen.RevertPixel(pixel.Item1, pixel.Item2);
        });
        firstRun = false;
    }

    public void Rotate() {
        switch (currentDir) {
            case Dir.Up:
                currentDir = Dir.Right;
                break;
            case Dir.Right:
                currentDir = Dir.Down;
                break;
            case Dir.Down:
                currentDir = Dir.Left;
                break;
            case Dir.Left:
                currentDir = Dir.Up;
                break;
            default:
                currentDir = Dir.Right;
                break;
        }
    }

    // Command movement

    private void FixedUpdate() {
        if (playing && playFrame > playFrameSpeed) {
            ProcessNextInstruction();
            playFrame = 0;
        }

        playFrame++;
    }

    public void Play(Level level) {
        // Initialize level details
        for (int x = 0; x < 32; x++) {
            for (int y = 0; y < 22; y++) {
                if (level.GetTile(x, y) == 2) goal = (x, y);
            }
        }
        currentLevel = level;
        currentDir = (Dir)level.startDir;
        instructions = board.BuildInstructionList();

        // Initialize stack
        currentStack = new List<Instructions>();
        currentStack.AddRange(instructions[0]);
        currentInstruction = 0;

        // Begin playing
        Debug.Log("Beginning instruction sequence:");
        playing = true;
    }

    public void ProcessNextInstruction() {
        // Check end conditions
        if (currentPos == goal) {
            Stop();
            levelManager.LevelComplete();
            board.TransitionToSuccessState();
        }

        if (currentInstruction >= currentStack.Count) {
            Stop();
            return;
        }

        // Process next instruction
        Debug.Log("Processing " + currentInstruction + " " + currentStack[currentInstruction]);
        switch (currentStack[currentInstruction]) {
            case Instructions.Forward:
                InstructForward();
                break;
            case Instructions.TurnLeft:
                InstructTurnLeft();
                break;
            case Instructions.TurnRight:
                InstructTurnRight();
                break;
            case Instructions.Function2:
                InstructPushStack(instructions[1]);
                break;
            case Instructions.Function3:
                InstructPushStack(instructions[2]);
                break;
            case Instructions.Function4:
                InstructPushStack(instructions[3]);
                break;
            case Instructions.Function5:
                InstructPushStack(instructions[4]);
                break;
        }
        BuildToScreen(currentPos.Item1, currentPos.Item2);
        currentInstruction++;
    }

    public void InstructPushStack(List<Instructions> func) {
        currentStack.InsertRange(currentInstruction+1, func);
        PrintStack();
    }

    public void Stop() {
        Debug.Log("Stopping execution...");
        playing = false;
    }

    public void InstructForward() {
        switch (currentDir) {
            case Dir.Up:
                if (IsTileOpen(currentPos.Item1, currentPos.Item2 + 1)) currentPos.Item2++;
                break;
            case Dir.Right:
                if (IsTileOpen(currentPos.Item1 + 1, currentPos.Item2)) currentPos.Item1++;
                break;
            case Dir.Down:
                if (IsTileOpen(currentPos.Item1, currentPos.Item2 - 1)) currentPos.Item2--;
                break;
            case Dir.Left:
                if (IsTileOpen(currentPos.Item1 - 1, currentPos.Item2)) currentPos.Item1--;
                break;
        }
    }

    public bool IsTileOpen(int x, int y) {
        return currentLevel.GetTile(x, y) == 0 || currentLevel.GetTile(x, y) == 2;
    }

    public void InstructTurnRight() {
        Rotate();
        Rotate();
        Rotate();
    }
    public void InstructTurnLeft() {
        Rotate();
    }

    public void PrintStack() {
        string O = "";
        currentStack.ForEach(delegate (Instructions i) {
            O += i + " ";
        });
        Debug.Log(O);
    }
}
