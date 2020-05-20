// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿/*********************************************************************
*
* Project Name" 185-SenderBase
*
* Description: 
*
* Date of creation: 
*
* Author: 
*
* NOTES:
*               "FldOrder" for UX 10 to 
*********************************************************************/
//#define TESTDIRECTUPDATES
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

using nsCDEngine.Engines;
using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.ViewModels;
using nsCDEngine.Engines.StorageService;
using nsCDEngine.Engines.NMIService;
using nsCDEngine.Engines.ThingService;
using nsTheThingToPublish;
using TheCommonMessageContracts;
//using CDMyOPCUAClient.Contracts;

#if LEGACYONCHANGE
namespace nsTheSenderBase
{
    public abstract partial class TheSenderBase : TheConnectionBase
    {

        public long ThingEventsGenerated
        {
            get { return (long) TheThing.GetSafePropertyNumber(MyBaseThing, "ThingEventsGenerated"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "ThingEventsGenerated", value); }
        }
        public long PropertiesPerThingGenerated
        {
            get { return (long) TheThing.GetSafePropertyNumber(MyBaseThing, "PropertiesPerThingGenerated"); }
            set { TheThing.SetSafePropertyNumber(MyBaseThing, "PropertiesPerThingGenerated", value); }
        }


#region SENDPARAMETERS

        int numAmqpConnections = 32;

        TimeSpan SendBatchFlushInterval = new TimeSpan(0, 0, 5);

        const int numThings = 1;
        const int numPropertiesPerThing = 10;
        const int thingDelayBetweenUpdates = 5000;

#endregion

        class RegisteredThing
        {
            public ICDEThing thingToSend;
            public Timer sendTimer;
        }
        List<RegisteredThing> registeredThings = new List<RegisteredThing>();


        CancellationTokenSource stressCancelationTokenSource = new CancellationTokenSource();

        async void StressLoop(object stressThingObj)
        {
            Random r = new Random();
            TheThing stressThing = stressThingObj as TheThing;
            //cdeP prop = new cdeP(stressThing);


            var timer = new System.Threading.Timer(GenerateStressThingData, stressThing, 0, thingDelayBetweenUpdates);


            bool bCanceled = false;
            do
            {
                try
                {
                    await TheCommonUtils.TaskDelayOneEye(5000, 100);
                    if (stressCancelationTokenSource.IsCancellationRequested)
                    {
                        bCanceled = true;
                    }
                }
                catch (TaskCanceledException)
                {
                    bCanceled = true;
                }
            } while (!bCanceled);

            timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            timer.Dispose();
        }


        DateTimeOffset lastGenerateTime = DateTimeOffset.Now;
        object lastGenerateTimeLock = new object();
        long propGenerateCount = 0;
        int valueCount = 0;

        void GenerateStressThingData(object stressThingObj)
        {
            if (stressCancelationTokenSource.IsCancellationRequested)
            {
                return;
            }

            TheThing stressThing = stressThingObj as TheThing;
            var now = DateTimeOffset.Now;
            for (int i = 1; i <= numPropertiesPerThing; i++)
            {
#if !TESTDIRECTUPDATES
                var value = (double)valueCount++;
                if (valueCount % 25 == 0)
                {
                    value = double.NaN;
                }
                stressThing.SetProperty(String.Format("Prop{0}", i), value, now);
#else
                    cdeP prop = new cdeP(stressThing);
                    prop.Name = String.Format("Prop{0}", i);
                    prop.Value = valueCount++; //r.Next(0, 1000);
                    prop.cdeCTIM = now;
                    OnThingUpdate(stressThing, prop);
#endif
                var newCount = System.Threading.Interlocked.Increment(ref propGenerateCount);
            }
            if (now - lastGenerateTime > sendReportInterval)
            {
                lock (lastGenerateTimeLock)
                {
                    now = DateTimeOffset.Now;
                    if (now - lastGenerateTime > sendReportInterval)
                    {
                        var elapsed = (now - lastGenerateTime).TotalMilliseconds / 1000;
                        lastGenerateTime = now;
                        var newCount = System.Threading.Interlocked.Exchange(ref propGenerateCount, 0);
                        //TheBaseAssets.MySYSLOG.WriteToLog(95218, new TSM(MyBaseThing.EngineName, "On Thing Update", eMsgLevel.l6_Debug, String.Format("Generate Rate: {0,11:N2} properties/s", newCount / elapsed)));
                    }
                }
            }
        }


