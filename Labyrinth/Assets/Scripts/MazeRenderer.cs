using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{


    private int width = 10;


    private int height = 10;

    private float size = 1f;

    private GameObject wallPrefab;

    private GameObject floorPrefab;

    private GameObject PlayerPrefab;

    private GameObject CameraPrefab;

    private GameObject EndPrefab;

    // Start is called before the first frame update
    void Start()
    {
        var maze = MazeGenerator.Generate(width, height);
        PlayerPrefab = Resources.Load("Prefabs/Player") as GameObject;
        CameraPrefab = Resources.Load("Prefabs/Cam") as GameObject;
        EndPrefab = Resources.Load("Prefabs/EndPoint") as GameObject;
        wallPrefab = Resources.Load("Prefabs/Wall") as GameObject;
        floorPrefab = Resources.Load("Prefabs/Floor") as GameObject;

        Draw(maze);
    }

    private void Draw(WallState[,] maze)
    {

        var floor = Instantiate(floorPrefab, transform);
        var player = Instantiate(PlayerPrefab, transform);
        player.transform.position = new Vector3(Random.Range(-4, +4), 1, Random.Range(-4, +4));
        var camera = Instantiate(CameraPrefab, transform);
        var end = Instantiate(EndPrefab, transform);
        end.transform.position = new Vector3(Random.Range(-4, +4), 1, Random.Range(-4, +4));
        floor.transform.localScale = new Vector3(width, 1, height);
        floor.transform.position = new Vector3(-10, 0, -10);

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                var cell = maze[i, j];
                var position = new Vector3(-width / 2 + i, 0, -height / 2 + j);

                if (cell.HasFlag(WallState.UP))
                {
                    var topWall = Instantiate(wallPrefab, transform) as GameObject;
                    topWall.transform.position = position + new Vector3(0, 0, size / 2);
                    topWall.transform.localScale = new Vector3(size, topWall.transform.localScale.y, topWall.transform.localScale.z);
                }

                if (cell.HasFlag(WallState.LEFT))
                {
                    var leftWall = Instantiate(wallPrefab, transform) as GameObject;
                    leftWall.transform.position = position + new Vector3(-size / 2, 0, 0);
                    leftWall.transform.localScale = new Vector3(size, leftWall.transform.localScale.y, leftWall.transform.localScale.z);
                    leftWall.transform.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == width - 1)
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        var rightWall = Instantiate(wallPrefab, transform) as GameObject;
                        rightWall.transform.position = position + new Vector3(+size / 2, 0, 0);
                        rightWall.transform.localScale = new Vector3(size, rightWall.transform.localScale.y, rightWall.transform.localScale.z);
                        rightWall.transform.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        var bottomWall = Instantiate(wallPrefab, transform) as GameObject;
                        bottomWall.transform.position = position + new Vector3(0, 0, -size / 2);
                        bottomWall.transform.localScale = new Vector3(size, bottomWall.transform.localScale.y, bottomWall.transform.localScale.z);
                    }
                }
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
