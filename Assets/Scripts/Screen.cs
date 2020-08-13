using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Screen : MonoBehaviour {
    // References
    public GameObject pixelPrefab;

    // State
    private List<List<GameObject>> pixels;

    public GameObject PixelAt(int x, int y) {
        return pixels[x][y];
    }

    public void Init() {
        if (pixels != null && pixels.Count > 0) {
            for (int x = 0; x < 64; x++) {
                for (int y = 0; y < 64; y++) {
                    Object.Destroy(pixels[x][y]);
                }
            }
            pixels.Clear();
        }
        pixels = new List<List<GameObject>>();
        for (int x = 0; x < 64; x++) {
            pixels.Add(new List<GameObject>());
            for (int y = 0; y < 64; y++) {
                GameObject px = Instantiate(pixelPrefab);
                px.transform.position = new Vector3(x, y, 0);
                px.transform.parent = transform;
                pixels[x].Add(px);
            }
        }
    }

    public void Load(Texture2D input) {
        for (int x = 0; x < input.width; x++) {
            for (int y = 0; y < input.height; y++) {
                SetPixelColor(x, y, input.GetPixel(x, y));
            }
        }
    }

    public void SetPixelColor(int x, int y, Color color) {
        if (x < 0 || x > 63 || y < 0 || y > 63) return;
        PixelAt(x, y).GetComponent<Pixel>().SetColor(color);
    }

    public void RevertPixel(int x, int y) {
        if (x < 0 || x > 63 || y < 0 || y > 63) return;
        PixelAt(x, y).GetComponent<Pixel>().RevertColor();
    }

    public void SetBlockColor(int x, int y, Color color) {
        PixelAt(x, y).GetComponent<Pixel>().SetColor(color);
        PixelAt(x + 1, y).GetComponent<Pixel>().SetColor(color);
        PixelAt(x, y + 1).GetComponent<Pixel>().SetColor(color);
        PixelAt(x + 1, y + 1).GetComponent<Pixel>().SetColor(color);
    }

    public void RevertBlockColor(int x, int y) {
        PixelAt(x, y).GetComponent<Pixel>().RevertColor();
        PixelAt(x + 1, y).GetComponent<Pixel>().RevertColor();
        PixelAt(x, y + 1).GetComponent<Pixel>().RevertColor();
        PixelAt(x + 1, y + 1).GetComponent<Pixel>().RevertColor();
    }

    public void SetPixelsColor(int x0, int y0, int width, int height, Color color) {
        for (int x = x0; x < x0 + width; x++) {
            for (int y = y0; y < y0 + height; y++) {
                SetPixelColor(x, y, color);
            }
        }
    }

    public void RevertPixelsColor(int x0, int y0, int width, int height) {
        for (int x = x0; x < x0 + width; x++) {
            for (int y = y0; y < y0 + height; y++) {
                RevertPixel(x, y);
            }
        }
    }

    public void SetPixelParent(int x, int y, int xTag, int yTag, PixelParent parent) {
        if (x < 0 || x > 63 || y < 0 || y > 63) return;
        if (PixelAt(x, y).GetComponent<Pixel>().x != -1) return;
        PixelAt(x, y).GetComponent<Pixel>().SetParent(parent);
        PixelAt(x, y).GetComponent<Pixel>().SetPos(xTag, yTag);
    }

    public void RevertPixelParent(int x, int y) {
        if (x < 0 || x > 63 || y < 0 || y > 63) return;
        PixelAt(x, y).GetComponent<Pixel>().RevertParent();
        PixelAt(x, y).GetComponent<Pixel>().RevertPos();
    }

    public void SetBlockParent(int x, int y, int xTag, int yTag, PixelParent parent) {
        SetPixelParent(x, y, xTag, yTag, parent);
        SetPixelParent(x + 1, y, xTag, yTag, parent);
        SetPixelParent(x, y + 1, xTag, yTag, parent);
        SetPixelParent(x + 1, y + 1, xTag, yTag, parent);
    }

    public void SetPixelsParent(int x0, int y0, int width, int height, int xTag, int yTag, PixelParent parent) {
        for (int x = x0; x < x0 + width; x++) {
            for (int y = y0; y < y0 + height; y++) {
                SetPixelParent(x, y, xTag, yTag, parent);
            }
        }
    }

    public void RevertColorList(List<(int, int)> pixelsToRevert) {
        for (int i = 0; i < pixelsToRevert.Count; i++) {
            (int, int) old = pixelsToRevert[i];
            RevertPixel(pixelsToRevert[i].Item1, pixelsToRevert[i].Item2);
        }
    }
}
