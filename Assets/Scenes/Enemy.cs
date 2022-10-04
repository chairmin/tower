using UnityEngine;

public class Enemy : GameBehavior
{
    EnemyFactory originFactory;

    public EnemyFactory OriginFactory
    {
        get => originFactory;
        set
        {
            Debug.Assert(originFactory == null, "Redefined origin factory!");
            originFactory = value;
        }
    }

    public void SpawnOn(GameTile tile)
    {
        //transform.localPosition = tile.transform.localPosition;
        Debug.Assert(tile.NextTileOnPath != null, "Nowhere to go!", this);
        tileFrom = tile;
        tileTo = tile.NextTileOnPath;
        //positionFrom = tileFrom.transform.localPosition;
        //positionTo = tileTo.transform.localPosition;
        //positionTo = tileFrom.ExitPoint;
        //transform.localRotation = tileFrom.PathDirection.GetRotation();
        progress = 0f;
        PrepareIntro();
    }

    public override bool GameUpdate()
    {
        if (Health <= 0f)
        {
            OriginFactory.Reclaim(this);
            return false;
        }

        progress += Time.deltaTime * progressFactor;
        while (progress >= 1f)
        {
            //tileFrom = tileTo;
            //tileTo = tileTo.NextTileOnPath;
            if (tileTo == null)
            {
                OriginFactory.Reclaim(this);
                return false;
            }
            //positionFrom = positionTo;
            //positionTo = tileTo.transform.localPosition;
            //positionTo = tileFrom.ExitPoint;
            //transform.localRotation = tileFrom.PathDirection.GetRotation();
            //progress -= 1f;
            progress = (progress - 1f) / progressFactor;
            PrepareNextState();
            progress *= progressFactor;
        }
        // 位置插值
        if (directionChange == DirectionChange.None)
        {
            transform.localPosition =
            Vector3.LerpUnclamped(positionFrom, positionTo, progress);
        }
        // 旋转方向插值
        //if (directionChange != DirectionChange.None)
        else
        {
            float angle = Mathf.LerpUnclamped(
                directionAngleFrom, directionAngleTo, progress
            );
            transform.localRotation = Quaternion.Euler(0f, angle, 0f);
        }
        return true;
    }

    GameTile tileFrom, tileTo;
    Vector3 positionFrom, positionTo;
    float progress;

    Direction direction;
    DirectionChange directionChange;
    float directionAngleFrom, directionAngleTo;

    void PrepareIntro()
    {
        positionFrom = tileFrom.transform.localPosition;
        positionTo = tileFrom.ExitPoint;
        direction = tileFrom.PathDirection;
        directionChange = DirectionChange.None;
        directionAngleFrom = directionAngleTo = direction.GetAngle();
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        //progressFactor = 2f;
        progressFactor = 2f * speed;
    }

    void PrepareNextState()
    {
        tileFrom = tileTo;
        tileTo = tileTo.NextTileOnPath;
        positionFrom = positionTo;
        if (tileTo == null)
        {
            PrepareOutro();
            return;
        }

        positionTo = tileFrom.ExitPoint;
        directionChange = direction.GetDirectionChangeTo(tileFrom.PathDirection);
        direction = tileFrom.PathDirection;
        directionAngleFrom = directionAngleTo;

        switch (directionChange)
        {
            case DirectionChange.None: PrepareForward(); break;
            case DirectionChange.TurnRight: PrepareTurnRight(); break;
            case DirectionChange.TurnLeft: PrepareTurnLeft(); break;
            default: PrepareTurnAround(); break;
        }
    }

    void PrepareForward()
    {
        transform.localRotation = direction.GetRotation();
        directionAngleTo = direction.GetAngle();
        model.localPosition = Vector3.zero;
        model.localPosition = new Vector3(pathOffset, 0f);
        //progressFactor = 1f;
        progressFactor = speed;
    }

    void PrepareTurnRight()
    {
        directionAngleTo = directionAngleFrom + 90f;
        //model.localPosition = new Vector3(-0.5f, 0f);
        model.localPosition = new Vector3(pathOffset - 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        //progressFactor = 1f / (Mathf.PI * 0.25f);
        //progressFactor = 1f / (Mathf.PI * 0.5f * (0.5f - pathOffset));
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f + pathOffset));
    }

    void PrepareTurnLeft()
    {
        directionAngleTo = directionAngleFrom - 90f;
        //model.localPosition = new Vector3(0.5f, 0f);
        model.localPosition = new Vector3(pathOffset + 0.5f, 0f);
        transform.localPosition = positionFrom + direction.GetHalfVector();
        //progressFactor = 1f / (Mathf.PI * 0.25f);
        //progressFactor = 1f / (Mathf.PI * 0.5f * (0.5f + pathOffset));
        progressFactor = speed / (Mathf.PI * 0.5f * (0.5f - pathOffset));
    }

    void PrepareTurnAround()
    {
        //directionAngleTo = directionAngleFrom + 180f;
        directionAngleTo = directionAngleFrom + (pathOffset < 0f ? 180f : -180f);
        //model.localPosition = Vector3.zero;
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localPosition = positionFrom;
        //progressFactor = 2f;
        //progressFactor =
        //    1f / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
        progressFactor =
            speed / (Mathf.PI * Mathf.Max(Mathf.Abs(pathOffset), 0.2f));
    }

    [SerializeField]
    Transform model = default;

    float progressFactor;

    void PrepareOutro()
    {
        positionTo = tileFrom.transform.localPosition;
        directionChange = DirectionChange.None;
        directionAngleTo = direction.GetAngle();
        //model.localPosition = Vector3.zero;
        model.localPosition = new Vector3(pathOffset, 0f);
        transform.localRotation = direction.GetRotation();
        //progressFactor = 2f;
        progressFactor = 2f * speed;
    }

    float pathOffset;
    public void Initialize(float scale, float speed, float pathOffset)
    {
        Scale = scale;
        model.localScale = new Vector3(scale, scale, scale);
        this.speed = speed;
        this.pathOffset = pathOffset;

        Health = 100f * scale;
    }

    float speed;

    public float Scale { get; private set; }

    float Health { get; set; }
    public void ApplyDamage(float damage)
    {
        Debug.Assert(damage >= 0f, "Negative damage applied.");
        Health -= damage;
    }
}