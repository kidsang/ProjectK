// 检查是否忘记调用Dispose方法
#define CHECK_DISPOSE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.ProjectK.Base
{
    public abstract class Disposable : IDisposable
    {
        private bool disposed = false;
        private bool onDisposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            OnDispose();
            disposed = true;

            if (!onDisposed)
                Log.Error("SubClass did not call SuperClass.OnDispose()!");
        }

        virtual protected void OnDispose()
        {
            onDisposed = true;
        }

        public bool Disposed
        {
            get { return disposed; }
        }

#if CHECK_DISPOSE
        private string classString;
        private string createStack;

        public Disposable()
        {
            classString = this.ToString();
            createStack = Environment.StackTrace;
        }

        ~Disposable()
        {
            if (!disposed && !CheckDisposeUtils.ShutingDown)
                CheckDisposeUtils.LogCreateStack(classString, createStack);
        }
#endif
    }

    public class DisposableBehaviour : MonoBehaviour, IDisposable
    {
        private bool disposed = false;
        private bool onDisposed = false;

        public void Dispose()
        {
            if (disposed)
                return;

            OnDispose();
            disposed = true;

            if (!onDisposed)
                Log.Error("SubClass did not call SuperClass.OnDispose()!");
        }

        virtual protected void OnDispose()
        {
            onDisposed = true;
        }

        public bool Disposed
        {
            get { return disposed; }
        }

#if CHECK_DISPOSE
        private string classString;
        private string createStack;
        virtual public void Awake()
        {
            classString = this.ToString();
            createStack = Environment.StackTrace;
        }

        virtual public void OnDestroy()
        {
            if (!disposed && !CheckDisposeUtils.ShutingDown)
                CheckDisposeUtils.LogCreateStack(classString, createStack);
        }

        virtual public void OnApplicationQuit()
        {
            CheckDisposeUtils.ShutingDown = true;
        }
#endif
    }

#if CHECK_DISPOSE
    internal class CheckDisposeUtils
    {
        internal static bool ShutingDown = false;

        internal static void LogCreateStack(string classString, string createStack)
        {
            string[] arr = createStack.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            createStack = "";
            for (int i = 2; i < arr.Length; ++i)
                createStack += arr[i] + "\n";

            Debug.LogError("未调用Dispose方法 [" + classString + "]\n创建堆栈：\n" + createStack);
        }
    }
#endif
}
