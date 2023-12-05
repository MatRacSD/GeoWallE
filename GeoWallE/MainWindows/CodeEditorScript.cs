using Godot;
using Compiler;
using System;

public partial class CodeEditorScript : CodeEdit
{
	OutputConsole outputConsole;
	string valuex = "null";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		  outputConsole = GetNode<OutputConsole>("/root/Main/CodeEditor/OutputConsole");
		
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
		if(Input.IsKeyPressed(Key.F5))
		{
			valuex = CompilerUtils.Run(Text);
			GD.Print(valuex);
			
			
			
			outputConsole.Text = valuex;
			
		}
		
	}

	
}
