using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

class MazeGenerator : MonoBehaviour
{
    public GameObject mapGenerator;
    public GameObject car;
    public GameObject npcs;

    public GameObject pendingGuy;

	private char EMPTY = ' ';
	private char ROAD = '#';
	private char CROSSING = '#';
	private char CONSTRUCTING_ROAD = '@';
	private char TOWN_SQUARE = '*';
    private char SHOP = '$';
    private char WORKSHOP = '%';
    private char HOUSE = '^';

    public int mapWidth = 32;
	public int mapHeight = 32;
	public int numberOfNodes = 3;
    public int amountOfShops = 8;
    public int amountOfHouses = 15;
    public int amountOfWorkshops = 2;
    public int amountOfPending = 5;
    public float spawnSafeArea = 30.0f;
    public int roadRequiredDistance = 2;
    public float crossingRequiredDistance = 30.0f;

    public int townSquareWidth = 7;
    public int townSquareHeight = 7;

	private int[,] DIRECTIONS = new int[4, 2]{
		{ -1, 0 },
		{ 0, 1 },
		{ 1, 0 },
		{ 0, -1 }
	};
	private int[] shuffled_indices = new int[4] {
		0,1,2,3
	};

    public void Shuffle(int[] array)
    {
        
        int n = array.Count();
        while (n > 1)
        {
            n--;
            int i = UnityEngine.Random.Range(0, 4);
            int temp = array[i];
            array[i] = array[n];
            array[n] = temp;
        }
    }
    
    private bool inBounds(int y, int x) {
        return y >= 0 && y < mapHeight && x >= 0 && x < mapWidth;
    }