        class ThingPendingEvent
        {
            public ThingEvent thingEvent;
            public DateTimeOffset timeBucket;
            public Timer timer;
        }
        class ThingEventBuffer
        {
            public ThingEventBuffer(TheSenderBase sender, TheSenderThing senderThing)
            {
                this.pendingEvents = new ConcurrentDictionary<DateTimeOffset, ThingPendingEvent>();
                if (sender.PreserveSendOrderAllThings)
                {
                    // Use the same send buffer for all things, so we can preserve the send order more easily
                    this.sendBuffer = sender.AllThingSendBuffer;
                }
                else
                {
                    this.sendBuffer = new BlockingCollection<ThingEvent>();
                }
                this.senderThing = senderThing;
            }
            public ConcurrentDictionary<DateTimeOffset, ThingPendingEvent> pendingEvents;
            public BlockingCollection<ThingEvent> sendBuffer;
            public TheSenderThing senderThing;

            public long LastTimeSentInTicks = 0; // The timestamp of the last event we sent for this thing. Used to prevent sending older events out-of-order 

            public void AddEventToSendBuffer(ThingEvent eventToSend)
            {
                if (this.senderThing.RetryLastValueOnly)
                {
                    ThingEvent thingEvent;
                    // Remove any pending events before adding a new one
                    while (this.sendBuffer.TryTake(out thingEvent, 0)) { }
                }
                this.sendBuffer.Add(eventToSend);
            }
        }

        TaskCompletionSource<int> eventPerThingAdded = new TaskCompletionSource<int>();

        ConcurrentDictionary<string, ThingEventBuffer> eventsPerThing = new ConcurrentDictionary<string, ThingEventBuffer>();

        // The timestamp of the last event we sent for any thing in this connection. Used to prevent sending older events out-of-order 
        long LastTimeSentAllThingsInTicks = DateTime.MinValue.Ticks;

        // Shared send buffer. Only used when we preserver order across all things in this connection
        BlockingCollection<ThingEvent> AllThingSendBuffer = new BlockingCollection<ThingEvent>();


        void OnThingSample(object param)
        {
            ICDEThing thingToSample = param as ICDEThing;
            if (thingToSample == null)
            {
                return;
            }

            string thingID = TheCommonUtils.cdeGuidToString(thingToSample.GetBaseThing().cdeMID);

            var thingEvent = new ThingEvent();

            thingEvent.ThingId = thingID;
            thingEvent.Address = thingToSample.GetBaseThing().Address;
            thingEvent.DeviceType = thingToSample.GetBaseThing().DeviceType;

            ThingEventBuffer thingEventBuffer = GetEventBufferForThing(thingID);
            foreach (var property in thingToSample.GetBaseThing().GetAllProperties())
            {
                thingEvent.SetProperty(property, thingEventBuffer.senderThing.ChangeNaNToNull);
            }

            thingEventBuffer.AddEventToSendBuffer(thingEvent);
            Interlocked.Increment(ref _pendingKPIs.EventGeneratedCount);
            Interlocked.Add(ref _pendingKPIs.EventGeneratedPropertyCount, thingEvent.Properties.Count);
        }

#if TESTUPDATERATEONLY
        static long count = 0;
#endif

