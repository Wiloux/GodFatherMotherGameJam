using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour {
    public enum Endings {
        BLUE_BOT,
        BLUE_TOP,
        RED_BOT,
        RED_TOP
    }

    const int MAX_PLAYER = 2;
    public static GameManager instance;

    public GameObject multiplayerPanel;
    public TerrainDestruction terrainDestruction;
    public Camera mainCamera;

    [Header("Rewired")]
    private List<int> assignedJoysticks = null;
    private Controller[] playersController = null;

    [Header("Game")]
    public LayerMask whatIsGround;

    public GameObject gridObject;

    public Sprite player2ArmVisuals;

    bool gameStarted;

    public Animator endAnimator;

    private bool ended = false;

    public GameObject playerPrefab;
    public List<Transform> playerSpawns = new List<Transform>();

    private void Awake() {
        instance = this;


        gridObject = transform.Find("Grid").gameObject;
        terrainDestruction = gridObject.GetComponent<TerrainDestruction>();
        assignedJoysticks = new List<int>();
        ReInput.ControllerConnectedEvent += OnControllerConnected;

        if (mainCamera == null) { mainCamera = Camera.main; }
    }

    void Start() {
        ShowMultPanel(true);

        playersController = new Controller[MAX_PLAYER];

        AssignAllJoysticksToSystemPlayer(true);
    }

    void Update() {
        if (!gameStarted)
            UpdateMultPanel();
    }

    private void ShowMultPanel(bool state = true) {
        if (multiplayerPanel == null) { return; }
        multiplayerPanel.gameObject.SetActive(state);
    }

    private void UpdateMultPanel() {
        if (multiplayerPanel == null) { return; }

        // P1 Start game
        if (playersController[0] != null && ReInput.players.GetPlayer("P1").GetButtonDown("Start game")) {
            BeginGame();
        }

        // Change Portraits
        for (int i = 0; i < playersController.Length; i++) {
            if (playersController[i] != null) {
                Rewired.Player player = ReInput.players.GetPlayer("P" + (i + 1));
                if (player.GetButtonDown("Leave game")) {
                    RemoveController(i);
                    multiplayerPanel.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(true);
                    multiplayerPanel.transform.GetChild(0).GetChild(i).GetChild(1).gameObject.SetActive(false);
                }
            }
        }

        // Join game
        Controller controller = null;

        if (ReInput.players.GetSystemPlayer().GetButtonDown("Join game")) {
            controller = ReInput.players.GetSystemPlayer().controllers.GetLastActiveController();
        }

        if (controller == null) { return; }

        if (AddController(controller)) {
            for (int i = 0; i < playersController.Length; i++) {
                if (playersController[i] != null) {
                    multiplayerPanel.transform.GetChild(0).GetChild(i).GetChild(0).gameObject.SetActive(false);
                    multiplayerPanel.transform.GetChild(0).GetChild(i).GetChild(1).gameObject.SetActive(true);
                    //  multiplayerPanel.ChangePortraitSprite(i, 0);
                }
            }
        }
    }

    void BeginGame() {
        MapInformation map = GetAMap();

        if (map.cameraMove) {
            mainCamera.GetComponent<CameraController>().start = map.start;
            mainCamera.GetComponent<CameraController>().end = map.end;
            mainCamera.GetComponent<CameraController>().StartMove();
        }

        playerSpawns.Add(map.blueSpawn);
        playerSpawns.Add(map.redSpawn);

        //playerSpawns.Add(gridObject.transform.GetChild(mapID).Find("P1Spawn"));
        //playerSpawns.Add(gridObject.transform.GetChild(mapID).Find("P2Spawn"));
        //terrainDestruction.terrain = map.GetComponent<Tilemap>();
        terrainDestruction.terrain = map.GetComponent<Tilemap>();

        gameStarted = true;
        multiplayerPanel.SetActive(false);
        for (int i = 0; i < playersController.Length; i++) {
            if (playersController[i] == null) { return; }

            GameObject spawnedPlayer = Instantiate(playerPrefab);
            if (i != 0) {
                spawnedPlayer.transform.position = playerSpawns[1].position;
                spawnedPlayer.GetComponent<PlayerController>().playerID = 1;
                spawnedPlayer.transform.Find("RocketRotate/RocketRoot/RocketCharacter1/CharacterArm").GetComponent<SpriteRenderer>().sprite = player2ArmVisuals;
                spawnedPlayer.transform.Find("CharacterRoot/character1").gameObject.SetActive(false);
            } else {
                spawnedPlayer.transform.position = playerSpawns[0].position;
                spawnedPlayer.transform.Find("CharacterRoot/character2").gameObject.SetActive(false);

            }
            spawnedPlayer.GetComponent<PlayerController>().playerController = ReInput.players.GetPlayer(i);
        }
    }

    private MapInformation GetAMap() {
        int mapID = -1;
        for (int i = 0; i < gridObject.transform.childCount; i++) {
            GameObject child = gridObject.transform.GetChild(i).gameObject;
            if (child.activeSelf) {
                mapID = i;
            }
        }

        if (mapID == -1) {
            mapID = Random.Range(0, gridObject.transform.childCount);
        }

        for (int i = 0; i < gridObject.transform.childCount; i++) {
            gridObject.transform.GetChild(i).gameObject.SetActive(false);
        }

        gridObject.transform.GetChild(mapID).gameObject.SetActive(true);
        return gridObject.transform.GetChild(mapID).gameObject.GetComponent<MapInformation>();
    }

    private bool AddController(Controller controller) {
        for (int i = 0; i < playersController.Length; i++) {
            if (controller == playersController[i]) {
                return false;
            } else if (playersController[i] == null) {
                playersController[i] = controller;

                Rewired.Player player = ReInput.players.GetPlayer("P" + (i + 1));
                player.controllers.ClearAllControllers();
                player.controllers.AddController(controller, true);

                if (player.controllers.hasKeyboard) {
                    player.controllers.hasMouse = true;
                } else {
                    player.controllers.hasMouse = false;
                }

                return true;
            }
        }
        return false;
    }

    private bool RemoveController(int index) {
        if (playersController[index] == null) { return false; }

        ReInput.players.GetSystemPlayer().controllers.AddController(playersController[index], true);
        playersController[index] = null;
        return true;
    }
    private void OnControllerConnected(ControllerStatusChangedEventArgs args) {
        if (args.controllerType != ControllerType.Joystick) { return; }
        if (assignedJoysticks.Contains(args.controllerId)) { return; }

        ReInput.players.GetSystemPlayer().controllers.AddController(args.controllerType, args.controllerId, true);
    }

    private void AssignAllJoysticksToSystemPlayer(bool removeFromOtherPlayers) {
        IList<Joystick> joysticks = ReInput.controllers.Joysticks;
        for (int i = 0; i < ReInput.controllers.joystickCount; i++) {
            ReInput.players.GetSystemPlayer().controllers.AddController(joysticks[i], removeFromOtherPlayers);
        }
    }

    public void StartEndScreen(Endings endings) {
        StartEndScreen((int)endings);
    }

    public void StartEndScreen(int index) {
        if (ended) { return; }
        endAnimator.SetFloat("Ending", index);
        ended = true;
    }

    IEnumerator SlowMotion(float amount, float time) {
        float timePassed = 0f;
        float baseTimeScale = Time.timeScale;
        while (timePassed < time) {
            Time.timeScale = Mathf.Lerp(baseTimeScale, amount, timePassed / time);
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }
    }
}
