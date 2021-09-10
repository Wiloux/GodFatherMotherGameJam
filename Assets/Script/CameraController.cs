using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class CameraController : MonoBehaviour {
    public Transform objectToFollow;

    public Transform start = null, end = null;
    [SerializeField] private float scrollTime = 10f, scrollTimeDecay = 0.1f;

    private float actualScrollTime = 0f;
    private Vector2 startPos = Vector2.zero, endPos = Vector2.zero;

    private CameraEngine engine = null;

    [HideInInspector] public bool move = false;

    private void Awake() {
        engine = GetComponent<CameraEngine>();
    }

    private void Start() {
        //StartMove();
    }

    private void Update() {
        if (move) {
            if (objectToFollow != null) {
                engine.SetPosition(objectToFollow.position.To2D());
                return;
            }

            if (start != null && end != null) {
                if (Vector2.Dot(endPos - startPos, engine.Position - endPos) >= 0) {
                    Vector2 temp = endPos; endPos = startPos; startPos = temp;
                    actualScrollTime = actualScrollTime * (1f - scrollTimeDecay);
                    engine.SetPositionIn(endPos, actualScrollTime);
                }
            }
        }
    }

    public void StartMove() {
        if (objectToFollow != null) {
            engine.SetPosition(objectToFollow.position.To2D());
            return;
        }

        if (start == null || end == null) { Debug.LogWarning("start or end not assigned"); return; }
        startPos = start.position.To2D(); endPos = end.position.To2D();
        engine.Position = startPos;
        actualScrollTime = scrollTime;
        engine.SetPositionIn(endPos, actualScrollTime);

        move = true;
    }

    public void StopMove() {
        engine.StopAllCoroutines();
        move = false;
    }
}
