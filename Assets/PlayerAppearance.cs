using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAppearance : MonoBehaviour {

    private Color[] colors = new Color[4] {
        new Color(0f, 194f, 48f), // green
        new Color(194f, 0f, 110f),// magenta
        new Color(0f, 68f, 194f), // blue
        new Color(194f, 184f, 0f) // yellow
    };

    private SpriteRenderer sr;
    private int playerIndex;

    void Start () {
        sr = GetComponentInChildren<SpriteRenderer>();
        this.SetPlayerIndex(0);
	}

    public void SetPlayerNameFloatie(string name) {

    }

    public void SetPlayerIndex(int i) {
        this.playerIndex = i;
        this.SetPlayerColor(colors[i]);
    }

    public void SetPlayerColor(Color color) {
        this.sr.color = color;
    }
}
