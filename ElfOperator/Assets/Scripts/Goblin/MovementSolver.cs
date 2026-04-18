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
        var start = goblin.transform.position;
        var end = start + offset;
        
        var vy = Mathf.Sqrt(2f * gravityUp * maxHeight);
        var timeUp = vy / gravityUp;
        
        
        var apexY = start.y + maxHeight;
        var fallDistance = apexY - end.y;
        var timeDown = Mathf.Sqrt(2f * fallDistance / gravityDown);

        var totalTime = timeUp + timeDown;
        var vx = (end.x - start.x) / totalTime;

        var elapsed = 0f;
        
        while (elapsed < timeUp)
        {
            elapsed += Time.deltaTime;
            var t = elapsed;
            var x = start.x + vx * t;
            var y = start.y + vy * t - 0.5f * gravityUp * t * t;
            goblin.transform.position = new Vector2(x, y);
            yield return null;
        }

        var apexX = start.x + vx * timeUp;
        
        var fallElapsed = 0f;
        while (fallElapsed < timeDown)
        {
            fallElapsed += Time.deltaTime;
            var x = apexX + vx * fallElapsed;
            var y = apexY - 0.5f * gravityDown * fallElapsed * fallElapsed;
            goblin.transform.position = new Vector2(x, y);
            yield return null;
        }
        
        var checkBlockPos = goblin.transform.position + new Vector3(0, -0.3f, 0);
        var gridPosWithOffset = new Vector2Int((int)checkBlockPos.x, (int)checkBlockPos.y);
        while (!mapper.IsSolid(gridPosWithOffset))
        {
            fallElapsed += Time.deltaTime;
            var x = goblin.transform.position.x;
            var y = apexY - 0.5f * gravityDown * fallElapsed * fallElapsed;
            goblin.transform.position = new Vector2(x, y);
            
            checkBlockPos = goblin.transform.position + new Vector3(0, -0.3f, 0);
            gridPosWithOffset = new Vector2Int((int)checkBlockPos.x, (int)checkBlockPos.y);
            yield return null;
        }

        var endPos = new Vector2Int((int)Mathf.Floor(goblin.transform.position.x),
            (int)Mathf.Floor(goblin.transform.position.y));
        goblin.transform.position = endPos + new Vector2(0.5f, 0.5f);
        mapper.UpdateGoblinPosition(goblinIndex, endPos);
    }
}
