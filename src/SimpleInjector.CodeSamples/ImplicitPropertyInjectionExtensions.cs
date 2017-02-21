﻿namespace SimpleInjector.CodeSamples
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    using SimpleInjector.Advanced;

    public static class ImplicitPropertyInjectionExtensions
    {
        [DebuggerStepThrough]
        public static void AutoWirePropertiesImplicitly(this ContainerOptions options)
        {
            options.PropertySelectionBehavior = new ImplicitPropertyInjectionBehavior(options.Container);
        }
    }
    
    public class ImplicitPropertyInjectionBehavior : IPropertySelectionBehavior {
        private readonly IPropertySelectionBehavior original;
        private readonly ContainerOptions options;

        internal ImplicitPropertyInjectionBehavior(Container container) {
            this.options = container.Options;
            this.original = container.Options.PropertySelectionBehavior;
        }

        public bool SelectProperty(Type t, PropertyInfo p) =>
            this.IsImplicitInjectable(t, p) || this.original.SelectProperty(t, p);

        private bool IsImplicitInjectable(Type t, PropertyInfo p) =>
            IsInjectableProperty(p) && this.CanBeResolved(t, p);

        private static bool IsInjectableProperty(PropertyInfo property) =>
            property.CanWrite && property.GetSetMethod(nonPublic: false)?.IsStatic == false;

        private bool CanBeResolved(Type t, PropertyInfo property) =>
            this.GetProducer(new InjectionConsumerInfo(t, property)) != null;

        private InstanceProducer GetProducer(InjectionConsumerInfo info) =>
            this.options.DependencyInjectionBehavior.GetInstanceProducer(info, false);
    }
}