using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    private List<GameObject> team;
    private float resources;
    private DnDAcademy academy;
    private int teamNum;

    private void Start()
    {
        academy = FindObjectOfType<DnDAcademy>();
        if (this.team == null) { this.team = new List<GameObject>(); }
    }

    public void SetTeam(List<GameObject> team, int teamNum)
    {
        if (this.team == null) { this.team = new List<GameObject>(); }
        this.team.AddRange(team);
        this.teamNum = teamNum;

        foreach(GameObject member in team)
        {
            member.GetComponent<UnitAgent>().castle = this;
        }
    }

    public void Step()
    {
        foreach(GameObject unit in team)
        {
            UnitAgent agent = unit.GetComponent<UnitAgent>();

            switch (agent.GetUnitType())
            {
                case UnitAgent.Warrior:
                    if (agent.IsDead() && resources > 100)
                    {
                        agent.Done();
                        resources -= 100;
                    }
                    break;
                case UnitAgent.Worker:
                    if (agent.IsDead() && resources > 50)
                    {
                        agent.Done();
                        resources -= 50;
                    }
                    break;
            }
        }
        if (!academy.training)
        {
            if (team.Count < 6 && resources > 50)
            {
                GameObject newUnit = academy.BuildUnit(UnitAgent.Worker, teamNum);
                team.Add(newUnit);
                newUnit.GetComponent<UnitAgent>().castle = this;
                newUnit.GetComponent<UnitAgent>().ResetAgent();
                resources -= 50;
            }
            else if (resources > 100)
            {
                GameObject newUnit = academy.BuildUnit(UnitAgent.Warrior, teamNum);
                team.Add(newUnit);
                newUnit.GetComponent<UnitAgent>().castle = this;
                newUnit.GetComponent<UnitAgent>().ResetAgent();
                resources -= 100;
            }
        }
    }

    public void AddResources(float resources)
    {
        this.resources += resources;
    }
}
