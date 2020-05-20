// SPDX-FileCopyrightText: 2009-2020 TRUMPF Laser GmbH, authors: C-Labs
//
// SPDX-License-Identifier: MPL-2.0

﻿using nsCDEngine.BaseClasses;
using nsCDEngine.Communication;
using nsCDEngine.Engines;
using nsCDEngine.Engines.ThingService;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TheCommonMessageContracts;

namespace nsTheSenderBase
{
#if NET35
    interface IObservable<T>
    {
        IDisposable Subscribe(IObserver<T> observer);
    }
    interface IObserver<T>
    {
        void OnNext(T value);
        void OnCompleted();
        void OnError(Exception e);
    }
#endif

    class TheThingMatcher : IObservable<TheThing>
    {
        TheThingReference _thingReference;

        public TheThingMatcher(TheThingReference thingReference)
        {
            Initialize(thingReference);
        }

        private void Initialize(TheThingReference thingReference)
        { 
            _thingReference = thingReference;
            if (thingReference.PropertiesToMatch != null)
            {
                _thingReference.PropertiesToMatch = new Dictionary<string, object>(thingReference.PropertiesToMatch);
            }
            if (!string.IsNullOrEmpty(thingReference.EngineName))
            {
                if (_thingReference.PropertiesToMatch == null) _thingReference.PropertiesToMatch = new Dictionary<string, object>();
                _thingReference.PropertiesToMatch[nameof(thingReference.EngineName)] = thingReference.EngineName;
            }
            if (!string.IsNullOrEmpty(thingReference.DeviceType))
            {
                if (_thingReference.PropertiesToMatch == null) _thingReference.PropertiesToMatch = new Dictionary<string, object>();
                _thingReference.PropertiesToMatch[nameof(thingReference.DeviceType)] = thingReference.DeviceType;
            }
            if (!string.IsNullOrEmpty(thingReference.FriendlyName))
            {
                if (_thingReference.PropertiesToMatch == null) _thingReference.PropertiesToMatch = new Dictionary<string, object>();
                _thingReference.PropertiesToMatch[nameof(thingReference.FriendlyName)] = thingReference.FriendlyName;
            }
            if (!string.IsNullOrEmpty(thingReference.ID))
            {
                if (_thingReference.PropertiesToMatch == null) _thingReference.PropertiesToMatch = new Dictionary<string, object>();
                _thingReference.PropertiesToMatch[nameof(thingReference.ID)] = thingReference.ID;
            }
            if (!string.IsNullOrEmpty(thingReference.Address))
            {
                if (_thingReference.PropertiesToMatch == null) _thingReference.PropertiesToMatch = new Dictionary<string, object>();
                _thingReference.PropertiesToMatch[nameof(thingReference.Address)] = thingReference.Address;
            }
        }
        public TheThingMatcher(TheMessageAddress thingAddress, Dictionary<string, object> propertiesToMatch)
        {
            var thingReference = new TheThingReference { EngineName = thingAddress.EngineName, ThingMID = thingAddress.ThingMID, PropertiesToMatch = propertiesToMatch };
            Initialize(thingReference);
        }

        // TODO Make this an extension method to keep the data contract clean?
        public IEnumerable<TheThing> GetMatchingThings()
        {
            return GetMatchingThings(null);
        }
        private IEnumerable<TheThing> GetMatchingThings(Subscription subscription)
        {
            var things = new List<TheThing>();
            if (_thingReference.ThingMID.HasValue && _thingReference.ThingMID != Guid.Empty)
            {
                var thing = TheThingRegistry.GetThingByMID(_thingReference.ThingMID.Value, true);
                if (thing != null)
                {
                    things.Add(thing);
                }
                return things;
            }

            IBaseEngine baseEngine = null;
            List<TheThing> matchingThings = null;
            if (!string.IsNullOrEmpty(_thingReference.EngineName))
            {
                baseEngine = TheThingRegistry.GetBaseEngine(_thingReference.EngineName);
                if (baseEngine == null)
                {
                    TheCDEngines.RegisterNewMiniRelay(_thingReference.EngineName);
                    baseEngine = TheThingRegistry.GetBaseEngine(_thingReference.EngineName);
                }
                if (baseEngine != null)
                {
                    subscription?.AddSubscription(baseEngine, this);
                    matchingThings = TheThingRegistry.GetThingsOfEngine(_thingReference.EngineName, true, true);
                }
            }
            else
            {
                object deviceTypeObj = null;
                if (_thingReference.PropertiesToMatch?.TryGetValue("DeviceType", out deviceTypeObj) == true)
                {
                    var deviceType = TheCommonUtils.CStr(deviceTypeObj);
                    if (!string.IsNullOrEmpty(deviceType))
                    {
                        matchingThings = TheThingRegistry.GetThingsByFunc("*", t => t.DeviceType.Equals(deviceType, StringComparison.Ordinal), true);
                    }
                }
            }

            if (matchingThings != null)
            {
                foreach (var thing in matchingThings)
                {
                    if (Matches(thing))
                    {
                        if (baseEngine == null)
                        {
                            subscription?.AddSubscription(thing.GetBaseEngine(), this);
                        }
                        things.Add(thing);
                    }
                }
            }

            return things;
        }

        bool Matches(TheThing thingToMatch)
        {
            if (_thingReference.PropertiesToMatch == null)
            {
                return true;
            }
            foreach (var propToMatch in _thingReference.PropertiesToMatch)
            {
                if (!Regex.IsMatch(TheThing.GetSafePropertyString(thingToMatch, propToMatch.Key), TheCommonUtils.CStr(propToMatch.Value)))
                {
                    return false;
                }
            }
            return true;
        }

        public IDisposable Subscribe(IObserver<TheThing> observer)
        {
            var subscription = new Subscription(observer);
            foreach (var thing in GetMatchingThings(subscription))
            {
                observer.OnNext(thing);
            }
            if (!subscription.HasSubscriptions)
            {
                subscription.Unregister();
            }
            return subscription;
        }

        class Subscription : IDisposable
        {
            private Dictionary<IBaseEngine, Action<ICDEThing, object>> _registrations;
            private IObserver<TheThing> _observer;
            private bool _hasCompleted;

            public Subscription(IObserver<TheThing> observer)
            {
                _observer = observer;
            }
            public void AddSubscription(IBaseEngine baseEngine, TheThingMatcher thingTemplate)
            {
                if (_registrations?.ContainsKey(baseEngine) == true)
                    return;
                var action = baseEngine.RegisterEvent(eEngineEvents.ThingRegistered,
                    (t, o) =>
                    {

                        var thing = (o as ICDEThing)?.GetBaseThing();
                        if (thing != null)
                        {
                            if (thingTemplate.Matches(thing))
                            {
                                _observer?.OnNext(thing);
                            }
                        }
                    });
                if (_registrations == null)
                {
                    _registrations = new Dictionary<IBaseEngine, Action<ICDEThing, object>>();
                }
                _registrations.Add(baseEngine, action);
            }
            public bool HasSubscriptions
            {
                get { return _registrations != null && _registrations.Count > 0; }
            }
            public void Unregister()
            {
                if (_registrations != null)
                {
                    try
                    {
                        foreach (var registration in _registrations)
                        {
                            registration.Key.UnregisterEvent(eEngineEvents.ThingRegistered, registration.Value);
                        }
                        _registrations = null;
                    }
                    catch { }
                }
                if (!_hasCompleted)
                {
                    _hasCompleted = true;
                    _observer.OnCompleted();
                }
            }

            public void Dispose()
            {
                Unregister();
            }
        }

    }




}
