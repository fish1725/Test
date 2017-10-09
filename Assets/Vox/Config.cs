using UnityEngine;
using System.Collections;

namespace Vox
{
    public static class Env
    {
        //! Size of chunk's side
        public const int ChunkPow = 5;

        //! Size of block when rendering
        public const float BlockSize = 1f;

        #region DO NOT CHANGE THESE!

        public const float BlockSizeHalf = BlockSize / 2f;
        public const float BlockSizeInv = 1f / BlockSize;
        public static readonly Vector3 HalfBlockOffset = new Vector3(BlockSizeHalf, BlockSizeHalf, BlockSizeHalf);
        public static readonly Vector3 HalfBlockOffsetInv = new Vector3(BlockSizeInv, BlockSizeInv, BlockSizeInv);

        //! Padding added to the size of block faces to fix floating point issues
        //! where tiny gaps can appear between block faces
        public const float BlockFacePadding = 0.005f;

        public const int ChunkPow2 = ChunkPow << 1;
        public const int ChunkMask = (1 << ChunkPow) - 1;

        //! Internal chunk size including room for edge fields as well so that we do not have to check whether we are within chunk bounds.
        //! This means we will ultimately consume a bit more memory in exchange for more performance
        public const int ChunkPadding = 0;
        public const int ChunkPadding2 = ChunkPadding * 2;

        //! Visible chunk size
        public const int ChunkSize = (1 << ChunkPow) - 2 * ChunkPadding;
        public const int ChunkSize1 = ChunkSize - 1;
        public const int ChunkSizePow2 = ChunkSize * ChunkSize;
        public const int ChunkSizePow3 = ChunkSize * ChunkSizePow2;

        //! Internal chunk size (visible size + padding)
        public const int ChunkSizePlusPadding = ChunkSize + ChunkPadding;
        public const int ChunkSizeWithPadding = ChunkSize + ChunkPadding * 2;
        public const int ChunkSizeWithPaddingPow2 = ChunkSizeWithPadding * ChunkSizeWithPadding;
        public const int ChunkSizeWithPaddingPow3 = ChunkSizeWithPadding * ChunkSizeWithPaddingPow2;

        #endregion
    }
}