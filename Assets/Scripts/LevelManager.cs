using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

[System.Serializable]
class LevelSerial {
    public List<Level> levels;
}

enum State {
    Play,
    Dev,
}

enum DevState {
    View,
    PaintBackground,
    PaintWall,
    PaintEnd,
    PaintPlayer,
}

public class LevelManager : MonoBehaviour, PixelParent {
    // Config
    public Color gameBackgroundColor;
    public Color gameWallColor;
    public Color gameEndColor;

    // References
    public Screen screen;
    public Player player;
    public Selector devSelector;
    public Selector playerSelector;
    public Board board;

    public Function f1;
    public Function f2;
    public Function f3;
    public Function f4;
    public Function f5;

    // State
    private LevelSerial levelContainer;
    private Color[] colorCodes;
    private State state = State.Play;
    private DevState devState = DevState.View;
    private (int, int) hoveredTile;
    private bool lockStart = false;
    public int currentLevel;
    public int finalScore;
    public int bestScore;

    public void Save() {
        string marshaled = JsonUtility.ToJson(levelContainer, true);
        System.IO.File.WriteAllText(Application.dataPath + "/levels.json", marshaled);
        Debug.Log("Saved to: " + Application.dataPath + "/levels.json");
    }

    public Level Get(int i) {
        if (i >= levelContainer.levels.Count) return null;
        return levelContainer.levels[i];
    }

    public Level CreateEmpty() {
        Level level = new Level(32, 22);

        // Build walls around edge
        for (int x = 0; x < 32; x++) {
            for (int y = 0; y < 22; y++) {
                if (x == 0 || y == 0 || x == 31 || y == 21) {
                    level.SetTile(x, y, 1);
                }
            }
        }

        levelContainer.levels.Add(level);
        return level;
    }

    public void BuildToScreen(int level, bool leavePaths) {
        currentLevel = level;
        Level L = Get(level);
        player.currentDir = (Dir)L.startDir;

        // Start/stop buttons
        ReparentButtons();
        
        for (int x = 0; x < 32; x++) {
            for (int y = 0; y < 22; y++) {
                if (leavePaths && screen.PixelAt(x * 2, y * 2).GetComponent<Pixel>().GetColor() == player.gamePathColor) continue;
                if (L.GetTile(x, y) == -1) {
                    player.BuildToScreen(x, y);
                    continue;
                }
                screen.SetBlockColor(x * 2, y * 2, colorCodes[L.GetTile(x, y)]);
                screen.SetBlockParent(x * 2, y * 2, x, y, this);
            }
        }

        // Distribute saved instructions to functions
        Level funcs = Get(level);
        loadFunctionInstSave(f1, 0, funcs);
        loadFunctionInstSave(f2, 1, funcs);
        loadFunctionInstSave(f3, 2, funcs);
        loadFunctionInstSave(f4, 3, funcs);
        loadFunctionInstSave(f5, 4, funcs);
    }

    private void loadFunctionInstSave(Function func, int funcNum, Level level) {
        var instList = new List<Instructions>();
        for (int i = 0; i < 15; i++) {
            var slotInst = (Instructions)level.GetInstruction(funcNum, i);
            instList.Add(slotInst);
        }
        func.LoadInstructionList(instList);
    }

    public void ReparentButtons() {
        screen.SetBlockParent(3, 64 - 20, -10, -10, this);
        screen.SetBlockParent(5, 64 - 20, -1, -1, this);
        screen.SetBlockParent(7, 64 - 20, -11, -11, this);
        lockStart = false;
    }

    public void LevelComplete() {
        // Save level completion, unlock next nevel
        levelContainer.levels[currentLevel].complete = true;
        if (currentLevel + 1 < levelContainer.levels.Count) {
            levelContainer.levels[currentLevel + 1].unlocked = true;
        }

        int score = 0;
        List<List<Instructions>> inst = board.BuildInstructionList();
        for (int l = 0; l < inst.Count; l++) {
            for (int i = 0; i < inst[l].Count; i++) {
                score++;
            }
        }

        levelContainer.levels[currentLevel].playerScore = score;
        finalScore = score;
        bestScore = levelContainer.levels[currentLevel].bestScore;

        playerSelector.Reset();
        player.Revert();
        Save();
        board.TransitionToSuccessState();
    }

