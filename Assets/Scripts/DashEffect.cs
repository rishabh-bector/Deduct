using UnityEngine;
using System.Collections;

public class DashEffect : MonoBehaviour {
    // Config
    public Color dashColor;
    public int startX;
    public int startY;
    public int width;
    public int height;
    public int speed;

    // References
    public Screen screen;

    // State
    private int x;
    private int y;
    private (int, int) velocity = (0, 0);
    private bool firstRun = true;
    private bool running = true;
    private bool justStoppedRunning = false;

    public void START() { running = true; }
    public void STOP() {
        running = false;
        justStoppedRunning = true;
    }
     
    private void Start() {
        x = startX;
        y = startY;
    }

    private void FixedUpdate() {
        if (!running && !justStoppedRunning) return;

        if (x == startX && y == startY) {
            velocity = (0, speed);
        }

        if (x == startX && y == startY + height - 1) {
            velocity = (speed, 0);
        }

        if (x == startX + width - 1 && y == startY + height - 1) {
            velocity = (0, -speed);
        }

        if (x == startX + width - 1 && y == startY) {
            velocity = (-speed, 0);
        }

        if (!firstRun) screen.RevertPixel(x, y);
        else firstRun = false;

        if (justStoppedRunning) {
            justStoppedRunning = false;
            return;
        }

        x += velocity.Item1;
        y += velocity.Item2;
        screen.SetPixelColor(x, y, dashColor);
    }
}
