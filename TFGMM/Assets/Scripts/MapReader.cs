using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MapReader : MonoBehaviour
{
    public GameObject grassPrefab;
    public GameObject waterPrefab;
    public GameObject obstaclePrefab;
    public GameObject bushPrefab;




    public string mapsDir = "Maps"; // Directorio por defecto
    public string mapName = "mapa1.map"; // Fichero por defecto

    int numCols;
    int numRows;
    GameObject[] vertexObjs;

    // Start is called before the first frame update
    void Awake()
    {
        LoadMap();
    }
    public void LoadMap()
    {
        string path = Application.dataPath + "/" + mapsDir + "/" + mapName;
            StreamReader strmRdr = new StreamReader(path);
            using (strmRdr)
            {
                int j = 0;
                int i = 0;
                int id = 0;
                string line;

                Vector3 position = Vector3.zero;
                Vector3 scale = Vector3.zero;
                line = strmRdr.ReadLine();// non-important line
                line = strmRdr.ReadLine();// height
                numRows = int.Parse(line.Split(' ')[1]);
                line = strmRdr.ReadLine();// width
                numCols = int.Parse(line.Split(' ')[1]);
                line = strmRdr.ReadLine();// "map" line in file

                vertexObjs = new GameObject[numRows * numCols];


                for (i = 0; i < numRows; i++)
                {
                    line = strmRdr.ReadLine();
                    for (j = 0; j < numCols; j++)
                    {
                        position.x = j*2;
                        position.z = i*2;
                        position.y = 1;


                        if (line[j] == 'C')
                        {
                            vertexObjs[id] = Instantiate(grassPrefab, position, Quaternion.identity) as GameObject;
                        }
                        else if (line[j] == 'S')
                        {
                            vertexObjs[id] = Instantiate(grassPrefab, position, Quaternion.identity) as GameObject;
                            vertexObjs[id].gameObject.tag = "Spawn";
                        }
                        else if (line[j] == 'T')
                        {
                            vertexObjs[id] = Instantiate(grassPrefab, position, Quaternion.identity) as GameObject;
                            vertexObjs[id] = Instantiate(obstaclePrefab, new Vector3(position.x, position.y +1, position.z), Quaternion.identity) as GameObject;
                        }
                        else if (line[j] == 'A')
                        {
                            vertexObjs[id] = Instantiate(waterPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity) as GameObject;
                            vertexObjs[id].transform.Rotate(new Vector3(-90.0f, 0f, 0f));
                        }
                        else if (line[j] == 'B')
                        {
                            vertexObjs[id] = Instantiate(bushPrefab, new Vector3(position.x, position.y + 1, position.z), Quaternion.identity) as GameObject;
                        }

                }
                }
            }
        }
    }
