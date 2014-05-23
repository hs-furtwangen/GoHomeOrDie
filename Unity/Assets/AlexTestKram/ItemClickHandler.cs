using UnityEngine;

// ReSharper disable once CheckNamespace
public class ItemClickHandler : MonoBehaviour
{

    private GameObject _player;
    private PlayerInteraction _playerMovementScript;

	// Use this for initialization
// ReSharper disable once UnusedMember.Local
	void Start ()
	{
	    _player = GameObject.Find("/Player");
	    _playerMovementScript = _player.GetComponent<PlayerInteraction>();
	}
	
// ReSharper disable once UnusedMember.Local
	void OnMouseDown ()
	{
	    _playerMovementScript.ItemMovement(gameObject);
    }
}
