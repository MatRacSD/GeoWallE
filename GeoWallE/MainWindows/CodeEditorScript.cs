using Godot;
using Compiler;
using System;
using System.IO;
using System.Collections.Generic;
using System.Formats.Asn1;
using Godot.NativeInterop;
using System.Runtime.InteropServices;

public partial class CodeEditorScript : CodeEdit
{
	private OutputConsole outputConsole;
	private Sprite2D plane;

	private TextEdit saveBox;
	private bool canPressF5 = true;
	private Timer delayTimer;
	string valuex = "null";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		outputConsole = GetNode<OutputConsole>("/root/Main/CodeEditor/OutputConsole");
		plane = GetNode<Sprite2D>("/root/Main/Plane/PlaneSpriteContainer");
		saveBox = GetNode<TextEdit>("/root/Main/CodeEditor/TextEdit");

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

		if (canPressF5 && Input.IsKeyPressed(Key.F5))
		{
			var planeSprites = plane.GetChildren();
			foreach (var child in planeSprites)
			{
				child.QueueFree();
			}
			canPressF5 = false;
			Compiler.State state = new();
			state.Run(Text);

				
			outputConsole.Text = ">>\n" ;

			//state.toDraw
			foreach (var item in state.errors)
			{
			   outputConsole.Text += item.Error;	
			}
			foreach (var item in state.constants)
			{
				outputConsole.Text += " constant:" +  item.Key.GetType() + "--Value: "+ item.Value.Value.GetType() + "\n";
			}
			foreach (Compiler.Node fig in state.toDraw)
			{
				switch (fig.GetType().ToString())
				{
					case "Compiler.PointNode":

						Sprite2D sprite = new Sprite2D();
						sprite.Texture = GD.Load<Texture2D>("res://Point.png");

						double[] asd = ((PointNode)fig).GetPair();
						sprite.Position = new Vector2((float)asd[0], (float)asd[1]);
						sprite.Scale = new Vector2(0.01f, 0.01f);
						plane.AddChild(sprite);
						continue;

					case "Compiler.LineNode":

						Line2D line2D = new Line2D();
						LineNode toDraw = (LineNode)fig;
						double[] p1 = toDraw.pointA.GetPair();
						double[] p2 = toDraw.pointB.GetPair();
						float[] toDrawPoints = CalculateLinePoints((float)p1[0], (float)p1[1], (float)p2[0], (float)p2[1]);

						line2D.Points = new Vector2[] { new(toDrawPoints[0], toDrawPoints[1]), new(toDrawPoints[2], toDrawPoints[3]) };
						line2D.DefaultColor = new Color(1, 0.3f, 0.3f, 1);
						line2D.Width = 2;
						plane.AddChild(line2D);
						 break;
					case "Compiler.CircleNode":
						CircleNode c = (CircleNode)fig;
						Circle circle = new((float)((PointNode)c.center).xValue, (float)((PointNode)c.center).yValue, Math.Abs((float)c.radio.Value), 100);
						var circleLines = circle.Draw();
						foreach (var cline in circleLines)
						{
							plane.AddChild(cline);
						}
						continue;
					case "Compiler.SegmentNode":
						 Line2D line2D2 = new Line2D();
						 SegmentNode toDraw2 = (SegmentNode)fig;
						double[] p11 = toDraw2.pointA.GetPair();
						double[] p21 = toDraw2.pointB.GetPair();
						line2D2.Points = new Vector2[]{new((float)p11[0],(float)p11[1]),new((float)p21[0],(float)p21[1])};
						line2D2.DefaultColor = new Color(1, 0.3f, 0.3f, 1);
						line2D2.Width = 2;
						plane.AddChild(line2D2);
						break;
						case "Compiler.RayNode":
						Line2D line2D1 = new();
						RayNode rayToDraw = (RayNode)fig;
						double[] p12 = rayToDraw.pointA.GetPair();
						double[] p22 = rayToDraw.pointB.GetPair();
						float[] proyection = CalculateRayPoints((float)p12[0],(float)p12[1],(float)p22[0],(float)p22[1]);
						line2D1.Points = new Vector2[]{new((float)p12[0],(float)p12[1]),new(proyection[0],proyection[1])};
						line2D1.DefaultColor = new Color(1, 0.3f, 0.3f, 1);
						line2D1.Width = 2;
						plane.AddChild(line2D1);
						break;
						case "Compiler.ArcNode":
						ArcNode arc = (ArcNode)fig;
						var center = arc.center.GetPair();
						var arcp1 = arc.p1.GetPair();
						var arcp2 = arc.p2.GetPair();
						var arcR = (float)arc.radio.Value;
						
						Arc arcToDraw = new((float)center[0],(float)center[1],new float[]{(float)arcp1[0],(float)arcp1[1]},new float[]{(float)arcp2[0],(float)arcp2[1]},arcR);
						Line2D[] arcLines = arcToDraw.Draw();
						foreach (Line2D item in arcLines)
						{
							plane.AddChild(item);
						}
						break;
					default:
						continue;
				}
			}
			foreach (string item in state.toPrint)
			{
				outputConsole.Text += item + "\n";
			}
			outputConsole.Text += "<< ";
			/*
			Circle circulo = new(100,100,100,100);
			 var asdd=circulo.Draw();
			 foreach (var item in asdd)
			 {
				plane.AddChild(item);
			 }*/

			 //Arc arc = new(0,0,new float[]{100,-100},new float[]{100,-200},70);
			 
			delayTimer.Stop();
			delayTimer.Start();




		}
		if(canPressF5 && Input.IsKeyPressed(Key.F9))
		{
			canPressF5 = false;
			try{
			CreateFile(saveBox.Text,Text);
			}
			catch(Exception e)
			{
				outputConsole.Text = e.ToString();
			}
			delayTimer.Stop();
			delayTimer.Start();
		}
	}
	public void CreateFile(string title, string text)
	{
		string filename = title + ".geo";
		File.WriteAllText(filename, text);
	}
	public float[] CalculateLinePoints(float x1, float y1, float x2, float y2) ///Para representar la linea
	{
		float[] result = new float[4];

		if (x1 == x2) // The line is vertical
		{
			// The x-coordinates are the same for both points, and y-coordinates are at the edges of the plane
			result[0] = x1; // x-coordinate for point 1
			result[1] = -360; // y-coordinate for point 1
			result[2] = x1; // x-coordinate for point 2
			result[3] = 360; // y-coordinate for point 2
		}
		else // The line is not vertical
		{
			// Calculate the slope of the line
			float m = (y2 - y1) / (x2 - x1);

			// Calculate the y-intercept of the line
			float b = y1 - m * x1;

			// Calculate the x-coordinates of the points where the line intersects the edges of the plane
			float xLeft = -360;
			float xRight = 360;

			// Calculate the corresponding y-coordinates
			float yLeft = m * xLeft + b;
			float yRight = m * xRight + b;

			// Set the points in the result array
			result[0] = xLeft;
			result[1] = yLeft;
			result[2] = xRight;
			result[3] = yRight;
		}

		return result;
	}
	public float[] CalculateRayPoints(float x1,float y1,float x2,float y2)
	{
float[] result = new float[2];

		if (x1 == x2) // The line is vertical
		{
			if(y1 < y2)
			{
result[0] = x1; // x-coordinate for point 1
			result[1] = 360;
			}
			else{
			// The x-coordinates are the same for both points, and y-coordinates are at the edges of the plane
			result[0] = x1; // x-coordinate for point 1
			result[1] = -360; // y-coordinate for point 1
			 // y-coordinate for point 2
		}}
		else // The line is not vertical
		{
			
			// Calculate the slope of the line
			float m = (y2 - y1) / (x2 - x1);

			// Calculate the y-intercept of the line
			float b = y1 - m * x1;

			// Calculate the x-coordinates of the points where the line intersects the edges of the plane
			float xLeft = -360;
			float xRight = 360;

			// Calculate the corresponding y-coordinates
			float yLeft = m * xLeft + b;
			float yRight = m * xRight + b;

			// Set the points in the result array
			if(x1 > x2)
			{
			   result[0] = xLeft;
			result[1] = yLeft;	
			}
			else 
			{
				result[0] = xRight;
			result[1] = yRight;
			}
			
		}

		return result;
	}
	public class Circle
	{
		private float centerX;
		private float centerY;
		private float radius;
		private int segments;

		public Circle(float centerX, float centerY, float radius, int segments)
		{
			this.centerX = centerX;
			this.centerY = centerY;
			this.radius = radius;
			this.segments = segments;
		}

		public Line2D[] Draw()
		{
			Line2D[] lines = new Line2D[segments];

			for (int i = 0; i < segments; i++)
			{
				// Calculate the start and end angles for this segment
				float startAngle = (float)(i * 2 * Math.PI / segments);
				float endAngle = (float)((i + 1) * 2 * Math.PI / segments);

				// Calculate the start and end points for this segment
				float startX = centerX + radius * (float)Math.Cos(startAngle);
				float startY = centerY + radius * (float)Math.Sin(startAngle);
				float endX = centerX + radius * (float)Math.Cos(endAngle);
				float endY = centerY + radius * (float)Math.Sin(endAngle);

				// Create a new Line2D object for this segment
				Line2D line2D = new();
				line2D.Points = new Vector2[] { new(startX, startY), new(endX, endY) };
				line2D.Width = 2;
				line2D.DefaultColor = new Color(1, 0.3f, 0.3f, 1);
				lines[i] = line2D;
			}

			return lines;
		}
	}
	public class Arc
	{

	
	private float centerX;
	private float centerY;
	private float radius;
	private int segments;
	private float startAngle;
	private float endAngle;

	public Arc(float centerX, float centerY, float[] p2, float[] p1, float m)
	{
		this.centerX = centerX;
		this.centerY = centerY;
		this.radius = m;

		// Calculate the start and end angles based on the given points
		this.startAngle = (float) Math.Atan2(p1[1] - centerY, p1[0] - centerX);
		this.endAngle = (float) Math.Atan2(p2[1] - centerY, p2[0] - centerX);

		// Ensure that the arc is drawn counter-clockwise
		if (endAngle <= startAngle)
		{
			endAngle += (float) (2 * Math.PI);
		}

		// Calculate the number of segments based on the radius
		this.segments = (int)Math.Max(10, radius * 2); // Here we use a simple heuristic: 10 segments per radius unit
	}

	public Line2D[] Draw()
	{
		List<Line2D> lines = new List<Line2D>();

		for (int i = 0; i < segments; i++)
		{
			// Calculate the angles for this segment
			float segmentStartAngle = startAngle + i * (endAngle - startAngle) / segments;
			float segmentEndAngle = startAngle + (i + 1) * (endAngle - startAngle) / segments;

			// Calculate the start and end points for this segment
			float startX = centerX + radius * (float) Math.Cos(segmentStartAngle);
			float startY = centerY + radius * (float) Math.Sin(segmentStartAngle);
			float endX = centerX + radius * (float) Math.Cos(segmentEndAngle);
			float endY = centerY + radius * (float) Math.Sin(segmentEndAngle);

			// Create a new Line2D object for this segment
		   
		
			// Create a new Line2D object for this segment
			Line2D line = new();
			line.Points = new Vector2[]{new(startX,startY),new(endX,endY)};
			line.Width = 2;
			line.DefaultColor = new Color(1, 0.3f, 0.3f, 1);
			lines.Add(line);
		}

		return lines.ToArray();
	}
}
}




