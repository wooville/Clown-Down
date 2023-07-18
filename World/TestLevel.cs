using Godot;
using System;

public partial class TestLevel : Node2D
{
	//4 rooms 12 wide with walls
	const int roomWidth = 12;
	const int horRooms = 3;
	int width;
	//3 rooms 9 high with walls
	const int roomHeight = 9;
	const int vertRooms = 3;
	int height; 

	Vector2I reference = new Vector2I(0,0); 
	
	[Export]
	public Godot.TileMap map;

	const int numOptions = 1;
	int[,,] setUpArray;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		height = roomHeight*vertRooms + vertRooms + 1;
		width = roomWidth*horRooms + horRooms + 1;

		map = GetChild(3) as Godot.TileMap;

		setupSetupArray();

		for (int i = 0; i < height; i++) {
			map.SetCell(0, new Vector2I(0,i),1, reference);
			map.SetCell(0, new Vector2I(width-1,i),1, reference);
		}
		for (int i = 0; i < width; i++) {
			map.SetCell(0, new Vector2I(i,0),1, reference);
			map.SetCell(0, new Vector2I(i,height-1),1, reference);
		}

		// Step through each room pair starting at 0,0
		for (int i = 0; i < horRooms; i++){
			for (int j = 0; j < vertRooms; j++) {
				if (i == 0 && j == 0) {
					buildRoom(i,j,0);
				} else {
					buildRoom(i,j,0);
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
				if (setUpArray[seed,i,j] == 1) {
					map.SetCell(0, new Vector2I(startX + i, startY + j),1, reference);
				}
			}
		}
	}

	public void setupSetupArray() {
		setUpArray = new int[numOptions,roomWidth,roomHeight] {{ { 0, 0, 0, 1, 0, 1, 0, 1, 0},{ 0, 0, 0, 1, 0, 1, 0, 1, 0},
																{ 0, 0, 0, 0, 0, 0, 0, 0, 0},{ 0, 1, 0, 1, 0, 1, 0, 1, 0},
																{ 0, 0, 0, 0, 0, 0, 0, 0, 0},{ 0, 0, 0, 0, 0, 0, 0, 0, 0},
																{ 1, 1, 1, 1, 0, 1, 1, 1, 1},{ 0, 0, 0, 0, 0, 0, 0, 0, 0},
																{ 0, 1, 0, 1, 0, 1, 0, 1, 0},{ 0, 0, 0, 0, 0, 0, 0, 0, 0},
																{ 0, 1, 0, 1, 0, 1, 0, 1, 0},{ 0, 1, 0, 1, 0, 1, 0, 1, 0}}}; 
	}
}
