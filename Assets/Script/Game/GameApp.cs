using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Ulian;
using UnityEngine;

public class GameApp : UnitySingleton<GameApp>
{
	Vector3 startPos = new Vector3(0, 5, 4);
	float degree = 90;
	// 游戏逻辑入口
	public void GameStart()
	{
		this.EnterGame();
	}

	public void EnterGame()
	{
		GameObject playerPrefab = ResMgr.Instance.GetAssetCache<GameObject>("Prefab/Player.prefab");
		GameObject player = GameObject.Instantiate(playerPrefab, startPos, Quaternion.Euler(0, degree, 0));
		player.name = playerPrefab.name;

		GameObject ghostPrefab = ResMgr.Instance.GetAssetCache<GameObject>("Prefab/Ghost.prefab");
		GameObject ghost = GameObject.Instantiate(ghostPrefab, player.transform.position, Quaternion.Euler(0, degree, 0));
		GhostLocomotion ghostLocomotion = ghost.GetComponent<GhostLocomotion>();
		ghostLocomotion.player = player.transform;


        GameObject freelookPrefab = ResMgr.Instance.GetAssetCache<GameObject>("Prefab/FreeLook.prefab");
		GameObject freelook = GameObject.Instantiate(freelookPrefab, player.transform.position, Quaternion.Euler(0, 0, 0));
		freelook.name = freelookPrefab.name;
		freelook.transform.parent = player.transform;

		CinemachineFreeLook cinemachineFreeLook = freelook.GetComponent<CinemachineFreeLook>();
		cinemachineFreeLook.Follow = ghost.transform;
		cinemachineFreeLook.LookAt = ghostLocomotion.transform.Find("LookAt"); ;
		cinemachineFreeLook.m_YAxis.Value = 0.7f;

    }
}
