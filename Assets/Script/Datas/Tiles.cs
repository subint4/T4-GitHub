using UnityEngine;
using UnityEngine.EventSystems;

public class Tiles : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isOccupied = false;
    public Tower currentTower = null;
    private Transform parentTransform;
    public float tileXPosition;

    private void Start()
    {
        parentTransform = transform.parent;
        if (parentTransform == null)
        {
            Debug.LogError("[Tiles] 부모 Transform을 찾을 수 없습니다!");
        }

        tileXPosition = parentTransform != null ? parentTransform.position.x : transform.position.x;
    }
    // 마우스 클릭 & 터치 인식
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[Tiles] OnPointerClick() 호출됨 - 위치: {transform.position}, isOccupied: {isOccupied}");

        if (isOccupied)
        {
            Debug.LogWarning($"[Tiles] 타워 배치 불가: {transform.position} (isOccupied: {isOccupied})");
            return;
        }

        int selectedTowerID = TowerManager.Instance.GetSelectedTowerID();
        if (selectedTowerID == -1)
        {
            Debug.LogWarning("[Tiles] 선택된 타워가 없습니다!");
            return;
        }

        SpawnTower(selectedTowerID);
    }

    // 터치 입력 감지 (손가락이 화면을 눌렀을 때)
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log($"[Tiles] OnPointerDown() 호출됨 - 위치: {transform.position}");
    }

    // 터치 입력 감지 (손가락을 떼었을 때)
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log($"[Tiles] OnPointerUp() 호출됨 - 위치: {transform.position}");
    }

    // 터치 입력을 직접 감지 (멀티 터치 포함)
    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // 첫 번째 터치 감지
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("[Tiles] Touch 감지됨 - 터치 시작");
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(touch.position);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == transform)
                    {
                        Debug.Log("[Tiles] 터치한 타일 감지됨 - OnPointerClick() 실행");
                        OnPointerClick(null);
                    }
                }
            }
        }
    }

    private void SpawnTower(int towerID)
    {
        Debug.Log($"[Tiles] SpawnTower() 호출됨 - 타워 ID: {towerID}");

        GameObject towerPrefab = TowerManager.Instance.GetTowerPrefab(towerID);
        if (towerPrefab == null)
        {
            Debug.LogError($"[Tiles] Tower ID {towerID}에 대한 프리팹을 찾을 수 없습니다!");
            return;
        }

        GameObject newTowerObj = Instantiate(towerPrefab, transform.position, Quaternion.identity);
        Tower newTower = newTowerObj.GetComponent<Tower>();

        if (newTower != null)
        {
            PlaceTower(newTower);
            Debug.Log($"[Tiles] 타워 배치 완료: Tower ID {towerID} (위치: {transform.position})");
        }
        else
        {
            Debug.LogError("[Tiles] 생성된 타워에 Tower 컴포넌트가 없습니다!");
        }
    }

    public void PlaceTower(Tower tower)
    {
        isOccupied = true;
        currentTower = tower;
        tower.currentTile = this;
        Debug.Log($"[Tiles] 타워 배치됨: {transform.position}");
    }
}
