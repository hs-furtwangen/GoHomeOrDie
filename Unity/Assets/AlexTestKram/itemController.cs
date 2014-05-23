using UnityEngine;

public class itemController : MonoBehaviour
{

    private GameObject player;
    private movement playerMovementScript;

	// Use this for initialization
	void Start ()
	{
	    player = GameObject.Find("/Player");
	    playerMovementScript = player.GetComponent<movement>();
	}
	
	void OnMouseDown ()
	{
	    playerMovementScript.ItemMovement(this.gameObject);
    }
}
