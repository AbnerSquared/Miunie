// This file is part of Miunie.
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

using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Miunie.Discord.Embeds
{
    internal static class Paginator
    {
        public static IEnumerable<string> GetValues<T>(
            IEnumerable<T> set,
            int index,
            int pageSize,
            string defaultOnUnderflow = null)
        {
            var strings = set.Select(x => x.ToString()).ToList();

            if (defaultOnUnderflow != null && GetValueCountAtPage(set.Count(), pageSize, index) != pageSize)
            {
                for (int i = strings.Count(); i < pageSize; i++)
                {
                    strings.Add(defaultOnUnderflow);
                }
            }

            return GroupAt(strings, index, pageSize);
        }

        public static IEnumerable<string> GetValues<T>(
            IEnumerable<T> set,
            int index,
            int pageSize,
            Func<T, string> writer,
            string defaultOnUnderflow = null)
        {
            var strings = set.Select(x => writer.Invoke(x)).ToList();

            if (defaultOnUnderflow != null && GetValueCountAtPage(set.Count(), pageSize, index) != pageSize)
            {
                for (int i = strings.Count(); i < pageSize; i++)
                {
                    strings.Add(defaultOnUnderflow);
                }
            }

            return GroupAt(strings, index, pageSize);
        }

        public static IEnumerable<T> GroupAt<T>(
            IEnumerable<T> set,
            int index,
            int pageSize,
            bool defaultOnUnderflow = false)
        {
            int maxPages = GetPageCount(set.Count(), pageSize);

            if (index < 0 || index >= maxPages)
            {
                return set;
            }

            var remainder = set.Skip(pageSize * index);
            var group = new List<T>();

            for (int i = 0; i < pageSize; i++)
            {
                if (defaultOnUnderflow)
                {
                    group.Add(remainder.ElementAtOrDefault(i));
                }
                else
                {
                    if (remainder.Count() - 1 < i)
                    {
                        continue;
                    }
                    else
                    {
                        group.Add(remainder.ElementAt(i));
                    }
                }
            }

            return group;
        }

        public static EmbedBuilder PaginateEmbedWithFields<T>(
            IEnumerable<T> set,
            EmbedBuilder embed,
            int index,
            int pageSize,
            Func<T, (string, string)> writer,
            PaginatorFooterHandling footerHandling = PaginatorFooterHandling.Auto)
        {
            if (pageSize > EmbedBuilder.MaxFieldCount)
            {
                throw new ArgumentException("The specified field count is larger than 25.");
            }

            var group = GroupAt(set, index, pageSize);
            var fields = new List<EmbedFieldBuilder>();

            for (int i = 0; i < pageSize; i++)
            {
                var value = group.ElementAtOrDefault(i);

                (string name, string content) = writer.Invoke(value);

                var field = new EmbedFieldBuilder()
                    .WithName(name)
                    .WithValue(content)
                    .WithIsInline(false);

                fields.Add(field);
            }

            if (CanCreateFooter(set.Count(), pageSize, footerHandling))
            {
                _ = embed.WithFooter(GetPageFooter(index, set.Count(), pageSize, embed.Footer?.Text));
            }

            embed.Fields = fields;

            return embed;
        }

        public static EmbedBuilder PaginateEmbed<T>(
            IEnumerable<T> set,
            EmbedBuilder embed,
            int index,
            int pageSize,
            PaginatorFooterHandling footerHandling = PaginatorFooterHandling.Auto,
            string defaultOnUnderflow = null)
        {
            if (CanCreateFooter(set.Count(), pageSize, footerHandling))
            {
                _ = embed.WithFooter(GetPageFooter(index, set.Count(), pageSize, embed.Footer?.Text));
            }

            return embed.WithDescription(Paginate(set, index, pageSize, defaultOnUnderflow));
        }

        public static EmbedBuilder PaginateEmbed<T>(
            IEnumerable<T> set,
            EmbedBuilder embed,
            int index,
            int pageSize,
            Func<T, string> writer,
            PaginatorFooterHandling footerHandling = PaginatorFooterHandling.Auto,
            string defaultOnUnderflow = null)
        {
            if (CanCreateFooter(set.Count(), pageSize, footerHandling))
            {
                _ = embed.WithFooter(GetPageFooter(index, set.Count(), pageSize, embed.Footer?.Text));
            }

            return embed.WithDescription(Paginate(set, index, pageSize, writer, defaultOnUnderflow));
        }

        public static string Paginate<T>(IEnumerable<T> set, int index, int pageSize, string defaultOnUnderflow = null)
        {
            var group = GetValues(set, index, pageSize, defaultOnUnderflow);
            StringBuilder page = new StringBuilder();

            foreach (string item in group)
            {
                _ = page.AppendLine(item);
            }

            return page.ToString();
        }

        public static string Paginate<T>(
            IEnumerable<T> set,
            int index,
            int pageSize,
            Func<T, string> writer,
            string defaultOnUnderflow = null)
        {
            var group = GetValues(set, index, pageSize, writer, defaultOnUnderflow);
            StringBuilder page = new StringBuilder();

            foreach (string item in group)
            {
                _ = page.AppendLine(item);
            }

            return page.ToString();
        }

        private static bool CanCreateFooter(int collectionSize, int pageSize, PaginatorFooterHandling handling)
            => handling switch
            {
                PaginatorFooterHandling.On => true,
                PaginatorFooterHandling.Off => false,
                _ => GetPageCount(collectionSize, pageSize) > 1
            };

        private static string GetPageFooter(int index, int collectionSize, int pageSize, string preFooter = null)
        {
            StringBuilder footer = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(preFooter))
            {
                _ = footer.Append($"{preFooter} | ");
            }

            _ = footer.Append($"Page {index + 1} of {GetPageCount(collectionSize, pageSize)}");

            return footer.ToString();
        }

        private static int GetPageCount(int collectionSize, int pageSize)
        {
            return (int)Math.Ceiling((double)collectionSize / pageSize);
        }

        private static int GetValueCountAtPage(int collectionSize, int pageSize, int index)
        {
            index = Clamp(0, GetPageCount(collectionSize, pageSize) - 1, index);
            return Clamp(0, pageSize, collectionSize - (pageSize * index));
        }

        private static int Clamp(int min, int max, int value)
            => value < min
            ? min
            : value > max
            ? max
            : value;
    }
}
