using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PixelParent {
    void OnPixelMouseEnter(int x, int y);
    void OnPixelMouseExit(int x, int y);
    void OnPixelMouseDown(int x, int y);
}

public class Pixel : MonoBehaviour {
    // References
    public SpriteRenderer sprite;

    // State
    public PixelParent parent;
    public PixelParent oldParent;
    public int ptag;
    public int x = -1;
    public int y = -1;
    private int oldX = -1;
    private int oldY = -1;
    private List<Color> previousColors = new List<Color>();

    public void SetColor(Color col) {
        if (previousColors.Count > 0) previousColors.Add(sprite.color);
        else previousColors.Add(col);
        sprite.color = col;
    }

    public void RevertColor() {
        if (previousColors.Count == 0) return;
        sprite.color = previousColors[previousColors.Count - 1];
        if (previousColors.Count > 0) previousColors = previousColors.GetRange(0, previousColors.Count - 1);
    }

    public void SetParent(PixelParent newParent) {
        if (newParent == parent) return;
        oldParent = parent;
        parent = newParent;
    }

    public void RevertParent() {
        parent = oldParent;
    }

    public void RevertPos() {
        x = oldX;
        y = oldY;
    }

    public void SetPos(int iX, int iY) {
        oldX = x;
        oldY = y;
        x = iX;
        y = iY;
    }

    private void OnMouseExit() {
        if (parent != null) parent.OnPixelMouseExit(x, y);
    }

    private void OnMouseEnter() {
        if (parent != null) parent.OnPixelMouseEnter(x, y);
    }

    private void OnMouseDown() {
        if (parent != null) parent.OnPixelMouseDown(x, y);
    }
}
