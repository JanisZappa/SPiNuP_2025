using System;
using System.Threading;
using UnityEngine;

namespace Test
{
	public class ThreadingTest : MonoBehaviour, IThreadJob
	{
		public int count;
		public int max;

		[Space(10)] public float stepLength;
		public float waitLength;

		public void Work()
		{
			count++;
		}

		public bool IsDone => count >= max;

		private ThreadJob job;

		public void CallBack(bool success)
		{
			Debug.Log(success ? "Done Counting" : "Thread Aborted");
		}

		private void Awake()
		{
			job = new ThreadJob(this, stepLength, waitLength);
		}


		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
				job.Abort();
		}
	}


	public interface IThreadJob
	{
		void Work();
		void CallBack(bool success);
		bool IsDone { get; }
	}


	public class ThreadJob
	{
		private readonly Thread _thread;

		private readonly IThreadJob _threadJob;
		private readonly int _stepMs;
		private readonly int _waitMs;


		public ThreadJob(IThreadJob threadJob, float stepLength, float sleepTime)
		{
			_threadJob = threadJob;
			_stepMs = (int)(stepLength * 1000);
			_waitMs = (int)(sleepTime * 1000);
			_thread = new Thread(ThreadedWork);

			_thread.Start();
		}


		private void ThreadedWork()
		{
			DateTime _lastStep = DateTime.Now;

			while (!_threadJob.IsDone)
			{
				_threadJob.Work();

				if ((DateTime.Now - _lastStep).Milliseconds > _stepMs)
				{
					Thread.Sleep(_waitMs);
					_lastStep = DateTime.Now;
				}
			}

			_threadJob.CallBack(true);
		}


		public void Abort()
		{
			if (_thread.IsAlive)
				_thread.Abort();

			_threadJob.CallBack(false);
		}
	}
}
