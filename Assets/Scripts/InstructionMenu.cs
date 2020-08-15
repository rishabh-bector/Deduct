using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionMenu : MonoBehaviour, PixelParent {
    // Config
    public Color menuBorderColor;
    public Color menuBackgroundColor;
    public Color forwardColor;
    public Color rightColor;
    public Color leftColor;

    // References
    public Screen screen;
    public Selector selector;
    public LevelManager levelManager;
    public Board board;

    // State
    public int numInstructions = 7;
    public int lastXPos = -1;
    public Function parent;
    public Dictionary<Instructions, Color> instColors;

    private List<(int, int)> oldSet;
    private List<(int, int)> oldParent;

    private void Start() {
        oldSet = new List<(int, int)>();
        oldParent = new List<(int, int)>();
        instColors = new Dictionary<Instructions, Color>();

        // Instruction -> color mapping
        instColors.Add(Instructions.Forward, forwardColor);
        instColors.Add(Instructions.TurnLeft, leftColor);
        instColors.Add(Instructions.TurnRight, rightColor);
        instColors.Add(Instructions.Function2, board.f2Color);
        instColors.Add(Instructions.Function3, board.f3Color);
        instColors.Add(Instructions.Function4, board.f4Color);
        instColors.Add(Instructions.Function5, board.f5Color);
    }

    public void DestroySetMenu() {
        for (int i = 0; i < oldParent.Count; i++) {
            (int, int) old = oldParent[i];
            screen.RevertPixelParent(old.Item1, old.Item2);
        }

        for (int i = 0; i < oldSet.Count; i++) {
            (int, int) old = oldSet[i];
            screen.RevertPixel(old.Item1, old.Item2);
        }

        levelManager.ReparentButtons();
    }

    public void BuildSetMenu(int x, Function parent) {
        this.parent = parent;

        int yTop = 64 - 17;
        int xLeft = x - 2;
        int width = 12;
        int height = 12;

        // Remove old set
        selector.Destroy();
        if (oldSet != null && oldSet.Count > 0 && xLeft == oldSet[0].Item1 && yTop == oldSet[0].Item2) {
            DestroySetMenu();
            oldParent.Clear();
            oldSet.Clear();
            return;
        }
        DestroySetMenu();
        oldParent.Clear();
        oldSet.Clear();

        // Build set frame
        for (int x0 = 0; x0 < width; x0++) {
            for (int y0 = 0; y0 < height; y0++) {
                if (x0 == 0 || y0 == 0 || x0 == 11 || y0 == 11) {
                    screen.SetPixelColor(xLeft + x0, yTop - y0, menuBorderColor);
                } else screen.SetPixelColor(xLeft + x0, yTop - y0, menuBackgroundColor);
                oldSet.Add((xLeft + x0, yTop - y0));
            }
        }

        // Build set instructions
        levelManager.LockButtons();
        int row = yTop - 2;
        for (int i = 0; i < numInstructions; i++) {
            int xPos = (i % 3) * 3;
            Color col = instColors[(Instructions)i];

            screen.SetPixelColor(x + xPos, row, col);
            screen.SetPixelColor(x + xPos + 1, row, col);
            screen.SetPixelColor(x + xPos, row - 1, col);
            screen.SetPixelColor(x + xPos + 1, row - 1, col);

            screen.SetPixelParent(x + xPos, row, i, 0, this);
            screen.SetPixelParent(x + xPos + 1, row, i, 0, this);
            screen.SetPixelParent(x + xPos, row - 1, i, 0, this);
            screen.SetPixelParent(x + xPos + 1, row - 1, i, 0, this);

            Debug.Log("Set parent for " + i + " at " + x + xPos + " " + row);

            oldSet.Add((x + xPos, row));
            oldSet.Add((x + xPos + 1, row));
            oldSet.Add((x + xPos, row - 1));
            oldSet.Add((x + xPos + 1, row - 1));

            oldParent.Add((x + xPos, row));
            oldParent.Add((x + xPos + 1, row));
            oldParent.Add((x + xPos, row - 1));
            oldParent.Add((x + xPos + 1, row - 1));

            if (xPos == 6) row -= 3;
        }

        lastXPos = x;
    }

    public void OnPixelMouseEnter(int x, int y) {
        if (x == selector.selectX && y == selector.selectY) return;
        int xPos = lastXPos + (x % 3) * 3;
        int yPos = 64 - 17 - 2;
        for (int i = 0; i < x; i++) if (i % 3 == 2) yPos -= 3;
        selector.BuildSelector(xPos, yPos);
    }

    public void OnPixelMouseExit(int x, int y) { }

    public void OnPixelMouseDown(int x, int y) {
        parent.OnInstructionSelect((Instructions)x);
    }
}