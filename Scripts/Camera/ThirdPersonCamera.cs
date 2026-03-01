using Godot;
using System;

namespace Diablo.Scripts.Camera
{
    public partial class ThirdPersonCamera : Camera3D
    {
        [Export]
        public NodePath TargetPath = new NodePath("");
        [Export]
        public bool AutoFindTarget = true;
        [Export]
        public string TargetGroup = "player";

        [Export]
        public float Distance = 5.0f;
        [Export]
        public float MinDistance = 2.0f;
        [Export]
        public float MaxDistance = 12.0f;
        [Export]
        public float ZoomSpeed = 1.0f;

        [Export]
        public float Yaw = 0.0f; // radians
        [Export]
        public float Pitch = -0.2f;
        [Export]
        public float MinPitch = -1.0472f; // -60 degrees
        [Export]
        public float MaxPitch = 1.0472f; // 60 degrees

        [Export]
        public float RotationSensitivity = 0.003f;
        [Export]
        public float PanSensitivity = 0.002f;
        [Export]
        public float SmoothSpeed = 10.0f;

        private Node3D _target = null;
        private float _desiredDistance;
        private float _desiredYaw;
        private float _desiredPitch;
        private Vector3 _pivotOffset = Vector3.Zero;
        private Vector3 _desiredPivotOffset = Vector3.Zero;

        public override void _Ready()
        {
            _desiredDistance = Distance;
            _desiredYaw = Yaw;
            _desiredPitch = Pitch;

            if (TargetPath != null && TargetPath != new NodePath(""))
            {
                if (HasNode(TargetPath))
                {
                    _target = GetNode<Node3D>(TargetPath);
                }
                else if (AutoFindTarget)
                {
                    var candidates = GetTree().GetNodesInGroup(TargetGroup);
                    if (candidates.Count > 0)
                    {
                        _target = candidates[0] as Node3D;
                    }
                }
            }
            else if (AutoFindTarget)
            {
                var candidates = GetTree().GetNodesInGroup(TargetGroup);
                if (candidates.Count > 0)
                {
                    _target = candidates[0] as Node3D;
                }
            }
        }

        public void SetTarget(Node3D node)
        {
            _target = node;
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseMotion motion)
            {
                if (Input.IsActionPressed("camera_rotate"))
                {
                    _desiredYaw -= motion.Relative.X * RotationSensitivity;
                    _desiredPitch -= motion.Relative.Y * RotationSensitivity;
                    _desiredPitch = Mathf.Clamp(_desiredPitch, MinPitch, MaxPitch);
                }
                else if (Input.IsActionPressed("camera_pan"))
                {
                    var right = GlobalTransform.Basis.X;
                    var up = GlobalTransform.Basis.Y;
                    _desiredPivotOffset += (-right * motion.Relative.X + up * motion.Relative.Y) * PanSensitivity * _desiredDistance;
                }
            }

            if (@event is InputEventMouseButton mb)
            {
                if (mb.ButtonIndex == MouseButton.WheelUp && mb.Pressed)
                {
                    _desiredDistance = Math.Max(MinDistance, _desiredDistance - ZoomSpeed);
                }
                else if (mb.ButtonIndex == MouseButton.WheelDown && mb.Pressed)
                {
                    _desiredDistance = Math.Min(MaxDistance, _desiredDistance + ZoomSpeed);
                }
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            // keyboard zoom
            if (Input.IsActionPressed("camera_zoom_in"))
            {
                _desiredDistance = Math.Max(MinDistance, _desiredDistance - ZoomSpeed * (float)delta * 60f);
            }
            if (Input.IsActionPressed("camera_zoom_out"))
            {
                _desiredDistance = Math.Min(MaxDistance, _desiredDistance + ZoomSpeed * (float)delta * 60f);
            }

            // Smoothly interpolate
            Yaw = Mathf.Lerp(Yaw, _desiredYaw, Mathf.Clamp((float)delta * SmoothSpeed, 0f, 1f));
            Pitch = Mathf.Lerp(Pitch, _desiredPitch, Mathf.Clamp((float)delta * SmoothSpeed, 0f, 1f));
            Distance = Mathf.Lerp(Distance, _desiredDistance, Mathf.Clamp((float)delta * SmoothSpeed, 0f, 1f));
            _pivotOffset = _pivotOffset.Lerp(_desiredPivotOffset, Mathf.Clamp((float)delta * SmoothSpeed, 0f, 1f));

            if (_target != null)
            {
                var targetPos = _target.GlobalTransform.Origin + _pivotOffset;
                var rot = new Basis(Vector3.Up, Yaw).Rotated(Vector3.Right, Pitch);
                var offset = rot * new Vector3(0f, 0f, Distance);
                GlobalPosition = targetPos - offset;
                LookAt(targetPos, Vector3.Up);
            }
        }
    }
}
