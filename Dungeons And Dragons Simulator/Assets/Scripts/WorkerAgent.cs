using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerAgent : UnitAgent
{
    public float newReward;
    private float resources;

    public override void InitializeAgent()
    {
        unitType = Worker;
        sight = 3;
        base.InitializeAgent();
    }

    public override void CollectObservations()
    {
        base.CollectObservations();
        AddVectorObs(newReward);
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //Debug.Log("Taking action");

        int action = Mathf.FloorToInt(vectorAction[0]);

        Coordinates2D newPos = new Coordinates2D(x, y);

        switch (action)
        {
            case Up:
                newPos = Coordinates2D.add(newPos, new Coordinates2D(1, 0));
                break;
            case UpRight:
                newPos = Coordinates2D.add(newPos, new Coordinates2D(1, -1));
                break;
            case DownRight:
                newPos = Coordinates2D.add(newPos, new Coordinates2D(0, -1));
                break;
            case Down:
                newPos = Coordinates2D.add(newPos, new Coordinates2D(-1, 0));
                break;
            case DownLeft:
                newPos = Coordinates2D.add(newPos, new Coordinates2D(-1, 1));
                break;
            case UpLeft:
                newPos = Coordinates2D.add(newPos, new Coordinates2D(0, 1));
                break;
            case Interact:
                if (x == homeX && y == homeY)
                {
                    if (newReward > 0)
                    {
                        castle.AddResources(resources);
                        Debug.Log("Went home with resources");
                        newReward = 0;
                        resources = 0;
                        SetReward(1f);
                        Done();
                    }
                    else
                    {
                        Debug.Log("Did Nothing In Town");
                        AddReward(-0.02f);
                    }
                }
                else if (newReward == 0)
                {
                    resources = gameState.GetHex(newPos).GetComponent<Hex>().Gather();
                    if (resources == 0)
                    {
                        AddReward(-0.02f);
                        Debug.Log("Nothing to Harvest");
                    }
                    else
                    {
                        newReward = 1;
                        newPos = null;
                        Debug.Log("Harvested");
                    }
                }
                else
                {
                    Debug.Log("Did Nothing Out Of Town");
                    AddReward(-0.02f);
                }
                break;
        }

        if (newPos != null)
        {
            Move(newPos);
        }
    }

    public override void AgentReset()
    {
        if (academy.training)
        {
            ResetAgent();
        }
    }

    public override void ResetAgent()
    {
        resources = 0;
        newReward = 0;
        base.ResetAgent();
    }
}
