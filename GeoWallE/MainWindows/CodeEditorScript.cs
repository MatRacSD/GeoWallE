using Godot;
using Compiler;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using Godot.NativeInterop;

public partial class CodeEditorScript : CodeEdit
{
	private OutputConsole outputConsole;
	private Sprite2D plane;
	private bool canPressF5 = true;
	private Timer delayTimer;
	string valuex = "null";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		  outputConsole = GetNode<OutputConsole>("/root/Main/CodeEditor/OutputConsole");
		  plane = GetNode<Sprite2D>("/root/Main/Plane/PlaneSpriteContainer");
		   // Create a timer node
		delayTimer = new Timer();
		AddChild(delayTimer);

		// Set the timer's wait time to the desired delay
		delayTimer.WaitTime = 0.5f; // 1 second delay

		// Connect the timer's timeout signal to a method
		delayTimer.Connect("timeout", new Callable(this, nameof(EnableF5Press)));

		// Start the timer
		delayTimer.Start();
	}
	private void EnableF5Press()
	{
		// Enable F5 key presses after the delay
		canPressF5 = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		if(canPressF5 && Input.IsKeyPressed(Key.F5))
		{
			var  planeSprites = plane.GetChildren();
			foreach (var child in planeSprites)
			{
				child.QueueFree();
			}
			canPressF5 = false;
			State state = new();
			List<PointNode> pointNodes = state.RunTest1(Text);
			//if(true)outputConsole.Text = pointNodes[0].name;
			outputConsole.Text = " ";
			
			foreach (ErrorNode item in state.errorNodes)
			{
				outputConsole.Text = "An exception ocurred: " + item.Error + "\n";
			}
			 foreach (PointNode pn in pointNodes)
			{
				Sprite2D sprite = new Sprite2D();
				sprite.Texture = GD.Load<Texture2D>("res://Point.png");
				
				double[] asd = pn.GetPair();
				sprite.Position = new Vector2((float)asd[0],(float)asd[1]);
				sprite.Scale = new Vector2(0.01f,0.01f);
				plane.AddChild(sprite);
			}
			delayTimer.Stop();
			delayTimer.Start();
			
			
			
			
		}
		
	}

	
}
