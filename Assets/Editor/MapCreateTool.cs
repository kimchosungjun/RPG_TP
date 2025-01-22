using UnityEngine;
using UnityEditor;

public class MapCreateTool : EditorWindow
{
    private GameObject selectedPrefab; // 배치할 프리팹
    private bool placementModeActive = false; // 배치 모드 활성화 상태
    private float placementHeight = 1.0f; // 물체 위에 배치할 시작 높이
    private float fallTimeLimit = 5.0f; // 최대 중력 작동 시간 (초)

    [MenuItem("Tools/Map Placement Tool")]
    public static void ShowWindow()
    {
        GetWindow<MapCreateTool>("Map Placement Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Map Placement Tool", EditorStyles.boldLabel);

        // 프리팹 선택 필드
        selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab to Place", selectedPrefab, typeof(GameObject), false);

        // 배치 높이 입력 필드
        placementHeight = EditorGUILayout.FloatField("Initial Spawn Height", placementHeight);

        // 도움말 출력
        EditorGUILayout.HelpBox(
            "1. Select a prefab to place.\n" +
            "2. Click on the Scene View to place the prefab.\n" +
            "3. The prefab will drop due to gravity and stop when it collides with another object.",
            MessageType.Info
        );

        // 배치 모드 활성화 / 비활성화 버튼
        if (!placementModeActive)
        {
            if (GUILayout.Button("Activate Placement Mode"))
            {
                SceneView.duringSceneGui += OnSceneGUI; // SceneView에 이벤트 등록
                placementModeActive = true; // 배치 모드 활성화
            }
        }
        else
        {
            if (GUILayout.Button("Deactivate Placement Mode"))
            {
                SceneView.duringSceneGui -= OnSceneGUI; // SceneView에서 이벤트 제거
                placementModeActive = false; // 배치 모드 비활성화
            }
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        // 마우스 이벤트 처리
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) // 좌클릭 확인
        {
            if (selectedPrefab == null)
            {
                Debug.LogWarning("No prefab selected! Please select a prefab in the tool window.");
                return;
            }

            // 마우스 클릭 위치에 Raycast
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 클릭 위치의 위쪽으로 약간 높이 띄워서 생성
                Vector3 spawnPosition = hit.point + Vector3.up * placementHeight;

                // 프리팹 생성
                GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
                newObject.transform.position = spawnPosition;

                // 중력을 적용하여 충돌 후 멈추도록 설정
                ApplyGravityAndFixPosition(newObject);

                // 작업 취소(Undo) 등록
                Undo.RegisterCreatedObjectUndo(newObject, "Place Prefab");

                // 새로 생성된 오브젝트를 선택
                Selection.activeGameObject = newObject;

                // SceneView 갱신
                SceneView.RepaintAll();
            }

            // 이벤트 사용(중복 실행 방지)
            e.Use();
        }
    }

    private void ApplyGravityAndFixPosition(GameObject obj)
    {
        // Rigidbody와 Collider가 있는지 확인하고, 없으면 추가
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
            rb = obj.AddComponent<Rigidbody>();

        Collider col = obj.GetComponent<Collider>();
        if (col == null)
            col = obj.AddComponent<BoxCollider>(); // 기본적으로 BoxCollider를 추가

        // Rigidbody 설정
        rb.useGravity = true;
        rb.isKinematic = false; // 물리 시뮬레이션 활성화
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 정확한 충돌 감지를 위해 설정

        // 일정 시간 후 Rigidbody를 제거하고 고정
        obj.AddComponent<PlacementFinalizer>().Initialize(fallTimeLimit);
    }

    private void OnDisable()
    {
        // 창이 닫힐 때 이벤트 제거
        SceneView.duringSceneGui -= OnSceneGUI;
        placementModeActive = false;
    }
}

public class PlacementFinalizer : MonoBehaviour
{
    private float timeLimit;
    private float elapsedTime;

    public void Initialize(float timeLimit)
    {
        this.timeLimit = timeLimit;
        elapsedTime = 0f;
    }

    private void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;

        // Rigidbody가 충돌로 멈췄거나 시간이 초과되었을 때 고정
        if (elapsedTime >= timeLimit || GetComponent<Rigidbody>().velocity.magnitude < 0.01f)
        {
            FinalizePlacement();
        }
    }

    private void FinalizePlacement()
    {
        // Rigidbody 제거 및 물리 정지
        Destroy(GetComponent<Rigidbody>());
        Destroy(this); // Finalizer 스크립트 제거
    }
}