    private void Start() {
        colorCodes = new Color[5];
        colorCodes[0] = gameBackgroundColor;
        colorCodes[1] = gameWallColor;
        colorCodes[2] = gameEndColor;

        levelContainer = new LevelSerial {
            levels = new List<Level>()
        };
        try {
            string marshaled = System.IO.File.ReadAllText(Application.dataPath + "/levels.json");
            if (marshaled.Length == 0) throw new Exception("Level file corrupt, rebuilding...");
            levelContainer = JsonUtility.FromJson<LevelSerial>(marshaled);
            Debug.Log("Read from: " + Application.dataPath + "/levels.json");
            Debug.Log("Found: " + levelContainer.levels.Count + " levels");
        } catch (Exception e) {
            Debug.Log("Read json failed: " + e.ToString());
            levelContainer = new LevelSerial {
                levels = new List<Level>()
            };
            CreateEmpty();
            Save();
        }
    }

    private void Update() {
        UpdateInputs();
        if (state == State.Dev) DevUpdate();
    }

    private void UpdateInputs() {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D)) {
            if (state == State.Play) state = State.Dev;
            else {
                Save();
                state = State.Play;
            }
        }

        if (state == State.Dev) {
            if (Input.GetKeyDown(KeyCode.B)) devState = DevState.PaintBackground;
            if (Input.GetKeyDown(KeyCode.W)) devState = DevState.PaintWall;
            if (Input.GetKeyDown(KeyCode.P)) devState = DevState.PaintPlayer;
            if (Input.GetKeyDown(KeyCode.E)) devState = DevState.PaintEnd;

            if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.N)) {
                CreateEmpty();
                currentLevel++;
                BuildToScreen(currentLevel, false);
                Save();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                for (int x = 1; x < 31; x++) {
                    for (int y = 1; y < 21; y++) {
                        int t = 0;
                        if (x <= 20) t = levelContainer.levels[currentLevel].GetTile(x + 1, y);
                        levelContainer.levels[currentLevel].SetTile(x, y, t);
                    }
                }
                BuildToScreen(currentLevel, false);
            }
        }

        if (state == State.Play) {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Save();
                board.TransitionToLevelState();
            }
        }
    }

    private void DevUpdate() {
        if (devState == DevState.PaintBackground) DevPaintUpdate(0);
        if (devState == DevState.PaintWall) DevPaintUpdate(1);
        if (devState == DevState.PaintPlayer) DevPaintUpdate(-1);
        if (devState == DevState.PaintEnd) DevPaintUpdate(2);
    }
    
    private void DevPaintUpdate(int tile) {
        if (Input.GetMouseButton(0)) {
            if (tile == -1 && devState == DevState.PaintPlayer) {
                if (player.currentPos.Item1 != -1) levelContainer.levels[currentLevel].SetTile(player.currentPos.Item1, player.currentPos.Item2, 0);
                levelContainer.levels[currentLevel].SetTile(hoveredTile.Item1, hoveredTile.Item2, -1);
            } else {
                levelContainer.levels[currentLevel].SetTile(hoveredTile.Item1, hoveredTile.Item2, tile);
            }

            BuildToScreen(currentLevel, true);
            levelContainer.levels[currentLevel].startDir = (int)player.currentDir;
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            player.Rotate();
            player.BuildToScreen(player.currentPos.Item1, player.currentPos.Item2);
            levelContainer.levels[currentLevel].startDir = (int)player.currentDir;
        }
    }

    private void OnDestroy() {
        Save();
    }

    public void OnPixelMouseExit(int x, int y) {
        playerSelector.BuildSelector(-1, -1);
    }

    public void OnPixelMouseEnter(int x, int y) {
        if (state == State.Dev && x != 0 && y != 0 && x != 31 && y != 21) {
            devSelector.BuildSelector((x) * 2, (y) * 2 + 1);
            hoveredTile = (x, y);
        }   else if (state == State.Play && x == -10 && !lockStart) playerSelector.BuildSelector(3, 64 - 19);
            else if (state == State.Play && x == -11 && !lockStart) playerSelector.BuildSelector(7, 64 - 19);

        if (x == -1) {
            devSelector.BuildSelector(-1, -1);
            playerSelector.BuildSelector(-1, -1);
        }
    }
    public void OnPixelMouseDown(int x, int y) {
        board.audioManager.PlayClick();
        if (state == State.Play) {
            if (x == -10 && !lockStart) {
                BuildToScreen(currentLevel, false);
                player.Play(levelContainer.levels[currentLevel]);
            }
            if (x == -11 && !lockStart) {
                player.Stop();
                BuildToScreen(currentLevel, false);
            }
        }
    }

    public void LockButtons() { lockStart = true;  }
}

