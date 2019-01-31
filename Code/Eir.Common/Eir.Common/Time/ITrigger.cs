using System;
using System.Threading.Tasks;

namespace Eir.Common.Time
{
    /*
     * This trigger mechanism provides the means to invert the reponsibility around
     * functionality that need to be triggered, for instance on an interval. Instead
     * of having the class containing the functionality orchestrating a Timer and running
     * the interval mechanism, it is provided with an ITrigger, that will invoke
     * a registered method on a given interval.
     * 
     * The upside is that you can also provide another trigger implementation, where you
     * have exact control over when it trigs. This is extremely useful for writing automated
     * tests for interval based functionality...
     * 
     */

    public interface ITrigger : IDisposable
    {
        void RegisterAction(Func<Task> action);
        void UnregisterAction(Func<Task> action);
    }
}