using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSpawner : MonoBehaviour {

    public PieceType type;
    private Piece currentPiece;

    public void Spawn()
    {
        int amtOfObj = 0;

        switch(type)
        {
            case PieceType.jump:
                amtOfObj = LevelManager.Instance.jumps.Count;
                break;
            case PieceType.slide:
                amtOfObj = LevelManager.Instance.slides.Count;
                break;
            case PieceType.longblock:
                amtOfObj = LevelManager.Instance.longblocks.Count;
                break;
            case PieceType.ramp:
                amtOfObj = LevelManager.Instance.ramps.Count;
                break;
        }

        currentPiece = LevelManager.Instance.GetPiece(type, Random.Range(0, amtOfObj));
        currentPiece.gameObject.SetActive(true);
        currentPiece.transform.SetParent(transform, false);
    }

    public void Despawn()
    {
        currentPiece.gameObject.SetActive(false);
    }
}
