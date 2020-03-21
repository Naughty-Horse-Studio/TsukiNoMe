/// Deacon of Freedom Development (2020) v1
/// If you have any questions feel free to write me at email --- Phil-James_Lapuz@outlook.com ---
using System.Collections.Generic;
using UnityEngine;

public class CoverList : MonoBehaviour
{
    public List<Cover> covers;

    public int maxCoverFindRange = 10;

    private void Start()
    {
        covers.AddRange(FindObjectsOfType<Cover>());
    }
    
    public Cover FindClosestCover(Vector3 myPos, Vector3 enemyPos)
    {
        if (covers == null) return null;
        if (enemyPos == null) return null;

        Cover cover = null;

        var bestDistance = 1000f;

        RaycastHit hit;

        foreach (var _cover in covers)
        {
            var distance = Vector3.Distance(myPos, _cover.transform.position);
            var direction = _cover.transform.position - enemyPos;

            if (Physics.Raycast(enemyPos, direction, out hit, Mathf.Infinity))
            {
                if (hit.collider.name != _cover.m_collider.name)
                {
                    if (distance < bestDistance && Vector3.Distance(_cover.transform.position, enemyPos) > 5)
                    {
                        if (!_cover.occupied)
                        {
                                cover = _cover;
                                bestDistance = distance;
                            
                        }
                    }
                }
            }
        }

        return cover;
    }
}
