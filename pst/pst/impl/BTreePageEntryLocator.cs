﻿using pst.interfaces;
using pst.encodables;
using pst.utilities;

namespace pst.impl
{
    class BTreePageEntryLocator<TKey, TEntry> : IBTreePageEntryLocator<TKey, TEntry>
    {
        private readonly IDecoder<TEntry> entryDecoder;

        private readonly IBTreePageEntriesComparer<TKey, TEntry> pageEntriesComparer;

        public BTreePageEntryLocator(IDecoder<TEntry> entryDecoder, IBTreePageEntriesComparer<TKey, TEntry> pageEntriesComparer)
        {
            this.entryDecoder = entryDecoder;
            this.pageEntriesComparer = pageEntriesComparer;
        }

        public TEntry FindEntry(BTPage page, TKey key)
        {
            using (var parser = BinaryDataParser.OfValue(page.Entries))
            {
                var entries =
                    parser
                    .TakeAndSkip(
                        page.NumberOfEntriesInPage,
                        page.EntrySize,
                        entryDecoder);

                return pageEntriesComparer.GetMatchingEntry(entries, key);
            }
        }
    }
}