    public Tilemap[] houses;
    private char[,] GenerateMaze()
	{
		char[,] map = new char[mapHeight, mapWidth];

		for (int y = 0; y < mapHeight; ++y)
		{
			for (int x = 0; x < mapWidth; ++x)
			{
                map[y, x] = EMPTY;
            }
		}

        int townSquareStartX = UnityEngine.Random.Range(0, mapWidth - townSquareWidth);
        int townSquareStartY = UnityEngine.Random.Range(0, mapHeight - townSquareHeight);

		for (int y = townSquareStartY; y < townSquareStartY + townSquareHeight; ++y)
		{
			for (int x = townSquareWidth; x < townSquareHeight; ++x)
			{
                map[y, x] = TOWN_SQUARE;
            }
		}

		int[,] crossings = new int[numberOfNodes, 2];
        int[,] shops = new int[amountOfShops, 2];
        int shopsPlaced = 0;
        int[,] workshops = new int[amountOfWorkshops, 2];
        int workshopsPlaced = 0;
        int[,] houses = new int[amountOfHouses, 2];

		for (int node_id = 0; node_id < numberOfNodes; ++node_id) { 
			int randomX = UnityEngine.Random.Range(0, mapWidth);
            int randomY = UnityEngine.Random.Range(0, mapHeight);
			
			// prevent putting crossings on already occupied tiles
			if (map[randomY, randomX] != EMPTY) {
				--node_id;
				continue;
			}

			map[randomY, randomX] = CROSSING;
			crossings[node_id, 0] = randomY;
			crossings[node_id, 1] = randomX;

            int randomDir = UnityEngine.Random.Range(0, 4);
            int newX = DIRECTIONS[randomDir,1] + DIRECTIONS[(randomDir + 1) % 4,1] + randomX;
            int newY = DIRECTIONS[randomDir,0] + DIRECTIONS[(randomDir + 1) % 4,0] + randomY;

            if (inBounds(newY, newX)) {
                if (UnityEngine.Random.Range(0, 2) == 0 && shopsPlaced < amountOfShops) {
                    shops[shopsPlaced, 0] = newY;
                    shops[shopsPlaced, 1] = newX;
                    map[newY, newX] = SHOP;
                    shopsPlaced++;
                } else if (workshopsPlaced < amountOfWorkshops) {
                    workshops[workshopsPlaced, 0] = newY;
                    workshops[workshopsPlaced, 1] = newX;
                    map[newY, newX] = WORKSHOP;
                    workshopsPlaced++;
                }
            }
		}

		int[,,] distances = new int[numberOfNodes, mapHeight, mapWidth];
		for (int y = 0; y < mapHeight; ++y) {
            for (int x = 0; x < mapWidth; ++x)
            {
				for (int node_id = 0; node_id < numberOfNodes; ++node_id) {
					distances[node_id, y, x] = 9999999;
				}
			}
        }

        for (int node_id = 0; node_id < numberOfNodes; ++node_id) {
			int myX = crossings[node_id, 1];
			int myY = crossings[node_id, 0];
			distances[node_id, myY, myX] = 0;

			for (int step = 0; step < mapHeight * mapWidth; ++step) {
				for (int y = 0; y < mapHeight; ++y) {
					for (int x = 0; x < mapWidth; ++x) {
						if (distances[node_id, y, x] == step) {
							for (int direction = 0; direction < DIRECTIONS.GetLength(0); ++direction) {
								// Debug.Log("Direction_id " + direction.ToString());
								int nextX = x + DIRECTIONS[direction, 1];
								int nextY = y + DIRECTIONS[direction, 0];

								if (nextX >= 0 && nextX < mapWidth && nextY >= 0 && nextY < mapHeight && (map[nextY, nextX] == EMPTY || map[nextY, nextX] == CROSSING))
								{
									if (distances[node_id, nextY, nextX] > step + 1) {
										// Debug.Log("Setting x=" + nextX.ToString() + ", y=" + nextY.ToString() + " to " + (step + 1).ToString());
                                        distances[node_id, nextY, nextX] = step + 1;
                                    }
                                }
							}
						}
					}
				}
			}

			for (int to_node = 0; to_node < numberOfNodes; ++to_node) {
				if (node_id == to_node) {
					continue;
				}
				int currY = crossings[to_node, 0];
				int currX = crossings[to_node, 1];
				int destY = crossings[node_id, 0];
				int destX = crossings[node_id, 1];

				// Debug.Log("Going from node (" + destX + ", " + destY + ") to (" + currX + ", " + currY + ")");
				bool doWalk = true;
				int distance = distances[node_id, currY, currX];
				for(int step = distance - 1; step > 0 && doWalk; --step)
				{
                    Shuffle(shuffled_indices);
                    for (int d = 0; d < DIRECTIONS.GetLength(0); ++d)
					{
						int direction = shuffled_indices[d];
						if (currX + DIRECTIONS[direction, 1] >= 0 && currX + DIRECTIONS[direction, 1] < mapWidth && currY + DIRECTIONS[direction, 0] >= 0 && currY + DIRECTIONS[direction, 0] < mapHeight)
						{
							if (map[currY + DIRECTIONS[direction, 0], currX + DIRECTIONS[direction, 1]] == ROAD) { 
								doWalk = false;
							}
                            if (distances[node_id, currY + DIRECTIONS[direction, 0], currX + DIRECTIONS[direction, 1]] == step)
                            {
								map[currY + DIRECTIONS[direction, 0], currX + DIRECTIONS[direction, 1]] = CONSTRUCTING_ROAD;
								currY += DIRECTIONS[direction, 0];
								currX += DIRECTIONS[direction, 1];
								break;
                            }
                        }
					}
				}

				for (int y = 0; y < mapHeight; ++y)
				{
					for (int x = 0; x < mapWidth; ++x) {
						if (map[y, x] == CONSTRUCTING_ROAD)
						{
							map[y, x] = ROAD;
						}
					}
				}

			}
		}

        // dodawanie domków
        int housesPlaced = 0;
        while (housesPlaced < amountOfHouses) {
            int randomX = UnityEngine.Random.Range(0, mapWidth);
            int randomY = UnityEngine.Random.Range(0, mapHeight);

            if (map[randomY, randomX] != EMPTY) {
                continue;
            }

            bool roadNearby = false;
            bool crossingNearby = false;

            for (int i = 0; i < crossings.GetLength(0); ++i){
                if (Vector2.Distance(new Vector2(crossings[i,0], crossings[i,0]), new Vector2(randomX, randomY)) <= crossingRequiredDistance) {
                    crossingNearby = true;
                }
            }

            if (!crossingNearby) {
                continue;
            }

            for (int offX = randomX-roadRequiredDistance; offX <= randomX+roadRequiredDistance; ++offX) {
                for (int offY = randomY-roadRequiredDistance; offY <= randomY+roadRequiredDistance; ++offY) {
                    if (inBounds(offY, offX)) {
                        if (map[offY, offX] == ROAD) {
                            roadNearby = true;
                            break;
                        }
                    }
                }
                if (roadNearby) {
                    map[randomY, randomX] = HOUSE;
                    housesPlaced++;
                    break;
                }
            }
        }

        GameManager.crossings = crossings;
        GameManager.workshops = workshops;
        GameManager.shops = shops;

		return map;
	}

