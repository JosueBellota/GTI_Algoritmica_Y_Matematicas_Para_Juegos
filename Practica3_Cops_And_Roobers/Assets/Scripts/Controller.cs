using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller : MonoBehaviour
{
    //GameObjects
    public GameObject board;
    public GameObject[] cops = new GameObject[2];
    public GameObject robber;
    public Text rounds;
    public Text finalMessage;
    public Button playAgainButton;

    //Otras variables
    Tile[] tiles = new Tile[Constants.NumTiles];
    private int roundCount = 0;
    private int state;
    private int clickedTile = -1;
    private int clickedCop = 0;
                    
    void Start()
    {        
        InitTiles();
        InitAdjacencyLists();
        state = Constants.Init;
    }
        
    //Rellenamos el array de casillas y posicionamos las fichas
    void InitTiles()
    {
        for (int fil = 0; fil < Constants.TilesPerRow; fil++)
        {
            GameObject rowchild = board.transform.GetChild(fil).gameObject;            

            for (int col = 0; col < Constants.TilesPerRow; col++)
            {
                GameObject tilechild = rowchild.transform.GetChild(col).gameObject;                
                tiles[fil * Constants.TilesPerRow + col] = tilechild.GetComponent<Tile>();                         
            }
        }
                
        cops[0].GetComponent<CopMove>().currentTile=Constants.InitialCop0;
        cops[1].GetComponent<CopMove>().currentTile=Constants.InitialCop1;
        robber.GetComponent<RobberMove>().currentTile=Constants.InitialRobber;           
    }

    public void InitAdjacencyLists()
    {
        for (int i = 0; i < tiles.Length; i++)
        {
            Tile t = tiles[i];
            t.adjacency.Clear();

            int fila = i / 8;
            int columna = i % 8;

            // Arriba
            if (fila > 0)
                t.adjacency.Add(i - 8);
            // Abajo
            if (fila < 7)
                t.adjacency.Add(i + 8);
            // Izquierda
            if (columna > 0)
                t.adjacency.Add(i - 1);
            // Derecha
            if (columna < 7)
                t.adjacency.Add(i + 1);
        }
    }


    //Reseteamos cada casilla: color, padre, distancia y visitada
    public void ResetTiles()
    {        
        foreach (Tile tile in tiles)
        {
            tile.Reset();
        }
    }

    public void ClickOnCop(int cop_id)
    {
        switch (state)
        {
            case Constants.Init:
            case Constants.CopSelected:                
                clickedCop = cop_id;
                clickedTile = cops[cop_id].GetComponent<CopMove>().currentTile;
                tiles[clickedTile].current = true;

                ResetTiles();
                FindSelectableTiles(clickedTile);

                state = Constants.CopSelected;                
                break;            
        }
    }

    public void ClickOnTile(int t)
    {                     
        clickedTile = t;

        switch (state)
        {            
            case Constants.CopSelected:
                //Si es una casilla roja, nos movemos
                if (tiles[clickedTile].selectable)
                {                  
                    cops[clickedCop].GetComponent<CopMove>().MoveToTile(tiles[clickedTile]);
                    cops[clickedCop].GetComponent<CopMove>().currentTile=tiles[clickedTile].numTile;
                    tiles[clickedTile].current = true;   
                    
                    state = Constants.TileSelected;
                }                
                break;
            case Constants.TileSelected:
                state = Constants.Init;
                break;
            case Constants.RobberTurn:
                state = Constants.Init;
                break;
        }
    }

    public void FinishTurn()
    {
        switch (state)
        {            
            case Constants.TileSelected:
                ResetTiles();

                state = Constants.RobberTurn;
                RobberTurn();
                break;
            case Constants.RobberTurn:                
                ResetTiles();
                IncreaseRoundCount();
                if (roundCount <= Constants.MaxRounds)
                    state = Constants.Init;
                else
                    EndGame(false);
                break;
        }

    }

    public void RobberTurn()
    {
        int start = robber.GetComponent<RobberMove>().currentTile;

        // Casillas alcanzables desde el ladrón
        FindSelectableTiles(start);
        List<Tile> reachable = new List<Tile>();
        foreach (Tile t in tiles)
        {
            if (t.selectable)
                reachable.Add(t);
        }

        if (reachable.Count == 0)
            return;

        // Distancias desde cada policía
        int cop0Tile = cops[0].GetComponent<CopMove>().currentTile;
        int cop1Tile = cops[1].GetComponent<CopMove>().currentTile;
        int[] distFromCop0 = GetDistancesFrom(cop0Tile);
        int[] distFromCop1 = GetDistancesFrom(cop1Tile);

        Tile bestTile = reachable[0];
        int bestMinDist = -1;

        foreach (Tile t in reachable)
        {
            int d0 = distFromCop0[t.numTile];
            int d1 = distFromCop1[t.numTile];
            int minDist = Mathf.Min(d0, d1);

            if (minDist > bestMinDist)
            {
                bestMinDist = minDist;
                bestTile = t;
            }
        }

        robber.GetComponent<RobberMove>().MoveToTile(bestTile);
    }



    public void EndGame(bool end)
    {
        if(end)
            finalMessage.text = "You Win!";
        else
            finalMessage.text = "You Lose!";
        playAgainButton.interactable = true;
        state = Constants.End;
    }

    public void PlayAgain()
    {
        cops[0].GetComponent<CopMove>().Restart(tiles[Constants.InitialCop0]);
        cops[1].GetComponent<CopMove>().Restart(tiles[Constants.InitialCop1]);
        robber.GetComponent<RobberMove>().Restart(tiles[Constants.InitialRobber]);
                
        ResetTiles();

        playAgainButton.interactable = false;
        finalMessage.text = "";
        roundCount = 0;
        rounds.text = "Rounds: ";

        state = Constants.Restarting;
    }

    public void InitGame()
    {
        state = Constants.Init;
         
    }

    public void IncreaseRoundCount()
    {
        roundCount++;
        rounds.text = "Rounds: " + roundCount;
    }

    public void FindSelectableTiles(int start, int avoid = -1)
    {
        foreach (Tile t in tiles)
            t.Reset();

        Queue<Tile> queue = new Queue<Tile>();
        Tile startTile = tiles[start];
        startTile.visited = true;
        startTile.distance = 0;
        startTile.current = true;

        queue.Enqueue(startTile);

        while (queue.Count > 0)
        {
            Tile t = queue.Dequeue();

            foreach (int adj in t.adjacency)
            {
                Tile neighbor = tiles[adj];

                if (!neighbor.visited && adj != avoid)
                {
                    if (t.numTile != avoid)
                    {
                        neighbor.parent = t;
                        neighbor.visited = true;
                        neighbor.distance = t.distance + 1;

                        if (neighbor.distance <= 2)
                        {
                            neighbor.selectable = true;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }
        }

        startTile.selectable = false;
    }

    // Este método debe estar FUERA de cualquier otro
    private int[] GetDistancesFrom(int start)
    {
        int[] distances = new int[tiles.Length];
        for (int i = 0; i < distances.Length; i++)
            distances[i] = int.MaxValue;

        Queue<Tile> queue = new Queue<Tile>();
        distances[start] = 0;
        queue.Enqueue(tiles[start]);

        while (queue.Count > 0)
        {
            Tile t = queue.Dequeue();

            foreach (int adj in t.adjacency)
            {
                if (distances[adj] == int.MaxValue)
                {
                    distances[adj] = distances[t.numTile] + 1;
                    queue.Enqueue(tiles[adj]);
                }
            }
        }

        return distances;
    }  
        


}
       

