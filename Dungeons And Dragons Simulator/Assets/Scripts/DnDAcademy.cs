using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using UnityEngine.UI;

public class DnDAcademy : Academy
{
    private float timer;
    public float turnLength;
    public GameObject resetButton;

    private List<List<GameObject>> teams;
    private List<GameObject> castles;
    private GameState gameState;
    private GameController controller;
    public int maxRange;

    public bool training;
    public bool player;
    public GameObject WorkerPrefab;
    public Brain WorkerBrain;
    public GameObject WarriorPrefab;
    public Brain WarriorBrain;
    public GameObject CastlePrefab;
    public int numTeams;
    public Material[] Colors;

    private void Start()
    {
        Monitor.SetActive(true);

        if (!training)
        {
            teams = new List<List<GameObject>>();

            while (teams.Count < numTeams)
            {
                teams.Add(new List<GameObject>());
            }

            Agent[] agents = FindObjectsOfType(typeof(Agent)) as Agent[];
            for (int i = 0; i < agents.Length; i++)
            {
                UnitAgent unit = agents[i].GetComponent<UnitAgent>();

                teams[unit.team].Add(agents[i].gameObject);
            }

            castles = new List<GameObject>();

            for (int i = 0; i < teams.Count; i++)
            {
                Castle castle = Instantiate(CastlePrefab).GetComponent<Castle>();
                castle.SetTeam(teams[i], i);
                MeshRenderer mesh = castle.GetComponentInChildren<MeshRenderer>();
                Material[] mat = mesh.materials;
                mat[0] = Colors[i];
                mesh.materials = mat;

                castles.Add(castle.gameObject);
            }

            gameState = FindObjectOfType(typeof(GameState)) as GameState;
            controller = FindObjectOfType(typeof(GameController)) as GameController;
            maxRange = 20;

            resetButton = Instantiate(resetButton);
            resetButton.GetComponentInChildren<Button>().onClick.AddListener(ResetAcademy);

            ResetAcademy();
        }
    }

    public override void InitializeAcademy()
    {
        if (training)
        {
            teams = new List<List<GameObject>>();

            Agent[] agents = FindObjectsOfType(typeof(Agent)) as Agent[];
            for (int i = 0; i < agents.Length; i++)
            {
                UnitAgent unit = agents[i].GetComponent<UnitAgent>();
                while (teams.Count <= unit.team)
                {
                    teams.Add(new List<GameObject>());
                }

                teams[unit.team].Add(agents[i].gameObject);

            }
            gameState = FindObjectOfType(typeof(GameState)) as GameState;
            controller = FindObjectOfType(typeof(GameController)) as GameController;
            maxRange = 20;

            castles = new List<GameObject>();

            for (int i = 0; i < teams.Count; i++)
            {
                Castle castle = Instantiate(CastlePrefab).GetComponent<Castle>();
                castle.SetTeam(teams[i], i);
                MeshRenderer mesh = castle.GetComponentInChildren<MeshRenderer>();
                Material[] mat = mesh.materials;
                mat[0] = Colors[i];
                mesh.materials = mat;

                castles.Add(castle.gameObject);
            }
        }
    }

    private void Update()
    {
        if (!training && !player)
        {
            timer += Time.deltaTime;

            if (timer > turnLength)
            {
                Step();
                timer = 0;
            }
        }
        else if (player) 
        {
            if (Input.GetKeyDown("w") ||
                Input.GetKeyDown("e") ||
                Input.GetKeyDown("d") ||
                Input.GetKeyDown("s") ||
                Input.GetKeyDown("a") ||
                Input.GetKeyDown("q") ||
                Input.GetKeyDown("space"))
            {
                Step();
            }
        }
    }

