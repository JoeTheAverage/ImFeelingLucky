﻿using UnityEngine;
using System.Collections.Generic;

public class Room {
	private float door_width;
	private int direction;
	private Vector3 room_centre;
	private List<string> referrals;
	int doors_spawned = 0;

	public Room(RoomInfo info, int direction, float door_width, string origin_url, Vector3 last_edge_center) {
		this.door_width = door_width;
		this.direction = direction;

		referrals = new List<string>();
		referrals.Add (origin_url);
		referrals.AddRange (info.referrals);

		int door_space_count = 12 + 8 * (int) (Mathf.Floor((referrals.Count - 1) / 4));
		Debug.Log (door_space_count);
		float edgeLength = door_width * door_space_count/4;
		Vector3 centre_modifier = new Vector3();
		switch (direction) {
		case 0:
			centre_modifier = new Vector3(0, 0, edgeLength / 2);
			break;
		case 1:
			centre_modifier = new Vector3(edgeLength/2, 0, 0);
			break;
		case 2:
			centre_modifier = new Vector3(0, 0, -(edgeLength / 2));
			break;
		case 3:
			centre_modifier = new Vector3(-(edgeLength/2), 0, 0);
			break;
		default:
			room_centre = Vector3.zero;
			break;
		}

		room_centre = last_edge_center + centre_modifier;
		BuildFloor (edgeLength);
		BuildCeiling (edgeLength);

		int edge_index = direction;
		if (direction == -1) {
			edge_index = 0;
		}

		bool wall = true;
		for (int i = 0; i < (int) (door_space_count/4); i++) {
			SpawnDoorOrWallOnAllEdges(wall, i, edgeLength);
			wall = !wall;
		}
	}

	private void SpawnDoorOrWallOnAllEdges(bool wall, int offset, float edgeLength) {
		for (int i = 0; i < 4; i++) {
			if (!wall) {
				if (doors_spawned < referrals.Count) {
					SpawnDoorOrWall(false, i, edgeLength, offset);
					doors_spawned++;
				} else {
					SpawnDoorOrWall(true, i, edgeLength, offset);
				}
			} else {
				SpawnDoorOrWall(true, i, edgeLength, offset);
			}
		}
	}

	private void SpawnDoorOrWall(bool iswall, int corner, float edgeLength, int offset) {
		Vector3 origin_point = Vector3.zero;
		Vector3 direction = Vector3.zero;
		Color color = Color.white;
		switch (corner) {
		case 0:
			color = Color.red;
			direction = new Vector3(1, 0, 0);
			origin_point = room_centre + new Vector3(-(edgeLength/2), 0, (edgeLength/2)) + direction * offset * door_width;
			break;
		case 1:
			color = Color.blue;
			direction = new Vector3(0, 0, -1);
			origin_point = room_centre + new Vector3((edgeLength/2), 0, (edgeLength/2)) + direction * offset * door_width;
			break;
		case 2:
			color = Color.green;
			direction = new Vector3(-1, 0, 0);
			origin_point = room_centre + new Vector3((edgeLength/2), 0, -(edgeLength/2)) + direction * offset * door_width;
			break;
		case 3:
			color = Color.yellow;
			direction = new Vector3(0, 0, 1);
			origin_point = room_centre + new Vector3(-(edgeLength/2), 0, -(edgeLength/2)) + direction * offset * door_width;
			break;
		default:
			break;
		}

		if (iswall) {
			GameObject wall = GameObject.CreatePrimitive (PrimitiveType.Cube);
			wall.transform.localScale = new Vector3 (door_width, edgeLength, 1);
			wall.transform.position = origin_point
				+ direction * door_width/2 
					+ Vector3.up * wall.transform.localScale.y / 2;
			wall.transform.right = direction;
			wall.GetComponent<Renderer> ().material.color = color;
		} else {
			string r = "fucccccck";
			Door door = new Door (r, origin_point + direction * door_width / 2, direction);
//			referrals.RemoveAt (0);
			GameObject above = GameObject.CreatePrimitive (PrimitiveType.Cube);
			above.transform.localScale = new Vector3 (door_width, edgeLength - door.GetHeight(), 1);
			above.transform.position = origin_point
				+ direction * door_width/2 +
					Vector3.up * (door.GetHeight() + above.transform.localScale.y /2);
			above.GetComponent<Renderer> ().material.color = color;
			above.transform.right = direction;
		}
	}

	private void BuildFloor(float edgeLength) {
		GameObject floor = GameObject.CreatePrimitive (PrimitiveType.Cube);
		floor.transform.localScale = new Vector3 (edgeLength, 1, edgeLength);
		floor.transform.position = new Vector3 (0, -0.5f, 0);
	}

	private void BuildCeiling(float edgeLength) {
		GameObject floor = GameObject.CreatePrimitive (PrimitiveType.Cube);
		floor.transform.localScale = new Vector3 (edgeLength, 1, edgeLength);
		floor.transform.position = new Vector3 (0, 0.5f + edgeLength, 0);
	}
}