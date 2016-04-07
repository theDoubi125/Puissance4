using UnityEngine;
using System.Collections;

public class GameCell : MonoBehaviour
{
    private int x, y;
    private GameManager manager;

    public void Init(int x, int y, GameManager manager)
    {
        this.manager = manager;
        this.x = x;
        this.y = y;
    }

    public void OnClick()
    {
        manager.ActionAt(x);
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
