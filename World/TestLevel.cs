using Godot;
using System;
using System.Collections.Generic;

public partial class TestLevel : Node2D
{
	//4 rooms 12 wide with walls
	const int roomWidth = 12;
	const int horRooms = 3;
	int width;
	//3 rooms 9 high with walls
	const int roomHeight = 12;
	const int vertRooms = 3;
	int height; 

	Vector2I reference = new Vector2I(0,0); 
	
	[Export]
	public Godot.TileMap map;

	const int numOptions = 3;
	int[,,] setUpArray;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var random = new Random();
		height = roomHeight*vertRooms + vertRooms + 1;
		width = roomWidth*horRooms + horRooms + 1;

		// map = GetChild(3) as Godot.TileMap;

		setupSetupArray();

		for (int i = 0; i < height; i++) {
			map.SetCell(0, new Vector2I(-2,i),1, reference);
			map.SetCell(0, new Vector2I(width-1,i),1, reference);
		}
		for (int i = 0; i < width; i++) {
			map.SetCell(0, new Vector2I(i,-2),1, reference);
			map.SetCell(0, new Vector2I(i,height-1),1, reference);
		}

		// Step through each room pair starting at 0,0
		for (int i = 0; i < horRooms; i++){
			for (int j = 0; j < vertRooms; j++) {
				if (i == 0 && j == 0) {
					buildRoom(i,j,1);
				} else {
					buildRoom(i,j,random.Next(1, numOptions));
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{ 
	}

	public void buildRoom(int x, int y, int seed) {
		int startX = x*(roomWidth + 1);
		int startY = y*(roomHeight + 1);

		for (int i = 0; i < roomWidth; i++) {
			for (int j = 0; j < roomHeight; j++) {
				map.SetCell(0, new Vector2I(startX + i, startY + j), 1, new Vector2I(setUpArray[seed,i,j], 0));

				// switch (setUpItemsArray[seed,i,j]){
				// 	case 1:

				// 		break;
				// }
					
			}
		}
	}

	public void setupSetupArray() {
		setUpArray = new int[numOptions,roomWidth,roomHeight] {{ { 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0},{ 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0},
																{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},{ 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0},
																{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},{ 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
																{ 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1},{ 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
																{ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0},{ 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
																{ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0},{ 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0}},

																{ { 1, 1, 1, 1, 1, 1, 1,1, 1, 1, 1, 1},{ 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
																{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
																
																{ { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
																{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}}; 
		
		// setUpItemsArray = new int[numOptions,roomWidth,roomHeight] {{ { 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0},{ 0, 0, 0, 1, 0, 1, 0, 1, 0, 0, 1, 0},
		// 														{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},{ 0, 1, 0, 1, 0, 0, 1, 0, 1, 0, 1, 0},
		// 														{ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0},{ 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
		// 														{ 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1},{ 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
		// 														{ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0},{ 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0},
		// 														{ 0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 0},{ 0, 1, 0, 1, 0, 1, 0, 0, 1, 0, 1, 0}},

		// 														{ { 1, 1, 1, 1, 1, 1, 1,1, 1, 1, 1, 1},{ 1, 1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
		// 														{ 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 0},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}},
																
		// 														{ { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
		// 														{ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}}}; 
	}
}
