﻿using pst.interfaces;
using pst.utilities;
using pst.encodables.ndb.blocks;
using pst.encodables.ndb.blocks.data;

namespace pst.impl.decoders.ndb.blocks.data
{
    class InternalDataBlockDecoder : IDecoder<InternalDataBlock>
    {
        private readonly IDecoder<BlockTrailer> trailerDecoder;

        public InternalDataBlockDecoder(IDecoder<BlockTrailer> trailerDecoder)
        {
            this.trailerDecoder = trailerDecoder;
        }

        public InternalDataBlock Decode(BinaryData encodedData)
        {
            var parser = BinaryDataParser.OfValue(encodedData);

            var blockType = parser.TakeAndSkip(1).ToInt32();
            var blockLevel = parser.TakeAndSkip(1).ToInt32();
            var numberOfEntries = parser.TakeAndSkip(2).ToInt32();
            var totalByteCount = parser.TakeAndSkip(4).ToInt32();
            var entries = parser.TakeAndSkip(numberOfEntries * 8);
            var padding = BinaryData.Empty();

            var remainingTo64Boundary = Utilities.GetRemainingToNextMultipleOf(numberOfEntries * 8 + 8, 64);

            if (remainingTo64Boundary > 0)
            {
                padding = parser.TakeAndSkip(remainingTo64Boundary);
            }

            var trailer = parser.TakeAndSkip(16, trailerDecoder);

            return new InternalDataBlock(
                blockType,
                blockLevel,
                numberOfEntries,
                totalByteCount,
                entries,
                padding,
                trailer);
        }
    }
}
