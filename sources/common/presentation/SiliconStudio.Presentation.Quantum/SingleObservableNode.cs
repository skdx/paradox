﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using SiliconStudio.Core;
using SiliconStudio.Core.Reflection;
using SiliconStudio.Quantum;

namespace SiliconStudio.Presentation.Quantum
{
    public abstract class SingleObservableNode : ObservableNode
    {
        public readonly string[] ReservedNames = { "Owner", "Name", "DisplayName", "Path", "Parent", "Root", "Type", "IsPrimitive", "IsVisible", "IsReadOnly", "Value", "TypedValue", "Index", "Guid", "Children", "Commands", "AssociatedData", "HasList", "HasDictionary", "CombinedNodes", "HasMultipleValues", "HasMultipleInitialValues", "ResetInitialValues", "DistinctInitialValues" };

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleObservableNode"/> class.
        /// </summary>
        /// <param name="ownerViewModel">The <see cref="ObservableViewModel"/> that owns the new <see cref="SingleObservableNode"/>.</param>
        /// <param name="baseName">The base name of this node. Can be null if <see cref="index"/> is not. If so a name will be automatically generated from the index.</param>
        /// <param name="parentNode">The parent node of the new <see cref="SingleObservableNode"/>, or <c>null</c> if the node being created is the root node of the view model.</param>
        /// <param name="index">The index of this content in the model node, when this node represent an item of a collection. <c>null</c> must be passed otherwise</param>
        protected SingleObservableNode(ObservableViewModel ownerViewModel, string baseName, ObservableNode parentNode, object index = null)
            : base(ownerViewModel, parentNode, index)
        {
            if (baseName == null && index == null)
                throw new ArgumentException("baseName and index can't be both null.");

            CombineMode = CombineMode.CombineOnlyForAll;
            SetName(baseName);
        }

        /// <summary>
        /// Gets or sets the <see cref="CombineMode"/> of this single node.
        /// </summary>
        public CombineMode CombineMode { get; set; }

        public VirtualObservableNode CreateVirtualChild(string name, Type contentType, int? order, object initialValue, NodeCommandWrapperBase valueChangedCommand = null, IReadOnlyDictionary<string, object> nodeAssociatedData = null)
        {
            var observableChild = VirtualObservableNode.Create(Owner, name, this, order, contentType, initialValue, valueChangedCommand);
            if (nodeAssociatedData != null)
            {
                foreach (var data in nodeAssociatedData)
                {
                    observableChild.AddAssociatedData(data.Key, data.Value);
                }
            }
            AddChild(observableChild);
            return observableChild;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("{0}: [{1}]", Name, Value);
        }

        private void SetName(string nodeName)
        {
            var index = Index;
            nodeName = nodeName != null ? nodeName.Replace(".", "-") : null;
            
            if (!string.IsNullOrWhiteSpace(nodeName))
            {
                Name = nodeName;
                DisplayName = Regex.Replace(nodeName, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 ");
            }
            else if (index != null)
            {
                // TODO: make a better interface for custom naming specification
                var propertyKey = index as PropertyKey;
                if (propertyKey != null)
                {
                    string name = propertyKey.Name.Replace(".", "-");

                    if (name == "Key")
                        name = propertyKey.PropertyType.Name.Replace(".", "-");

                    Name = name;
                    var parts = propertyKey.Name.Split('.');
                    DisplayName = parts.Length == 2 ? string.Format("{0} ({1})", parts[1], parts[0]) : name;
                }
                else
                {
                    if (index.GetType().IsNumeric())
                        Name = "Item " + index.ToString().Replace(".", "-");
                    else
                        Name = index.ToString().Replace(".", "-");

                    DisplayName = Name;
                }
            }

            if (ReservedNames.Contains(Name))
            {
                Name += "_";
            }
        }
    }
}
