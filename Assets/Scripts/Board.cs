using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour, PixelParent {
    // Config
    public Color editorBackgroundColor;
    public Color viewBackgroundColor;
    public Color startColor;
    public Color stopColor;
    public Color f1Color;
    public Color f2Color;
    public Color f3Color;
    public Color f4Color;
    public Color f5Color;
    public Color selectorColor;
    public Color instructionBorderColor;
    public Color levelBarColor;
    public Color levelBackgroundColor;
    public Color levelSelectColor;
    public Color levelToCompleteColor;
    public Color levelLockedColor;

    // References
    public Screen screen;
    public LevelManager levelManager;
    public AudioManager audioManager;
    public Function function1;
    public Function function2;
    public Function function3;
    public Function function4;
    public Function function5;
    public Texture2D titleScreen;
    public Texture2D levelScreen;
    public Texture2D successScreen;
    public Selector titleSelector;
    public Selector githubSelector;
    public Selector mainLevelSelector;
    public Selector secondaryLevelSelector;
    public Selector functionSelector;
    public DashEffect dashEffect;

    // State
    private int levelSelect = 0;

    private void Start() {
        TransitionToTitleState();       
    }

    // ----------------------------------------
    // Drawing screens
    // ----------------------------------------

    private void BuildTitle() {
        screen.Load(titleScreen);
        
        for (int x = 0; x < 64; x++) {
            for (int y = 0; y < 64; y++) {
                screen.SetPixelParent(x, y, -1, -1, this);
            }
        }

        screen.SetPixelsParent(24, 27, 17, 10, 24, 28, this);
        screen.SetPixelsParent(24, 17, 17, 10, 24, 18, this);
        screen.SetPixelsParent(60, 2, 5, 8, 60, 2, this);
    }

    private void BuildLevelView() {
        screen.Load(levelScreen);

        for (int x = 0; x < 64; x++) {
            for (int y = 0; y < 64; y++) {
                screen.SetPixelParent(x, y, -1, -1, this);
            }
        }

        // Level select buttons
        screen.SetPixelsParent(4, 37, 6, 10, 4, 37, this);
        screen.SetPixelsParent(55, 37, 6, 10, 55, 37, this);

        // Back button and start button
        screen.SetPixelsParent(1, 57, 6, 6, 1, 57, this);
        screen.SetPixelsParent(28, 24, 6, 6, 28, 24, this);

        // Levels
        BuildLevelBar();
    }

    private void BuildLevelBar() {
        // Background bar
        screen.SetPixelsColor(11, 37, 42, 6, levelBarColor);

        for (int i = 0; i < 10; i++) {
            Level level = levelManager.Get(i);
            Color col = levelBackgroundColor;
            if (level == null) continue;
            if (level.unlocked && level.complete) col = levelSelectColor;
            else if (level.unlocked) col = levelToCompleteColor;
            else col = levelLockedColor;

            screen.SetBlockColor(13 + i * 4, 39, col);
            if (i == levelSelect) {
                screen.SetPixelsColor(12 + i * 4, 38, 4, 4, col);
            }
        }
    }

    private void BuildGame() {
        BuildEditor();
        BuildView();
    }

    private void BuildEditor() {
        for (int x = 0; x < 64; x++) {
            for (int y = 0; y < 64; y++) {
                if (y >= 64 - 16) {
                    screen.SetPixelColor(x, y, editorBackgroundColor);
                }
            }
        }

        function1.Build(2, f1Color, viewBackgroundColor);
        function2.Build(2+13, f2Color, viewBackgroundColor);
        function3.Build(2 + 13 * 2, f3Color, viewBackgroundColor);
        function4.Build(2 + 13 * 3, f4Color, viewBackgroundColor);
        function5.Build(2 + 13 * 4, f5Color, viewBackgroundColor);
    }

    private void BuildView() {
        for (int x = 0; x < 64; x++) {
            for (int y = 0; y < 64; y++) {
                if (y < 64 - 16) {
                    screen.SetPixelColor(x, y, viewBackgroundColor);
                }
            }
        }

        for (int x = 0; x < 64; x++) {
            for (int y = 0; y < 64; y++) {
                if (x > 1 && y > 1 && x < 62 && y < 64 - 20) {
                    screen.SetPixelColor(x, y, f1Color);
                }
            }
        }

        BuildButtons();
        levelManager.BuildToScreen(levelSelect, false);
    }

    private void BuildButtons() {
        screen.SetBlockColor(3, 64 - 20, startColor);
        screen.SetBlockColor(7, 64 - 20, stopColor);
    }

    private void BuildSuccessView() {
        screen.Load(successScreen);

        for (int x = 0; x < 64; x++) {
            for (int y = 0; y < 64; y++) {
                screen.SetPixelParent(x, y, -1, -1, this);
            }
        }

        screen.SetPixelsParent(24, 27, 17, 10, 24, 31, this);
    }

    public List<List<Instructions>> BuildInstructionList() {
        List<List<Instructions>> inst = new List<List<Instructions>>();
        inst.Add(function1.GetInstructionList());
        inst.Add(function2.GetInstructionList());
        inst.Add(function3.GetInstructionList());
        inst.Add(function4.GetInstructionList());
        inst.Add(function5.GetInstructionList());
        return inst;
    }

    // Impl PixelParent
    public void OnPixelMouseEnter(int x, int y) {
        if (y == 2) {
            githubSelector.BuildSelector(x, y);
            return;
        }

        if (y == 37) {
            mainLevelSelector.BuildSelector(x, y);
            return;
        }

        if (y == 57 || y == 24) {
            secondaryLevelSelector.BuildSelector(x, y);
            return;
        }

        titleSelector.BuildSelector(x, y);
        if (x == -1) {
            githubSelector.BuildSelector(x, y);
            mainLevelSelector.BuildSelector(x, y);
            secondaryLevelSelector.BuildSelector(x, y);
        }
    }

    public void OnPixelMouseExit(int x, int y) { }

    public void OnPixelMouseDown(int x, int y) {
        if (y == 28 || y == 31) {
            audioManager.PlayClick();
            TransitionToLevelState();
        }
        if (y == 57) {
            audioManager.PlayClick();
            TransitionToTitleState();
        }
        if (y == 24) {
            audioManager.PlayClick();
            TransitionToGameState();
        }

        if (y == 18) {
            audioManager.PlayClick();
            Debug.Log("Quitting...");
            Application.Quit();
        }
        if (y == 2) {
            audioManager.PlayClick();
            Debug.Log("Opening URL...");
            Application.OpenURL("https://github.com/rishabh-bector/digicode");
        }

        if (x == 55 && y == 37) {
            audioManager.PlayClick();
            if (levelManager.Get(levelSelect + 1) != null) if (levelManager.Get(levelSelect + 1).unlocked) levelSelect++;
            if (levelSelect > 10) levelSelect = 10;
            BuildLevelBar();
        }
        if (x == 4 && y == 37) {
            audioManager.PlayClick();
            levelSelect--;
            if (levelSelect < 0) levelSelect = 0;
            BuildLevelBar();
        }
    }

    // ----------------------------------------
    // State machine
    // ----------------------------------------

    public void TransitionToLevelState() {
        audioManager.PlayClick();
        functionSelector.Reset();
        screen.Init();
        dashEffect.STOP();
        BuildLevelView();
    }

    public void TransitionToGameState() {
        screen.Init();
        BuildGame();
        audioManager.FadeOutSource();
    }

    public void TransitionToTitleState() {
        functionSelector.Reset();
        screen.Init();
        dashEffect.START();
        BuildTitle();
    }

    public void TransitionToSuccessState() {
        screen.Init();
        dashEffect.STOP();
        BuildSuccessView();
        screen.DrawDigits(levelManager.finalScore, 40, 17);
        screen.DrawDigits(levelManager.bestScore, 40, 8);
    }
}
