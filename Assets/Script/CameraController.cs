using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private Transform start = null, end = null;
    [SerializeField] private float scrollTime = 10f, scrollTimeDecay = 0.1f;

    private float actualScrollTime = 0f;
    private Vector2 startPos = Vector2.zero, endPos = Vector2.zero;

    private CameraEngine engine = null;

    private void Awake() {
        engine = GetComponent<CameraEngine>();
    }

    private void Start() {
        startPos = start.position.To2D(); endPos = end.position.To2D();
        engine.Position = startPos;
        actualScrollTime = scrollTime;
        engine.SetPositionIn(endPos, actualScrollTime);
    }

    private void Update() {
        if (Vector2.Dot(endPos - startPos, engine.Position - endPos) >= 0) {
            Vector2 temp = endPos; endPos = startPos; startPos = temp;
            actualScrollTime = scrollTime * (1f - scrollTimeDecay);
            engine.SetPositionIn(endPos, actualScrollTime);
        }
    }
}