[System.Serializable]
public class Level {
    public bool unlocked;
    public bool complete;
    public int startDir;
    public int playerScore;
    public int bestScore;
    public int[] f1;
    public int[] f2;
    public int[] f3;
    public int[] f4;
    public int[] f5;

    [SerializeField]
    public int[] tiles;

    public Level(int width, int height) {
        unlocked = true;
        complete = false;

        tiles = new int[width * height];
        for (int i = 0; i < tiles.Length; i++) {
            tiles[i] = 0;
        }

        InitInstructions();
    }

    public void InitInstructions() {
        f1 = new int[15];
        f2 = new int[15];
        f3 = new int[15];
        f4 = new int[15];
        f5 = new int[15];

        Debug.Log("INIT INST " + f1);

        for (int i = 0; i < 15; i++) {
            f1[i] = 7;
            f2[i] = 7;
            f3[i] = 7;
            f4[i] = 7;
            f5[i] = 7;
        }
    }

    public void SetTile(int x, int y, int t) { tiles[getInd(x, y)] = t; }
    public int GetTile(int x, int y) { return tiles[getInd(x, y)]; }
    private int getInd(int x, int y) { return x + (y * 32); }

    public int GetInstruction(int func, int slot) {
        if (f1 == null) InitInstructions();
        try { var _ = f1[slot]; } catch (Exception) { InitInstructions(); }
        int toReturn = 0;
        switch (func) {
            case 0:
                toReturn = f1[slot];
                break;
            case 1:
                toReturn = f2[slot];
                break;
            case 2:
                toReturn = f3[slot];
                break;
            case 3:
                toReturn = f4[slot];
                break;
            case 4:
                toReturn = f5[slot];
                break;
        }
        return toReturn;
    }

    public void SetInstruction(int func, int slot, int inst) {
        Debug.Log("Setting ins " + func + " " + slot + " " + inst);
        switch (func) {
            case 0:
                f1[slot] = inst;
                break;
            case 1:
                f2[slot] = inst;
                break;
            case 2:
                f3[slot] = inst;
                break;
            case 3:
                f4[slot] = inst;
                break;
            case 4:
                f5[slot] = inst;
                break;
        }
    }

    public void Wipe(int func) {
        if (func == 0) {
            f1 = new int[15];
            for (int i = 0; i < 15; i++) { f1[i] = 7; }
        }
        if (func == 1) {
            f2 = new int[15];
            for (int i = 0; i < 15; i++) { f2[i] = 7; }
        }
        if (func == 2) {
            f3 = new int[15];
            for (int i = 0; i < 15; i++) { f3[i] = 7; }
        }
        if (func == 3) {
            f4 = new int[15];
            for (int i = 0; i < 15; i++) { f4[i] = 7; }
        }
        if (func == 4) {
            f5 = new int[15];
            for (int i = 0; i < 15; i++) { f5[i] = 7; }
        }
    }
}
