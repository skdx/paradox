﻿// Copyright (c) 2014 Silicon Studio Corp. (http://siliconstudio.co.jp)
// This file is distributed under GPL v3. See LICENSE.md for details.
using System.ServiceModel;

using SiliconStudio.Core.Diagnostics;

namespace SiliconStudio.BuildEngine
{
    [ServiceContract]
    public interface IForwardSerializableLogRemote
    {
        [OperationContract(IsOneWay = true)]
        [UseParadoxDataContractSerializer]
        void ForwardSerializableLog(SerializableLogMessage message);
    }
}
