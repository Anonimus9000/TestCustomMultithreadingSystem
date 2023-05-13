using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultithreadingTest
{
    public class Dispatcher : MonoBehaviour
    {
        private Queue<Action> _updateQueue;
        private Queue<Action> _lateUpdateQueue;
        private Queue<Action> _fixedUpdateQueue;

        public void Initialize(int capacity)
        {
            _updateQueue = new Queue<Action>(capacity);
            _lateUpdateQueue = new Queue<Action>(capacity);
            _fixedUpdateQueue = new Queue<Action>(capacity);
        }

        public void InvokeInFrame(Action action)
        {
            _updateQueue.Enqueue(action);
        }
        
        public void InvokeLateFrame(Action action)
        {
            _lateUpdateQueue.Enqueue(action);
        }
        
        public void InvokeFixedFrame(Action action)
        {
            _fixedUpdateQueue.Enqueue(action);
        }

        private void Update()
        {
            while (_updateQueue.Count > 0)
            {
                var dequeue = _updateQueue.Dequeue();
                dequeue?.Invoke();
            }
        }

        private void LateUpdate()
        {
            while (_lateUpdateQueue.Count > 0)
            {
                var dequeue = _lateUpdateQueue.Dequeue();
                dequeue?.Invoke();
            }
        }

        private void FixedUpdate()
        {
            while (_fixedUpdateQueue.Count > 0)
            {
                var dequeue = _fixedUpdateQueue.Dequeue();
                dequeue?.Invoke();
            }
        }
    }
}