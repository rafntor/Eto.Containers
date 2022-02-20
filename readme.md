# Eto.Containers

[![Build](https://github.com/rafntor/Eto.Containers/actions/workflows/build.yml/badge.svg)](https://github.com/rafntor/Eto.Containers/actions/workflows/build.yml)
[![NuGet](https://img.shields.io/nuget/v/Eto.Containers)](https://www.nuget.org/packages/Eto.Containers/)
[![License](https://img.shields.io/github/license/rafntor/Eto.Containers)](LICENSE)

Provides some extra [Eto.Forms](https://github.com/picoe/Eto) containers that adds various Zoom/Drag/Scroll-features:
- `Eto.Containers.DragScrollable` - a scrollable container where content can also be dragged.
- `Eto.Containers.DragScrollable` - a scrollable container where content can also be dragged.
- `Eto.Containers.DragScrollable` - a scrollable container where content can also be dragged.
- `Eto.Containers.DragScrollable` - a scrollable container where content can also be dragged.

Demo applications : https://nightly.link/rafntor/Eto.Containers/workflows/build/master

## Quickstart

Use NuGet to install [`Eto.Containers`](https://www.nuget.org/packages/Eto.Containers/), then use some of the following in your Form or Container:
```cs
   this.Content = new DragScrollable { Content = SomeOtherControl };
```
