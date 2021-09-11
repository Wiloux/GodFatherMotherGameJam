using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ToolsBoxEngine;

public class CameraEngine : MonoBehaviour {

    #region Coroutines variables
    private Coroutine setPositionRoutine;
    private Coroutine shakeRoutine;
    #endregion

    #region Properties
    public Vector2 Position { get { return transform.position; } set { SetPosition(value.To3D(transform.position.z)); } }
    #endregion

    void Start() {

    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    public void SetPosition(Vector2 position) {
        SetPosition(position.To3D(transform.position.z));
    }

    public void SetRotation(Quaternion rotation) {
        transform.rotation = rotation;
    }

    public void SetPositionIn(Vector3 position, float time, bool absolute = true) {
        if (absolute && setPositionRoutine != null) { StopCoroutine(setPositionRoutine); }
        setPositionRoutine = StartCoroutine(ISetPositionIn(position, time));
    }

    public void SetPositionIn(Vector2 position, float time, bool absolute = true) {
        SetPositionIn(position.To3D(transform.position.z), time, absolute);
    }

    public void Shake(Vector2 delta, float time, float shakeTime = 0f) {
        if (shakeTime == 0f) { shakeTime = delta.magnitude / 10f; }

        if (shakeRoutine != null) { StopCoroutine(shakeRoutine); }
        shakeRoutine = StartCoroutine(IShake(delta, time, shakeTime));
    }

    #region Couroutines

    private IEnumerator ISetPositionIn(Vector3 position, float time) {
        Vector3 basePosition = transform.position;
        Vector3 deltaVector = position - basePosition;
        int frameNumber = Mathf.FloorToInt(time * 60f);
        Vector3 stepVector = deltaVector / (float)frameNumber;
        for (int i = 0; i < frameNumber; i++) {
            transform.position += stepVector;
            yield return new WaitForSeconds(1f/60f);
        }
    }

    private IEnumerator ISetRotationIn(Quaternion rotation, float time) {
        Quaternion baseRotation = transform.localRotation;
        int frameNumber = Mathf.FloorToInt(time * 60f);
        for (int i = 0; i < frameNumber; i++) {
            transform.localRotation = Quaternion.Slerp(baseRotation, rotation, (float)i /(float)frameNumber);
            yield return new WaitForSeconds(1f / 60f);
        }
    }

    private IEnumerator IShake(Vector2 delta, float time, float shakeTime) {
        float timePassed = 0f;
        float clock = -1;

        SetPositionIn(transform.position + (Vector3)(delta), shakeTime/2f, false);
        yield return new WaitForSeconds(shakeTime/2f + 0.01f);

        while (timePassed < time - shakeTime/2f) {
            SetPositionIn(transform.position + (Vector3)(delta * clock * 2f), shakeTime, false);
            yield return new WaitForSeconds(shakeTime + 0.01f);
            clock *= -1;
            timePassed += shakeTime;
        }

        SetPositionIn(transform.position + (Vector3)(delta * clock), shakeTime/2f, false);
    }

    #region Wait Coroutines

    IEnumerator Wait(IEnumerator numerator, float time) {
        yield return new WaitForSeconds(time);
        StartCoroutine(numerator);
    }

    IEnumerator Wait<T>(Tools.BasicDelegate<T> function, T arg, float time) {
        yield return new WaitForSeconds(time);
        function(arg);
    }

    IEnumerator Wait<T1, T2>(Tools.BasicDelegate<T1, T2> function, T1 arg1, T2 arg2, float time) {
        yield return new WaitForSeconds(time);
        function(arg1, arg2);
    }

    IEnumerator Wait<T1, T2, T3>(Tools.BasicDelegate<T1, T2, T3> function, T1 arg1, T2 arg2, T3 arg3, float time) {
        yield return new WaitForSeconds(time);
        function(arg1, arg2, arg3);
    }

    #endregion

    #endregion
}
