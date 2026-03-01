using Godot;
using System;

public partial class Playground : Node3D
{
	private double timer = 0;
	private const double INTERVAL = 5.0; // 5秒间隔

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("游戏开始运行");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer += delta;
		
		if (timer >= INTERVAL)
		{
			GD.Print("hello");
			timer = 0; // 重置计时器
		}
	}
}
