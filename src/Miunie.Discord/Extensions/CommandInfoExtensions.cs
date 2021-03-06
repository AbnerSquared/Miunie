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

using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Miunie.Discord
{
    internal static class CommandInfoExtensions
    {
        internal static TAttribute FindAttribute<TAttribute>(this CommandInfo command)
            where TAttribute : Attribute
            => command.Attributes.FirstOrDefault(x => x is TAttribute) as TAttribute;

        internal static TPrecondition FindPrecondition<TPrecondition>(this CommandInfo command)
            where TPrecondition : PreconditionAttribute
            => command.Preconditions.FirstOrDefault(x => x is TPrecondition) as TPrecondition;

        internal static IEnumerable<TAttribute> FindAttributes<TAttribute>(this CommandInfo command)
            where TAttribute : Attribute
            => command.Attributes.Where(x => x is TAttribute).Select(x => x as TAttribute);
    }
}
