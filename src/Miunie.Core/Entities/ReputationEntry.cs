﻿// This file is part of Miunie.
//
//  Miunie is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  Miunie is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with Miunie. If not, see <https://www.gnu.org/licenses/>.

using System;

namespace Miunie.Core.Entities
{
    public class ReputationEntry
    {
        public ReputationEntry(ulong targetId, string targetName, DateTime givenAt, ReputationType type, bool isfromInvoker = false)
        {
            TargetId = targetId;
            TargetName = targetName;
            GivenAt = givenAt;
            Type = type;
            IsFromInvoker = isfromInvoker;
        }

        public ulong TargetId { get; set; }

        public string TargetName { get; set; }

        public DateTime GivenAt { get; set; }

        public ReputationType Type { get; set; }

        public bool IsFromInvoker { get; set; }
    }
}
