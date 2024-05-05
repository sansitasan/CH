using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Laser : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 10;
    public Transform _laserPoint;
    public LineRenderer _lineRenderer;
    public Vector3 LookDir;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void Init()
    {
        _lineRenderer.enabled = true;
        enabled = true;
    }

    private void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        var hit = Physics2D.Raycast(transform.position, LookDir);
        if (hit)
        {
            if (LookDir.y == 1)
            {
                Draw2DRay(_laserPoint.position, hit.point + new Vector2(0, 0.5f));
            }
            else
                Draw2DRay(_laserPoint.position, hit.point);
            if (hit.transform.CompareTag("Player"))
            {
                //dead
                hit.transform.TryGetComponent<PlayerModel>(out var player);
                if (player != null)
                {
                    player?.PlayerInput(PlayerStates.Dead);
                    _lineRenderer.enabled = false;
                    enabled = false;
                }
            }
        }

        else
        {
            Draw2DRay(_laserPoint.position, _laserPoint.position + LookDir * defDistanceRay);
        }
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        _lineRenderer.SetPosition(0, startPos);
        _lineRenderer.SetPosition(1, endPos);
    }
}
