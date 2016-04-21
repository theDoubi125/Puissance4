using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    [SerializeField]
    private int m_w, m_h;
    public int w { get { return m_w; } set { int oldw = m_w; m_w = value; if (oldw != m_w) Reset(); } }
    public int h { get { return m_h; } set { int oldh = m_h;  m_h = value; if(oldh != m_h) Reset(); } }

    public Transform cellPrefab;
    public GameObject victoryPanel;
    public Text victoryText;
    public Color[] playerColors = { Color.red, Color.green };
    public Color[] victoryColors = { Color.blue, Color.yellow };


    private GameGrid grid;
    private GameCell[] cells;
    private int playerTurn = 0;
    private int lastActionX = -1;
    private MCTSIA ia;
    private bool gameFinished = false;

    private bool useThreads = true;

    public void setUseThreads()
    {
        useThreads = !useThreads;
    }

	void Start()
    {
        ia = GetComponent<MCTSIA>();
        Reset();
	}
	
	void Update()
    {

	}

    public void Reset()
    {
        foreach (var cell in transform.GetComponentsInChildren<GameCell>())
            DestroyImmediate(cell.gameObject);
        cells = new GameCell[w*h];
        for(int i=0; i< w; i++)
        {
            for(int j=0; j<h; j++)
            {
                Transform cellInstance = Transform.Instantiate<Transform>(cellPrefab);
                cellInstance.GetComponent<GameCell>().Init(i, j, this);
                cells[i + j * w] = cellInstance.GetComponent<GameCell>();
                cellInstance.parent = transform;
            }
        }
        grid = new GameGrid(w, h);
        victoryPanel.SetActive(false);
        gameFinished = false;
    }

    public void NewGame()
    {
        grid = new GameGrid(w, h);
        UpdateColors();
        victoryPanel.SetActive(false);
        gameFinished = false;
    }

    void UpdateColors()
    {
        for(int i=0; i < w; i++)
        {
            for(int j=0; j < h; j++)
            {
                Image img = cells[i + j * w].GetComponent<Image>();
                if (grid.CellAt(i, j) >= 0)
                    img.color = playerColors[grid.CellAt(i, j)];
                else
                    img.color = Color.white;
            }
        }
    }

    void UpdateVictoryColors(int x)
    {
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                Image img = cells[i + j * w].GetComponent<Image>();
                if (grid.CellAt(i, j) >= 0)
                    img.color = playerColors[grid.CellAt(i, j)];
                else
                    img.color = Color.white;
            }
        }
        
        List<IntVec> victoryCells = grid.GetVictoryCells(x);
        for (int k = 0; k < victoryCells.Count; k++)
        {
            print(victoryCells[k].x + " " + victoryCells[k].y);
            Image img = cells[victoryCells[k].x + victoryCells[k].y * w].GetComponent<Image>();
            img.color = victoryColors[grid.CellAt(victoryCells[k].x, victoryCells[k].y)];
        }
    }

    public void ActionAt(int x)
    {
        if (!gameFinished)
        {
            if(grid.FillCell(x, 0))
            {
                UpdateVictoryColors(x);
                ShowVictoryPanel(0);
            }
            else
            {
                UpdateColors();
                lastActionX = x;

                int iaAction;
                if (useThreads)
                    iaAction = ia.CalcNextActionParallel(grid, 1);
                else
                    iaAction = ia.CalcNextAction(grid, 1);
                
                if (grid.FillCell(iaAction, 1))
                {
                    UpdateVictoryColors(iaAction);
                    ShowVictoryPanel(1);
                }
                else
                    UpdateColors();
                lastActionX = iaAction;
            }
        }
    }

    public void CancelLastAction()
    {
        if (lastActionX >= 0)
        {
            grid.CancelAction(lastActionX);
            lastActionX = -1;
            playerTurn = 1 - playerTurn;
            UpdateColors();
        }
    }

    public void ShowVictoryPanel(int playerId)
    {
        victoryPanel.SetActive(true);
        victoryText.text = "Victoire du joueur " + (playerId + 1) + "!";
        gameFinished = true;
    }
}