        void OnThingUpdate(ICDEThing thing, object param)
        {
            try
            {
                var property = param as cdeP;
#if TESTUPDATERATEONLY
                //if (property != null && property.HasChanged)
                {
                    //var newCount = System.Threading.Interlocked.Increment(ref count);
                    var newCount = count++;
                    if (newCount > 5000)
                    {
                        newCount = System.Threading.Interlocked.Exchange(ref count, 0);
                        var now = DateTime.Now;
                        Console.WriteLine("Rate: {0} properties/s", newCount / (now - lastSendTime).TotalSeconds);
                        lastSendTime = now;
                    }
                }
                return;
#endif
                if (property == null)
                {
                    return;
                }

                var baseThing = thing.GetBaseThing();
                string thingID = TheCommonUtils.cdeGuidToString(baseThing.cdeMID);
                string thingAddress = baseThing.Address;

                ThingEventBuffer thingEventBuffer = GetEventBufferForThing(thingID);

                if (!property.HasChanged && !thingEventBuffer.senderThing.SendUnchangedValue)
                {
                    return;
                }

                ThingPendingEvent pendingEvent = null;
                var lastUpdate = property.cdeCTIM;

                const int ticksPerMillisecond = 1000000 / 100; // 1 tick = 100 nanoseconds, 1 millisecond = 1,000,000 nano seconds
                var timeBucketInTicks = thingEventBuffer.senderThing.ChangeBufferTimeBucketSize * ticksPerMillisecond;

                var eventTimeBucket =
                    timeBucketInTicks == 0
                        ? lastUpdate
                        : new DateTimeOffset(new DateTime(((long)Math.Truncate((double)lastUpdate.Ticks / timeBucketInTicks)) * timeBucketInTicks), lastUpdate.Offset); // Round to the nearest time bucket

                bool bFound = false;
                int retryCount = 0;
                do
                {
                    retryCount++;
                    bFound = thingEventBuffer.pendingEvents.TryGetValue(eventTimeBucket, out pendingEvent);
                    if (!bFound)
                    {
                        if (GetPreserveOrderForSenderThing(thingEventBuffer.senderThing) && thingEventBuffer.LastTimeSentInTicks > eventTimeBucket.Ticks
                            || this.PreserveSendOrderAllThings && this.LastTimeSentAllThingsInTicks > eventTimeBucket.Ticks)
                        {
                            TheBaseAssets.MySYSLOG.WriteToLog(95227, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "On Thing Update", eMsgLevel.l2_Warning, String.Format("Received change out of order for thing {0}, timestamp {1}. Previous event time: {2}. Dropping the change.", thingID, lastUpdate,
                                this.PreserveSendOrderAllThings ? this.LastTimeSentAllThingsInTicks : thingEventBuffer.LastTimeSentInTicks)));
                            // TODO May want to make it configurable if older events should be sent rather than dropped
                            this._pendingKPIs.EventDroppedOutOfOrderCount++;
                            return;
                        }
                        pendingEvent = new ThingPendingEvent();
                        pendingEvent.thingEvent = new ThingEvent();
                        pendingEvent.timeBucket = eventTimeBucket;
                        pendingEvent.thingEvent.ThingId = thingID;
                        pendingEvent.thingEvent.Address = thingAddress;
                        pendingEvent.thingEvent.DeviceType = baseThing.DeviceType;
                        pendingEvent.thingEvent.SetProperty(property, thingEventBuffer.senderThing.ChangeNaNToNull);
                        pendingEvent.timer = new Timer(
                            OnPendingEventTimer, new PendingEventAndBufferParam { PendingEvent = pendingEvent, EventBuffer = thingEventBuffer },
                            thingEventBuffer.senderThing.ChangeBufferLatency, Timeout.Infinite);

                        if (thingEventBuffer.pendingEvents.TryAdd(eventTimeBucket, pendingEvent))
                        {
                            bFound = true;
                        }
                        else
                        {
                            pendingEvent.timer?.Change(Timeout.Infinite, Timeout.Infinite);
                            pendingEvent.timer?.Dispose();
                            pendingEvent.timer = null;
                            if (retryCount > 2)
                            {
                                TheBaseAssets.MySYSLOG.WriteToLog(95228, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "On Thing Update", eMsgLevel.l6_Debug, String.Format("Race to create first pending event for thing {0}, timestamp {1}", thingID, lastUpdate)));
                            }
                        }
                    }
                    else
                    {
                        var timer = pendingEvent.timer;
                        if (timer != null)
                        {
                            try
                            {
                                timer.Change(thingEventBuffer.senderThing.ChangeBufferLatency, Timeout.Infinite);
                                pendingEvent.thingEvent.SetProperty(property, thingEventBuffer.senderThing.ChangeNaNToNull);
                                if (!thingEventBuffer.pendingEvents.Contains(new KeyValuePair<DateTimeOffset, ThingPendingEvent>(eventTimeBucket, pendingEvent)))
                                {
                                    // The event has been sent off by another thread, most likely before we updated it, but potentially even after
                                    // Error on the side of caution and retry placing the event (could result in duplicates, which can happen for other reasons in the pipeline anyway)
                                    bFound = false;
                                }

                            }
                            catch
                            {
                                bFound = false;
                            }
                        }
                        else
                        {
                            bFound = false;
                        }
                    }
                } while (!bFound);

            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95229, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "On Thing Update", eMsgLevel.l1_Error, String.Format("Exception: {0}", GetAggregateExceptionMessage(e))));
            }
        }

        class PendingEventAndBufferParam
        {
            public ThingPendingEvent PendingEvent;
            public ThingEventBuffer EventBuffer;
        }
        void OnPendingEventTimer(object param)
        {
            var p = param as PendingEventAndBufferParam;
            //ThingPendingEvent timedOutPendingEvent = p.Item1;
            //ThingEventBuffer thingEventBuffer = p.Item2;
            AddPendingEventToSendBuffer(p.PendingEvent, p.EventBuffer);
        }

        void AddPendingEventToSendBuffer(ThingPendingEvent timedOutPendingEvent, ThingEventBuffer thingEventBuffer)
        {
            ThingEvent eventToSend;
            lock (timedOutPendingEvent)
            {
                var timer = timedOutPendingEvent.timer;
                if (timer != null)
                {
                    //timedOutPendingEvent.timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer.Dispose();
                    timedOutPendingEvent.timer = null;
                }
                else
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95230, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "On Pending Event Timer", eMsgLevel.l6_Debug, "Pending event timer fired but was already removed from PendingEvent"));
                }

                if (timedOutPendingEvent.thingEvent != null)
                {
                    eventToSend = timedOutPendingEvent.thingEvent;
                    timedOutPendingEvent.thingEvent = null;

                    if (GetPreserveOrderForSenderThing(thingEventBuffer.senderThing))
                    {
                        // Make sure we send any older pending events (timer race conditions etc.), so we preserve send order

                        //Assumption: ConcurrentDictionary's enumeration is thread safe (vs. Add of new time buckets and Remove of existing time buckets during enumeration)
                        if (this.PreserveSendOrderAllThings)
                        {
                            // Get all older pending events across all things, order and send them
                            var previousTime = Interlocked.Read(ref LastTimeSentAllThingsInTicks);
                            while (previousTime < timedOutPendingEvent.timeBucket.Ticks)
                            {
                                if (Interlocked.CompareExchange(ref LastTimeSentAllThingsInTicks, timedOutPendingEvent.timeBucket.Ticks, previousTime) == previousTime)
                                {
                                    var olderPendingEvents = new List<ThingPendingEvent>();
                                    var allEventBuffers = from eventBuffer in this.eventsPerThing.Values select eventBuffer;
                                    foreach (var eventBuffer in allEventBuffers)
                                    {
                                        olderPendingEvents.AddRange(from pe in thingEventBuffer.pendingEvents where pe.Key < timedOutPendingEvent.timeBucket select pe.Value);
                                    }
                                    foreach (var olderEventToSend in olderPendingEvents.OrderBy(pe => pe.timeBucket))
                                    {
                                        AddPendingEventToSendBuffer(olderEventToSend, eventsPerThing[olderEventToSend.thingEvent.ThingId]);
                                    }
                                }
                                previousTime = Interlocked.Read(ref LastTimeSentAllThingsInTicks);
                            }
                        }
                        else
                        {
                            // Get all pending events for this thing, order and send them
                            var previousTime = Interlocked.Read(ref thingEventBuffer.LastTimeSentInTicks);
                            while (previousTime < timedOutPendingEvent.timeBucket.Ticks)
                            {
                                if (Interlocked.CompareExchange(ref thingEventBuffer.LastTimeSentInTicks, timedOutPendingEvent.timeBucket.Ticks, previousTime) == previousTime)
                                {
                                    previousTime = thingEventBuffer.LastTimeSentInTicks;
                                    var olderPendingEvents = from pe in thingEventBuffer.pendingEvents where pe.Key < timedOutPendingEvent.timeBucket orderby pe.Key select pe.Value;
                                    foreach (var olderEventToSend in olderPendingEvents)
                                    {
                                        AddPendingEventToSendBuffer(olderEventToSend, thingEventBuffer);
                                    }
                                }
                                previousTime = Interlocked.Read(ref thingEventBuffer.LastTimeSentInTicks);
                            }
                        }
                    }

                    thingEventBuffer.AddEventToSendBuffer(eventToSend);
                    Interlocked.Increment(ref _pendingKPIs.EventGeneratedCount);
                    Interlocked.Add(ref _pendingKPIs.EventGeneratedPropertyCount, eventToSend.Properties.Count);

                    ThingPendingEvent oldPendingEvent;
                    if (thingEventBuffer.pendingEvents.TryRemove(timedOutPendingEvent.timeBucket, out oldPendingEvent))
                    {
                        if (oldPendingEvent != timedOutPendingEvent && !GetPreserveOrderForSenderThing(thingEventBuffer.senderThing))
                        {
                            // This should never happen: it would mean that two timers fired for the same pending event
                            TheBaseAssets.MySYSLOG.WriteToLog(95231, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "On Pending Event Timer", eMsgLevel.l1_Error, "Unexpected condition when removing event from pending buffer"));
                        }
                    }
                    else
                    {
                        if (!GetPreserveOrderForSenderThing(thingEventBuffer.senderThing))
                        {
                            // This should also never happen unless two timers fired for the same pending event
                            TheBaseAssets.MySYSLOG.WriteToLog(95232, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "On Pending Event Timer", eMsgLevel.l1_Error, "Failed to remove event from pending buffer"));
                        }
                    }

                    //if (eventToSend.Properties.Count != numPropertiesPerThing)
                    //{
                    //    Console.WriteLine("Sending {0} properties", eventToSend.Properties.Count);
                    //}
                }
                else
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95233, new TSM(MyBaseThing.EngineName, "On Pending Event Timer", eMsgLevel.l1_Error, "Pending event timed out but was already consumed"));
                }

            }

        }

        ThingEventBuffer GetEventBufferForThing(string thingID)
        {
            ThingEventBuffer thingEventBuffer = null;
            while (thingEventBuffer == null && !eventsPerThing.TryGetValue(thingID, out thingEventBuffer))
            {
                var senderThing = MySenderThings.MyMirrorCache.GetEntryByFunc(at => Guid.Equals(new Guid(at.ThingMID),new Guid(thingID)));
                //if (senderThing == null) continue;
                thingEventBuffer = new ThingEventBuffer (this, senderThing);
                if (eventsPerThing.TryAdd(thingID, thingEventBuffer))
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95234, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "On Pending Event Timer", eMsgLevel.l6_Debug, String.Format("Added thing {0}", thingID)));
                    bool bSuccess = false;
                    do
                    {
                        bSuccess = eventPerThingAdded.TrySetResult(1);
                        if (!bSuccess)
                        {
                            TheBaseAssets.MySYSLOG.WriteToLog(95235, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "GetEventBufferForThing", eMsgLevel.l6_Debug, String.Format("Failed to signal for new thing {0}. Retrying.", thingID)));
                        }
                        else
                        {
                            TheBaseAssets.MySYSLOG.WriteToLog(95236, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "GetEventBufferForThing", eMsgLevel.l6_Debug, String.Format("Signaled for new thing {0}.", thingID)));
                        }
                        TaskCompletionSource<int> previous;
                        do
                        {
                            previous = eventPerThingAdded;
                        } while (previous.Task.IsCompleted && Interlocked.CompareExchange(ref eventPerThingAdded, new TaskCompletionSource<int>(), previous) != previous);
                    } while (!bSuccess);
                }
                else
                {
                    thingEventBuffer = null;
                }
            }
            return thingEventBuffer;
        }

        List<Task> sendLoopTasks = new List<Task>();

        class SendTaskList
        {
            public List<Task> taskList;
            public ReaderWriterLockSlim taskListLock;
        }
        List<SendTaskList> allSendTaskLists = new List<SendTaskList>();

        bool bDrainAndExitSender = false;

        async void SendLoop()
        {
            try
            {
                var sendTasks = new List<Task>();
                var sendTasksLock = new ReaderWriterLockSlim();
                allSendTaskLists.Add(new SendTaskList { taskList = sendTasks, taskListLock = sendTasksLock }); // Track these for KPI purposes

                IEnumerable<string> thingIds = new List<string>(eventsPerThing.Keys);
                IEnumerable<string> addedThingIds = thingIds;
                var tasks = new List<Task>();

                while (!bDrainAndExitSender && !sendCancelationTokenSource.IsCancellationRequested && TheBaseAssets.MasterSwitch)
                {

                    try
                    {
                        if (this.PreserveSendOrderAllThings)
                        {
                            // To preserve order across all things we must use a single send loop
                            if (tasks.Count == 0)
                            {
                                addedThingIds = new List<string> { eventsPerThing.Keys.First() };
                            }
                            else
                            {
                                // We already have a send loop task: don't create any new ones
                                addedThingIds = new List<string>();
                            }
                        }
                        // Create one send loop task per thing
                        foreach (var thingId in addedThingIds)
                        {
                            TheBaseAssets.MySYSLOG.WriteToLog(95010,TSM.L(eDEBUG_LEVELS.VERBOSE)? null : new TSM(MyBaseThing.EngineName, "GetEventBufferForThing", eMsgLevel.l6_Debug, String.Format("Starting send loop for thing {0}", thingId)));
                            var myClient = GetNextConnection();
                            tasks.Add(Task.Factory.StartNew(async () =>
                            {
                                var thingEventBuffers = eventsPerThing[thingId];
                                IEventConverter eventConverter = EventConverter.EventConverters[thingEventBuffers.senderThing.EventFormat];
                                do
                                {
                                    try
                                    {
                                        ThingEvent thingEvent = null;
                                        if (thingEventBuffers.sendBuffer.TryTake(out thingEvent, 5 * 1000, sendCancelationTokenSource.Token))
                                        {
                                            if (this.PreserveSendOrderAllThings)
                                            {
                                                // We are using a single send buffer and send loop to preserve order across all things
                                                // Have to retrieve the per-thing items every time
                                                thingEventBuffers = eventsPerThing[thingEvent.ThingId];
                                                eventConverter = EventConverter.EventConverters[thingEventBuffers.senderThing.EventFormat];
                                            }

                                            await SendEventsAsync(myClient, thingEvent, thingEventBuffers.senderThing, thingEventBuffers.sendBuffer, sendTasks, sendTasksLock, eventConverter);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        if (!(e is OperationCanceledException && sendCancelationTokenSource.IsCancellationRequested))
                                        {
                                            TheBaseAssets.MySYSLOG.WriteToLog(95237, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "SendLoop", eMsgLevel.l1_Error, String.Format("Exception in send task for thing {0}: {1}", thingId, GetAggregateExceptionMessage(e))));
                                        }
                                    }
                                } while (!bDrainAndExitSender && !sendCancelationTokenSource.IsCancellationRequested);
                                await TheCommonUtils.TaskWhenAll(sendTasks);

                                ReportExceptionsAndRemoveCompleted(sendTasksLock, sendTasks);

                            }));
                        }

                        addedThingIds = new List<string>();

                        try
                        {
                            await TheCommonUtils.TaskWhenAny(tasks.Union(new List<Task> { eventPerThingAdded.Task, TheCommonUtils.TaskDelayOneEye(5 * 1000, 100) }));
                        }
                        catch (TaskCanceledException) { }

                        var newThingIds = new List<string>(eventsPerThing.Keys);
                        addedThingIds = newThingIds.Except(thingIds);
                        thingIds = newThingIds;

                        LogTaskExceptions(tasks);
                        tasks.RemoveAll((t) => t.IsCompleted);
                    }
                    catch (Exception e)
                    {
                        if (!(e is OperationCanceledException && sendCancelationTokenSource.IsCancellationRequested))
                        {
                            TheBaseAssets.MySYSLOG.WriteToLog(95238, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "SendLoop", eMsgLevel.l1_Error, String.Format("Exception in send loop - continuing loop: {0}", GetAggregateExceptionMessage(e))));
                        }
                    }
                }

                try
                {
                    await TheCommonUtils.TaskWhenAny(new List<Task> { TheCommonUtils.TaskDelayOneEye(-1, 100), TheCommonUtils.TaskWhenAll(tasks) });
                }
                catch (TaskCanceledException) { }

                ReportExceptionsAndRemoveCompleted(sendTasksLock, sendTasks);

                if (sendTasks.Count>0)
                {
                    TheBaseAssets.MySYSLOG.WriteToLog(95239, TSM.L(eDEBUG_LEVELS.VERBOSE) ? null : new TSM(MyBaseThing.EngineName, "SendLoop", eMsgLevel.l2_Warning, String.Format("Pending send tasks after loop has closed: {0}", sendTasks.Count)));
                }
                allSendTaskLists.RemoveAll(t => t.taskList == sendTasks);
            }
            catch (Exception e)
            {
                TheBaseAssets.MySYSLOG.WriteToLog(95240, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "SendLoop", eMsgLevel.l1_Error, String.Format("Exception in send loop - terminating loop: {0}", GetAggregateExceptionMessage(e))));
            }
            TheBaseAssets.MySYSLOG.WriteToLog(95011, TSM.L(eDEBUG_LEVELS.OFF) ? null : new TSM(MyBaseThing.EngineName, "SendLoop", eMsgLevel.l1_Error, String.Format("Exiting loop")));
        }

    }

}
#endif