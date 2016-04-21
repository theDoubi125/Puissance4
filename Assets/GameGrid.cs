using UnityEngine;
using System.Collections.Generic;

public class IntVec
{
    public int x, y;

    public IntVec(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

public class GameGrid
{
    public int[] cells;
    public int w, h;
    System.Random rand = new System.Random();

    public GameGrid(int w, int h)
    {
        this.w = w;
        this.h = h;
        this.cells = new int[w * h];
        for (int i = 0; i < w * h; i++)
            cells[i] = -1;
    }

    public GameGrid(GameGrid grid)
    {
        this.w = grid.w;
        this.h = grid.h;
        this.cells = new int[w * h];
        for (int i = 0; i < w * h; i++)
            cells[i] = grid.cells[i];
    }

    public int GetLastCellY(int x)
    {
        int y;
        for (y = 0; y < h && cells[x + y * w] >= 0; y++) ;
        return y - 1;
    }

    public bool FillCell(int x, int playerId)
    {
        int y;
        for (y = 0; y < h && cells[x + y * w] >= 0; y++);
        if (y < h)
        {
            cells[x + y * w] = playerId;
            return HasWon(x, y);
        }
        else return false;
    }

    public bool IsGridFull()
    {
        for (int i = 0; i < w; i++)
            if (cells[i + (h - 1) * w] == -1)
                return false;
        return true;
    }

    private static int test = 0;

    public bool FillRandomCell(int playerId)
    {
        List<int> available = new List<int>();

        for (int i = 0; i < w; i++)
            if (cells[i + (h - 1) * w] == -1)
                available.Add(i);
        
        int id = (int)(rand.NextDouble() * available.Count);
        if (id == available.Count)
            id--;
        return FillCell(available[id], playerId);
    }

    public void CancelAction(int x)
    {
        int y;
        for (y = 0; y < h && cells[x + y * w] >= 0; y++) ;
        cells[x + (y-1) * w] = -1;
    }

    public int CellAt(int x, int y)
    {
        return cells[x + y * w];
    }

    public int GetWinner()
    {
        for(int i=0; i<w-3; i++)
        {
            for(int j=0; j< h; j++)
            {
                int playerId = CellAt(i, j);
                if(playerId >= 0)
                {
                    for(int k=1; k<4; k++)
                    {
                        if(CellAt(i+k, j) != playerId)
                        {
                            playerId = -1;
                            break;
                        }
                    }
                    if (playerId >= 0)
                        return playerId;
                }
            }
        }
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h - 3; j++)
            {
                int playerId = CellAt(i, j);
                if (playerId >= 0)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        if (CellAt(i, j + k) != playerId)
                        {
                            playerId = -1;
                            break;
                        }
                    }
                    if (playerId >= 0)
                        return playerId;
                }
            }
        }

        for (int i = 0; i < w - 3; i++)
        {
            for (int j = 0; j < h - 3; j++)
            {
                int playerId = CellAt(i, j);
                if (playerId >= 0)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        if (CellAt(i + k, j + k) != playerId)
                        {
                            playerId = -1;
                            break;
                        }
                    }
                    if (playerId >= 0)
                        return playerId;
                }
                playerId = CellAt(i, j+3);
                if (playerId >= 0)
                {
                    for (int k = 1; k < 4; k++)
                    {
                        if (CellAt(i + k, j+3 - k) != playerId)
                        {
                            playerId = -1;
                            break;
                        }
                    }
                    if (playerId >= 0)
                        return playerId;
                }
            }
        }
        return -1;
    }

    public bool HasWon(int x, int y)
    {
        int[] vecx = new int[] { 1, 0, 1, 1 };
        int[] vecy = new int[] { 0, 1, 1, -1 };
        
        for(int i=0; i<vecx.Length; i++)
        {
            int jmax = 1, jmin = 1;
            while (x + vecx[i] * jmax >= 0 && x + vecx[i] * jmax < w
                && y + vecy[i] * jmax >= 0 && y + vecy[i] * jmax < h
                && CellAt(x + vecx[i] * jmax, y + vecy[i] * jmax) == CellAt(x, y))
                jmax++;
            while (x - vecx[i] * jmin >= 0 && x - vecx[i] * jmin < w
                && y - vecy[i] * jmin >= 0 && y - vecy[i] * jmin < h
                && CellAt(x - vecx[i] * jmin, y - vecy[i] * jmin) == CellAt(x, y))
                jmin++;
            if (jmax + jmin - 1 >= 4)
                return true;
        }
        return false;
    }

    public List<IntVec> GetVictoryCells(int x)
    {
        int y = GetLastCellY(x);
        List<IntVec> result = new List<IntVec>();
        int[] vecx = new int[] { 1, 0, 1, 1 };
        int[] vecy = new int[] { 0, 1, 1, -1 };

        for (int i = 0; i < vecx.Length; i++)
        {
            int jmax = 1, jmin = 1;
            while (x + vecx[i] * jmax >= 0 && x + vecx[i] * jmax < w
                && y + vecy[i] * jmax >= 0 && y + vecy[i] * jmax < h
                && CellAt(x + vecx[i] * jmax, y + vecy[i] * jmax) == CellAt(x, y))
                jmax++;
            while (x - vecx[i] * jmin >= 0 && x - vecx[i] * jmin < w
                && y - vecy[i] * jmin >= 0 && y - vecy[i] * jmin < h
                && CellAt(x - vecx[i] * jmin, y - vecy[i] * jmin) == CellAt(x, y))
                jmin++;
            Debug.Log(jmax + " " + jmin);
            Debug.Log(CellAt(x, y));
            if (jmax + jmin - 1 >= 4)
            {
                for (int j = 0; j < jmax + jmin - 1; j++)
                    result.Add(new IntVec(x + vecx[i] * (j - jmin + 1), y + vecy[i] * (j - jmin + 1)));
            }
        }
        return result;
    }
}
