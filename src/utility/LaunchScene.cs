using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;

namespace Underworld
{
	[Meta(typeof(IAutoNode))]
    public partial class LaunchScene : Node
	{
        public override void _Notification(int what) => this.Notify(what);

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
		{
			GD.Print("Launch scene now loading Underworld.tscn.");
			GetTree().ChangeSceneToFile("res://Underworld.tscn");
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}
	}
}
