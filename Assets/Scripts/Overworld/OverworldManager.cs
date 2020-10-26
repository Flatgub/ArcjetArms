using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OverworldManager : MonoBehaviour
{
    public OverworldNode nodePrefab;
    public OverworldLink linkPrefab;
    
    private Transform linksContainer = null;
    private Transform nodesContainer = null;

    public int NumberOfLevels = 3;
    public int MinNodesPerLevel = 1;
    public int MaxNodesPerLevel = 3;

    public float distanceBetweenLevels = 2;
    public float heightOfLevels = 3;
    public float paddingWithinLevel = 1.5f;

    private List<List<OverworldNode>> allLevels;
    private Vector3 rootPosition;

    public OverworldNode playerAt;

    private string StashedMap;

    // Start is called before the first frame update
    void Start()
    {
        GameplayContext.InDebugMode = false;

        heightOfLevels = MaxNodesPerLevel * paddingWithinLevel;

        ClearLevel();

        if (GameplayContext.OverworldMap == null)
        {
            GenerateMap();
            SetPlayerAt(allLevels[0][0]);
            GameplayContext.OverworldMap = MapToAbstract();
        }
        else
        {
            AbstractToMap(GameplayContext.OverworldMap);
        }

    }


    private void LinkNodes(OverworldNode from, OverworldNode to)
    {
        OverworldLink link = Instantiate(linkPrefab, linksContainer);
        link.Link(from, to);
    }

    private void LinkLevels(List<OverworldNode> from, List<OverworldNode> to)
    {
        int leftIndex = 0;
        int rightIndex = 0;

        while (leftIndex < from.Count && rightIndex < to.Count)
        {
            OverworldNode a = from[leftIndex];
            OverworldNode b = to[rightIndex];
            LinkNodes(a, b);

            if (leftIndex == from.Count-1 || a.outwardLinks.Count == 0)
            {
                rightIndex++;
            }
            else if (rightIndex == to.Count-1 || b.inwardLinks.Count == 0)
            {
                leftIndex++;
            }
            else
            {
                switch (UnityEngine.Random.Range(0, 3))
                {
                    case 0: leftIndex++ ;break;
                    case 1: rightIndex++; break;
                    case 2: leftIndex++; rightIndex++; break;

                }
            }
        }

        
    }

    private List<OverworldNode> CreateLevel(int num, Vector3 centerpos)
    {
        Vector3 pos = new Vector3(centerpos.x, centerpos.y, centerpos.z);

        float distBetween = heightOfLevels / (num + 1.0f);
        float totalDistance = distBetween * num;
        float currentY = pos.y - (heightOfLevels / 2) + distBetween;
        pos.y = currentY;

        List<OverworldNode> level = new List<OverworldNode>();
        

        for (int o = 0; o < num; o++)
        {
            OverworldNode n = Instantiate(nodePrefab, pos, Quaternion.identity, nodesContainer);
            n.onClicked += ClickNode;
            level.Add(n);
            pos.y += distBetween;
        }

        return level;
    }

    private void GenerateMap()
    {
        rootPosition = new Vector3((NumberOfLevels-1) * distanceBetweenLevels * -0.5f, 0, 0);
        Vector3 pos = rootPosition;

        //spawn all levels
        for (int i = 0; i < NumberOfLevels; i++)
        {

            int nodes = UnityEngine.Random.Range(MinNodesPerLevel, MaxNodesPerLevel + 1);
            if (i == 0 || i == NumberOfLevels - 1)
            {
                nodes = 1;
            }

            List<OverworldNode> level = CreateLevel(nodes, pos);

            pos.x += distanceBetweenLevels;
            allLevels.Add(level);
        }

        //link all levels
        for (int i = 0; i < NumberOfLevels - 1; i++)
        {
            List<OverworldNode> aNodes = allLevels[i];
            List<OverworldNode> bNodes = allLevels[i + 1];

            LinkLevels(aNodes, bNodes);

        }

        //cull pass
        /*
        foreach (List<OverworldNode> level in allLevels)
        {
            foreach (OverworldNode node in level)
            {
                if (node.inwardLinks.Count == 0)
                {
                    //node.appearance.color = Color.gray;
                }
            }
        }*/
    }

    private void ClearLevel()
    {
        if (linksContainer != null)
        {
            Destroy(linksContainer.gameObject);
        }
        if (nodesContainer != null)
        {
            Destroy(nodesContainer.gameObject);
        }

        linksContainer = new GameObject("Links").transform;
        linksContainer.SetParent(transform);
        nodesContainer = new GameObject("Nodes").transform;
        nodesContainer.SetParent(transform);

        allLevels = new List<List<OverworldNode>>();
    }

    private void SetPlayerAt(OverworldNode node)
    {
        foreach (List<OverworldNode> level in allLevels)
        {
            foreach (OverworldNode n in level)
            {
                n.appearance.color = Color.white;
            }
        }

        playerAt = node;
        node.appearance.color = Color.green;
        foreach (OverworldNode n in node.outwardLinks)
        {
            n.appearance.color = Color.blue;
        }
    }

    private void ClickNode(OverworldNode node)
    {
        if (playerAt.outwardLinks.Contains(node))
        {
            SetPlayerAt(node);
            GoToEncounter();
        }
    }

    public void GoToEncounter()
    {
        GameplayContext.OverworldMap = MapToAbstract();
        SceneManager.LoadScene("CombatEncounter");
    }

    public void GoToLoadoutScreen()
    {
        GameplayContext.OverworldMap = MapToAbstract();
        SceneManager.LoadScene("InventoryMenu");
    }

    private Tuple<int, int> FindPlayerAt()
    {
        if (playerAt == null)
        {
            return null;
        }

        for (int lvlID = 0; lvlID < NumberOfLevels; lvlID++)
        {
            List<OverworldNode> lvl = allLevels[lvlID];
            for (int nodeID = 0; nodeID < lvl.Count; nodeID++)
            {
                OverworldNode node = lvl[nodeID];
                if (node == playerAt)
                {
                    return new Tuple<int, int>(lvlID, nodeID);
                }
            }
        }

        return null;
    }

    [Serializable]
    struct AbstractNode
    {
        public List<int> links;
    }

    [Serializable]
    struct AbstractLevel
    {
        public int numNodes;
        public List<AbstractNode> nodes;
    }

    [Serializable]
    struct AbstractMap
    {
        public int numLevels;
        public List<AbstractLevel> levels;
        public int playerLevel;
        public int playerNode;
    }

    private AbstractLevel LevelToAbstract(List<OverworldNode> level, List<OverworldNode> next)
    {
        AbstractLevel lvl = new AbstractLevel();
        lvl.numNodes = level.Count;
        lvl.nodes = new List<AbstractNode>();

        if (next != null)
        {
            foreach (OverworldNode node in level)
            {
                AbstractNode absnode = new AbstractNode();
                absnode.links = new List<int>();

                foreach (OverworldNode other in node.outwardLinks)
                {
                    int otherindex = next.FindIndex((a) => { return a == other; });
                    absnode.links.Add(otherindex);
                }

                lvl.nodes.Add(absnode);
            }
        }

        return lvl;
    }

    private string MapToAbstract()
    {

        List<AbstractLevel> levels = new List<AbstractLevel>();

        for (int levelIndex = 0; levelIndex < allLevels.Count - 1; levelIndex++)
        {
            List<OverworldNode> thisLevel = allLevels[levelIndex];
            List<OverworldNode> nextLevel = allLevels[levelIndex + 1];

            AbstractLevel lvl = LevelToAbstract(thisLevel, nextLevel);
            lvl.numNodes = thisLevel.Count;
            levels.Add(lvl);           
        }

        AbstractLevel finallvl = LevelToAbstract(allLevels[allLevels.Count-1], null);
        finallvl.numNodes = 1;
        levels.Add(finallvl);

        AbstractMap map = new AbstractMap();
        map.numLevels = allLevels.Count;
        map.levels = levels;
        Tuple<int, int> playerPos = FindPlayerAt();
        if (playerPos != null)
        {
            map.playerLevel = playerPos.Item1;
            map.playerNode = playerPos.Item2;
        }

        return JsonUtility.ToJson(map);

    }

    private void AbstractToMap(string abstractMap)
    {
        ClearLevel();
        AbstractMap absMap = JsonUtility.FromJson<AbstractMap>(abstractMap);

        rootPosition = new Vector3((NumberOfLevels - 1) * distanceBetweenLevels * -0.5f, 0, 0);
        Vector3 pos = rootPosition;

        //generate all levels
        NumberOfLevels = absMap.numLevels;
        foreach(AbstractLevel absLevel in absMap.levels)
        {
            List<OverworldNode> level = CreateLevel(absLevel.numNodes, pos);

            pos.x += distanceBetweenLevels;
            allLevels.Add(level);
        }

        //link all nodes
        for (int i = 0; i < NumberOfLevels-1; i++)
        {
            AbstractLevel absLevel = absMap.levels[i];
            List<OverworldNode> leftLevel = allLevels[i];
            List<OverworldNode> rightLevel = allLevels[i + 1];

            for (int nodeId = 0; nodeId < absLevel.numNodes; nodeId++)
            {
                AbstractNode absNode = absLevel.nodes[nodeId];
                OverworldNode leftNode = leftLevel[nodeId];
                foreach (int link in absNode.links)
                {
                    OverworldNode rightNode = rightLevel[link];
                    LinkNodes(leftNode, rightNode);
                }
            }
        }

        SetPlayerAt(allLevels[absMap.playerLevel][absMap.playerNode]);

    }

    // Update is called once per frame
    void Update()
    {

    }

    
}


