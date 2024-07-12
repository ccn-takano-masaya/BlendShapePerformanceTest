using UnityEngine;

namespace Cocone.P3B.Test
{
    public class GameObjectController : MonoBehaviour
    {
        private enum CommandType
        {
            None,
            Move,
            Rotate,
            RotateAround,
        }

        private Vector3 origianlPosition;
        private Quaternion originalRotation;
        private CommandType currentCommand;
        private Vector3 target;
        private Vector3 axis;
        private float speed;

        public bool isRunning => currentCommand != CommandType.None;

        void Start()
        {
            origianlPosition = transform.position;
            originalRotation = transform.rotation;
        }

        public void Move(Vector3 direction, float speed)
        {
            currentCommand = CommandType.Move;
            this.target = direction;
            this.speed = speed;
        }

        public void Rotate(Vector3 axis, float speed)
        {
            currentCommand = CommandType.Rotate;
            this.axis = axis;
            this.speed = speed;
        }

        public void RotateAround(Vector3 target, Vector3 axis, float speed)
        {
            currentCommand = CommandType.RotateAround;
            this.target = target;
            this.axis = axis;
            this.speed = speed;
        }

        public void Stop()
        {
            currentCommand = CommandType.None;
        }

        public void Init()
        {
            origianlPosition = transform.position;
            originalRotation = transform.rotation;
        }

        public void Reset()
        {
            transform.position = origianlPosition;
            transform.rotation = originalRotation;
        }

        void Update()
        {
            switch (currentCommand)
            {
                case CommandType.Move:
                    transform.Translate(target * Time.deltaTime * speed, Space.World);
                    break;
                case CommandType.Rotate:
                    transform.Rotate(axis, Time.deltaTime * speed, Space.World);
                    break;
                case CommandType.RotateAround:
                    transform.RotateAround(target, axis, Time.deltaTime * speed);
                    break;
            }
        }
    }
}