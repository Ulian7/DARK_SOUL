using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Ulian;
using UnityEngine;

public class GameApp : UnitySingleton<GameApp>
{
	Vector3 startPos = new Vector3(0, 5, 4);
	float degree = 180;
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
		PlayerManager playerManager = player.GetComponent<PlayerManager>();
		playerManager.lockOnTransform = player.transform.Find("Lock On Transform");
		

		GameObject ghostPrefab = ResMgr.Instance.GetAssetCache<GameObject>("Prefab/Ghost.prefab");
		GameObject ghost = GameObject.Instantiate(ghostPrefab, player.transform.position, Quaternion.Euler(0, degree, 0));
		GhostLocomotion ghostLocomotion = ghost.GetComponent<GhostLocomotion>();
		ghostLocomotion.player = player.transform;
		
		/*GameObject adjustCameraPrefab = ResMgr.Instance.GetAssetCache<GameObject>("Prefab/FreeLook.prefab");
		GameObject adjustCamera = GameObject.Instantiate(adjustCameraPrefab, player.transform.position, Quaternion.Euler(0, 0, 0));
		adjustCamera.name = "FreeLookCamera";
		adjustCamera.transform.parent = player.transform;
		CinemachineFreeLook cinemachineFreeLook = adjustCamera.GetComponent<CinemachineFreeLook>();
		cinemachineFreeLook.Follow = GameObject.Find("Ghost(Clone)").transform;
		cinemachineFreeLook.LookAt = GameObject.Find("LookAt").transform;
		cinemachineFreeLook.m_YAxis.Value = 0.7f;
		cinemachineFreeLook.Priority = 11;

        GameObject freelookPrefab = ResMgr.Instance.GetAssetCache<GameObject>("Prefab/FreeLook.prefab");
		GameObject freelook = GameObject.Instantiate(freelookPrefab, player.transform.position, Quaternion.Euler(0, 0, 0));
		freelook.name = "LockOnCamera";
		freelook.transform.parent = player.transform;
		freelook.AddComponent<CameraManager>();
		inputHandler.cameraManager = freelook.GetComponent<CameraManager>();*/
		
		InputHandler inputHandler = player.GetComponent<InputHandler>();
		inputHandler.cameraHandler = GameObject.Find("Camera Holder").GetComponent<CameraHandler>();
		PlayerLocomotion playerLocomotion = player.GetComponent<PlayerLocomotion>();
		playerLocomotion.cameraHandler = inputHandler.cameraHandler;
		
		GameObject UI = GameObject.Find("Player UI");
		UIManager uiManager = UI.GetComponent<UIManager>();
		uiManager.playerInventory = player.GetComponent<PlayerInventory>();
		uiManager.InitializeEquipmentWindow();
	}
}
