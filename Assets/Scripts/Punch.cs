using UnityEngine;

public class Punch : MonoBehaviour
{
    [Header("Punch Settings")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float faceMultiplier = 1.5f;
    [SerializeField] private float maxDistance = 30f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || TouchBegan())
        {
            TryHit();
        }
    }
    
    private void TryHit()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
        {
            var face = hit.collider.GetComponent<FaceHitbox>();
            if (face)
            {
                var headHealthFromFace = face.GetComponentInParent<HeadHealth>();
                if (headHealthFromFace != null)
                {
                    headHealthFromFace.Hit(damage * faceMultiplier, hit.point, hit.normal);
                    return;
                }
            }

            var headHealthGeneral = hit.collider.GetComponentInParent<HeadHealth>();
            if (headHealthGeneral != null)
            {
                headHealthGeneral.Hit(damage, hit.point, hit.normal);
            }
        }
        else
        {
            Debug.Log("Missed");
        }
    }
    
    private bool TouchBegan()
    {
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
    }
}