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

    // State
    private LevelSerial levelContainer;
    private Color[] colorCodes;
    private State state = State.Play;
    private DevState devState = DevState.View;
    private int currentLevel;
    private (int, int) hoveredTile;
    private bool lockStart = false;

    public void Save() {
        string marshaled = JsonUtility.ToJson(levelContainer, true);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/levels.json", marshaled);
        Debug.Log("Saved to: " + Application.persistentDataPath + "/levels.json");
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

    public void BuildToScreen(int level) {
        currentLevel = level;
        Level L = Get(level);
        player.currentDir = (Dir)L.startDir;

        // Start/stop buttons
        ReparentButtons();
        
        for (int x = 0; x < 32; x++) {
            for (int y = 0; y < 22; y++) {
                if (L.GetTile(x, y) == -1) {
                    player.BuildToScreen(x, y);
                    continue;
                }
                screen.SetBlockColor(x * 2, y * 2, colorCodes[L.GetTile(x, y)]);
                screen.SetBlockParent(x * 2, y * 2, x, y, this);
            }
        }
    }

    public void ReparentButtons() {
        screen.SetBlockParent(3, 64 - 20, -10, -10, this);
        screen.SetBlockParent(5, 64 - 20, -1, -1, this);
        screen.SetBlockParent(7, 64 - 20, -11, -11, this);
        lockStart = false;
    }

    public void LevelComplete() {
        levelContainer.levels[currentLevel].complete = true;
        if (currentLevel + 1 < levelContainer.levels.Count) {
            levelContainer.levels[currentLevel + 1].unlocked = true;
        }
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
            string marshaled = System.IO.File.ReadAllText(Application.persistentDataPath + "/levels.json");
            if (marshaled.Length == 0) throw new Exception("Level file corrupt, rebuilding...");
            levelContainer = JsonUtility.FromJson<LevelSerial>(marshaled);
            Debug.Log("Read from: " + Application.persistentDataPath + "/levels.json");
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
                BuildToScreen(currentLevel);
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

            BuildToScreen(currentLevel);
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
        if (state == State.Play) {
            if (x == -10 && !lockStart) {
                BuildToScreen(currentLevel);
                player.Play(levelContainer.levels[currentLevel]);
            }
            if (x == -11 && !lockStart) {
                player.Stop();
                BuildToScreen(currentLevel);
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
    [SerializeField]
    private int[] tiles;

    public Level(int width, int height) {
        unlocked = false;
        tiles = new int[width * height];
        for (int i = 0; i < tiles.Length; i++) {
            tiles[i] = 0;
        }
    }

    public void SetTile(int x, int y, int t) { tiles[getInd(x, y)] = t; }
    public int GetTile(int x, int y) { return tiles[getInd(x, y)]; }

    private int getInd(int x, int y) { return x + (y * 32); }
}
