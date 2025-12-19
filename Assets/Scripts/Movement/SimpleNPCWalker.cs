using UnityEngine;
using System.Collections.Generic;

public class SimpleNPCWalker : MonoBehaviour
{
    [Header("Movement Settings")]
    public List<Transform> waypoints;
    public float moveSpeed = 2f;

    [Header("Components")]
    public Animator animator;

    private int currentWaypointIndex = 0;

    void Update()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        MoveTowardsWaypoint();
    }

    private void MoveTowardsWaypoint()
    {
        Transform target = waypoints[currentWaypointIndex];

        Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
        Vector2 normalizedDir = direction.normalized;

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetFloat("moveX", normalizedDir.x);
            animator.SetFloat("moveY", normalizedDir.y);
        }

        transform.position = Vector2.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        }
    }
}