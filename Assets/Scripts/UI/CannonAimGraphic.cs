using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CannonAimGraphic : MonoBehaviour
{
    [SerializeField] private float enabledOpacity = 0.5f;
    [SerializeField] private DecalProjector aimLeft;
    [SerializeField] private DecalProjector aimRight;

    private ShipControllerTest player;

    private void Awake()
    {
        player = transform.parent.GetComponent<ShipControllerTest>();
    }

    private void Start()
    {
        aimLeft.fadeFactor = 0f;
        aimRight.fadeFactor = 0f;
    }

    private void Update()
    {
        if (player.GetCurrentView() != CameraManager.View.ThirdPerson)
        {
            aimLeft.fadeFactor = 0f;
            aimRight.fadeFactor = 0f;
            return;
        }
        bool lookingRight = Vector3.Dot(Camera.main.transform.forward, player.transform.right) > 0;
        aimLeft.fadeFactor = lookingRight ? 0f : enabledOpacity;
        aimRight.fadeFactor = lookingRight ? enabledOpacity : 0f;
    }

}
