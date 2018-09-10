using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {

    public bool SHOW_COLLIDERS = true; // $$

    public static LevelManager Instance { set; get; }
    
    // Level spawning
    private const float DISTANCE_BEFORE_SPAWN = 100.0f;
    private const int INITIAL_SEGMENTS = 10;
    private const int INITIAL_TRANSITION_SEGMENTS = 2;
    private const int MAX_SEGMENTS_ON_SCREEN = 15;
    public Transform cameraContainer;
    private int amountOfActiveSegments;
    private int continiousSegments;
    private int currentSpawnZ;
    private int currentLevel;
    private int y1, y2, y3;

    // List of Pieces
    public List<Piece> ramps = new List<Piece>();
    public List<Piece> longblocks = new List<Piece>();
    public List<Piece> jumps = new List<Piece>();
    public List<Piece> slides = new List<Piece>();

    [HideInInspector]
    public List<Piece> pieces = new List<Piece>(); // All the pieces in the pool

    // List of Segments
    public List<Segment> avialableSegments = new List<Segment>();
    public List<Segment> avialableTransitions = new List<Segment>();

    [HideInInspector]
    public List<Segment> segments = new List<Segment>();

    // Gameplay
    private bool isMoving = false;

    private void Awake()
    {
        Instance = this;
        //cameraContainer = Camera.current.transform;
        currentSpawnZ = 0;
        currentLevel = 0;
    }

    private void Start()
    {
        for (int i = 0; i < INITIAL_SEGMENTS; i++)
        {
            if (i < INITIAL_TRANSITION_SEGMENTS)
                SpawnTransition();
            else
                GenerateSegment();
        }
    }

    private void Update()
    {
        if (currentSpawnZ - cameraContainer.position.z < DISTANCE_BEFORE_SPAWN)
            GenerateSegment();

        if (amountOfActiveSegments >= MAX_SEGMENTS_ON_SCREEN)
        {
            segments[amountOfActiveSegments - 1].Despawn();
            amountOfActiveSegments--;
        }
    }

    private void GenerateSegment()
    {
        SpawnSegment();

        // Chance to spawn a segment to continue the last one. Up to 5 chances.
        if (Random.Range(0f, 1f) < (continiousSegments * 0.25f))
        {
            // Spawn transition segment
            // Reset the continiousSegments
            continiousSegments = 0;
            SpawnTransition();
        }
        else
        {
            continiousSegments++;
        }
    }

    private void SpawnSegment()
    {
        List<Segment> possibleSeg = avialableSegments.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleSeg.Count);

        Segment s = GetSegment(id, false);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.lenght;
        amountOfActiveSegments++;
        s.Spawn();
    }

    private void SpawnTransition()
    {
        List<Segment> possibleTransition = avialableTransitions.FindAll(x => x.beginY1 == y1 || x.beginY2 == y2 || x.beginY3 == y3);
        int id = Random.Range(0, possibleTransition.Count);

        Segment s = GetSegment(id, true);

        y1 = s.endY1;
        y2 = s.endY2;
        y3 = s.endY3;

        s.transform.SetParent(transform);
        s.transform.localPosition = Vector3.forward * currentSpawnZ;

        currentSpawnZ += s.lenght;
        amountOfActiveSegments++;
        s.Spawn();
    }

    public Segment GetSegment(int id, bool transition)
    {
        Segment s = null;
        s = segments.Find(x => x.SegId == id && x.transition == transition && !x.gameObject.activeSelf);

        if (s == null)
        {
            // Either get the segment from AviableTransitions or AviableSegments depending on bool transition
            GameObject go = Instantiate((transition) ? avialableTransitions[id].gameObject : avialableSegments[id].gameObject) as GameObject;
            s = go.GetComponent<Segment>();

            s.SegId = id;
            s.transition = transition;

            segments.Insert(0, s);
        }
        else
        {
            segments.Remove(s);
            segments.Insert(0, s);
        }

        return s;
    }

    public Piece GetPiece(PieceType pt, int visualIndex)
    {
        Piece p = pieces.Find(x => x.type == pt && x.visualIndex == visualIndex && !x.gameObject.activeSelf);

        if (p == null)
        {
            GameObject go = null;

            if (pt == PieceType.ramp)
            {
                go = ramps[visualIndex].gameObject;
            }
            else if (pt == PieceType.longblock)
            {
                go = longblocks[visualIndex].gameObject;
            }
            else if (pt == PieceType.jump)
            {
                go = jumps[visualIndex].gameObject;
            }
            else if (pt == PieceType.slide)
            {
                go = slides[visualIndex].gameObject;
            }

            go = Instantiate(go);
            p = go.GetComponent<Piece>();
            pieces.Add(p);
        }

        return p;
    }
}
