using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    [SerializeField]
    private List<Transform> _obstacles = new List<Transform>();
    [SerializeField]
    private List<Vector3> _originPoses = new List<Vector3>();
    [SerializeField]
    private List<Laser> _lasers = new List<Laser>();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).TryGetComponent<Obstacle>(out var obstacle))
            {
                _obstacles.Add(obstacle.transform);
                _originPoses.Add(obstacle.transform.position);
            }

            else if (transform.GetChild(i).TryGetComponent<Laser>(out var laser))
            {
                _lasers.Add(laser);
            }
        }
    }

    public void Restart()
    {
        for (int i = 0; i < _obstacles.Count; ++i)
        {
            _obstacles[i].position = _originPoses[i];
        }

        for (int i = 0; i < _lasers.Count; ++i)
        {
            _lasers[i]?.Init();
        }
    }
}
