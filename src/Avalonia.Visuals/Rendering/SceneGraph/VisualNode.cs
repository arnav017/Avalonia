﻿// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Collections.Generic;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Avalonia.Rendering.SceneGraph
{
    public class VisualNode : IVisualNode
    {
        public VisualNode(IVisual visual, IVisualNode parent)
        {
            if (parent == null && visual.VisualParent != null)
            {
                throw new AvaloniaInternalException(
                    "Attempted to create root VisualNode for parented visual.");
            }

            Visual = visual;
            Parent = parent;
            Children = new List<ISceneNode>();
        }

        public IVisual Visual { get; }
        public IVisualNode Parent { get; }
        public Matrix Transform { get; set; }
        public Rect ClipBounds { get; set; }
        public bool ClipToBounds { get; set; }
        public Geometry GeometryClip { get; set; }
        public double Opacity { get; set; }
        public IBrush OpacityMask { get; set; }
        public List<ISceneNode> Children { get; }
        public bool SubTreeUpdated { get; set; }

        IReadOnlyList<ISceneNode> IVisualNode.Children => Children;

        public VisualNode Clone(IVisualNode parent)
        {
            return new VisualNode(Visual, parent)
            {
                Transform = Transform,
                ClipBounds = ClipBounds,
                ClipToBounds = ClipToBounds,
                GeometryClip = GeometryClip,
                Opacity = Opacity,
                OpacityMask = OpacityMask,
            };
        }

        public bool HitTest(Point p)
        {
            foreach (var child in Children)
            {
                var drawNode = child as IDrawNode;

                if (drawNode?.HitTest(p) == true)
                {
                    return true;
                }
            }

            return false;
        }

        public void Render(IDrawingContextImpl context)
        {
            context.Transform = Transform;

            if (Opacity != 1)
            {
                context.PushOpacity(Opacity);
            }

            if (ClipToBounds)
            {
                context.PushClip(ClipBounds * Transform.Invert());
            }

            foreach (var child in Children)
            {
                child.Render(context);
            }

            if (ClipToBounds)
            {
                context.PopClip();
            }

            if (Opacity != 1)
            {
                context.PopOpacity();
            }
        }
    }
}