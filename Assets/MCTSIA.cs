using UnityEngine;
using System.Collections.Generic;
using System.Threading;

public class MCTSIA : MonoBehaviour
{
    public int playerId;
    public int tries = 1000;
    public int tryMoves = 10;
    public int maxTriesPerThread = 250;

    public UnityEngine.UI.InputField inputFieldTries;
    public UnityEngine.UI.InputField inputFieldTryMoves;
    public UnityEngine.UI.InputField inputFieldMaxTriesPerThread;

    void Start()
    {
        inputFieldTries.text = tries.ToString();
        inputFieldTryMoves.text = tryMoves.ToString();
        inputFieldMaxTriesPerThread.text = maxTriesPerThread.ToString();
    }

    public void SetTries()
    {
        int _tries = int.Parse(inputFieldTries.text);
        if (_tries < 0)
            _tries = 0;

        tries = _tries;
    }

    public void SetTryMoves()
    {
        int _tryMoves = int.Parse(inputFieldTryMoves.text);
        if (_tryMoves < 0)
            _tryMoves = 0;

        tryMoves = _tryMoves;
    }

    public void SetMaxTriesPerThread()
    {
        int _maxTriesPerThread = int.Parse(inputFieldMaxTriesPerThread.text);
        if (_maxTriesPerThread < 1)
            _maxTriesPerThread = 1;

        maxTriesPerThread = _maxTriesPerThread;
    }

    public int CalcNextAction(GameGrid grid, int playerTurn)
    {
        int[] triesPerMove = new int[grid.w];
        int[] fitnessPerMove = new int[grid.w];

        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < tries; i++)
        {
            System.Random rand = new System.Random();
            int x = rand.Next(0, grid.w);
            fitnessPerMove[x] += MakeTry(x, playerTurn, grid);
            ++triesPerMove[x];
        }

        stopwatch.Stop();
        print("CalcNextAction elapsed time=" + stopwatch.ElapsedMilliseconds);

        float maxRatio = float.NegativeInfinity;
        int maxRatioPos = -1;
        for (int i = 0; i < grid.w; i++)
        {
            if (maxRatio < (float)fitnessPerMove[i] / triesPerMove[i])
            {
                maxRatio = (float)fitnessPerMove[i] / triesPerMove[i];
                maxRatioPos = i;
            }
        }
        return maxRatioPos;
    }

    public int CalcNextActionParallel(GameGrid grid, int playerTurn)
    {
        int[] triesPerMove = new int[grid.w];
        int[] fitnessPerMove = new int[grid.w];

        int threadCount = 0;

        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        int upperBound = (int)Mathf.Ceil((float)tries / maxTriesPerThread);
        for (int i = 0; i < upperBound; i++)
        {
            int startIndex = i;
            int endIndex = Mathf.Min(i + maxTriesPerThread, tries);
            Thread thread = new Thread(() => computeForParallel(startIndex, endIndex, grid, playerTurn, ref triesPerMove, ref fitnessPerMove, ref threadCount));
            Interlocked.Increment(ref threadCount);
            thread.Start();
        }
        
        while (threadCount > 0) ;

        stopwatch.Stop();
        print("CalcNextActionParallel elapsed time=" + stopwatch.ElapsedMilliseconds);

        float maxRatio = float.NegativeInfinity;
        int maxRatioPos = -1;
        for (int i=0; i<grid.w; i++)
        {
            if (maxRatio < (float)fitnessPerMove[i]/triesPerMove[i])
            {
                maxRatio = (float)fitnessPerMove[i] / triesPerMove[i];
                maxRatioPos = i;
            }
        }
        return maxRatioPos;
    }

    void computeForParallel(int startIndex, int endIndex, GameGrid grid, int playerTurn, ref int[] triesPerMove, ref int[] fitnessPerMove, ref int threadCount)
    {
        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();

        System.Random rand = new System.Random();
        int[] triesPerMoveCopy = new int[grid.w];
        int[] fitnessPerMoveCopy = new int[grid.w];

        for (int i = startIndex; i < endIndex; ++i)
        {
            int x = rand.Next(0, grid.w);
            int t = MakeTry(x, playerTurn, grid);
            fitnessPerMoveCopy[x] += t;
            ++triesPerMoveCopy[x];
        }

        for (int i = 0; i < fitnessPerMove.Length; ++i)
            if (fitnessPerMoveCopy[i] != 0)
                Interlocked.Add(ref fitnessPerMove[i], fitnessPerMoveCopy[i]);

        for (int i = 0; i < triesPerMove.Length; ++i)
            if (triesPerMoveCopy[i] != 0)
                Interlocked.Add(ref triesPerMove[i], triesPerMoveCopy[i]);

        Interlocked.Decrement(ref threadCount);

        stopwatch.Stop();
        print("thread " + Thread.CurrentThread.ManagedThreadId + ": computeForParallel elapsed time=" + stopwatch.ElapsedMilliseconds);
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
