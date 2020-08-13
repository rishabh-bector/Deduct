using UnityEngine;
using System.Collections.Generic;

public class Function : MonoBehaviour, PixelParent {
    // References
    public Screen screen;
    public InstructionMenu instructionMenu;
    public Selector selector;

    // State
    private int functionX;
    private List<Instructions> instructions;

    private void Start() {
        instructions = new List<Instructions>();
        for (int i = 0; i < 15; i++) {
            instructions.Add(Instructions.None);
        }
    }

    public void Build(int x, Color color, Color viewBackgroundColor) {
        functionX = x;
        int instInd = 0;
        for (int x0 = 0; x0 < 8; x0++) {
            // Colored bars
            screen.SetPixelColor(x + x0, 63, color);
            screen.SetPixelColor(x + x0, 63 - 15, color);
        }

        for (int y = 62; y > 62 - 14; y -= 3) {
            for (int x0 = 0; x0 < 8; x0++) {
                if (x0 != 2 && x0 != 5 && x0 != 8) {
                    screen.SetPixelColor(x + x0, y, viewBackgroundColor);
                    screen.SetPixelColor(x + x0, y - 1, viewBackgroundColor);

                    if (screen.PixelAt(x + x0, y).GetComponent<Pixel>().x != -1) continue;

                    screen.PixelAt(x + x0, y).GetComponent<Pixel>().ptag = instInd;
                    screen.PixelAt(x + x0, y - 1).GetComponent<Pixel>().ptag = instInd;
                    screen.PixelAt(x + x0 + 1, y).GetComponent<Pixel>().ptag = instInd;
                    screen.PixelAt(x + x0 + 1, y - 1).GetComponent<Pixel>().ptag = instInd;
                    instInd++;

                    screen.SetPixelParent(x + x0, y, x + x0, y, this);
                    screen.SetPixelParent(x + x0, y - 1, x + x0, y, this);
                    screen.SetPixelParent(x + x0 + 1, y, x + x0, y, this);
                    screen.SetPixelParent(x + x0 + 1, y - 1, x + x0, y, this);
                    screen.SetPixelParent(x + x0 + 2, y, -2, -2, this);
                    screen.SetPixelParent(x + x0 + 2, y - 1, -2, -2, this);
                }
            }
        }
    }

    public void OnPixelMouseEnter(int x, int y) {
        if (x == selector.selectX && y == selector.selectY) return;
        selector.BuildSelector(x, y);
    }
    public void OnPixelMouseExit(int x, int y) {
        if (x != selector.selectX || y != selector.selectY) return;
    }

    public void OnPixelMouseDown(int x, int y) {
        if (selector.locked) {
            selector.Unlock();
            instructionMenu.BuildSetMenu(functionX, this);
            selector.BuildSelector(x, y);
            return;
        }
        instructionMenu.BuildSetMenu(functionX, this);
        selector.BuildSelector(x, y);
        selector.Lock();
    }

    public void OnInstructionSelect(Instructions i) {
        Color col = instructionMenu.instColors[i];
        int xPos = selector.selectX;
        int yPos = selector.selectY;

        screen.SetPixelColor(xPos, yPos, col);
        screen.SetPixelColor(xPos, yPos - 1, col);
        screen.SetPixelColor(xPos + 1, yPos, col);
        screen.SetPixelColor(xPos + 1, yPos - 1, col);

        int instInd = screen.PixelAt(xPos, yPos).GetComponent<Pixel>().ptag;
        Debug.Log("Inserted " + i + " into slot " + instInd);
        instructions[instInd] = i;

        selector.Unlock();
        instructionMenu.BuildSetMenu(functionX, this);
    }

    public List<Instructions> GetInstructionList() {
        return instructions;
    }
}
