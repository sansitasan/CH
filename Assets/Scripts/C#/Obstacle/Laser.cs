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

    private void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        var hit = Physics2D.Raycast(transform.position, LookDir);
        if (hit)
        {
            Draw2DRay(_laserPoint.position, hit.point);
            if (hit.transform.CompareTag("Player"))
            {
                //dead
                hit.transform.TryGetComponent<PlayerModel>(out var player);
                player.PlayerInput(PlayerStates.Dead);
                _lineRenderer.enabled = false;
                enabled = false;
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
