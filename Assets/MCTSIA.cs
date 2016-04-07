using UnityEngine;
using System.Collections;

public class MCTSIA : MonoBehaviour
{
    public int playerId;
    public int tries = 1000;
    public int tryMoves = 10;

    public UnityEngine.UI.InputField inputFieldTries;
    public UnityEngine.UI.InputField inputFieldTryMoves;
    
    void Start()
    {
        inputFieldTries.text = tries.ToString();
        inputFieldTryMoves.text = tryMoves.ToString();
    }

    public void SetTries()
    {
        int _tries = int.Parse(inputFieldTries.text);
        if (_tries <= 0)
            _tries = 0;

        tries = _tries;
    }

    public void SetTryMoves()
    {
        int _tryMoves = int.Parse(inputFieldTryMoves.text);
        if (_tryMoves <= 0)
            _tryMoves = 0;

        tryMoves = _tryMoves;
    }

    public int CalcNextAction(GameGrid grid, int playerTurn)
    {
        int[] triesPerMove = new int[grid.w];
        int[] fitnessPerMove = new int[grid.w];
        for(int i=0; i< tries; i++)
        {
            int x = Random.Range(0, grid.w);
            fitnessPerMove[x] += MakeTry(x, playerTurn, grid);
            triesPerMove[x]++;
        }
        float maxRatio = float.NegativeInfinity;
        int maxRatioPos = -1;
        for(int i=0; i<grid.w; i++)
        {
            if(maxRatio < (float)fitnessPerMove[i]/triesPerMove[i])
            {
                maxRatio = (float)fitnessPerMove[i] / triesPerMove[i];
                maxRatioPos = i;
            }
        }
        return maxRatioPos;
    }

    public int MakeTry(int x, int playerTurn, GameGrid grid)
    {
        GameGrid gridCopy = new GameGrid(grid);
        if (gridCopy.FillCell(x, playerTurn))
            return 1;
        playerTurn = 1 - playerTurn;
        for (int i=0; i< tryMoves; i++)
        {
            if (gridCopy.IsGridFull())
            {
                return 0;
            }
            else
            {
                if (gridCopy.FillRandomCell(playerTurn))
                {
                    return (playerTurn == playerId) ? 1 : -1;
                }
                playerTurn = 1 - playerTurn;

            }
        }
        return 0;
    }
}
