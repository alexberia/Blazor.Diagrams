using Blazor.Diagrams.Core.Events;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;
using Blazor.Diagrams.Core.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.Diagrams.Core.Behaviors
{
    public class MomentumBehavior : Behavior
    {
        private readonly Dictionary<NodeModel, NodeMomentum> _momentums = new();
        private bool _isDragging = false;

        private readonly Dictionary<MovableModel, Point> _initialPositions;
        private double? _lastClientX;
        private double? _lastClientY;
        private bool _moved;
        private NodeModel? _movingNode;
        public MomentumBehavior(Diagram diagram) : base(diagram)
        {
            _initialPositions = new Dictionary<MovableModel, Point>();
            Diagram.PointerDown += OnPointerDown;
            Diagram.PointerMove += OnPointerMove;
            Diagram.PointerUp += OnPointerUp;
        }

        private void OnPointerDown(Model? model, PointerEventArgs e)
        {
            if (!Diagram.Options.Momentum.Enabled)
                return;

            _movingNode = null;
            _momentums.Clear();

            if (model is NodeModel node)
            {
                _movingNode = node;

                if (!_momentums.ContainsKey(node))
                    _momentums[node] = new NodeMomentum();

                var data = _momentums[node];
                data.LastPosition = node.Position;
                data.LastTime = DateTime.Now;
            }

        }

        private void OnPointerMove(Model? model, PointerEventArgs e)
        {
            if (!Diagram.Options.Momentum.Enabled)
                return;

            if (_movingNode != null)
            {
                var node = _movingNode;

                if (!_momentums.ContainsKey(node))
                    _momentums[node] = new NodeMomentum();

                var data = _momentums[node];
                var now = DateTime.Now;
                var dt = (now - data.LastTime).TotalSeconds;

                if (dt > 0)
                {
                    var dx = node.Position.X - data.LastPosition.X;
                    var dy = node.Position.Y - data.LastPosition.Y;

                    data.Velocity = new Point(dx / dt, dy / dt);
                    data.LastPosition = node.Position;
                    data.LastTime = now;
                }
            }
        }

        private void OnPointerUp(Model? model, PointerEventArgs e)
        {
            if (!Diagram.Options.Momentum.Enabled)
                return;

            if (_movingNode is not null && _momentums.TryGetValue(_movingNode, out var momentum))
            {
                var node = _movingNode;
                _movingNode = null;

                if (momentum.Velocity == Point.Zero)
                    return;

                _ = ApplyMomentumAsync(node, momentum.Velocity);
            }
        }

        private async Task ApplyMomentumAsync(NodeModel node, Point velocity)
        {
            double friction = Diagram.Options.Momentum.friction; // quanto menor, mais rápido para. 0.88
            double threshold = Diagram.Options.Momentum.threshold; // parar quando velocidade for baixa
            int interval = Diagram.Options.Momentum.interval; // aproximadamente 60fps

            while (Math.Abs(velocity.X) > threshold || Math.Abs(velocity.Y) > threshold)
            {
                var pos = node.Position;
                var newX = pos.X + velocity.X * 0.016; // 0.016 = 1/60 segundo
                var newY = pos.Y + velocity.Y * 0.016;

                node.SetPosition(newX, newY);
                velocity = new Point(velocity.X * friction, velocity.Y * friction);

                await Task.Delay(interval);
            }
        }

        public override void Dispose()
        {
            Diagram.PointerDown -= OnPointerDown;
            Diagram.PointerMove -= OnPointerMove;
            Diagram.PointerUp -= OnPointerUp;
        }
    }
}
