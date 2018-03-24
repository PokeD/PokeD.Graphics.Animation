﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace PokeD.Graphics.Content.Pipeline.MaterialAnimation
{
    public sealed class MaterialAnimationChannel : ICollection<MaterialAnimationKeyframe>
    {
        private readonly List<MaterialAnimationKeyframe> _keyframes = new List<MaterialAnimationKeyframe>();

        /// <summary>
        /// Gets the number of keyframes in the collection.
        /// </summary>
        public int Count => _keyframes.Count;

        /// <summary>
        /// Gets the keyframe at the specified index position.
        /// </summary>
        public MaterialAnimationKeyframe this[int index] => _keyframes[index];

        /// <summary>
        /// Returns a value indicating whether the object is read-only.
        /// </summary>
        bool ICollection<MaterialAnimationKeyframe>.IsReadOnly => false;

        /// <summary>
        /// Initializes a new instance of AnimationChannel.
        /// </summary>
        public MaterialAnimationChannel() { }

        /// <summary>
        /// To satisfy ICollection
        /// </summary>
        /// <param name="item"></param>
        void ICollection<MaterialAnimationKeyframe>.Add(MaterialAnimationKeyframe item) => _keyframes.Add(item);

        /// <summary>
        /// Adds a new keyframe to the collection, automatically sorting the contents according to keyframe times.
        /// </summary>
        /// <param name="item">Keyframe to be added to the channel.</param>
        /// <returns>Index of the new keyframe.</returns>
        public int Add(MaterialAnimationKeyframe item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // Find the correct place at which to insert it, so we can know the index to return.
            // The alternative is Add, Sort then return IndexOf, which would be horribly inefficient
            // and the order returned by Sort would change each time for keyframes with the same time.

            // BinarySearch returns the index of the first item found with the same time, or the bitwise
            // complement of the next largest item found.
            int index = _keyframes.BinarySearch(item);
            if (index >= 0)
            {
                // If a match is found, we do not know if it is at the start, middle or end of a range of
                // keyframes with the same time value.  So look for the end of the range and insert there
                // so we have deterministic behaviour.
                while (index < _keyframes.Count)
                {
                    if (item.CompareTo(_keyframes[index]) != 0)
                        break;
                    ++index;
                }
            }
            else
            {
                // If BinarySearch returns a negative value, it is the bitwise complement of the next largest
                // item in the list.  So we just do a bitwise complement and insert at that index.
                index = ~index;
            }
            _keyframes.Insert(index, item);

            return index;
        }

        /// <summary>
        /// Removes all keyframes from the collection.
        /// </summary>
        public void Clear() => _keyframes.Clear();

        /// <summary>
        /// Searches the collection for the specified keyframe.
        /// </summary>
        /// <param name="item">Keyframe being searched for.</param>
        /// <returns>true if the keyframe exists; false otherwise.</returns>
        public bool Contains(MaterialAnimationKeyframe item) => _keyframes.Contains(item);

        /// <summary>
        /// To satisfy ICollection
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        void ICollection<MaterialAnimationKeyframe>.CopyTo(MaterialAnimationKeyframe[] array, int arrayIndex) => _keyframes.CopyTo(array, arrayIndex);

        /// <summary>
        /// Determines the index for the specified keyframe.
        /// </summary>
        /// <param name="item">Identity of a keyframe.</param>
        /// <returns>Index of the specified keyframe.</returns>
        public int IndexOf(MaterialAnimationKeyframe item) => _keyframes.IndexOf(item);

        /// <summary>
        /// Removes the specified keyframe from the collection.
        /// </summary>
        /// <param name="item">Keyframe being removed.</param>
        /// <returns>true if the keyframe was removed; false otherwise.</returns>
        public bool Remove(MaterialAnimationKeyframe item) => _keyframes.Remove(item);

        /// <summary>
        /// Removes the keyframe at the specified index position.
        /// </summary>
        /// <param name="index">Index of the keyframe being removed.</param>
        public void RemoveAt(int index) => _keyframes.RemoveAt(index);

        /// <summary>
        /// Returns an enumerator that iterates through the keyframes.
        /// </summary>
        /// <returns>Enumerator for the keyframe collection.</returns>
        public IEnumerator<MaterialAnimationKeyframe> GetEnumerator() => _keyframes.GetEnumerator();

        /// <summary>
        /// To satisfy ICollection
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => _keyframes.GetEnumerator();
    }
}