    public void SetEnvironment()
    {
        gameState.EmptyHexes();

        controller.InitMap();

        for (int i = 0; i < teams.Count; i++)
        {
            List<GameObject> team = teams[i];

            Hex teamLoc;
            bool validLoc = false;

            int x;
            int y;

            int count = 0;

            do
            {
                count++;

                x = Random.Range(-maxRange, maxRange);
                y = Random.Range(-maxRange, maxRange);

                if (Mathf.Abs(x + y) == Mathf.Abs(x) + Mathf.Abs(y))
                {
                    y = y - x;
                }

                x += gameState.centerX;
                y += gameState.centerY;

                Coordinates2D coords = new Coordinates2D(x, y);

                teamLoc = gameState.GetHex(coords).GetComponent<Hex>();

                if (teamLoc.getType() != GameState.HexTypes.Mountain &&
                    teamLoc.getType() != GameState.HexTypes.Water &&
                    teamLoc.unitInHex == null)
                {
                    validLoc = true;
                }
            } while (!validLoc && count < 50);

            if (!validLoc)
            {
                Debug.Log("Failed to find valid tiles.");
                SetEnvironment();
                return;
            }

            foreach (GameObject member in team)
            {
                UnitAgent unit = member.GetComponent<UnitAgent>();
                unit.homeX = x;
                unit.homeY = y;
            }

            castles[i].transform.position = teamLoc.transform.position;
        }

        timer = -1;
    }

    public override void AcademyReset()
    {
        if (training)
        {
            Debug.Log("Resetting");
            SetEnvironment();
        }
    }

    public void ResetAcademy()
    {
        SetEnvironment();

        foreach (List<GameObject> team in teams)
        {
            foreach (GameObject agent in team)
            {
                agent.GetComponent<UnitAgent>().Dead();
                agent.GetComponent<UnitAgent>().ResetAgent();
            }
        }
    }

    public override void AcademyStep()
    {
        if (!training)
        {
            return;
        }
        Step();
    }

    private void Step()
    {
        int activeTeams = 0;

        List<UnitAgent> activeAgents = new List<UnitAgent>();

        foreach (List<GameObject> team in teams)
        {
            bool hasUnits = false;

            foreach (GameObject agent in team)
            {
                if (!agent.GetComponent<UnitAgent>().IsDead())
                {
                    activeAgents.Add(agent.GetComponent<UnitAgent>());
                    hasUnits = true;
                }

                Monitor.Log("Reward: ", agent.GetComponent<UnitAgent>().GetCumulativeReward(), agent.transform);
            }

            if (hasUnits)
            {
                activeTeams++;
            }
        }

        if (activeTeams < 2)
        {
            foreach (UnitAgent agent in activeAgents)
            {
                agent.SetReward(1f);
                agent.Dead();
            }

            SetEnvironment();

            foreach (List<GameObject> team in teams)
            {
                foreach (GameObject agent in team)
                {
                    agent.GetComponent<UnitAgent>().ResetAgent();
                }
            }

            return;
        }

        foreach (UnitAgent agent in activeAgents)
        {
            if (!agent.IsDead()) { agent.RequestDecision(); }
        }

        foreach (GameObject hex in gameState.GetHexList())
        {
            hex.GetComponent<Hex>().Step();
        }
    }

    private GameObject BuildUnit(GameObject UnitPrefab, Brain brain, int team)
    {
        GameObject unit = Instantiate(UnitPrefab, transform);
        unit.GetComponent<UnitAgent>().team = team;
        teams[team].Add(unit);
        unit.GetComponent<UnitAgent>().GiveBrain(brain);
        MeshRenderer mesh = unit.GetComponentInChildren<MeshRenderer>();
        Material[] mats = mesh.materials;
        mats[0] = Colors[team];
        mesh.materials = mats;

        return unit;
    }

    public GameObject BuildUnit(int type, int team)
    {
        switch(type)
        {
            case UnitAgent.Warrior:
                return BuildUnit(WarriorPrefab, WarriorBrain, team);
            case UnitAgent.Worker:
                return BuildUnit(WorkerPrefab, WorkerBrain, team);
        }

        throw new System.Exception("Build Unit Arguments Invalid");
    }
}
