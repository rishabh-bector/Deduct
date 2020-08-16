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
    private Color viewBackgroundColor;
    private Color mainColor;

    private void Start() {
        if (instructions != null) return;
        instructions = new List<Instructions>();
        for (int i = 0; i < 15; i++) {
            instructions.Add(Instructions.None);
        }
    }

    private void FixedUpdate() {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Backspace) && instructionMenu.lastXPos == functionX) {
            Debug.Log("Clearing function!");
            instructions = new List<Instructions>();
            for (int i = 0; i < 15; i++) {
                instructions.Add(Instructions.None);
            }

            int cLevel = instructionMenu.levelManager.currentLevel;
            instructionMenu.levelManager.Get(cLevel).Wipe(instructionMenu.PosToFunc(functionX));
            instructionMenu.levelManager.Save();

            BuildSlots();
        }
    }

    public void Build(int functionX, Color mainColor, Color viewBackgroundColor) {
        this.functionX = functionX;
        this.viewBackgroundColor = viewBackgroundColor;
        this.mainColor = mainColor;
        for (int x0 = 0; x0 < 8; x0++) {
            // Colored bars
            screen.SetPixelColor(functionX + x0, 63, mainColor);
            screen.SetPixelColor(functionX + x0, 63 - 15, mainColor);
        }

        BuildSlots();
    }

    public void BuildSlots() {
        int instInd = 0;
        int x = functionX;
        for (int y = 62; y > 62 - 14; y -= 3) {
            for (int x0 = 0; x0 < 8; x0+= 3) {
                screen.SetPixelColor(x + x0, y, viewBackgroundColor);
                screen.SetPixelColor(x + x0, y - 1, viewBackgroundColor);
                screen.SetPixelColor(x + x0 + 1, y, viewBackgroundColor);
                screen.SetPixelColor(x + x0 + 1, y - 1, viewBackgroundColor);

                if (instInd < instructions.Count) {
                    Debug.Log("Rendering inst " + instructions[instInd]);
                    Color col = instructionMenu.instColors[instructions[instInd]];
                    screen.SetPixelColor(x + x0, y, col);
                    screen.SetPixelColor(x + x0, y - 1, col);
                    screen.SetPixelColor(x + x0 + 1, y, col);
                    screen.SetPixelColor(x + x0 + 1, y - 1, col);
                }

                screen.PixelAt(x + x0, y).GetComponent<Pixel>().ptag = instInd;
                screen.PixelAt(x + x0, y - 1).GetComponent<Pixel>().ptag = instInd;
                screen.PixelAt(x + x0 + 1, y).GetComponent<Pixel>().ptag = instInd;
                screen.PixelAt(x + x0 + 1, y - 1).GetComponent<Pixel>().ptag = instInd;
                instInd++;

                if (screen.PixelAt(x + x0, y).GetComponent<Pixel>().x != -1) continue;

                screen.SetPixelParent(x + x0, y, x + x0, y, this);
                screen.SetPixelParent(x + x0, y - 1, x + x0, y, this);
                screen.SetPixelParent(x + x0 + 1, y, x + x0, y, this);
                screen.SetPixelParent(x + x0 + 1, y - 1, x + x0, y, this);
                screen.SetPixelParent(x + x0 + 2, y, -2, -2, this);
                screen.SetPixelParent(x + x0 + 2, y - 1, -2, -2, this);
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
        instructionMenu.board.audioManager.PlayClick();

        if (selector.locked) {
            selector.Unlock();
            instructionMenu.BuildSetMenu(functionX, this);
            selector.BuildSelector(x, y);
            selector.Lock();
            return;
        }

        instructionMenu.BuildSetMenu(functionX, this);
        selector.BuildSelector(x, y);
        selector.Lock();
    }

    public void OnInstructionSelect(Instructions i) {
        instructionMenu.board.audioManager.PlayClick();

        Color col = instructionMenu.instColors[i];
        int xPos = selector.selectX;
        int yPos = selector.selectY;

        int instInd = screen.PixelAt(xPos, yPos).GetComponent<Pixel>().ptag;
        if (instInd >= 15 || instInd == -1) return;
        Debug.Log("Inserted " + i + " into slot " + instInd);
        instructions[instInd] = i;

        screen.SetPixelColor(xPos, yPos, col);
        screen.SetPixelColor(xPos, yPos - 1, col);
        screen.SetPixelColor(xPos + 1, yPos, col);
        screen.SetPixelColor(xPos + 1, yPos - 1, col);

        // Auto move to next slot for convenience
        selector.Unlock();
        (int, int) oldPos = (selector.selectX, selector.selectY);
        if (oldPos.Item2 - 3 > 46) {
            if (oldPos.Item1 - functionX < 6) selector.BuildSelector(oldPos.Item1 + 3, oldPos.Item2);
            else {
                selector.BuildSelector(functionX, oldPos.Item2 - 3);
            }
        }
        selector.Lock();
    }

    public List<Instructions> GetInstructionList() {
        List<Instructions> returnList = new List<Instructions>();
        instructions.ForEach(delegate (Instructions i) {
            if (i != Instructions.None) returnList.Add(i);
        });
        return returnList;
    }

    public void LoadInstructionList(List<Instructions> instructions) {
        Debug.Log("loading " + instructions[0]);
        Debug.Log("loading 2" + instructions[1]);
        this.instructions = instructions;
        Build(functionX, mainColor, viewBackgroundColor);
    }
}
