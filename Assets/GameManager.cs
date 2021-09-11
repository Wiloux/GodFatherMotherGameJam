using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

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
    private List<GameObject> players;
    public LayerMask whatIsGround;
    public LayerMask whatGoBoum;

    public GameObject gridObject;

    public Sprite player2ArmVisuals;

    bool gameStarted;

    public Animator endAnimator;

    public bool ended = false;

    public GameObject playerPrefab;
    public List<Transform> playerSpawns = new List<Transform>();

    public List<GameObject> maps;

    public bool canRestart = false;

    private Coroutine slowMotionRoutine = null;

    public SpriteRenderer background;

    private void Awake() {
        if (instance != null) {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        instance = this;

        gridObject = transform.Find("Grid").gameObject;
        terrainDestruction = gridObject.GetComponent<TerrainDestruction>();
        ReInput.ControllerConnectedEvent += OnControllerConnected;

        if (mainCamera == null) { mainCamera = Camera.main; }
    }

    public void Start() {
        ShowMultPanel(true);

        playersController = new Controller[MAX_PLAYER];

        AssignAllJoysticksToSystemPlayer(true);
        endAnimator.gameObject.SetActive(false);
        assignedJoysticks = new List<int>();
        players = new List<GameObject>();
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
        if (playersController[0] != null && ReInput.players.GetPlayer("P1").GetButtonDown("Start game") && playersController[1] != null) {
            BeginGame();
        }

        // Change Portraits
        for (int i = 0; i < playersController.Length; i++) {
            if (playersController[i] != null) {
                Rewired.Player player = ReInput.players.GetPlayer("P" + (i + 1));
                if (player.GetButtonDown("Leave game")) {
                    RemoveController(i);
                    multiplayerPanel.transform.GetChild(0).GetChild(i+2).GetChild(0).gameObject.SetActive(true);
                    multiplayerPanel.transform.GetChild(0).GetChild(i+2).GetChild(1).gameObject.SetActive(false);
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
                    multiplayerPanel.transform.GetChild(0).GetChild(i+2).GetChild(0).gameObject.SetActive(false);
                    multiplayerPanel.transform.GetChild(0).GetChild(i+2).GetChild(1).gameObject.SetActive(true);
                    //  multiplayerPanel.ChangePortraitSprite(i, 0);
                }
            }
        }
    }

    private void ResestMultPanel() {
        for (int i = 0; i < MAX_PLAYER; i++) {
            multiplayerPanel.transform.GetChild(0).GetChild(i + 2).GetChild(0).gameObject.SetActive(true);
            multiplayerPanel.transform.GetChild(0).GetChild(i + 2).GetChild(1).gameObject.SetActive(false);
        }
    }

    void BeginGame() {
        endAnimator.SetFloat("Ending", -1);
        MapInformation map = GetAMap();

        CameraController cameraController = mainCamera.GetComponent<CameraController>();

        if (map.cameraMove) {
            cameraController.start = map.start;
            cameraController.end = map.end;
            cameraController.StartMove();
        } else {
            cameraController.start = null;
            cameraController.end = null;
            cameraController.StopMove();
        }

        playerSpawns.Add(map.blueSpawn);
        playerSpawns.Add(map.redSpawn);

        background.color = map.backgroundTint;

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
                spawnedPlayer.transform.Find("RocketRoot/RocketCharacter1/CharacterArm").GetComponent<SpriteRenderer>().sprite = player2ArmVisuals;
                spawnedPlayer.transform.Find("CharacterRoot/character1").gameObject.SetActive(false);
            } else {
                spawnedPlayer.transform.position = playerSpawns[0].position;
                spawnedPlayer.transform.Find("CharacterRoot/character2").gameObject.SetActive(false);

            }
            spawnedPlayer.GetComponent<PlayerController>().playerController = ReInput.players.GetPlayer(i);
            players.Add(spawnedPlayer);
        }
    }

    private MapInformation GetAMap() {
        int mapID = -1;
        mapID = Random.Range(0, maps.Count);

        GameObject obj = Instantiate(maps[mapID], gridObject.transform);
        obj.SetActive(true);

        return obj.GetComponent<MapInformation>();
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
        endAnimator.gameObject.SetActive(true);
        endAnimator.SetFloat("Ending", index);
        ended = true;
        SlowMotion(0.2f);
    }

    public void SlowMotion(float targetTime) {
        if (slowMotionRoutine != null) { StopCoroutine(slowMotionRoutine); }
        slowMotionRoutine = StartCoroutine(ISlowMotion(targetTime, 0.1f));
    }

    public IEnumerator ISlowMotion(float amount, float time) {
        float timePassed = 0f;
        float baseTimeScale = Time.timeScale;
        while (timePassed < time) {
            Time.timeScale = Mathf.Lerp(baseTimeScale, amount, timePassed / time);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return new WaitForEndOfFrame();
            timePassed += Time.deltaTime;
        }
    }

    public void Clean() {
        endAnimator.SetFloat("Ending", -1);
        endAnimator.gameObject.SetActive(false);
        ended = false;
        for (int i = 0; i < players.Count; i++) {
            players[i].SetActive(false);
            Destroy(players[i]);
        }
        players.Clear();
        for (int i = 0; i < gridObject.transform.childCount; i++) {
            gridObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        playerSpawns.Clear();
        mainCamera.transform.position = new Vector3(0, 0, -10);
        mainCamera.GetComponent<CameraController>().StopMove();
        canRestart = false;
    }

    public void Restart() {
        Clean();
        BeginGame();
        //SceneManager.LoadScene("SceneFinal");
    }

    public void Menu() {
        Clean();
        ResestMultPanel();
        SceneManager.LoadScene("Menu");
    }
}
