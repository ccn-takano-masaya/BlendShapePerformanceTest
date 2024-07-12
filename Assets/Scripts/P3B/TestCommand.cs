using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Cocone.P3B.Test
{
    public abstract class TestCommand
    {
        public enum Direction
        {
            SelfForward,
            SelfBackward,
            SelfRight,
            SelfLeft,
            SelfUp,
            SelfDown,
            WorldForward,
            WorldBackward,
            WorldRight,
            WorldLeft,
            WorldUp,
            WorldDown,
        }

        public float duration { get; protected set; }

        public virtual async UniTask Execute<T, U>(TestBase<T, U> test) where T : TestInputBase where U : TestOutputBase {}

        protected Vector3 GetVector(Direction dir, Transform trans)
        {
            switch (dir)
            {
                case Direction.SelfForward:
                    return trans.forward;
                case Direction.SelfBackward:
                    return -trans.forward;
                case Direction.SelfRight:
                    return trans.right;
                case Direction.SelfLeft:
                    return -trans.right;
                case Direction.SelfUp:
                    return trans.up;
                case Direction.SelfDown:
                    return -trans.up;
                case Direction.WorldForward:
                    return Vector3.forward;
                case Direction.WorldBackward:
                    return Vector3.back;
                case Direction.WorldRight:
                    return Vector3.right;
                case Direction.WorldLeft:
                    return Vector3.left;
                case Direction.WorldUp:
                    return Vector3.up;
                case Direction.WorldDown:
                    return Vector3.down;
            }
            return Vector3.zero;
        }
    }

    public class IdleCommand : TestCommand
    {
        public IdleCommand(float duration)
        {
            this.duration = duration;
        }

        public override async UniTask Execute<T, U>(TestBase<T, U> test)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
        }

        public override string ToString()
        {
            return $"Idle for {duration} second{(duration > 1 ? "s" : string.Empty)}";
        }
    }

    public class MoveCommand : TestCommand
    {
        public Direction direction;
        public float speed;

        public MoveCommand(Direction direction, float speed, float duration)
        {
            this.direction = direction;
            this.speed = speed;
            this.duration = duration;
        }

        public override async UniTask Execute<T, U>(TestBase<T, U> test)
        {
            var controller = test.cameraController;
            controller.Move(GetVector(direction, controller.transform), speed);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            controller.Stop();
        }

        public override string ToString()
        {
            return $"Move {direction} at speed of {speed} unit/s for {duration} second{(duration > 1 ? "s" : string.Empty)}";
        }
    }

    public class RotateCommand : TestCommand
    {
        public Direction axis;
        public float speed;

        public RotateCommand(Direction axis, float speed, float angle = 360)
        {
            this.speed = speed;
            this.duration = Mathf.Abs(angle / speed);
            this.axis = axis;
        }

        public override async UniTask Execute<T, U>(TestBase<T, U> test)
        {
            var controller = test.cameraController;
            controller.Rotate(GetVector(axis, controller.transform), speed);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            controller.Stop();
        }

        public override string ToString()
        {
            return $"Rotate at speed of {speed} degree/s for {duration} second{(duration > 1 ? "s" : string.Empty)}";
        }
    }

    public class RecordCommand : TestCommand
    {
        public override async UniTask Execute<T, U>(TestBase<T, U> test)
        {
            test.cameraController.Init();
        }

        public override string ToString()
        {
            return $"Record Position";
        }
    }

    public class ResetCommand : TestCommand
    {
        public override async UniTask Execute<T, U>(TestBase<T, U> test)
        {
            test.cameraController.Reset();
        }

        public override string ToString()
        {
            return $"Reset Position";
        }
    }

    public class ScreenshotCommand : TestCommand
    {
        public string name;

        public ScreenshotCommand(string name)
        {
            this.name = name;
            this.duration = 0;
        }

        public override async UniTask Execute<T, U>(TestBase<T, U> test)
        {
            test.paused = true;
            await UniTask.DelayFrame(10);
            await test.TakeScreenShot(name);
            await UniTask.DelayFrame(10);
            test.paused = false;
        }

        public override string ToString()
        {
            return $"Take Screenshot";
        }
    }
}