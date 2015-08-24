/*
 * Copyright 2014 Niek Haarman
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//package com.nhaarman.listviewanimations.itemmanipulation.animateaddition;

//import android.support.annotation.NonNull;
//import android.util.Pair;

//import com.nhaarman.listviewanimations.util.Insertable;

//import java.util.ArrayList;
//import java.util.Arrays;
//import java.util.Collection;
//import java.util.HashSet;
//import java.util.Iterator;
//import java.util.List;
//import java.util.concurrent.atomic.AtomicInteger;

/**
 * A class to insert items only when there are no active items.
 * A pending index-item pair can have two states: active and pending. When inserting new items, the {@link com.nhaarman.listviewanimations.itemmanipulation
 * .AnimateAdditionAdapter.Insertable#add
 * (int, Object)} method will be called directly if there are no active index-item pairs.
 * Otherwise, pairs will be queued until the active list is empty.
 */


using Com.Nhaarman.ListviewAnimations.Util;
using Java.Util.Concurrent.Atomic;
using System.Collections.Generic;
namespace Com.Nhaarman.ListviewAnimations.ItemManiPulation.Animateaddition
{
    public class InsertQueue<T>
    {

        //@NonNull
        private Insertable mInsertable;

        //@NonNull
        private ICollection<AtomicInteger> mActiveIndexes = new HashSet<AtomicInteger>();

        //@NonNull
        private List<KeyValuePair<int, T>> mPendingItemsToInsert = new List<KeyValuePair<int, T>>();

        public InsertQueue(Insertable insertable)
        {
            mInsertable = insertable;
        }

        /**
         * Insert an item into the queue at given index. Will directly call {@link Insertable#add(int,
         * Object)} if there are no active index-item pairs. Otherwise, the pair will be queued.
         *
         * @param index the index at which the item should be inserted.
         * @param item  the item to insert.
         */
        public void insert(int index, T item)
        {
            if (mActiveIndexes.Count == 0 && mPendingItemsToInsert.Count == 0)
            {
                mActiveIndexes.Add(new AtomicInteger(index));
                //noinspection unchecked
                mInsertable.add(index, item);
            }
            else
            {
                mPendingItemsToInsert.Add(new KeyValuePair<int, T>(index, item));
            }
        }

        public void insert(KeyValuePair<int, T>[] indexItemPair)
        {
            insert(indexItemPair);
        }

        public void insert(ICollection<KeyValuePair<int, T>> indexItemPairs)
        {
            if (mActiveIndexes.Count == 0 && mPendingItemsToInsert.Count == 0)
            {
                foreach (KeyValuePair<int, T> pair in indexItemPairs)
                {
                    foreach (AtomicInteger existing in mActiveIndexes)
                    {
                        if (existing.IntValue() >= pair.Key)
                        {
                            existing.IncrementAndGet();
                        }
                    }
                    mActiveIndexes.Add(new AtomicInteger(pair.Key));

                    mInsertable.add(pair.Key, pair.Value);
                }
            }
            else
            {
                mPendingItemsToInsert.AddRange(indexItemPairs);
            }
        }

        /**
         * Clears the active states and inserts any pending pairs if applicable.
         */
        public void clearActive()
        {
            mActiveIndexes.Clear();
            insertPending();
        }

        /**
         * Clear the active state for given index. Will insert any pending pairs if this call leads to a state where there are no active pairs.
         *
         * @param index the index to remove.
         */
        public void removeActiveIndex(int index)
        {
            bool found = false;
            //for (IEnumerator<AtomicInteger> iterator = mActiveIndexes.GetEnumerator(); iterator.MoveNext() && !found; )
            //{
                
            //    if (iterator.Current.Get() == index)
            //    {
            //        iterator.remove();
            //        found = true;
            //    }
            //}

            foreach (AtomicInteger item in mActiveIndexes)
            {
                if (item.Get() == index)
                {
                    mActiveIndexes.Remove(item);
                    //found = true;
                    break;
                }
            }

            
             
                if (mActiveIndexes.Count == 0)
                {
                    insertPending();
                }
        }

        /**
         * Inserts pending items into the Insertable, and adds them to the active positions (correctly managing position shifting). Clears the pending items.
         */
        private void insertPending()
        {
            foreach (KeyValuePair<int, T> pi in mPendingItemsToInsert)
            {
                foreach (AtomicInteger existing in mActiveIndexes)
                {
                    if (existing.IntValue() >= pi.Key)
                    {
                        existing.IncrementAndGet();
                    }
                }
                mActiveIndexes.Add(new AtomicInteger(pi.Key));
                mInsertable.add(pi.Key, pi.Value);
            }
            mPendingItemsToInsert.Clear();
        }

        /**
         * Returns a collection of currently active indexes.
         */
        //@NonNull
        public ICollection<int> getActiveIndexes()
        {
            ICollection<int> result = new HashSet<int>();
            foreach (AtomicInteger i in mActiveIndexes)
            {
                result.Add(i.Get());
            }
            return result;
        }

        /**
         * Returns a {@code List} of {@code Pair}s with the index and items that are pending to be inserted, in the order they were requested.
         */
        //@NonNull
        public IList<KeyValuePair<int, T>> getPendingItemsToInsert()
        {
            return mPendingItemsToInsert;
        }
    }
}