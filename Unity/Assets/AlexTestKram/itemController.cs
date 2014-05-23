using UnityEngine;

public class ItemController : MonoBehaviour
{

    private GameObject player;
    private Movement playerMovementScript;

	// Use this for initialization
	void Start ()
	{
	    player = GameObject.Find("/Player");
	    playerMovementScript = player.GetComponent<Movement>();
	}
	
	void OnMouseDown ()
	{
	    playerMovementScript.ItemMovement(this.gameObject);
    }
}
