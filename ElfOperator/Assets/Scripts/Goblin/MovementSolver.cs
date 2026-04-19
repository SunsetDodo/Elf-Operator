using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovementSolver : MonoBehaviour
{

    [SerializeField] private float gravityUp;
    [SerializeField] private float gravityDown;
    [SerializeField] private float animationSpeed;
    [SerializeField] private Mapper mapper;

    [SerializeField] private List<GameObject> goblinProcessOrder;
    private Dictionary<Vector2Int, GameObject> _goblins;

    private Coroutine _moveCoroutine;
    
    private void Start()
    {
        _goblins = new Dictionary<Vector2Int, GameObject>();
        var startingPositions = new List<Vector2Int>();
        foreach (var goblinProcess in goblinProcessOrder)
        {
            var startPos =  new Vector2Int((int)goblinProcess.transform.position.x, (int)goblinProcess.transform.position.y);
            _goblins[startPos] =  goblinProcess;
            startingPositions.Add(startPos);
        }
        
        mapper.Setup(startingPositions);
    }
    
    private List<GameObject> FindGoblinChainLeft(GameObject startingGoblin)
    {
        
        var goblinChain = new List<GameObject> { startingGoblin };
        var startPos =  new Vector2Int((int)startingGoblin.transform.position.x, (int)startingGoblin.transform.position.y);
        var offset = new Vector2Int(1, 0);
        while (_goblins.ContainsKey(startPos + offset))
        {
            goblinChain.Add(_goblins[startPos + offset]);
        }
        
        return goblinChain;
    }
    
    private List<GameObject> FindGoblinChainRight(GameObject startingGoblin)
    {
        
        var goblinChain = new List<GameObject> { startingGoblin };
        var startPos =  new Vector2Int((int)startingGoblin.transform.position.x, (int)startingGoblin.transform.position.y);
        var offset = new Vector2Int(-1, 0);
        while (_goblins.ContainsKey(startPos + offset))
        {
            goblinChain.Add(_goblins[startPos + offset]);
        }
        
        return goblinChain;
    }
    
    private List<GameObject> FindGoblinChainUp(GameObject startingGoblin)
    {
        
        var goblinChain = new List<GameObject> { startingGoblin };
        var startPos =  new Vector2Int((int)startingGoblin.transform.position.x, (int)startingGoblin.transform.position.y);
        var offset = new Vector2Int(0, 1);
        while (_goblins.ContainsKey(startPos + offset))
        {
            goblinChain.Add(_goblins[startPos + offset]);
        }
        
        return goblinChain;
    }
    
    public void ProcessWalkLeft()
    {
        foreach (var goblin in goblinProcessOrder)
        {
            var chain = FindGoblinChainLeft(goblin);
            
        }
    }

    public Vector2 GetJumpRightOffset(GameObject goblin)
    {
        var offset =  new Vector3(2, 0, 0);
        var goblinPos = new Vector2Int((int)goblin.transform.position.x, (int)goblin.transform.position.y);

        if (mapper.IsSolid(goblinPos + new Vector2Int(0, 1)))
        {
            offset = Vector2.zero;
        }
            
        if (mapper.IsSolid(goblinPos + new Vector2Int(1, 1)))
        {
            offset = new Vector2(0, 1);
        }
        else if (mapper.IsSolid(goblinPos + new Vector2Int(1, 0)) ||
                 mapper.IsSolid(goblinPos + new Vector2Int(2, 1)))
        {
            offset = new Vector2(1, 1);
        } 
        else if (mapper.IsSolid(goblinPos + new Vector2Int(2, 0)))
        {
            offset = new Vector2(2, 1);
        }

        return offset;
    }

    public void ProcessJumpRight()
    {
        for (var i = 0; i < goblinProcessOrder.Count; i++)
        {
            var offset = GetJumpRightOffset(goblinProcessOrder[i]);
            if (offset == Vector2.zero)
            {
                _moveCoroutine = StartCoroutine(Wobble(.5f, .3f, 1));
            }
            _moveCoroutine = StartCoroutine(JumpTo(i, offset, 1.5f));
        }
    }
    
    private List<Vector2> SimulateJump(Vector2 start, Vector2 end, float maxHeight, int goblinIndex, float fixedStep = 1f / 60f) 
    {
        Vector2 actualEnd;

        // DONT ASK ME WHY IT NEEDS SPECIAL TREATMENT IDK!!!!! BUT IT DOESNT WORK WITHOUT IT
        if (Mathf.Approximately(start.x, end.x))
        {
            actualEnd = new Vector2(start.x, start.y);
        }
        else
        {
            var tx = Mathf.RoundToInt(end.x);
            var ty = Mathf.FloorToInt(end.y - 0.01f); 
            var safetyLimit = 100;

            while (safetyLimit > 0 && !mapper.IsSolidSkipGoblin(new Vector2Int(tx, ty), goblinIndex))
            {
                ty--;
                safetyLimit--;
            }
            actualEnd = new Vector2(end.x, ty + 1);
        }

        var vy = Mathf.Sqrt(2f * gravityUp * maxHeight);
        var timeUp = vy / gravityUp;
        var apexY = start.y + maxHeight;
        var fallDistance = Mathf.Max(0f, apexY - actualEnd.y);
        var timeDown = Mathf.Sqrt(2f * fallDistance / gravityDown);
        var vx = (actualEnd.x - start.x) / (timeUp + timeDown);

        var frames = new List<Vector2>();
        var pos = start;
        var velocity = new Vector2(vx, vy);

        for (var i = 0; i < 1000; i++)
        {
            if (velocity.y <= 0 && pos.y <= actualEnd.y + 0.001f)
            {
                frames.Add(actualEnd);
                break;
            }

            frames.Add(pos);

            if (velocity.x != 0f && Mathf.Abs(pos.x - actualEnd.x) < Mathf.Abs(velocity.x * fixedStep))
            {
                pos.x = actualEnd.x;
                velocity.x = 0f;
            }
            
            float currentGravity = (velocity.y > 0) ? gravityUp : gravityDown;
            velocity.y -= currentGravity * fixedStep;
            pos += velocity * fixedStep;
        }

        //DebugDrawTrajectory(frames);
        return frames;
    }

    private void DebugDrawTrajectory(List<Vector2> frames)
    {
        if (frames.Count < 2) return;
        for (var i = 1; i < frames.Count; i++)
            Debug.DrawLine(frames[i - 1], frames[i], Color.yellow, 5f);

        var apex = frames[0];
        foreach (var f in frames) if (f.y > apex.y) apex = f;
        Debug.Log($"[Jump] start={frames[0]} apex={apex} land={frames[^1]} frames={frames.Count}");
    }
    
    private IEnumerator Wobble(float duration, float strength, float speed)
    {
        var originalPos = transform.localPosition;
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            var offset = Mathf.Sin(elapsed * speed) * strength * (1f - elapsed / duration);
            transform.localPosition = originalPos + new Vector3(offset, 0f, 0f);
            yield return null;
        }

        transform.localPosition = originalPos;
    }
    
    private IEnumerator JumpTo(int goblinIndex, Vector3 offset, float maxHeight)
    {
        var goblin = goblinProcessOrder[goblinIndex];
        var start = (Vector2)goblin.transform.position;
        var end = start + (Vector2)offset;

        var fixedStep = 1f / 60f;
        var frames = SimulateJump(start, end, maxHeight, goblinIndex, fixedStep);

        var elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime;
            var frameIndex = Mathf.Min((int)(elapsed / fixedStep), frames.Count - 1);
            goblin.transform.position = frames[frameIndex];

            if (frameIndex >= frames.Count - 1) break;
            yield return null;
        }

        var lastPos = frames[^1];
        var endPos = new Vector2Int((int)Mathf.Round(lastPos.x), (int)Mathf.Round(lastPos.y));
        goblin.transform.position = (Vector2)endPos;
        mapper.UpdateGoblinPosition(goblinIndex, endPos);
    }
}
