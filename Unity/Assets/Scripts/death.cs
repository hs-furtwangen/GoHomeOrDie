using UnityEngine;
using System.Collections;

public class death : MonoBehaviour {

    private Animator _anim;
    private float _timer;

	// Use this for initialization
	void Start () {
        _anim = gameObject.GetComponent<Animator>();
        _anim.SetTrigger("Death");
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _timer += Time.deltaTime;

	    if (_timer > 6)
	    {
	        Application.LoadLevel(4);
	    }
	}
}
