using Godot;
using System;
using System.Collections.Generic;
using System.Net;

public partial class TestLevel : Node2D
{
	//4 rooms 12 wide with walls
	int tWalls;
	int bWalls;
	int width;

	int[] T;
	int[] tL;
	int[] B;
	int[] bL;

	//3 rooms 9 high with walls
	int lWalls;
	int rWalls;
	int height; 

	int[] L;
	int[] lL;
	int[] R;
	int[] rL;

	Vector2I reference = new Vector2I(0,0); 
	
	[Export]
	public Godot.TileMap map;

	const int numOptions = 3;
	int[,,] setUpArray;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var random = new Random();
		width = random.Next(30,50);
		height = random.Next(30,50);

		// Maybe make these dependent-ish on width/height
		tWalls = random.Next(1,4);
		bWalls = random.Next(5-tWalls,8-tWalls);
		lWalls = random.Next(1,4);
		rWalls = random.Next(5-lWalls,8-lWalls);

		T = new int[tWalls];
		tL = new int[tWalls];
		B = new int[bWalls];
		bL = new int[bWalls];
		L = new int[lWalls];
		lL = new int[lWalls];
		R = new int[rWalls];
		rL = new int[rWalls];

		int last = 0;
		for (int i=0; i<tWalls; i++) {
			int temp = random.Next(last + 5, last + 20);
			T[i] = temp;
			last = temp;
			temp = random.Next(5, height);
			tL[i] = temp;
		}
		last = 0;
		for (int i=0; i<bWalls; i++) {
			int temp = random.Next(last + 5, last + 20);
			B[i] = temp;
			last = temp;
			temp = random.Next(5, height);
			bL[i] = temp;
		}
		last = 0;
		for (int i=0; i<lWalls; i++) {
			int temp = random.Next(last + 5, last + 20);
			L[i] = temp;
			last = temp;
			temp = random.Next(5, height);
			lL[i] = temp;
		}
		last = 0;
		for (int i=0; i<rWalls; i++) {
			int temp = random.Next(last + 5, last + 20);
			R[i] = temp;
			last = temp;
			temp = random.Next(5, height);
			rL[i] = temp;
		}

		// map = GetChild(3) as Godot.TileMap;

		//setupSetupArray();

		for (int i = 0; i < height; i++) {
			map.SetCell(0, new Vector2I(0,i),1, reference);
			map.SetCell(0, new Vector2I(width-1,i),1, reference);
		}
		for (int i = 0; i < width; i++) {
			map.SetCell(0, new Vector2I(i,0),1, reference);
			map.SetCell(0, new Vector2I(i,height-1),1, reference);
		}

		for (int i = 0; i < tWalls; i++) {
			for (int j=0; j< tL[i]; j++) {
				map.SetCell(0, new Vector2I(T[i],j),1, reference);

			}
		}
		for (int i = 0; i < bWalls; i++) {
			for (int j=0; j< bL[i]; j++) {
				map.SetCell(0, new Vector2I(B[i],height-1-j),1, reference);
			}
		}
		for (int i = 0; i < lWalls; i++) {
			for (int j=0; j< lL[i]; j++) {
				map.SetCell(0, new Vector2I(j,L[i]),1, reference);
			}
		}
		for (int i = 0; i < rWalls; i++) {
			for (int j=0; j< rL[i]; j++) {
				map.SetCell(0, new Vector2I(width-1-j,R[i]),1, reference);
			}
		}





		// Step through each room pair starting at 0,0
		/*for (int i = 0; i < horRooms; i++){
			for (int j = 0; j < vertRooms; j++) {
				if (i == 0 && j == 0) {
					buildRoom(i,j,1);
				} else {
					buildRoom(i,j,random.Next(1, numOptions));
				}
			}
		}*/
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ 
	}

}
