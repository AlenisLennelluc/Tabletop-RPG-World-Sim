using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAgent : UnitAgent
{
    public override void InitializeAgent()
    {
        attack = 1;
        sight = 2;
        unitType = Warrior;
        base.InitializeAgent();
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
                    Debug.Log("Did Nothing");
                    AddReward(-0.2f);
                break;
        }

        if (newPos != null)
        {
            Move(newPos);
        }
    }
}