    private void PrintMaze(char[,] map)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < map.GetLength(1); i++)
        {
            for (int j = 0; j < map.GetLength(0); j++)
            {
                sb.Append(map[i, j]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }

    public Tilemap empty;
    public Tilemap left;
    public Tilemap down;
    public Tilemap leftDown;
    public Tilemap right;
    public Tilemap rightLeft;
    public Tilemap rightDown;
    public Tilemap rightDownLeft;
    public Tilemap up;
    public Tilemap upLeft;
    public Tilemap upDown;
    public Tilemap upDownLeft;
    public Tilemap upRight;
    public Tilemap upRightLeft;
    public Tilemap upRightDown;
    public Tilemap all;

    public Tilemap grass;
    public Tilemap water;

    public Tilemap shop1;
    public Tilemap workshop1;
    public Tilemap[] housesSprites;

    public Tilemap townSquare;

    private char[,] SURR = { 
        {' ', ' ', ' ', ' '},
        {' ', ' ', ' ', '#'},
        {' ', ' ', '#', ' '},
        {' ', ' ', '#', '#'},
        {' ', '#', ' ', ' '},
        {' ', '#', ' ', '#'},
        {' ', '#', '#', ' '},
        {' ', '#', '#', '#'},
        {'#', ' ', ' ', ' '},
        {'#', ' ', ' ', '#'},
        {'#', ' ', '#', ' '},
        {'#', ' ', '#', '#'},
        {'#', '#', ' ', ' '},
        {'#', '#', ' ', '#'},
        {'#', '#', '#', ' '},
        {'#', '#', '#', '#'},
    };

    private void RenderMaze(char[,] map){
        Tilemap[] tiles = {
            empty,
            left,
            down,
            leftDown,
            right,
            rightLeft,
            rightDown,
            rightDownLeft,
            up,
            upLeft,
            upDown,
            upDownLeft,
            upRight,
            upRightLeft,
            upRightDown,
            all
        };
        for (int y = 0; y < map.GetLength(0); ++y) {
            for (int x = 0; x < map.GetLength(1); ++x){
                Vector3 pos = Vector3.Scale(new Vector3(x, y, 0), GameManager.mapToWorldSpace);
                if (map[y,x] == ROAD || map[y,x] == CROSSING) {
                    for (int pattern = 0; pattern < 16; ++pattern){
                        bool matches = true;
                        for (int dir = 0; dir < 4; ++dir){
                            int newX = x + DIRECTIONS[dir,1];
                            int newY = y + DIRECTIONS[dir,0];

                            char tile = ' ';
                            if (!(newY < 0 || newY >= mapHeight || newX < 0 || newX >= mapWidth)) {
                                tile = map[newY,newX];
                                if (tile == TOWN_SQUARE || tile == SHOP || tile == WORKSHOP || tile == HOUSE) {
                                    tile = EMPTY;
                                }
                            }

                            if (tile != SURR[pattern,dir]) {
                                matches = false;
                                break;
                            }
                        }
                        if (matches) {
                            /* Debug.Log("matched " + SURR[pattern].ToString() + " with " + ); */
                            Instantiate(tiles[pattern], pos, Quaternion.identity, mapGenerator.transform);
                            car.transform.position = pos;
                            break;
                        }
                    }
                } else if (map[y,x] == EMPTY) {
                    Instantiate(grass, pos, Quaternion.identity, mapGenerator.transform);
                } else if (map[y,x] == SHOP) {
                    Instantiate(shop1, pos, Quaternion.identity, mapGenerator.transform);
                } else if (map[y,x] == WORKSHOP) {
                    Instantiate(workshop1, pos, Quaternion.identity, mapGenerator.transform);
                } else if (map[y,x] == HOUSE) {
                    Instantiate(housesSprites[UnityEngine.Random.Range(0, housesSprites.GetLength(0))], pos, Quaternion.identity, mapGenerator.transform);
                } else if (map[y,x] == TOWN_SQUARE) {
                    Instantiate(townSquare, pos, Quaternion.identity, mapGenerator.transform);
                }
            }
        }

        for (int y = -2; y < mapHeight + 2; ++y){
            Instantiate(water, Vector3.Scale(new Vector3(-1, y, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
            Instantiate(water, Vector3.Scale(new Vector3(-2, y, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
            Instantiate(water, Vector3.Scale(new Vector3(mapWidth, y, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
            Instantiate(water, Vector3.Scale(new Vector3(mapWidth + 1, y, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
        }
        for (int x = -2; x < mapHeight + 2; ++x){
            Instantiate(water, Vector3.Scale(new Vector3(x, -1, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
            Instantiate(water, Vector3.Scale(new Vector3(x, -2, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
            Instantiate(water, Vector3.Scale(new Vector3(x, mapHeight, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
            Instantiate(water, Vector3.Scale(new Vector3(x, mapHeight + 1, 0), GameManager.mapToWorldSpace), Quaternion.identity, mapGenerator.transform);
        }
    }

    public void Start()
	{
        //mapCamera.transform.position = new Vector3(car.transform.position.x, car.transform.position.y, -10);
		char[,] map = GenerateMaze();

        GameManager.map = map;

		PrintMaze(map);
        RenderMaze(map);

        GameManager.pendingPos = new List<int[]>();
	}

    public void Update() {
        if (!GameManager.nuked){
            spawnPending();
        }
    }

    private void spawnPending(){
        if (npcs.transform.childCount < amountOfPending) {
            Vector3 pos = Vector3.zero;
            int[] posGM = new int[2];
            bool positionOccupied;
            do {
                positionOccupied = false;
                pos.x = UnityEngine.Random.Range(0, GameManager.map.GetLength(1));
                pos.y = UnityEngine.Random.Range(0, GameManager.map.GetLength(0));
                posGM[0] = (int)pos.y;
                posGM[1] = (int)pos.x;

                for (int i = 0; i < GameManager.pendingPos.Count; ++i) {
                    if (posGM[0] == GameManager.pendingPos[i][0] && posGM[1] == GameManager.pendingPos[i][1]) { 
                        positionOccupied = true;
                    }
                }
            } while (GameManager.map[(int)pos.y, (int)pos.x] != ROAD || (Vector3.Scale(pos, new Vector3(5, -5, 1)) - car.transform.position).magnitude <= spawnSafeArea || positionOccupied);
            Debug.Log("Creating" + posGM[1].ToString() + " " + posGM[0].ToString());
            GameManager.start[0] = posGM[0];
            GameManager.start[1] = posGM[1];
            GameManager.pendingPos.Add(posGM);

            /* pendingPos[currentPending][] */
            Instantiate(pendingGuy, Vector3.Scale(new Vector3(pos.x, pos.y, 0), GameManager.mapToWorldSpace) + new Vector3(-1.5f, -1.5f, 0.0f), Quaternion.identity, npcs.transform);
            //Debug.Log(GameManager.pendingPos.Count());
        }
    }
}
