using UnityEngine;
using System.Collections.Generic;

public class Selector : MonoBehaviour {
    // Config
    public Color selectorColor;
    public int width;
    public int height;

    // References
    public Screen screen;

    // State
    public int selectX = -1;
    public int selectY = -1;
    private int oldSelectX = -1;
    private int oldSelectY = -1;
    private List<(int, int)> oldSelect;
    public bool locked = false;

    public void Lock() { locked = true; }
    public void Unlock() { locked = false;  }

    private void Start() {
        oldSelect = new List<(int, int)>();
    }

    public void Destroy() {
        for (int i = 0; i < oldSelect.Count; i++) {
            (int, int) old = oldSelect[i];
            screen.RevertPixel(old.Item1, old.Item2);
        }
        oldSelect.Clear();
    }

    public void BuildSelector(int x, int y) {
        if (locked) return;

        selectX = x;
        selectY = y;

        // Only build if location has changed
        // if (x == oldSelectX && y == oldSelectY) return;
        Destroy();

        // Build new selector
        if (x == -1 || y == -1) {
            oldSelectX = x;
            oldSelectY = y;
            return;
        };
        for (int x0 = 0; x0 < width; x0++) {
            for (int y0 = 0; y0 < height; y0++) {
                if (x0 == 0 || x0 == width - 1 || y0 == 0 || y0 == height - 1) {
                    if (x > 0 && y > 0 && x < 64 && y < 64) {
                        screen.SetPixelColor(x - 1 + x0, y - 2 + y0, selectorColor);
                        oldSelect.Add((x - 1 + x0, y - 2 + y0));
                    }
                }
            }
        }

        oldSelectX = x;
        oldSelectY = y;
    }
}
