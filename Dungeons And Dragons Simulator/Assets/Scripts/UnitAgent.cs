using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public abstract class UnitAgent : Agent
{
    protected DnDAcademy academy;
    protected GameState gameState;
    protected GameController controller;

    protected int x;
    protected int y;
    protected int sight;

    public int team;
    protected int attack;

    public int homeX;
    public int homeY;

    public Castle castle;

    protected bool dead;

    protected int unitType;

    public const int Worker = 0;
    public const int Warrior = 1;

    protected const int Up = 0;
    protected const int UpRight = 1;
    protected const int DownRight = 2;
    protected const int Down = 3;
    protected const int DownLeft = 4;
    protected const int UpLeft = 5;
    protected const int Interact = 6;

    private const int NoUnit = 0;
    private const int Friendly = 1;
    private const int Enemy = 2;

    public override void InitializeAgent()
    {
        academy = FindObjectOfType(typeof(DnDAcademy)) as DnDAcademy;
        gameState = FindObjectOfType(typeof(GameState)) as GameState;
        controller = FindObjectOfType(typeof(GameController)) as GameController;
 
        x = 0;
        y = 0;

        if (sight == 0)
        {
            sight = 2;
        }

        dead = false;
    }

    public override void CollectObservations()
    {
        List<GameObject> neighbors = gameState.getNeighbors(x, y, sight);
        foreach(GameObject neighbor in neighbors)
        {
            GameState.HexTypes type = neighbor.GetComponent<Hex>().getType();
            AddVectorObs((int)type, 9);

            GameObject other = neighbor.GetComponent<Hex>().unitInHex;

            if (other == null)
            {
                AddVectorObs(NoUnit, 3);
            }
            else if (other.GetComponent<UnitAgent>().team == team)
            {
                AddVectorObs(Friendly, 3);
            }
            else
            {
                AddVectorObs(Enemy, 3);
            }
        }

        Coordinates3D coordDist = Coordinates3D.AddCoordinates(
            new Coordinates3D(homeX, homeY), new Coordinates3D(-x, -y));
        AddVectorObs(Sigmoid(coordDist.x));
        AddVectorObs(Sigmoid(coordDist.y));
        AddVectorObs(Sigmoid(coordDist.z));
        AddVectorObs((Sigmoid(attack) - 0.5f) * 2);
    }

    public override void AgentReset()
    {
        ResetAgent();
    }

    public virtual void ResetAgent()
    {
        if (dead)
        {
            dead = false;

            Move(gameState.GetHex(new Coordinates2D(homeX, homeY)).GetComponent<Hex>());

            Debug.Log("Unit Reset");
        }
    }

    public void Move(Hex newHex)
    {
        if (newHex.unitInHex == null)
        {
            transform.position = newHex.transform.position;
            x = newHex.GetX();
            y = newHex.GetY();
            newHex.unitInHex = gameObject;
        }
        else
        {
            List<GameObject> neighbors = gameState.getNeighbors(newHex.GetX(), newHex.GetY(), 3);
            neighbors.Reverse();
            bool foundHex = false; 

            foreach(GameObject neighbor in neighbors)
            {
                Hex hexNe = neighbor.GetComponent<Hex>();
                if (hexNe.getType() != GameState.HexTypes.Mountain &&
                    hexNe.getType() != GameState.HexTypes.Water &&
                    hexNe.unitInHex == null)
                {
                    hexNe.unitInHex = gameObject;
                    transform.position = hexNe.transform.position;
                    x = hexNe.GetX();
                    y = hexNe.GetY();
                    foundHex = true;
                    break;
                }
            }
            
            if (!foundHex)
            {
                Dead();
            }
        }

        homeX = newHex.GetX();
        homeY = newHex.GetY();
    }

    protected void Move(Coordinates2D newPos)
    {
        Hex hex = gameState.GetHex(newPos).GetComponent<Hex>();

        if (hex.unitInHex != null)
        {
            if (!Fight(this, hex.unitInHex.GetComponent<UnitAgent>()))
            {
                return;
            }
        }

        GameState.HexTypes type = hex.getType();

        Coordinates3D coordDist = Coordinates3D.AddCoordinates(
            new Coordinates3D(homeX, homeY), new Coordinates3D(-x, -y));

        float dist = Mathf.Abs(coordDist.x) + Mathf.Abs(coordDist.y) + Mathf.Abs(coordDist.z);
        dist = dist / 2;
        dist = dist * .05f;
        dist = dist / (dist + 1);
        AddReward(Mathf.Min(-dist, 0));
        //Debug.Log(-dist);

        AddReward(-0.02f);

        switch (type)
        {
            case GameState.HexTypes.Mountain:
                AddReward(-0.02f);
                break;
            case GameState.HexTypes.Water:
                SetReward(-1f);
                Dead();
                Done();
                Debug.Log("In Water!");
                break;
            default:
                transform.position = hex.transform.position;
                gameState.GetHex(new Coordinates2D(x, y)).GetComponent<Hex>().unitInHex = null;
                x = newPos.x;
                y = newPos.y;
                hex.unitInHex = gameObject;
                break;
        }
    }

    static public float Sigmoid(float x)
    {
        const float e = 2.718281828459045235360287471352662497757247093699959574966f;

        float result = 1 / (1 + Mathf.Pow(e, -x));

        return result;
    }

    static protected bool Fight (UnitAgent attacker, UnitAgent defender)
    {
        if (attacker.team == defender.team)
        {
            attacker.AddReward(-0.05f);
            return false;
        }
        else if (attacker.attack >= defender.attack && attacker.attack > 0)
        {
            defender.SetReward(-.1f);
            defender.Dead();
            defender.Done();
            attacker.SetReward(1f);
            attacker.Done();
            Debug.Log("Attacker Beat Defender");
            return true;
        }
        else
        {
            attacker.SetReward(-.1f);
            attacker.Dead();
            attacker.Done();
            Debug.Log("Defender Beat Attacker");
            return false;
        }
    }

    public void Dead()
    {
        dead = true;
        gameState.GetHex(new Coordinates2D(x, y)).GetComponent<Hex>().unitInHex = null;

        transform.position += Vector3.down * 3;
    }

    public bool IsDead()
    {
        return dead;
    }

    public int GetUnitType()
    {
        return unitType;
    }
}