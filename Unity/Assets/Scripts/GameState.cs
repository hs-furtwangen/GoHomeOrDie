using UnityEngine;
using System.Collections;


public static class GameState {

	public enum State
	{
		paused,
		playing
	}

	static GameState()
	{
		TheState = State.playing;
	}

	private static State theState;
	public static State TheState {
		get {
			return theState;
		}
		set {
			theState = value;
		}
	}

}
